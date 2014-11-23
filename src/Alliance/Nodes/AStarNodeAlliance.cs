namespace Alliance
{
  /// <summary>
  /// A wrapper around the AStarNode found in MLA.Utilities. This is used to get the "IsWalkable" value
  /// directly from the associated cell.
  /// </summary>
  public class AStarNodeAlliance : AStarNode
  {
    private GridCell mAssociatedCell;

    /// <summary>
    /// Gets a value indicating if a cell is walkable.
    /// </summary>
    public override bool IsWalkable
    {
      get 
      {
        return mAssociatedCell.IsWalkable;
      }
    }

    /// <summary>
    /// Creates a node.
    /// </summary>
    /// <param name="associatedCell">The associated cell to pull data from.</param>
    public AStarNodeAlliance(GridCell cell) : 
      base(cell.X, cell.Y, cell.Column, cell.Row, cell.IsWalkable)
    {
      mAssociatedCell = cell;
    }
  }
}
