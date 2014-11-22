using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alliance
{
  public class Invader : Sprite
  {
    public const float SecondsPerFrame = 1f / 5.3456789f;

    public const float MinInvaderLevel = 1;
    public const float MaxInvaderLevel = 250;

    public const float MinInvaderLife = 1000;
    public const float MaxInvaderLife = 1000000;

    // These would be options that could be changed via the options
    public static float DefenseEffectiveness = 1.25f;
    public static float LifeFactor = 1f / 15000f;
    public static float AtkFactor = 5f / 10000f;
    public static float DefFactor = 1f / 25000f;

    static readonly Dictionary<Element, AColor> Colors;
    static readonly Dictionary<InvaderAttributes, Tuple<float, float>> MinMaxValues;
    static readonly Dictionary<int, string[]> BaseImageKeys;
    static Invader()
    {
      Colors = new Dictionary<Element, AColor>
      {
        {Element.Air, AColor.Yellow},
        {Element.Earth, AColor.DarkGreen},
        {Element.Electricity, AColor.Purple},
        {Element.Fire, AColor.Red},
        {Element.None, AColor.White},
        {Element.Water, AColor.Blue},
      };

      MinMaxValues = new Dictionary<InvaderAttributes, Tuple<float, float>>
      {
        {InvaderAttributes.Defense, new Tuple<float, float>(25, 1000)},
        {InvaderAttributes.Skill, new Tuple<float, float>(0, 50)},
        {InvaderAttributes.Speed, new Tuple<float, float>(5, 70)},
      };

      BaseImageKeys = new Dictionary<int, string[]>
      {
        {0, new string[]{"walker", "flapper"}},
        {1, new string[]{"biker", "glider"}},
        {2, new string[]{"walker", "flapper"}},
        {3, new string[]{"biker", "glider"}},
        {4, new string[]{"walker", "flapper"}},
        {5, new string[]{"biker", "glider"}},
      };
    }

    private readonly float LevelDenominator;

    private Dictionary<InvaderAttributes, float> mAttributes;
    private float mTotalElapsedSeconds = 0;
    private int mIndex = 0;

    public float MaximumLife { get; private set; }
    public float CurrentLife { get; private set; }
    public float Value { get; private set; }
    public float Experience { get; private set; }
    public float Level { get; private set; }
    public bool Flying { get; private set; }

    public Element Element { get; private set; }
    public InvaderMothership Parent { get; private set; }
    public DijkstraType DijkstraKey { get; private set; }
    public InvaderState State { get; private set; }
    public GridCell TargetCell { get; private set; }
    public GridCell CurrentCell { get; private set; }
    public GridCell GoalCell { get; private set; }

    public Invader(InvaderMothership motherShip, bool flying)
    {
      // initialize a low level invader
      State = InvaderState.Alive;
      Experience = 0;
      Parent = motherShip;
      Flying = flying;

      // set the attributes
      mAttributes = new Dictionary<InvaderAttributes, float>();

      // load in the minimum values
      MinMaxValues.Keys.ToList().ForEach(attr => mAttributes[attr] = MinMaxValues[attr].Item1);

      // set the level denominator
      LevelDenominator = (MaxInvaderLevel / (float)BaseImageKeys.Count);
    }

    /// <summary>
    /// Levels up this invader by adding on the experience points. Each experience is
    /// distributed to a certain attribute of the invader based on the weights that
    /// are passed in.
    /// </summary>
    /// <param name="experiencePts">The amount of experience points.</param>
    /// <param name="info">The info to use when distributing the experience points.</param>
    public void LevelUp(float experiencePts, float level, InvaderLevelUpInfo info)
    {
      // add on to the current experience
      Experience += experiencePts;

      // from here, determine the mu
      float mu = AMath.CalculatePercent(level, MinInvaderLevel, MaxInvaderLevel);

      // now, determine the level (make sure we're at least lvl1)
      Level = level;

      // determine the value
      Value = (float)Math.Ceiling(Level * 5);

      // calculate the maximum life
      MaximumLife = (float)Math.Floor(AMath.SmoothStep(MinInvaderLife, MaxInvaderLife, mu));

      // get the elements with the highest count
      int max = info.ElementCounts.Max(i => i.Value);
      Element[] elements = (from kvp in info.ElementCounts
                            where kvp.Value.Equals(max)
                            select kvp.Key).ToArray();

      // for now, set the first one to be our element
      Element = elements.Length == 1 ? elements[0] : Element.None;

      // create a dictionary
      List<InvaderAttributes> attributes = mAttributes.Keys.ToList();

      // based on the level, add on the to the attributes
      foreach (InvaderAttributes attribute in attributes)
      {
        Tuple<float, float> minmax = MinMaxValues[attribute];
        mAttributes[attribute] = (float)Math.Floor(AMath.SmoothStep(minmax.Item1, minmax.Item2, mu));

        // TODO:
        // here, we need to determine if we're going to increase/decrease the defense, skill, or speed
        // based on how many invaders made it. 
        //
        // An easy AI would take away abilities when invaders didn't
        // make it. It would also decrease defense and skill. Basically making it easier on the player.
        //
        // A normal AI wouldn't do anything.
        // A hard AI would very slightly increase the attributes
        // A difficult AI...you get the picture.
      }

      // set the color based on the element
      Color = Colors[Element];

      // TODO:
      // here, based on the skill, we would update the abilities.

      // set the base image key
      int key = (int)Math.Floor(Level / LevelDenominator);
      ImageKey = BaseImageKeys[key][Flying ? 1 : 0];

      // randomize the animation settings
      mIndex = AllianceSystem.Random.Next() % AllianceSystem.Images[ImageKey].NumberFrames;
      mTotalElapsedSeconds = AllianceSystem.Random.Next(10) * SecondsPerFrame;
    }

    /// <summary>
    /// Briefs the invader on it's mission. It should go from the start cell and try to
    /// reach the goal cell.
    /// </summary>
    /// <param name="start">The cell to start on.</param>
    /// <param name="goal">The goal to get to.</param>
    /// <param name="key">The key to assign to this invader to aid in path finding.</param>
    public void BriefOnMission(GridCell start, GridCell goal, DijkstraType key)
    {
      TargetCell = start;
      GoalCell = goal;
      DijkstraKey = key;

      float dx = (DijkstraKey == DijkstraType.LeftToRight ? TargetCell.Width * 2f : 0);
      float dy = (DijkstraKey == DijkstraType.TopToBottom ? TargetCell.Height * 2f : 0);

      float mux = AllianceSystem.Random.NextSingle();
      float muy = AllianceSystem.Random.NextSingle();

      X = TargetCell.X - (dx * AMath.SmoothStep(1, 5, mux));
      Y = TargetCell.Y - (dy * AMath.SmoothStep(1, 5, muy));

      Width = TargetCell.Width * (Flying ? 1.5f : 1f);
      Height = TargetCell.Height * (Flying ? 1.5f : 1f);

      Velocity = new APoint(mAttributes[InvaderAttributes.Speed]);
      AdjustOrientation();

      AImage texture = GetImage();
      Origin = new APoint(texture.Width / 2f, texture.Height / 2f);

      CurrentLife = MaximumLife;
      TargetCell = Flying ? goal : start;
    }

    public string GetHeader()
    {
      StringBuilder sb = new StringBuilder(ImageKey);
      sb[0] = char.ToUpper(sb[0]);
      return sb.ToString();
    }

    public string GetText()
    {
      StringBuilder text = new StringBuilder();
      text.AppendLine(string.Format("Level: {0}", Level));
      text.AppendLine(string.Format("Life: {0}", (double)CurrentLife));
      text.AppendLine(string.Format("Element: {0}", Element));
      text.AppendLine(string.Format("Status: {0}", State));
      text.AppendLine(string.Format("Speed: {0}", mAttributes[InvaderAttributes.Speed] * VelocityFactor));
      text.AppendLine(string.Format("Defense: {0}", mAttributes[InvaderAttributes.Defense]));
      text.AppendLine(string.Format("Skill: {0}", mAttributes[InvaderAttributes.Skill]));
      text.AppendLine(string.Format("Exp: {0}", Experience));
      return text.ToString();
    }

    public bool CanPlacePiece(Piece piece)
    {
      // if this invader can fly, placing pieces blocks nothing
      if (Flying)
        return true;

      if (CurrentCell != null)
      {
        // if the piece is on the current cell
        if (!Array.TrueForAll(piece.Cells, cell => !(cell.Equals(CurrentCell) || cell.Equals(TargetCell)))) return false;

        // if the current node distance is less than zero, then we can't get to the goal
        if (CurrentCell[DijkstraKey].Distance < 0) return false;
      }
      return true;
    }

    public void OnAttackedByProjectile(Projectile projectile)
    {
      // get the element for easier access
      Element element = projectile.Parent.Element;

      // compare the two elements
      sbyte comparison = Element.CompareWith(element);

      // determine how much to heal based on the projectile
      float lifeBoost = 0f;
      if (comparison == 0)
      {
        lifeBoost += Level * LifeFactor;
      }

      // determine how much to boost the attack based on the projectile
      float atkBoost = 0f;
      if (comparison == -1)
      {
        // we need to get the invader attribute that damages more based on the element
        atkBoost += Level * AtkFactor;
      }

      // determine how much to boost the defense based on the projectile
      float defBoost = 0f;
      if (comparison == 1)
      {
        // we need to get the invader attribute that damages less based on the element
        defBoost += Level * DefFactor;
      }

      // determine the attack and defense
      float attack = projectile.Attack + atkBoost;
      float defense = (mAttributes[InvaderAttributes.Defense] * DefenseEffectiveness) + defBoost;

      // now, get the lifeDelta. We subtract the life boost from the arrack
      float lifeDelta = (float)Math.Floor(attack - (defense + lifeBoost));

      //// here, we would check the abilities to see if there are any that can take the hits (like a forcefield).
      //foreach (DamageAbsorber absorber in mAbilities)
      //{
      //  attack = absorber.AbsorbDamange(this, attack);
      //  if (attack >= 0)
      //    break;
      //}

      // subtract the life delta
      CurrentLife -= Math.Max(1, lifeDelta);
    }

    public void Update(TimeSpan elapsed)
    {
      // if we're not alive, then don't call this
      if (State != InvaderState.Alive)
        return;

      // if our life is 0, then we're dead
      if (CurrentLife <= 0)
      {
        State = InvaderState.Dead;
      }

      // if we're alive, then update
      if (State == InvaderState.Alive)
      {
        // compute the velocity factor given the current cell that we're on
        AdjustVelocityFactor();

        // if we have no target cell, but we have a current cell
        if (TargetCell == null && CurrentCell != null)
        {
          // then move off of it
          MoveOffCurrentCell(elapsed);
        }
        else
        {
          // otherwise, move to the target cell
          MoveToTargetCell(elapsed);
        }

        // scale the seconds per frame
        float factor = SecondsPerFrame * (Flying ? 1.5f : 1f);

        // update the animation
        mTotalElapsedSeconds += (float)(elapsed.TotalSeconds);
        if (mTotalElapsedSeconds >= factor)
        {
          mTotalElapsedSeconds -= factor;
          mIndex = (mIndex + 1) % (AllianceSystem.Images[ImageKey].NumberFrames);
        }
      }

      // if we're not alive, then report back in
      if (State != InvaderState.Alive)
      {
        Parent.ReportBackIn(this);
      }
    }

    public override AImage GetImage()
    {
      return AllianceSystem.Images[ImageKey][mIndex].Texture;
    }

    protected override APoint[] GetImageHull()
    {
      return AllianceSystem.Images[ImageKey][mIndex].Hull;
    }

    protected override TextureDrawData GetTextureDrawData(APoint offset)
    {
      // get the draw data of the base
      TextureDrawData data = base.GetTextureDrawData(offset);

      // get the correct position
      APoint sz = new APoint(Size.Width, Size.Height);
      APoint position = Position + offset + (sz * .5f);

      // return the updated draw data
      return new TextureDrawData(data.Texture, data.TextureSize, position, data.Origin, data.Scale);
    }

    public void Draw(DrawParams dparams)
    {
      if (State != InvaderState.Alive)
        return;

      SpriteBatch spriteBatch = dparams.SpriteBatch;
      Vector2 offset = dparams.Offset;
      TextureDrawData data = GetTextureDrawData(offset);

      //draw the entity itself
      spriteBatch.Draw(
        data.Texture,
        data.Position,
        null,
        Color,
        Orientation,
        data.Origin,
        data.Scale,
        SpriteEffects.None,
        0f);

      // compute the bounds of the life bar
      Vector2 barPosition = data.Position - (Size.ToVector2() * .5f);
      barPosition.Y -= 4f;
      BoxF bar = new BoxF(barPosition, new SizeF(Width, 3f));

      // draw the life bar
      float width = bar.Width * ArithmeticHelper.CalculatePercent(CurrentLife, 0, MaximumLife);
      dparams.Graphics.FillRectangle(bar.X, bar.Y, width, bar.Height, Color.Green);
      dparams.Graphics.DrawRectangle(bar, Color.Black);

      //if (AllianceGame.GameOptions.DisplayLevelOfInvaders)
      //{
      //  // draw the level
      //  SpriteFont verdana = AllianceGame.Fonts["Georgia"];
      //  string text = Level.ToString();
      //  Vector2 pos = new Vector2(bar.X, bar.Y - (verdana.MeasureString(text).Y + 5f));
      //  spriteBatch.DrawString(verdana,
      //    text,
      //    pos,
      //    Color.Gold);
      //}

      // draw the level
      SpriteFont verdana = AllianceGame.Fonts["Georgia"];
      string text = Level.ToString();
      Vector2 pos = new Vector2(bar.X, bar.Y - (verdana.MeasureString(text).Y + 5f));
      spriteBatch.DrawString(verdana,
        text,
        pos,
        Color.Gold);
    }

    private void AdjustOrientation()
    {
      if (TargetCell == null)
      {
        Orientation = (DijkstraKey == DijkstraType.LeftToRight) ? 0f : MathHelper.PiOver2;
      }
      else
      {
        float dx = TargetCell.X - X;
        float dy = TargetCell.Y - Y;

        float desiredAngle = (float)Math.Atan2(dy, dx);
        float difference = MathHelper.WrapAngle(desiredAngle - Orientation);
        Orientation = MathHelper.WrapAngle(Orientation + difference);
      }
    }

    private void AdjustVelocityFactor()
    {
      // start out as one
      VelocityFactor = Vector2.One;

      // determine the factor
      Vector2 factor = Vector2.One;

      // if we have a target, then adjust the sign of the components
      if (TargetCell != null)
      {
        float sX = Math.Sign(TargetCell.X - X);
        float sY = Math.Sign(TargetCell.Y - Y);
        factor.X *= sX;
        factor.Y *= sY;
      }

      if (CurrentCell != null)
      {
        // here, check to see if the cell has any special properties that we should know about
        if ((CurrentCell.Piece is SpeedBumpPiece))
        {
          factor *= .1f;
          CurrentLife -= (CurrentCell.Piece as SpeedBumpPiece).Attack;
        }
      }

      // adjust the velocity factor
      VelocityFactor *= factor;
    }

    private void MoveOffCurrentCell(GameTime gameTime)
    {
      bool madeIt = false;
      float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (DijkstraKey == DijkstraType.LeftToRight)
      {
        X += time * Velocity.X * VelocityFactor.X;
        madeIt = (X > CurrentCell.Bounds.Right);
      }
      else if (DijkstraKey == DijkstraType.TopToBottom)
      {
        Y += time * Velocity.Y * VelocityFactor.Y;
        madeIt = (Y > CurrentCell.Bounds.Bottom);
      }

      if (madeIt)
      {
        State = InvaderState.MadeIt;
        OnReadyForNextCell();
      }
    }

    private void MoveToTargetCell(GameTime gameTime)
    {
      float tX = TargetCell.X;
      float tY = TargetCell.Y;

      float sX = Math.Sign(tX - X);
      float sY = Math.Sign(tY - Y);

      float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
      Position += (time * Velocity * VelocityFactor);

      X = AllianceUtilities.StepToNext(X, sX, tX);
      Y = AllianceUtilities.StepToNext(Y, sY, tY);

      bool readyForNextNode = (X == tX) && (Y == tY);
      if (readyForNextNode)
      {
        OnReadyForNextCell();
      }
    }

    private void OnReadyForNextCell()
    {
      // switch the current cell
      CurrentCell = TargetCell;

      // set the next cell
      if (TargetCell != null)
      {
        // get the dijkstra node
        DijkstraNodeAlliance parent = TargetCell[DijkstraKey].Parent as DijkstraNodeAlliance;
        TargetCell = (parent != null ? parent.Cell : null);
      }

      // compute the orientation
      AdjustOrientation();
    }
  }
}
