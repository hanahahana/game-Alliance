using System;
using System.Collections.Generic;
using System.Text;
using Alliance.Pieces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Alliance.Data;
using Alliance.Utilities;

namespace Alliance
{
  public class SelectionPiece
  {
    private static readonly Color ValidSelectionColor = Utils.NewAlpha(Color.Green, .5f);
    private static readonly Color InvalidSelectionColor = Utils.NewAlpha(Color.Red, .5f);

    private Piece mAssociatedPiece;
    public Piece AssociatedPiece
    {
      get { return mAssociatedPiece; }
    }

    private Cell[] mGroup;
    public Cell[] Group
    {
      get { return mGroup; }
    }

    private bool mIsValid;
    public bool IsValid
    {
      get { return mIsValid; }
    }

    private BoxF mBounds;
    public BoxF Bounds
    {
      get { return mBounds; }
    }

    private Color mSelectionColor;
    public Color SelectionColor
    {
      get { return mSelectionColor; }
    }

    public SelectionPiece()
    {
      ClearSelection();
    }

    public void SetSelection(List<Cell> group, Piece piece)
    {
      mSelectionColor = ValidSelectionColor;
      mAssociatedPiece = piece;
      mIsValid = true;
      SetGroup(group);
    }

    private void SetGroup(List<Cell> group)
    {
      Vector2 upperLeft = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 lowerRight = new Vector2(0, 0);
      Vector2 cellSize = new Vector2(0, 0);

      bool blocked = false;
      foreach(Cell cell in group)
      {
        upperLeft.X = Math.Min(cell.X, upperLeft.X);
        upperLeft.Y = Math.Min(cell.Y, upperLeft.Y);

        lowerRight.X = Math.Max(cell.X, lowerRight.X);
        lowerRight.Y = Math.Max(cell.Y, lowerRight.Y);

        cellSize.X = Math.Max(cell.Width, cellSize.X);
        cellSize.Y = Math.Max(cell.Height, cellSize.Y);

        if (cell.Type == CellType.Blocked || cell.IsOuter)
        {
          blocked = true;
        }
      }

      if (blocked)
      {
        mSelectionColor = InvalidSelectionColor;
      }

      Vector2 bottomRight = lowerRight + cellSize;
      mBounds = new BoxF(upperLeft, new SizeF(bottomRight.X - upperLeft.X, bottomRight.Y - upperLeft.Y));

      mIsValid = !blocked;
      mGroup = group.ToArray();
    }

    public void ClearSelection()
    {
      mAssociatedPiece = null;
      if (mGroup != null)
      {
        for (int i = 0; i < mGroup.Length; ++i)
          mGroup[i] = null;
        mGroup = null;
      }

      mBounds = BoxF.Empty;
      mIsValid = false;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
      if (mGroup != null)
      {
        Vector2 loc = mBounds.Location + offset;
        Shapes.FillRectangle(spriteBatch, loc.X, loc.Y, mBounds.Width, mBounds.Height, mSelectionColor);
      }
    }

    public Piece Build()
    {
      Piece piece = mAssociatedPiece.Place(mGroup, this);
      mIsValid = false;
      return piece;
    }
  }
}
