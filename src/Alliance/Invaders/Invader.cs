using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphicsSystem;

namespace Alliance
{
  /// <summary></summary>
  [Serializable]
  public class Invader : Sprite, ITextDisplay
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

    static readonly Dictionary<Element, GsColor> Colors;
    static readonly Dictionary<InvaderAttributes, MinMax> MinMaxValues;
    static readonly Dictionary<int, string[]> BaseImageKeys;
    static Invader()
    {
      Colors = new Dictionary<Element, GsColor>
      {
        {Element.Air, GsColor.Yellow},
        {Element.Earth, GsColor.DarkGreen},
        {Element.Electricity, GsColor.Purple},
        {Element.Fire, GsColor.Red},
        {Element.None, GsColor.White},
        {Element.Water, GsColor.Blue},
      };

      MinMaxValues = new Dictionary<InvaderAttributes, MinMax>
      {
        {InvaderAttributes.Defense, new MinMax(25, 1000)},
        {InvaderAttributes.Skill, new MinMax(0, 50)},
        {InvaderAttributes.Speed, new MinMax(5, 70)},
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
      MinMaxValues.Keys.ToList().ForEach(attr => mAttributes[attr] = MinMaxValues[attr].Min);

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
      float mu = Calculator.CalculatePercent(level, MinInvaderLevel, MaxInvaderLevel);

      // now, determine the level (make sure we're at least lvl1)
      Level = level;

      // determine the value
      Value = (float)Math.Ceiling(Level * 5);

      // calculate the maximum life
      MaximumLife = (float)Math.Floor(GsMath.SmoothStep(MinInvaderLife, MaxInvaderLife, mu));

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
        var minmax = MinMaxValues[attribute];
        mAttributes[attribute] = (float)Math.Floor(GsMath.SmoothStep(minmax.Min, minmax.Max, mu));

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
      mIndex = RandomGenerator.Next() % ImageProvider.GetFramedImage(ImageKey).NumberFrames;
      mTotalElapsedSeconds = RandomGenerator.Next(10) * SecondsPerFrame;
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

      float mux = RandomGenerator.NextSingle();
      float muy = RandomGenerator.NextSingle();

      X = TargetCell.X - (dx * GsMath.SmoothStep(1, 5, mux));
      Y = TargetCell.Y - (dy * GsMath.SmoothStep(1, 5, muy));

      Width = TargetCell.Width * (Flying ? 1.5f : 1f);
      Height = TargetCell.Height * (Flying ? 1.5f : 1f);

      Velocity = new GsVector(mAttributes[InvaderAttributes.Speed]);
      AdjustOrientation();

      var texture = GetImage();
      var size = ImageProvider.GetSize(texture);

      Origin = new GsVector(size.Width / 2f, size.Height / 2f);

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
      sbyte comparison = ElementComparer.CompareElements(Element, element);

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
          mIndex = (mIndex + 1) % (ImageProvider.GetFramedImage(ImageKey).NumberFrames);
        }
      }

      // if we're not alive, then report back in
      if (State != InvaderState.Alive)
      {
        Parent.ReportBackIn(this);
      }
    }

    public override GsImage GetImage()
    {
      return ImageProvider.GetFramedImage(ImageKey)[mIndex].Image;
    }

    protected override GsVector[] GetImageHull()
    {
      return ImageProvider.GetFramedImage(ImageKey)[mIndex].Hull;
    }

    protected override ImageParams GetTextureDrawData(GsVector offset)
    {
      // get the draw data of the base
      var data = base.GetTextureDrawData(offset);

      // get the correct position
      GsVector position = Position + offset + (Size.ToVector() * .5f);

      // return the updated draw data
      return new ImageParams(data.Image, data.ImageSize, position, data.Origin, data.Scale);
    }

    public void Draw(DrawParams dparams)
    {
      if (State != InvaderState.Alive)
        return;

      var graphics = dparams.Graphics;
      var offset = dparams.Offset;
      var data = GetTextureDrawData(offset);

      //draw the entity itself
      graphics.DrawImage(data, Color, offset, Orientation);

      // compute the bounds of the life bar
      GsVector barPosition = data.Position - (Size.ToVector() * .5f);
      barPosition.Y -= 4f;
      GsRectangle bar = new GsRectangle(barPosition, new GsSize(Width, 3f));

      // draw the life bar
      float width = bar.Width * Calculator.CalculatePercent(CurrentLife, 0, MaximumLife);
      graphics.FillRectangle(GsColor.Green, bar.X, bar.Y, width, bar.Height);
      graphics.DrawRectangle(GsColor.Black, bar);

      // draw the level
      var font = FontProvider.InvaderLevelFont;
      string text = Level.ToString();
      GsVector pos = new GsVector(bar.X, bar.Y - (GsTextMeasurer.MeasureString(font, text).Height + 5f));
      graphics.DrawString(font, text, pos, GsColor.Gold);
    }

    private void AdjustOrientation()
    {
      if (TargetCell == null)
      {
        Orientation = (DijkstraKey == DijkstraType.LeftToRight) ? 0f : GsMath.PiOver2;
      }
      else
      {
        float dx = TargetCell.X - X;
        float dy = TargetCell.Y - Y;

        float desiredAngle = (float)Math.Atan2(dy, dx);
        float difference = GsMath.WrapAngle(desiredAngle - Orientation);
        Orientation = GsMath.WrapAngle(Orientation + difference);
      }
    }

    private void AdjustVelocityFactor()
    {
      // start out as one
      VelocityFactor = GsVector.One;

      // determine the factor
      GsVector factor = GsVector.One;

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

    private void MoveOffCurrentCell(TimeSpan elapsed)
    {
      bool madeIt = false;
      float time = (float)elapsed.TotalSeconds;

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

    private void MoveToTargetCell(TimeSpan elapsed)
    {
      float tX = TargetCell.X;
      float tY = TargetCell.Y;

      float sX = Math.Sign(tX - X);
      float sY = Math.Sign(tY - Y);

      float time = (float)elapsed.TotalSeconds;
      Position += (time * Velocity * VelocityFactor);

      X = Calculator.StepToNext(X, sX, tX);
      Y = Calculator.StepToNext(Y, sY, tY);

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
