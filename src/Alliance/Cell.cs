using System;
using System.Collections.Generic;
using System.Text;
using Alliance.Data;
using Microsoft.Xna.Framework;
using Alliance.Pieces;
using MLA.Utilities;
using MLA.Utilities.Algorithms.Data;

namespace Alliance
{
  public enum CellType
  {
    Empty,
    Blocked,
  };

  public class Cell : IComparable<Cell>
  {
    private BoxF mBounds;
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

    private int mRow;
    public int Row
    {
      get { return mRow; }
    }

    private int mColumn;
    public int Column
    {
      get { return mColumn; }
    }

    private string mKey;
    public string Key
    {
      get { return mKey; }
    }

    private CellType mType;
    public CellType Type
    {
      get { return mType; }
      set { mType = value; }
    }

    private bool mIsPath;
    public bool IsPath
    {
      get { return mIsPath; }
      set { mIsPath = value; }
    }

    private bool mIsOuter;
    public bool IsOuter
    {
      get { return mIsOuter; }
    }

    private bool mIsThroughway;
    public bool IsThroughway
    {
      get { return mIsThroughway; }
    }

    private int mDistance;
    public int Distance
    {
      get { return mDistance; }
      set { mDistance = value; }
    }

    private Cell mParent;
    public Cell Parent
    {
      get { return mParent; }
      set { mParent = value; }
    }

    private Piece mPiece;
    public Piece Piece
    {
      get { return mPiece; }
      set { mPiece = value; }
    }

    public Cell(int column, int row, bool isOuter, bool isThroughway)
    {
      mColumn = column;
      mRow = row;
      mKey = Guid.NewGuid().ToString();
      mBounds = BoxF.Empty;
      mIsOuter = isOuter;
      mIsThroughway = isThroughway;
    }

    public override bool Equals(object obj)
    {
      Cell node = obj as Cell;
      if (node == null) return false;
      return node.mKey.Equals(mKey);
    }

    public override int GetHashCode()
    {
      return mKey.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("{0}", new Point(mColumn, mRow));
    }

    public void ResetDijkstra()
    {
      mIsPath = false;
      mDistance = int.MaxValue;
      mParent = null;
    }

    public void RemovePiece()
    {
      mPiece.Remove(this);
      mType = CellType.Empty;
      mPiece = null;
    }

    #region IComparable<Node> Members

    public int CompareTo(Cell other)
    {
      return mDistance.CompareTo(other.mDistance);
    }

    #endregion

    public AStarNode ToAStarNode()
    {
      bool empty = (mType == CellType.Empty);
      AStarNode node = new AStarNode(X, Y, Width, Height, mColumn, mRow, empty && (!mIsOuter || mIsThroughway));
      return node;
    }
  }
}
