using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// The abstract class for a tower. This contains default implementations of methods.
  /// </summary>
  [Serializable]
  public abstract partial class Piece : Sprite, ITextDisplay
  {
    protected static readonly TimeSpan MinTime = TimeSpan.Zero;
    protected static readonly TimeSpan MaxTime = TimeSpan.FromSeconds(4.0f);

    public const int MaxLevel = 5;
    public const float MaxProgress = 100f;
    public const float ProgressPerSecond = 50f;
    public const float Pad = 5;
    public const float TwoPad = Pad * 2;
    public const float DefaultProjectilesPerSecond = 2.5f;
    public const float DefaultProjectileSpeed = 300f;
    public const float DefaultProjectileLifeInSeconds = 1f;
    public const float DefaultTurnSpeed = 1.5f;
    public const int DefaultNumberProjectilesToFire = 1;

    protected TimeSpan mTimeChasingTarget;
    protected float[] mPriceAtLevels = new float[MaxLevel + 1];
    protected float mElapsedProjectileSeconds;
    protected List<Projectile> mQueuedProjectiles;

    public bool Selected { get; set; }
    public bool DisplayUpgradeInfo { get; set; }

    public float FuzzyLifeWeight { get; protected set; }
    public float FuzzyTimeWeight { get; protected set; }
    public float FuzzyDistanceWeight { get; protected set; }
    public float UpgradePercent { get; protected set; }
    public float LevelVisibility { get; protected set; }

    public float ProgressValue { get; protected set; }
    public float TurnSpeed { get; protected set; }
    public float Radius { get; protected set; }
    public float Attack { get; protected set; }
    public float ProjectilesPerSecond { get; protected set; }
    public float ProjectileSpeed { get; protected set; }
    public float ProjectileLifeInSeconds { get; protected set; }    
    public int Level { get; protected set; }
    public int NumberProjectilesToFire { get; protected set; }
    public Invader Target { get; protected set; }
    public GridCell[] Cells { get; protected set; }
    public PieceState State { get; protected set; }
    public PieceGrouping Grouping { get; protected set; }
    public string Description { get; protected set; }
    public string Name { get; protected set; }
    public string UltimateName { get; protected set; }
    public float Price { get; protected set; }
    public bool IsBlocking { get; protected set; }
    public bool FaceTarget { get; protected set; }
    public bool CanFireProjectiles { get; protected set; }
    public BoxF GridBounds { get; protected set; }
    public Element Element { get; protected set; }
    public PieceSpecialty Specialty { get; protected set; }

    public bool CanUpgrade { get { return Level < MaxLevel && Player.EnoughCashFor(this); } }
    public bool IsFire { get { return Element == Element.Fire; } }
    public bool IsWater { get { return Element == Element.Water; } }
    public bool IsAir { get { return Element == Element.Air; } }
    public bool IsEarth { get { return Element == Element.Earth; } }
    public bool IsElectricity { get { return Element == Element.Electricity; } }

    public Piece()
    {
      DisplayUpgradeInfo = true;
      TurnSpeed = DefaultTurnSpeed;
      FuzzyLifeWeight = .7f;
      FuzzyTimeWeight = .8f;
      FuzzyDistanceWeight = .6f;
      Description = string.Empty;
      Radius = 15f;
      Attack = 15f;
      Price = 15;
      UpgradePercent = 15;
      FaceTarget = true;
      IsBlocking = true;
      CanFireProjectiles = true;
      Name = string.Empty;
      UltimateName = string.Empty;
      Grouping = PieceGrouping.Two;
      ProjectilesPerSecond = DefaultProjectilesPerSecond;
      ProjectileSpeed = DefaultProjectileSpeed;
      ProjectileLifeInSeconds = DefaultProjectileLifeInSeconds;
      NumberProjectilesToFire = DefaultNumberProjectilesToFire;
      Selected = false;
      ImageKey = "turret";
      Element = Element.None;
      Specialty = PieceSpecialty.Both;
      mQueuedProjectiles = new List<Projectile>(50);
      LevelVisibility = 1;
    }

    protected abstract Piece CreatePiece(GridCell[] cells);
    protected abstract Projectile CreateProjectile();

    protected void SavePriceInfo()
    {
      if (Level < mPriceAtLevels.Length)
        mPriceAtLevels[Level] = Price;
    }

    protected void TurnToFaceTarget()
    {
      // if we have a target
      if (Target != null)
      {
        if (FaceTarget)
        {
          // if we're supposed to be facing the target, then set the velocity and orientation
          Velocity = AllianceUtilities.ComputeDirection(this, Target);
          Orientation = AllianceUtilities.ComputeOrientation(Orientation, Velocity, TurnSpeed);
        }
        else
        {
          // set the velocity of the projectile based on the current orientation
          Velocity = AllianceUtilities.ComputeProjectileDirection(Orientation);
        }
      }
    }

    protected void FireProjectile(GameTime gameTime)
    {
      if (Target != null)
      {
        BoxF inside = new BoxF(X + Pad, Y + Pad, Width - TwoPad, Height - TwoPad);
        SizeF actSize = new SizeF(Width - Pad, inside.Height);

        Vector2 myCenter = actSize.ToVector2() * .5f;
        for (int i = 0; i < NumberProjectilesToFire; ++i)
        {
          Projectile projectile = CreateProjectile();
          projectile.Velocity = Velocity * ProjectileSpeed;
          projectile.Position = inside.Location + myCenter;
          projectile.Orientation = Orientation;
          projectile.Attack = Attack;
          projectile.UpdateByFrameCount(gameTime, i * NumberProjectilesToFire);
          mQueuedProjectiles.Add(projectile);
        }
      }
    }

    public bool InvaderMatchesSpecialty(Invader target)
    {
      return (Specialty == PieceSpecialty.Both) || 
        (target.Flying && Specialty == PieceSpecialty.Flight) ||
        (!target.Flying && Specialty == PieceSpecialty.Ground);
    }

    protected bool InvalidTarget(float distance, float radiusSquared, Invader target, BoxF targetBounds)
    {
      // if the target isn't our specialty
      bool notOurSpecialty = !InvaderMatchesSpecialty(target);
      if (notOurSpecialty)
        return true;

      // if we can't see the target, then return true
      bool cantSeeTarget = distance > radiusSquared;
      if (cantSeeTarget)
        return true;

      // if the target reached the end, then return true
      bool targetReachedDestination = (target.TargetCell == null);
      if (targetReachedDestination)
        return true;

      // if the target cell hasn't even started yet
      bool targetNotStartedYet = !GridBounds.Contains(targetBounds);
      if (targetNotStartedYet)
        return true;

      // if the target is moving too fast, then return true
      bool targetMovingTooFast = AllianceUtilities.ComputeDirection(this, target) == Vector2.Zero;
      if (targetMovingTooFast)
        return true;

      // return false if we get here
      return false;
    }

    protected virtual void FinalizeSell()
    {
      State = PieceState.Sold;
      Player.SellPiece(this);
      Clear();
    }

    protected virtual float UpgradePrice(float factor)
    {
      return (float)Math.Round(Price * factor);
    }

    protected virtual float UpgradeAttack(float factor)
    {
      return (float)Math.Round(Attack * factor);
    }

    protected virtual float ComputeUpgradeFactor()
    {
      return 1f + (UpgradePercent / 100f);
    }

    protected virtual float UpgradeRadius(float factor)
    {
      return (Radius * factor);
    }

    protected virtual void FinalizeUpgrade()
    {
      State = PieceState.Upgraded;
      ++Level;

      // determine the factor to multiply by
      float factor = ComputeUpgradeFactor();

      // upgrade the attack, price and the radius
      Attack = UpgradeAttack(factor);
      Radius = UpgradeRadius(factor);
      Price = UpgradePrice(factor);

      // set the price
      SavePriceInfo();

      // upgrade the projectile variables
      UpgradeProjectileVariables(factor);
    }

    protected virtual void UpgradeProjectileVariables(float factor)
    {
      ProjectilesPerSecond *= factor;
      ProjectileSpeed *= factor;
      ProjectileLifeInSeconds *= (1.001f);
      NumberProjectilesToFire += (Level / 4);
    }

    public virtual void Update(GameTime gameTime)
    {
      // if we have a target, make sure it's still alive
      if (Target != null)
      {
        mTimeChasingTarget += gameTime.ElapsedGameTime;
      }

      // if we're not idle, then clear the projectiles
      if (State != PieceState.Idle)
      {
        // clear away the projectiles
        mQueuedProjectiles.Clear();
      }

      // once we've upgrade, set this back to idle
      if (State == PieceState.Upgraded)
      {
        State = PieceState.Idle;
      }

      // if we're selling or upgrading
      if (State == PieceState.Selling || State == PieceState.Upgrading)
      {
        UpdateSellingUpgrading(gameTime);
      }

      // if we're just idle
      if (State == PieceState.Idle)
      {
        UpdateIdle(gameTime);
      }
    }

    protected void UpdateIdle(GameTime gameTime)
    {
      // face the target
      TurnToFaceTarget();

      // if we can fire projectiles
      if (CanFireProjectiles)
      {
        // update the projectiles per second
        mElapsedProjectileSeconds += (float)(ProjectilesPerSecond * gameTime.ElapsedGameTime.TotalSeconds);
        if (mElapsedProjectileSeconds >= 1.0f)
        {
          // decrement the value
          mElapsedProjectileSeconds -= 1.0f;

          // fire a projectile
          FireProjectile(gameTime);
        }
      }
    }

    protected void UpdateSellingUpgrading(GameTime gameTime)
    {
      if (Player.State == PlayerState.Designing)
      {
        // if we're designing, then the sell/upgrade is instant
        ProgressValue = MaxProgress;
      }
      else
      {
        // update the current progress
        ProgressValue += (float)(ProgressPerSecond * gameTime.ElapsedGameTime.TotalSeconds);
      }

      // if we've exceeded the max progress
      if (ProgressValue >= MaxProgress)
      {
        // reset the progress
        ProgressValue = 0;

        // check the current state to determine the finalize action
        if (State == PieceState.Selling)
        {
          // if we were selling the piece, then finalize the sell
          FinalizeSell();
        }
        else
        {
          // otherwise, finalize the upgrading.
          FinalizeUpgrade();
        }
      }
    }

    public void Draw(DrawParams dparams)
    {
      Tuple<BoxF, BoxF> outin = GetOutsideInsideBounds(dparams.Offset);
      BoxF bounds = outin.First;
      BoxF inside = outin.Second;

      DrawBackground(dparams, bounds, inside);
      if (State == PieceState.Idle)
      {
        DrawWeaponBase(dparams, bounds, inside);
        DrawWeaponTower(dparams, dparams.Offset);
        DrawCurrentLevel(dparams, bounds, inside);
      }
      else if (State == PieceState.Selling || State == PieceState.Upgrading)
      {
        DrawProgressState(dparams, bounds, inside);
      }
    }

    protected virtual void DrawBackground(DrawParams dparams, BoxF bounds, BoxF inside)
    {
      Color bgColor = (Level == MaxLevel ? ColorHelper.Blend(Color.Beige, Color.SkyBlue, .5f) : Color.Beige);
      Color color = Selected ? Color.DarkGreen : bgColor;
      dparams.Graphics.FillRectangle(bounds, color);
    }

    protected virtual void DrawWeaponBase(DrawParams dparams, BoxF bounds, BoxF inside)
    {
      Texture2D wbase = AllianceGame.Images["towerBase"].Texture;
      Vector2 scale = MathematicsHelper.ComputeScale(new SizeF(wbase.Width, wbase.Height), bounds.Size);

      Color color = ColorHelper.NewAlpha(Color.Gray, .5f);
      SpriteBatch spriteBatch = dparams.SpriteBatch;

      spriteBatch.Draw(
        wbase,
        bounds.Location,
        null,
        color,
        0f,
        Vector2.Zero,
        scale,
        SpriteEffects.None,
        0f);
    }

    protected override TextureDrawData GetTextureDrawData(Vector2 offset)
    {
      Tuple<BoxF, BoxF> outin = GetOutsideInsideBounds(offset);
      BoxF bounds = outin.First;
      BoxF inside = outin.Second;

      Texture2D wtower = GetImage();
      SizeF imgSize = new SizeF(wtower.Width, wtower.Height);
      SizeF actSize = new SizeF(bounds.Width - Pad, inside.Height);

      Vector2 scale = MathematicsHelper.ComputeScale(imgSize, actSize);
      Vector2 imgCenter = imgSize.ToVector2() * .5f;
      Vector2 myCenter = actSize.ToVector2() * .5f;

      return new TextureDrawData(wtower, imgSize, inside.Location + myCenter, imgCenter, scale);
    }

    protected virtual void DrawWeaponTower(DrawParams dparams, Vector2 offset)
    {
      TextureDrawData data = GetTextureDrawData(offset);
      Color color = Color.Gray;

      SpriteBatch spriteBatch = dparams.SpriteBatch;
      spriteBatch.Draw(
        data.Texture,
        data.Position,
        null,
        color,
        Orientation,
        data.Origin,
        data.Scale,
        SpriteEffects.None,
        0f);
    }

    protected virtual void DrawCurrentLevel(DrawParams dparams, BoxF bounds, BoxF inside)
    {
      if (Level < MaxLevel)
      {
        float spacing = 2f;
        float dimension = (bounds.Width - (spacing * 5f)) / 8f;

        float x = bounds.X + spacing;
        float y = bounds.Bottom - (spacing + dimension);

        //for (int i = 0; i < Level; ++i, x += (spacing + dimension))
        //{
        //  dparams.Graphics.FillRectangle(x, y, dimension, dimension, Color.Black);
        //}

        SpriteFont font = AllianceGame.Fonts["Georgia"];
        string text = Level.ToString();
        Vector2 pos = new Vector2(bounds.Left + spacing, bounds.Bottom - (font.LineSpacing + spacing));
        dparams.SpriteBatch.DrawString(font, text, pos, Color.Black);
      }
    }

    protected void DrawProgressState(DrawParams dparams, BoxF bounds, BoxF inside)
    {
      float width = inside.Width;
      float height = (float)Math.Round(inside.Height / 3f);

      float x = inside.X + ((inside.Width / 2f) - (width / 2f));
      float y = inside.Y + ((inside.Height / 2f) - (height / 2f));

      float progressWidth = width * ArithmeticHelper.CalculatePercent(ProgressValue, 0, MaxProgress);
      float factor = ProgressValue / (MaxProgress);

      Color barFill = ColorHelper.Blend(Color.Red, Color.DarkGreen, factor);
      Color barBder = Color.Black;

      dparams.Graphics.FillRectangle(x, y, progressWidth, height, barFill);
      dparams.Graphics.DrawRectangle(x, y, width, height, barBder);
    }

    protected Tuple<BoxF, BoxF> GetOutsideInsideBounds(Vector2 offset)
    {
      BoxF bounds = new BoxF(X + offset.X, Y + offset.Y, Width, Height);
      BoxF inside = new BoxF(bounds.X + Pad, bounds.Y + Pad, bounds.Width - TwoPad, bounds.Height - TwoPad);
      return new Tuple<BoxF, BoxF>(bounds, inside);
    }

    public override string ToString()
    {
      return Name;
    }

    public float GetLifetimePrice()
    {
      // to get the lifetime we get the price it was to place the piece (mPriceAtLevels[0]). The 
      // price to upgrade the piece is the same amount. Then, the price to upgrade at each of the
      // other points to upgrade. The price at a certain level should be zero if the piece hasn't
      // been upgraded
      return mPriceAtLevels.Sum();
    }

    public Piece BuildFromChunk(BoxF gridBounds, GridCellChunk editedChunk)
    {
      GridCell[] cells = editedChunk.Cells.ToList().ToArray();
      Piece piece = CreatePiece(cells);
      piece.GridBounds = gridBounds;

      for (int i = 0; i < cells.Length; ++i)
      {
        GridCell cell = cells[i];
        cell.SetPiece(piece);
      }

      piece.Cells = cells;
      piece.Bounds = editedChunk.Bounds;
      piece.SavePriceInfo();

      if (!Player.PurchasePiece(piece))
      {
        throw new Exception("Somehow you placed this without enough money!");
      }

      return piece;
    }

    public void Clear()
    {
      for (int i = 0; i < Cells.Length; ++i)
      {
        Cells[i].RemovePiece();
      }
      Array.Clear(Cells, 0, Cells.Length);
    }

    public Projectile[] PopProjectiles()
    {
      Projectile[] retval = mQueuedProjectiles.ToArray();
      mQueuedProjectiles.Clear();
      return retval;
    }

    public void Sell()
    {
      if (State == PieceState.Idle)
      {
        ProgressValue = 0;
        State = PieceState.Selling;
      }
    }

    public void SellInstant()
    {
      FinalizeSell();
    }

    public void Upgrade()
    {
      if (State == PieceState.Idle && CanUpgrade)
      {
        if (!Player.PurchasePiece(this))
          throw new Exception("Somehow you upgraded this without enough money!");

        ProgressValue = 0;
        State = PieceState.Upgrading;
      }
    }

    public void ChooseTarget(IEnumerable<Invader> invaders, Vector2 offset)
    {
      Invader bestInvader = null;
      float bestFuzzyValue = 0.0f;
      float radiusSquared = Radius * Radius;
      BoxF pieceBounds = BoxF.Offset(Bounds, offset);

      foreach (Invader invader in invaders)
      {
        // calculate the position. In the future, we should get this directly from the invader
        Vector2 position = invader.Position + offset + (invader.Size.ToVector2() * .5f);
        Vector2 origin = invader.Origin;
        SizeF size = invader.Size;

        // calculate the bounds
        BoxF invaderBounds = new BoxF(position - (origin * .5f), size);

        // determine the distance
        float distance = Vector2.DistanceSquared(pieceBounds.Center, invaderBounds.Center);
        if (InvalidTarget(distance, radiusSquared, invader, invaderBounds))
          continue;

        // determine the fuzzy number
        float fuzzy = 0.0f;
        fuzzy += CalculateFuzzyLife(invader) * FuzzyLifeWeight;
        fuzzy += CalculateFuzzyTime(invader) * FuzzyTimeWeight;
        fuzzy += CalculateFuzzyDistance(distance) * FuzzyDistanceWeight;

        // if the fuzzy value is better, then select this invader
        if (fuzzy > bestFuzzyValue)
        {
          bestInvader = invader;
          bestFuzzyValue = fuzzy;
        }
      }

      if (bestInvader == null)
      {
        Target = null;
      }
      else if (bestInvader != Target)
      {
        mTimeChasingTarget = TimeSpan.Zero;
        Target = bestInvader;
      }
    }

    private float CalculateFuzzyDistance(float distance)
    {
      const float MinRadius = 5f;

      float max = Radius * Radius;
      float min = MinRadius * MinRadius;

      return (1f - ((distance - min) / (max - min)));
    }

    private float CalculateFuzzyLife(Invader invader)
    {
      return (1f - (invader.CurrentLife / invader.MaximumLife));
    }

    private float CalculateFuzzyTime(Invader invader)
    {
      // to calcuate the fuzzy time value, first we figure out how long we've
      // been chasing this mouse. For most mice, this will of course be no
      // time at all.
      TimeSpan time = TimeSpan.Zero;
      if (invader == Target)
      {
        time = mTimeChasingTarget;
      }

      // next we calculate fuzzyTime, which is a value ranging from 0 to 1. It
      // will increase as the amount of time we have spent chasing this mouse
      // increases. The value must be clamped to enforce the 0 to 1 rule.
      float fuzzyTime = (float)((time - MinTime).TotalSeconds / (MaxTime - MinTime).TotalSeconds);
      return MathHelper.Clamp(fuzzyTime, 0, 1);
    }

    #region ITextProvider Members

    public string GetHeader()
    {
      string header = Name;
      if (0 < Level)
      {
        if (Level < Piece.MaxLevel)
        {
          header = string.Concat(header, " Lvl ", Level);
        }
        else if (!(this is SpeedBumpPiece))
        {
          header = UltimateName;
        }
      }
      return header;
    }

    public string GetText()
    {
      StringBuilder text = new StringBuilder();
      List<string> lines = new List<string>();

      text.AppendLine(Description);
      float factor = ComputeUpgradeFactor();

      ++Level;
      float newAttack = UpgradeAttack(factor);
      double newPrice = UpgradePrice(factor);
      --Level;

      lines.Add(string.Format("Attack: {0}", Attack));
      lines.Add(string.Format("Cost: {0:c2}", Price));
      lines.Add(string.Format("Size: {0}", GetDisplayText(Grouping)));
      lines.Add(string.Format("Element: {0}", Element));

      if (DisplayUpgradeInfo && Level < MaxLevel)
      {
        lines[0] = string.Concat(lines[0], string.Format(" + {0}", newAttack - Attack));
        lines[1] = string.Concat(lines[1], string.Format(" + {0:c2}", newPrice - Price));
      }

      foreach (string line in lines)
        text.AppendLine(line);

      return text.ToString();
    }

    public static string GetDisplayText(PieceGrouping grouping)
    {
      string retval = string.Empty;
      switch (grouping)
      {
        case PieceGrouping.One: { retval = "1x1"; break; }
        case PieceGrouping.Two: { retval = "2x2"; break; }
        case PieceGrouping.Three: { retval = "3x3"; break; }
        case PieceGrouping.Four: { retval = "4x4"; break; }
      }
      return retval;
    }

    #endregion
  }
}
