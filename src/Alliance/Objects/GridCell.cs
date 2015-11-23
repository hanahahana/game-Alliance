using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using MLA.Utilities;
using MLA.Utilities.Algorithms.Data;

using Microsoft.Xna.Framework;

using Alliance.Pieces;
using Alliance.Data;
using Alliance.Entities;
using Alliance.Parameters;

namespace Alliance.Objects
{
  public enum GridCellType
  {
    Empty,
    Blocked,
  };

  [Flags()]
  public enum DebugAttributes
  {
    None,
    OccupiedByProjectile,
    TargetedByEntity,
  }

  public enum DijkstraType : int
  {
    Horizontal = 0,
    Vertical,
  }

  public class GridCell
  {
    private BoxF mBounds;
    private int mRow;
    private int mColumn;
    private string mKey;
    private GridCellType mType;
    private bool mIsOuter;
    private bool mIsThroughway;
    private Piece mPiece;
    private GridCell[] mAdjacentCells;
    private int mAdjacentCellsIdx;
    private DebugAttributes mAttributes;
    private DijkstraData[] mDijkstraData;

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

    public int Row
    {
      get { return mRow; }
    }

    public int Column
    {
      get { return mColumn; }
    }

    public string Key
    {
      get { return mKey; }
    }

    public GridCellType Type
    {
      get { return mType; }
    }

    public bool IsOuter
    {
      get { return mIsOuter; }
    }

    public bool IsThroughway
    {
      get { return mIsThroughway; }
    }

    public GridCell[] AdjacentCells
    {
      get { return mAdjacentCells; }
    }

    public DebugAttributes Attributes
    {
      get { return mAttributes; }
      set { mAttributes = value; }
    }

    public Piece Piece
    {
      get { return mPiece; }
    }

    public DijkstraData this[DijkstraType key]
    {
      get { return mDijkstraData[(int)key]; }
    }

    public GridCell(int column, int row, bool isOuter, bool isThroughway)
    {
      mColumn = column;
      mRow = row;
      mKey = Guid.NewGuid().ToString();
      mBounds = BoxF.Empty;
      mIsOuter = isOuter;
      mIsThroughway = isThroughway;
      mAdjacentCells = new GridCell[4];
      mAdjacentCellsIdx = 0;
      InitializeDijsktraData();
    }

    private void InitializeDijsktraData()
    {
      mDijkstraData = new DijkstraData[2];
      mDijkstraData[(int)DijkstraType.Horizontal] = new DijkstraData();
      mDijkstraData[(int)DijkstraType.Vertical] = new DijkstraData();
    }

    public override bool Equals(object obj)
    {
      GridCell node = obj as GridCell;
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

    public void RemovePiece()
    {
      mType = GridCellType.Empty;
      mPiece = null;
    }

    public void SetPiece(Piece piece)
    {
      if (piece.IsBlocking)
        mType = GridCellType.Blocked;
      mPiece = piece;
    }

    public AStarNode ToAStarNode()
    {
      bool empty = (mType == GridCellType.Empty);
      AStarNode node = new AStarNode(X, Y, Width, Height, mColumn, mRow, empty && (!mIsOuter || mIsThroughway));
      return node;
    }

    public void Add(GridCell adjacent)
    {
      mAdjacentCells[mAdjacentCellsIdx++] = adjacent;
    }
  }
}
