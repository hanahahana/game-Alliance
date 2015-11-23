using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Pieces;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Objects;
using MLA.Utilities.Helpers;
using Alliance.Components;

namespace Alliance.Invaders
{
  public enum InvaderState
  {
    Alive,
    MadeIt,
    Dead,
  };

  [Flags]
  public enum InvaderAttributes
  {
    None = 0,
    SpeedBumpResistant = 1,
    Selected = 2,
    FireResistant = 4,
  };

  public abstract class Invader : Sprite, ITextDisplay
  {
    public const float MaxMovementPerSecond = 77.7f;
    public const float MinMovementPerSecond = 11.1f;

    public const int MaxEntityLevel = 1000;
    public const int MinEntityLevel = 1;

    public const float MaxEntityLife = 8000000f;
    public const float MinEntityLife = 5000f;

    /// <summary>
    /// Samples that are used to determine life given a level
    /// </summary>
    protected static readonly List<Vector2> LvlLfeSamples = new List<Vector2>(
      new Vector2[] 
      { 
        new Vector2(MinEntityLevel, MinEntityLife), 
        new Vector2(MaxEntityLevel, MaxEntityLife) 
      });

    /// <summary>
    /// Samples that are used to determine MPS given a level
    /// </summary>
    protected static readonly List<Vector2> LvlMpsSamples = new List<Vector2>(
      new Vector2[]
      {
        new Vector2(MinEntityLevel, MinMovementPerSecond), 
        new Vector2(MaxEntityLevel, MaxMovementPerSecond) 
      });

    protected GridCell mTargetCell;
    protected GridCell mCurrentCell;
    protected GridCell mGoalCell;
    protected InvaderState mState;
    protected float mMPS;
    protected float mCurrentLife;
    protected int mLevel;
    protected Vector2 mOrigin;

    protected readonly string mID;
    protected readonly DijkstraType mDijkstraKey;

    public abstract float MaximumLife { get; }
    public abstract InvaderAttributes Attributes { get; set; }
    public abstract int Cash { get; }

    public GridCell TargetCell
    {
      get { return mTargetCell; }
    }

    public GridCell CurrentCell
    {
      get { return mCurrentCell; }
    }

    public int Level
    {
      get { return mLevel; }
    }

    public float MPS
    {
      get { return mMPS; }
      set { mMPS = value; }
    }

    public float CurrentLife
    {
      get { return mCurrentLife; }
      set { mCurrentLife = Math.Max(0, value); }
    }

    public InvaderState State
    {
      get { return mState; }
    }

    public string ID
    {
      get { return mID; }
    }

    protected override string ImageKey
    {
      get { return "tank"; }
    }

    protected override Vector2 Origin
    {
      get { return mOrigin; }
    }

    public Invader(GridCell startCell, GridCell goalCell, DijkstraType dijkstraKey)
    {
      mTargetCell = startCell;
      mGoalCell = goalCell;
      mState = InvaderState.Alive;

      float dx = (dijkstraKey == DijkstraType.Horizontal ? mTargetCell.Width * 2f : 0);
      float dy = (dijkstraKey == DijkstraType.Vertical ? mTargetCell.Height * 2f : 0);

      X = mTargetCell.X - dx;
      Y = mTargetCell.Y - dy;

      Width = mTargetCell.Width;
      Height = mTargetCell.Height;

      mMPS = MaxMovementPerSecond;
      ComputeOrientation();

      mID = Guid.NewGuid().ToString();
      mDijkstraKey = dijkstraKey;

      // get the projectile image and size
      Texture2D texture = GetImage();

      // compute the origin and the scale
      mOrigin = new Vector2(texture.Width / 2f, texture.Height / 2f);
    }

    public virtual bool CanPlacePiece(Piece piece)
    {
      if (mCurrentCell != null)
      {
        // if the piece is on the current cell
        if (PieceIsOnCurrentCell(piece)) return false;

        // if the current node distance is less than zero, then we can't get to the goal
        if (mCurrentCell[mDijkstraKey].Distance < 0) return false;
      }
      return true;
    }

    private bool PieceIsOnCurrentCell(Piece piece)
    {
      bool retval = false;
      foreach (GridCell cell in piece.Cells)
      {
        if (cell.Equals(mCurrentCell) || cell.Equals(mTargetCell))
        {
          retval = true;
          break;
        }
      }
      return retval;
    }

    private void ComputeOrientation()
    {
      if (mTargetCell == null)
      {
        mOrientation = (mDijkstraKey == DijkstraType.Horizontal) ? 
          0f : MathHelper.PiOver2;
      }
      else
      {
        float dx = mTargetCell.X - X;
        float dy = mTargetCell.Y - Y;

        float desiredAngle = (float)Math.Atan2(dy, dx);
        float difference = Utils.WrapAngle(desiredAngle - mOrientation);
        mOrientation = Utils.WrapAngle(mOrientation + difference);
      }
    }

    public virtual void Update(GameTime gameTime)
    {
      // if our life is 0, then we're dead
      if (mCurrentLife == 0)
      {
        mState = InvaderState.Dead;
      }

      // if we're alive, then update
      if (mState == InvaderState.Alive)
      {
        // compute the velocity given the current cell that we're on
        float velocity = ComputeVelocity();

        // if we have no target cell, but we have a current cell
        if (mTargetCell == null && mCurrentCell != null)
        {
          // then move off of it
          MoveOffCurrentCell(velocity, gameTime);
        }
        else
        {
          // otherwise, move to the target cell
          MoveToTargetCell(velocity, gameTime);
        }
      }
    }

    private float ComputeVelocity()
    {
      float retval = mMPS;
      if (mCurrentCell != null)
      {
        if (mCurrentCell.Piece is SpeedBumpPiece && !HasAttribute(InvaderAttributes.SpeedBumpResistant))
        {
          // we go 1/4 as fast when over a speed bump
          retval *= .25f;

          // if this is a speed bump piece and speed bumps affect me, then I lose life
          CurrentLife -= mCurrentCell.Piece.Attack;
        }
      }
      return retval;
    }

    private bool HasAttribute(InvaderAttributes attribute)
    {
      return (Attributes & attribute) == attribute;
    }

    private void MoveOffCurrentCell(float velocity, GameTime gameTime)
    {
      bool madeIt = false;
      if (mDijkstraKey == DijkstraType.Horizontal)
      {
        X += (float)gameTime.ElapsedGameTime.TotalSeconds * velocity;
        madeIt = (X > (mCurrentCell.X + mCurrentCell.Width));
      }
      else if (mDijkstraKey == DijkstraType.Vertical)
      {
        Y += (float)gameTime.ElapsedGameTime.TotalSeconds * velocity;
        madeIt = (Y > (mCurrentCell.Y + mCurrentCell.Height));
      }

      if (madeIt)
      {
        mState = InvaderState.MadeIt;
        OnReadyForNextCell();
      }
    }

    private void MoveToTargetCell(float velocity, GameTime gameTime)
    {
      float tX = mTargetCell.X;
      float tY = mTargetCell.Y;

      float dX = Math.Sign(tX - X);
      float dY = Math.Sign(tY - Y);

      X += (float)gameTime.ElapsedGameTime.TotalSeconds * dX * velocity;
      Y += (float)gameTime.ElapsedGameTime.TotalSeconds * dY * velocity;

      X = StepToNext(X, dX, tX);
      Y = StepToNext(Y, dY, tY);

      bool readyForNextNode = (X == tX) && (Y == tY);
      if (readyForNextNode)
      {
        OnReadyForNextCell();
      }
    }

    /// <summary>
    /// Changes a value to the target value given the associated delta. This is the equivalent of running the code:
    /// 
    /// if (dA &gt; 0)
    /// {
    ///   a = (a &gt;= tA ? tA : a);
    /// }
    /// else if (da &lt; 0)
    /// {
    ///   a = (a &lt;= tA ? tA : a);
    /// }
    /// </summary>
    /// <param name="a">The value to change.</param>
    /// <param name="dA">The delta value.</param>
    /// <param name="tA">The target value.</param>
    /// <returns>The changed value.</returns>
    private float StepToNext(float a, float dA, float tA)
    {
      return ((dA > 0 && a >= tA) || (dA < 0 && a <= tA)) ? tA : a;
    }

    private void OnReadyForNextCell()
    {
      // switch the current cell
      mCurrentCell = mTargetCell;

      // set the next cell
      if (mTargetCell != null)
        mTargetCell = mTargetCell[mDijkstraKey].Parent;

      // compute the orientation
      ComputeOrientation();
    }

    public virtual void OnAttackedByProjectile(Projectile projectile)
    {
      bool fireresistant = HasAttribute(InvaderAttributes.FireResistant);
      bool flameprojectile = projectile is FlameProjectile || projectile is FlamewaveProjectile;
      if (!(fireresistant && flameprojectile))
      {
        CurrentLife -= projectile.Attack;
      }
    }

    protected override DrawData GetDrawData(Vector2 offset)
    {
      // get the draw data of the base
      DrawData data = base.GetDrawData(offset);

      // get the correct position
      Vector2 position = Position + offset + (Size.ToVector2() * .5f);

      // return the updated draw data
      return new DrawData(data.Texture, data.TextureSize, position, data.Origin, data.Scale);
    }

    public virtual void Draw(DrawParams dparams)
    {
      if (mState != InvaderState.Alive)
        return;

      SpriteBatch spriteBatch = dparams.SpriteBatch;
      Vector2 offset = dparams.Offset;
      DrawData data = GetDrawData(offset);

      Color speedBumpResistant = Color.Red;
      Color fireResistant = Color.Black;

      Color color = Color.White;
      switch (Attributes)
      {
        case InvaderAttributes.FireResistant:
          {
            color = fireResistant;
            break;
          }
        case InvaderAttributes.SpeedBumpResistant:
          {
            color = speedBumpResistant;
            break;
          }
        case InvaderAttributes.FireResistant | InvaderAttributes.SpeedBumpResistant:
          {
            color = Utils.GetIntermediateColor(
              speedBumpResistant,
              fireResistant, .45f, 0f, 1f);
            break;
          }
      }

      //draw the entity itself
      spriteBatch.Draw(
        data.Texture,
        data.Position,
        null,
        color,
        mOrientation,
        data.Origin,
        data.Scale,
        SpriteEffects.None,
        0f);

      // compute the bounds of the life bar
      Vector2 barPosition = data.Position - (Size.ToVector2() * .5f);
      barPosition.Y -= 4f;
      BoxF bar = new BoxF(barPosition, new SizeF(Width, 3f));

      // draw the life bar
      Shapes.FillRectangle(spriteBatch,
        bar.X,
        bar.Y,
        bar.Width * Utils.CalculatePercent(mCurrentLife, 0, MaximumLife),
        bar.Height,
        Color.Green);
      Shapes.DrawRectangle(spriteBatch, bar, Color.Black);

      // draw the level
      SpriteFont verdana = AllianceGame.Fonts["Georgia"];
      string text = mLevel.ToString();
      Vector2 pos = new Vector2(bar.X, bar.Y - (verdana.MeasureString(text).Y + 5f));
      spriteBatch.DrawString(verdana,
        text,
        pos,
        Color.Gold);
    }

    #region ITextProvider Members

    public string GetHeader()
    {
      return GetType().Name;
    }

    public string GetText()
    {
      StringBuilder text = new StringBuilder();
      text.AppendLine("Abilities:");
      text.AppendLine(string.Format("{0}", Attributes));
      text.AppendLine();
      text.AppendLine(string.Format("Life: {0}", (double)CurrentLife));
      text.AppendLine(string.Format("Speed: {0}", MPS));
      text.AppendLine(string.Format("Level: {0}", mLevel));
      text.AppendLine(string.Format("Status: {0}", mState));
      return text.ToString();
    }

    #endregion
  }
}
