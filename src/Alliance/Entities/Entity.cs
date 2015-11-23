using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Pieces;
using Alliance.Projectiles;

namespace Alliance.Entities
{
  public enum EntityState
  {
    Alive,
    MadeIt,
    Dead,
  };

  [Flags()]
  public enum EntityAttributes
  {
    None,
    SpeedBumpNoAffect,
    Selected,
  };

  public abstract class Entity : ITextProvider
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

    protected Cell mTargetCell;
    protected Cell mCurrentCell;
    protected Cell mGoalCell;
    protected BoxF mBounds;
    protected float mOrientation;
    protected EntityState mState;
    protected float mMPS;
    protected float mCurrentLife;
    protected int mLevel;
    protected readonly string mID;

    public abstract float MaximumLife { get; }
    public abstract EntityAttributes Attributes { get; set; }
    public abstract int Cash { get; }

    public Cell TargetCell
    {
      get { return mTargetCell; }
    }

    public Cell CurrentCell
    {
      get { return mCurrentCell; }
    }

    public int Level
    {
      get { return mLevel; }
    }

    public BoxF Bounds
    {
      get { return mBounds; }
      set { mBounds = value; }
    }

    public SizeF Size
    {
      get { return mBounds.Size; }
      set { mBounds.Size = value; }
    }

    public Vector2 Position
    {
      get { return mBounds.Location; }
      set { mBounds.Location = value; }
    }

    public float X
    {
      get { return mBounds.X; }
      set { mBounds.X = value; }
    }

    public float Y
    {
      get { return mBounds.Y; }
      set { mBounds.Y = value; }
    }

    public float Width
    {
      get { return mBounds.Width; }
      set { mBounds.Width = value; }
    }

    public float Height
    {
      get { return mBounds.Height; }
      set { mBounds.Height = value; }
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

    public float Orientation
    {
      get { return mOrientation; }
    }

    public EntityState State
    {
      get { return mState; }
    }

    public string ID
    {
      get { return mID; }
    }

    public Entity(Cell startCell, Cell goalCell)
    {
      mTargetCell = startCell;
      mGoalCell = goalCell;
      mState = EntityState.Alive;
      mOrientation = 0f;

      mBounds.X = mTargetCell.X - mTargetCell.Width;
      mBounds.Y = mTargetCell.Y;
      mBounds.Width = mTargetCell.Width;
      mBounds.Height = mTargetCell.Height;

      mMPS = MaxMovementPerSecond;
      ComputeOrientation();
      mID = Guid.NewGuid().ToString();
    }

    public virtual bool CanPlacePiece(Piece piece)
    {
      if (mCurrentCell != null)
      {
        // if the piece is on the current cell
        if (PieceIsOnCurrentCell(piece)) return false;

        // if the current node distance is less than zero, then we can't get to the goal
        if (mCurrentCell.Distance < 0) return false;
      }
      return true;
    }

    private bool PieceIsOnCurrentCell(Piece piece)
    {
      bool retval = false;
      foreach (Cell cell in piece.Cells)
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
      float dx = (mTargetCell != null ? mTargetCell.X : mCurrentCell.X) - X;
      float dy = (mTargetCell != null ? mTargetCell.Y : mCurrentCell.Y) - Y;

      float desiredAngle = (float)Math.Atan2(dy, dx);
      float difference = Utils.WrapAngle(desiredAngle - mOrientation);
      mOrientation = Utils.WrapAngle(mOrientation + difference);
    }

    public virtual void Update(GameTime gameTime)
    {
      // if our life is 0, then we're dead
      if (mCurrentLife == 0)
      {
        mState = EntityState.Dead;
      }

      // if we're alive, then update
      if (mState == EntityState.Alive)
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
        if (mCurrentCell.Piece is SpeedBumpPiece && (Attributes & EntityAttributes.SpeedBumpNoAffect) == 0)
        {
          // we go 1/4 as fast when over a speed bump
          retval *= .25f;

          // if this is a speed bump piece and speed bumps affect me, then I lose life
          CurrentLife -= mCurrentCell.Piece.Attack;
        }
      }
      return retval;
    }

    private void MoveOffCurrentCell(float velocity, GameTime gameTime)
    {
      X += (float)gameTime.ElapsedGameTime.TotalSeconds * velocity;
      if (X > (mCurrentCell.X + mCurrentCell.Width))
      {
        mState = EntityState.MadeIt;
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

      if (dX > 0)
      {
        X = (X >= tX ? tX : X);
      }
      else if (dX < 0)
      {
        X = (X <= tX ? tX : X);
      }

      if (dY > 0)
      {
        Y = (Y >= tY ? tY : Y);
      }
      else if (dY < 0)
      {
        Y = (Y <= tY ? tY : Y);
      }

      bool readyForNextNode = (X == tX) && (Y == tY);
      if (readyForNextNode)
      {
        OnReadyForNextCell();
      }
    }

    private void OnReadyForNextCell()
    {
      BeforeCurrentCellChanged();
      mCurrentCell = mTargetCell;
      AfterCurrentCellChanged();

      bool noTarget = (mTargetCell == null);
      bool noCurrent = (mCurrentCell == null);

      if (!noTarget)
        mTargetCell = mTargetCell.Parent;

      if (!noTarget || !noCurrent)
        ComputeOrientation();
    }

    private void AfterCurrentCellChanged()
    {
      if (mCurrentCell != null)
      {
        mCurrentCell.Register(this);
      }
    }

    private void BeforeCurrentCellChanged()
    {
      if (mCurrentCell != null)
      {
        mCurrentCell.Unregister(this);
      }
    }

    protected virtual Texture2D GetEntityImage()
    {
      return AllianceGame.Textures["tank"];
    }

    public virtual Color[,] GetEntityImageData()
    {
      return AllianceGame.TextureData["tank"];
    }

    public virtual DrawData GetDrawData(Vector2 offset)
    {
      Texture2D texture = GetEntityImage();
      SizeF textureSize = new SizeF(texture.Width, texture.Height);

      Vector2 origin = textureSize.ToVector2() * .5f;
      Vector2 scale = Utils.ComputeScale(textureSize, Size);

      Vector2 myCenter = Size.ToVector2() * .5f;
      Vector2 position = mBounds.Location + offset + myCenter;

      return new DrawData(texture, textureSize, position, origin, scale);
    }

    public virtual Matrix ComputeTransform(Vector2 offset)
    {
      return ComputeTransform(GetDrawData(offset));
    }

    public virtual Matrix ComputeTransform(DrawData data)
    {
      // create the matrix for transforming the center
      Matrix transform =
        Matrix.CreateTranslation(-data.Origin.X, -data.Origin.Y, 0) *
        Matrix.CreateRotationZ(mOrientation) *
        Matrix.CreateScale(data.Scale.X, data.Scale.Y, 1f) *
        Matrix.CreateTranslation(data.Position.X, data.Position.Y, 0);

      // return the transform
      return transform;
    }

    public virtual Vector2 GetCenter(Vector2 offset)
    {
      // get the drawing data
      DrawData data = GetDrawData(offset);

      // get the center of the image
      Vector2 center = (data.TextureSize / 2f).ToVector2();

      // compute the transform
      Matrix transform = ComputeTransform(data);

      // return the center transformated
      Vector2 result;
      Vector2.Transform(ref center, ref transform, out result);
      return result;
    }

    public virtual BoxF GetBoundingBox(Vector2 offset)
    {
      // get the center
      Vector2 center = GetCenter(offset);

      // create a rough box that has the entity inside of it
      float dW = Width * .5f;
      float dH = Height * .5f;
      return new BoxF(
        center.X - dW,
        center.Y - dH,
        dW * 2f,
        dH * 2f);
    }

    public virtual void Draw(DrawParams dparams)
    {
      // get the necessary parameters
      SpriteBatch spriteBatch = dparams.SpriteBatch;
      Vector2 offset = dparams.Offset;

      if (mState != EntityState.Alive) return;
      DrawData data = GetDrawData(offset);

      // determine the color
      Color color = (Attributes & EntityAttributes.SpeedBumpNoAffect) == 0 ? Color.White : Color.Red;
      color = (Attributes & EntityAttributes.Selected) == 0 ? color : Color.LightGreen;

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
      text.AppendLine(string.Format("{0}", Position));
      return text.ToString();
    }

    #endregion
  }
}
