using System;

using Alliance.Objects;

using MLA.Utilities;
using MLA.Utilities.Algorithms.Data;

namespace Alliance.Pathfinding
{
  /// <summary>
  /// A wrapper around the DijkstraNode found in MLA.Utilities. This is used to store a link to the 
  /// associated GridCell.
  /// </summary>
  [Serializable]
  public class DijkstraNodeAlliance : DijkstraNode
  {
    private GridCell mAssociatedCell;

    /// <summary>
    /// Gets a value indicating if the node is illegal, meaning it isn't walkable.
    /// </summary>
    public override bool IsIllegal
    {
      get
      {
        return !mAssociatedCell.IsWalkable;
      }
    }

    /// <summary>
    /// Gets the GridCell associated with this node.
    /// </summary>
    public GridCell Cell
    {
      get
      {
        return mAssociatedCell;
      }
    }

    /// <summary>
    /// Creates a new node.
    /// </summary>
    /// <param name="cell">The cell to associate with this node.</param>
    /// <param name="type">The type of node this is. Used to link up with the cell.</param>
    public DijkstraNodeAlliance(GridCell cell, DijkstraType type)
      : base(new Index(cell.Column, cell.Row), !cell.IsWalkable)
    {
      mAssociatedCell = cell;
      mAssociatedCell.LinkDijkstraNode(this, type);
    }
  }
}
