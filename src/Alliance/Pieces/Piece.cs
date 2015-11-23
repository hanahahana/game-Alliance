using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Alliance.Data;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Utilities;

namespace Alliance.Pieces
{
  public enum PieceState
  {
    Idle,
    Upgrading,
    Selling,
    Sold,
  };

  public abstract class Piece
  {
    public abstract string Description { get; }
    public abstract string Name { get; }
    protected abstract Piece CreatePiece(Cell[] cells);

    protected Vector2 mPosition;
    protected Cell[] mCells;
    protected SizeF mSize;
    protected bool mSelected;
    protected PieceState mState;

    public Vector2 Position
    {
      get { return mPosition; }
    }

    public float X
    {
      get { return mPosition.X; }
    }

    public float Y
    {
      get { return mPosition.Y; }
    }

    public Cell[] Cells
    {
      get { return mCells; }
    }

    public SizeF Size
    {
      get { return mSize; }
    }

    public float Width
    {
      get { return mSize.Width; }
    }

    public float Height
    {
      get { return mSize.Height; }
    }

    public bool Selected
    {
      get { return mSelected; }
      set { mSelected = value; }
    }

    public PieceState State
    {
      get { return mState; }
    }

    public Piece Place(Cell[] cells)
    {
      Vector2 upperLeft = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 lowerRight = new Vector2(float.MinValue, float.MinValue);
      Vector2 cellSize = new Vector2(float.MinValue, float.MinValue);

      Piece piece = CreatePiece(cells);
      for (int i = 0; i < cells.Length; ++i)
      {
        Cell cell = cells[i];
        cell.Type = CellType.Blocked;
        cell.Piece = piece;

        upperLeft.X = Math.Min(cell.X, upperLeft.X);
        upperLeft.Y = Math.Min(cell.Y, upperLeft.Y);

        lowerRight.X = Math.Max(cell.X, lowerRight.X);
        lowerRight.Y = Math.Max(cell.Y, lowerRight.Y);

        cellSize.X = Math.Max(cell.Width, cellSize.X);
        cellSize.Y = Math.Max(cell.Height, cellSize.Y);
      }

      piece.mCells = cells;
      piece.mPosition = upperLeft;

      Vector2 bottomRight = lowerRight + cellSize;
      piece.mSize = new SizeF(bottomRight.X - upperLeft.X, bottomRight.Y - upperLeft.Y);

      return piece;
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

    public void Remove(Cell cell)
    {
      if (mCells != null && Array.IndexOf<Cell>(mCells, cell) > -1)
      {
        for (int i = 0; i < mCells.Length; ++i)
          mCells[i] = null;
        mCells = null;
      }
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
      const float Delta = 5f;
      const float Delta2 = Delta * 2;

      BoxF bounds = new BoxF(X + offset.X, Y + offset.Y, Width, Height);
      BoxF inside = bounds;

      inside.X += Delta;
      inside.Y += Delta;
      inside.Width -= Delta2;
      inside.Height -= Delta2;

      Color color = mSelected ? Color.DarkGreen : Color.Beige;
      Shapes.FillRectangle(spriteBatch, bounds, color);
      Shapes.DrawRectangle(spriteBatch, inside, Color.Black);
    }

    public void Sell()
    {
      //throw new Exception("The method or operation is not implemented.");
      mState = PieceState.Sold;
      for (int i = 0; i < mCells.Length; ++i)
      {
        mCells[i].Type = CellType.Empty;
      }
    }

    public void Upgrade()
    {
      //throw new Exception("The method or operation is not implemented.");
      mState = PieceState.Idle;
    }

    public void Update(GameTime gameTime)
    {
      //throw new Exception("The method or operation is not implemented.");
    }
  }
}
