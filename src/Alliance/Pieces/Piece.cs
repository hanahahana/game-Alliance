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
  }

  public abstract class Piece
  {
    public const int MaxLevel = 5;
    public const float MaxProgress = 100f;
    public const float ProgressPerSecond = 50f;
    public const float Delta = 5f;
    public const float Delta2 = Delta * 2;

    public abstract string Description { get; }
    public abstract string Name { get; }
    public abstract PieceGrouping Grouping { get; }
    protected abstract Piece CreatePiece(Cell[] cells);

    protected Vector2 mPosition;
    protected Cell[] mCells;
    protected SizeF mSize;
    protected bool mSelected;
    protected PieceState mState;
    protected float mProgress = 0;
    protected int mLevel;

    public Vector2 Position { get { return mPosition; } }
    public float X {get { return mPosition.X; }}
    public float Y { get { return mPosition.Y; } }
    public Cell[] Cells { get { return mCells; } }
    public SizeF Size { get { return mSize; } }
    public float Width {get { return mSize.Width; }}
    public float Height { get { return mSize.Height; } }
    public float Progress { get { return mProgress; } }
    public int Level { get { return mLevel; } }
    public bool CanUpgrade { get { return mLevel < MaxLevel; } }
    public PieceState State { get { return mState; } }
    public virtual bool IsBlocking { get { return true; } }

    public bool Selected
    {
      get { return mSelected; }
      set { mSelected = value; }
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

    public virtual void Update(GameTime gameTime)
    {
      // once we've upgrade, set this back to idle
      if (mState == PieceState.Upgraded)
      {
        mState = PieceState.Idle;
      }

      // if we're selling or upgrading
      if (mState == PieceState.Selling || mState == PieceState.Upgrading)
      {
        // update the current progress
        mProgress += (float)(ProgressPerSecond * gameTime.ElapsedGameTime.TotalSeconds);

        // if we've exceeded the max progress
        if (mProgress >= MaxProgress)
        {
          // reset the progress
          mProgress = 0;

          // check the current state to determine the finalize action
          if (mState == PieceState.Selling)
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
    }

    protected virtual void FinalizeSell()
    {
      mState = PieceState.Sold;
      Clear();
    }

    protected virtual void FinalizeUpgrade()
    {
      mState = PieceState.Upgraded;
      ++mLevel;
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
      BoxF bounds = new BoxF(X + offset.X, Y + offset.Y, Width, Height);
      BoxF inside = new BoxF(bounds.X + Delta, bounds.Y + Delta, bounds.Width - Delta2, bounds.Height - Delta2);

      DrawBackground(spriteBatch, bounds, inside);

      if (mState == PieceState.Idle)
      {
        DrawWeapon(spriteBatch, bounds, inside);
      }
      else if (mState == PieceState.Selling || mState == PieceState.Upgrading)
      {
        DrawProgressState(spriteBatch, bounds, inside);
      }
    }

    protected void DrawProgressState(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      float width = inside.Width;
      float height = 4f;

      float x = inside.X + ((inside.Width / 2f) - (width / 2f));
      float y = inside.Y + ((inside.Height / 2f) - (height / 2f));

      float progWidth = width * Utils.CalculatePercent(mProgress, 0, MaxProgress);
      Color progColor = Utils.GetIntermediateColor(Color.DarkBlue, Color.LightBlue, mProgress, 0, MaxProgress);

      Shapes.FillRectangle(spriteBatch, x, y, progWidth, height, progColor);
      Shapes.DrawRectangle(spriteBatch, x, y, width, height, Color.Black);
    }

    protected void DrawWeapon(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Shapes.DrawRectangle(spriteBatch, inside, Color.Black);
      if (mLevel < MaxLevel)
      {
        BoxF upgrade = new BoxF(inside.X + 2, inside.Bottom - 5, 3f, 3f);
        for (int l = 0; l < mLevel; ++l)
        {
          Shapes.FillRectangle(spriteBatch, upgrade, Color.Navy);
          upgrade.X += 4f;
        }
      }
      else
      {
        BoxF insideInside = new BoxF(inside.X + 1, inside.Y + 1, inside.Width - 1, inside.Height - 1);
        Shapes.FillRectangle(spriteBatch, insideInside, Color.Gold);
      }
    }

    protected void DrawBackground(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Color bgColor = (mLevel == MaxLevel ? Utils.GetIntermediateColor(Color.Beige, Color.SkyBlue, .5f, 0, 1f) : Color.Beige);
      Color color = mSelected ? Color.DarkGreen : bgColor;
      Shapes.FillRectangle(spriteBatch, bounds, color);
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
  }
}
