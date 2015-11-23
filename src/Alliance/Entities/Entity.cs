using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Alliance.Data;
using Alliance.Utilities;
using Alliance.Pieces;

namespace Alliance.Entities
{
  public enum EntityState
  {
    Alive,
    MadeIt,
    Dead,
  };

  public abstract class Entity
  {
    public const float MaxMovementPerSecond = 41.5f;
    public const float MinMovementPerSecond = 11.5f;

    protected Cell mTargetCell;
    protected Cell mCurrentCell;
    protected Cell mGoalCell;
    protected BoxF mBounds;
    protected float mOrientation;
    protected EntityState mState;
    protected float mMPS;

    public Cell TargetCell
    {
      get { return mTargetCell; }
    }

    public Cell CurrentCell
    {
      get { return mCurrentCell; }
    }

    public BoxF Bounds
    {
      get { return mBounds; }
      set { mBounds = value; }
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

    public float Orientation
    {
      get { return mOrientation; }
    }

    public EntityState State
    {
      get { return mState; }
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

    public virtual void Update(GameTime gameTime)
    {
      if (mState != EntityState.Alive) return;

      if (mTargetCell == null && mCurrentCell != null)
      {
        X += (float)gameTime.ElapsedGameTime.TotalSeconds * mMPS;
        if (X > (mCurrentCell.X + mCurrentCell.Width))
        {
          mState = EntityState.MadeIt;
        }
      }
      else
      {
        float tX = mTargetCell.X;
        float tY = mTargetCell.Y;

        float dX = tX - X;
        float dY = tY - Y;

        dX = ComputeDelta(dX);
        dY = ComputeDelta(dY);

        X += (float)gameTime.ElapsedGameTime.TotalSeconds * dX * mMPS;
        Y += (float)gameTime.ElapsedGameTime.TotalSeconds * dY * mMPS;

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
          mCurrentCell = mTargetCell;
          mTargetCell = mTargetCell.Parent;
          ComputeOrientation();
        }
      }
    }

    private float ComputeDelta(float delta)
    {
      if (delta < 0)
        return -1;
      if (delta > 0)
        return 1;
      return 0;
    }

    private void ComputeOrientation()
    {
      float dx = (mTargetCell != null ? mTargetCell.X : mCurrentCell.X) - X;
      float dy = (mTargetCell != null ? mTargetCell.Y : mCurrentCell.Y) - Y;

      float desiredAngle = (float)Math.Atan2(dy, dx);
      float difference = Utils.WrapAngle(desiredAngle - mOrientation);
      mOrientation = Utils.WrapAngle(mOrientation + difference);
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
      if (mState != EntityState.Alive) return;

      Texture2D texture = AllianceGame.Textures["tank"];
      Vector2 scale = new Vector2(Width / texture.Width, Height / texture.Height);
      Vector2 center = new Vector2(texture.Width / 2f, texture.Height / 2f);
      Vector2 cellCenter = new Vector2(Width / 2f, Height / 2f);

      spriteBatch.Draw(
        texture,
        mBounds.Location + offset + cellCenter, 
        null, 
        Color.White, 
        mOrientation, 
        center, 
        scale, 
        SpriteEffects.None, 
        0f);
    }
  }
}
