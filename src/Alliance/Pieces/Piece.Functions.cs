using System;
using System.Collections.Generic;
using System.Text;
using Alliance.Projectiles;
using Microsoft.Xna.Framework;
using Alliance.Data;
using Alliance.Entities;

namespace Alliance.Pieces
{
  public enum PieceState
  {
    Idle,
    Upgrading,
    Upgraded,
    Selling,
    Sold,
  };

  public enum PieceGrouping
  {
    /// <summary>
    /// 1x1
    /// </summary>
    One = 1,

    /// <summary>
    /// 2x2
    /// </summary>
    Two,

    /// <summary>
    /// 3x3
    /// </summary>
    Three,

    /// <summary>
    /// 4x4
    /// </summary>
    Four,
  };

  public partial class Piece
  {
    public Vector2 Position { get { return mPosition; } }
    public float X { get { return mPosition.X; } }
    public float Y { get { return mPosition.Y; } }
    public Cell[] Cells { get { return mCells; } }
    public SizeF Size { get { return mSize; } }
    public float Width { get { return mSize.Width; } }
    public float Height { get { return mSize.Height; } }
    public float Progress { get { return mProgress; } }
    public int Level { get { return mLevel; } }
    public bool CanUpgrade { get { return mLevel < MaxLevel; } }
    public PieceState State { get { return mState; } }
    public float Orientation { get { return mOrientation; } }

    public bool Selected
    {
      get { return mSelected; }
      set { mSelected = value; }
    }

    public override int GetHashCode()
    {
      return (new BoxF(mPosition, mSize)).GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Piece piece = obj as Piece;
      if (piece == null) return false;
      return piece.GetHashCode().Equals(this.GetHashCode());
    }

    public override string ToString()
    {
      return Name;
    }

    public Piece Place(Cell[] cells, SelectionPiece selection)
    {
      Piece piece = CreatePiece(cells);
      for (int i = 0; i < cells.Length; ++i)
      {
        Cell cell = cells[i];
        cell.SetPiece(piece);
      }

      piece.mCells = cells;
      piece.mPosition = selection.Bounds.Location;
      piece.mSize = selection.Bounds.Size;

      return piece;
    }

    public void Clear()
    {
      for (int i = 0; i < mCells.Length; ++i)
      {
        mCells[i].RemovePiece();
      }
      Array.Clear(mCells, 0, mCells.Length);
    }

    public Projectile[] PopProjectiles()
    {
      Projectile[] retval = mQueuedProjectiles.ToArray();
      mQueuedProjectiles.Clear();
      return retval;
    }

    public void Sell()
    {
      if (mState == PieceState.Idle)
      {
        mProgress = 0;
        mState = PieceState.Selling;
      }
    }

    public void Upgrade()
    {
      if (mState == PieceState.Idle && CanUpgrade)
      {
        mProgress = 0;
        mState = PieceState.Upgrading;
      }
    }

    public void SetTarget(Entity entity)
    {
      if (mTarget == null)
        mTarget = entity;
    }

    public void ClearTarget()
    {
      mTarget = null;
    }

    public static void FollowSelectionPiece(Piece piece, SelectionPiece mSelectionPiece)
    {
      if (piece != null && mSelectionPiece != null)
      {
        piece.mPosition = mSelectionPiece.Bounds.Location;
        piece.mSize = mSelectionPiece.Bounds.Size;
      }
    }
  }
}
