using System;
using System.Collections.Generic;
using System.Text;
using Alliance.Data;
using Microsoft.Xna.Framework;
using Alliance.Pieces;
using MLA.Utilities;
using MLA.Utilities.Algorithms.Data;
using Alliance.Entities;
using System.Collections.Specialized;

namespace Alliance
{
  public enum CellType
  {
    Empty,
    Blocked,
  };

  public enum DebugAttributes
  {
    None,
    OccupiedByProjectile,
  }

  public class Cell : IComparable<Cell>
  {
    private BoxF mBounds;
    private int mRow;
    private int mColumn;
    private string mKey;
    private CellType mType;
    private bool mIsPath;
    private bool mIsOuter;
    private bool mIsThroughway;
    private int mDistance;
    private Cell mParent;
    private Piece mPiece;
    private Cell[] mAdjacentCells;
    private int mAdjacentCellsIdx;
    private DebugAttributes mAttributes;
    private OrderedDictionary mRegisteredEntities;

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

    public CellType Type
    {
      get { return mType; }
    }

    public bool IsPath
    {
      get { return mIsPath; }
      set { mIsPath = value; }
    }

    public bool IsOuter
    {
      get { return mIsOuter; }
    }

    public bool IsThroughway
    {
      get { return mIsThroughway; }
    }

    public Cell[] AdjacentCells
    {
      get { return mAdjacentCells; }
    }

    public int Distance
    {
      get { return mDistance; }
      set { mDistance = value; }
    }

    public Cell Parent
    {
      get { return mParent; }
      set { mParent = value; }
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

    public int RegisteredEntitiesCount
    {
      get { return mRegisteredEntities.Count; }
    }

    public Cell(int column, int row, bool isOuter, bool isThroughway)
    {
      mColumn = column;
      mRow = row;
      mKey = Guid.NewGuid().ToString();
      mBounds = BoxF.Empty;
      mIsOuter = isOuter;
      mIsThroughway = isThroughway;
      mAdjacentCells = new Cell[4];
      mAdjacentCellsIdx = 0;
      mRegisteredEntities = new OrderedDictionary(50);
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
      mType = CellType.Empty;
      mPiece = null;
    }

    public void SetPiece(Piece piece)
    {
      if (piece.IsBlocking)
        mType = CellType.Blocked;
      mPiece = piece;
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

    public void Add(Cell adjacent)
    {
      mAdjacentCells[mAdjacentCellsIdx++] = adjacent;
    }

    public void Register(Entity entity)
    {
      mRegisteredEntities[entity] = entity;
    }

    public void Unregister(Entity entity)
    {
      mRegisteredEntities.Remove(entity);
    }

    public Entity GetMostRecentRegisteredEntity()
    {
      Entity retval = null;
      int idx = mRegisteredEntities.Count - 1;

      if (idx > -1)
      {
        retval = mRegisteredEntities[idx] as Entity;
      }

      return retval;
    }
  }
}
