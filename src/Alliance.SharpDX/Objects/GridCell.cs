using System;
using SharpDX;

namespace Alliance
{
  /// <summary>
  /// Represents a particular cell in a grid.
  /// </summary>
  [Serializable]
  public class GridCell : IComparable<GridCell>
  {
    public static Predicate<GridCell> CellIsAvailable = (GridCell cell) => 
    {
      return 
        cell.IsWalkable && 
        cell.Piece == null &&
        !cell.IsThroughway;
    };

    private DijkstraNodeAlliance[] mDijkstraData;

    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public int Row { get; private set; }
    public int Column { get; private set;  }
    public string Key { get; private set; }
    public GridCellType Type { get; private set; }
    public bool IsOuter { get; private set; }
    public bool IsThroughway { get; private set; }
    public Piece Piece { get; private set; }

    public Vector2 Location
    {
      get { return new Vector2(X, Y); }
      set { X = value.X; Y = value.Y; }
    }

    public SizeF Size
    {
      get { return new SizeF(Width, Height); }
      set { Width = value.Width; Height = value.Height; }
    }

    public BoxF Bounds
    {
      get { return new BoxF(Location, Size); }
      set { Location = value.Location; Size = value.Size; }
    }

    public DijkstraNodeAlliance this[DijkstraType key]
    {
      get { return mDijkstraData[(int)key]; }
    }

    public bool IsWalkable
    {
      get { return (Type == GridCellType.Empty) && (!IsOuter || IsThroughway); }
    }

    public GridCell(int column, int row, bool isOuter, bool isThroughway)
    {
      Column = column;
      Row = row;
      Key = string.Format("Row:{0}, Column:{1}", Row, Column);
      Bounds = BoxF.Empty;
      IsOuter = isOuter;
      IsThroughway = isThroughway;
      mDijkstraData = new DijkstraNodeAlliance[2];
    }

    public override bool Equals(object obj)
    {
      GridCell node = obj as GridCell;
      if (node == null) return false;
      return node.Key.Equals(Key);
    }

    public override int GetHashCode()
    {
      return Key.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("{0}", new Point(Column, Row));
    }

    public void RemovePiece()
    {
      Type = GridCellType.Empty;
      Piece = null;
    }

    public void SetPiece(Piece piece)
    {
      if (piece.IsBlocking)
        Type = GridCellType.Blocked;
      Piece = piece;
    }

    public void LinkDijkstraNode(DijkstraNodeAlliance node, DijkstraType type)
    {
      mDijkstraData[(int)type] = node;
    }

    public int CompareTo(GridCell other)
    {
      return AllianceUtilities.CompareVector2(Location, other.Location);
    }
  }
}
