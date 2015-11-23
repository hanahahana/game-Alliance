using System.Collections.Generic;


using MLA.Utilities.Algorithms.Data;
using MLA.Utilities.Algorithms.Pathfinding;

namespace Alliance.Pathfinding
{
  /// <summary>
  /// An object representing multiple dijkstra paths through a grid. Each path supported should also be found
  /// in <see cref="Alliance.Objects.DijkstraType"/>
  /// </summary>
  public class DijkstraPathsGrid
  {
    private Dictionary<DijkstraType, DijkstraNode[,]> mGrids = new Dictionary<DijkstraType,DijkstraNode[,]>();
    private Dictionary<DijkstraType, DijkstraNode> mGoal = new Dictionary<DijkstraType,DijkstraNode>();

    /// <summary>
    /// Gets or sets the grid for a specific dijkstra type.
    /// </summary>
    /// <param name="type">The type used to get or set a grid.</param>
    /// <returns>The grid corresponding to the Dijkstra type.</returns>
    public DijkstraNode[,] this[DijkstraType type]
    {
      get { return mGrids[type]; }
      set { mGrids[type] = value; }
    }

    /// <summary>
    /// Resets the entire grid by calling "Reset" on all the nodes in each of the grids defined.
    /// </summary>
    public void Reset()
    {
      foreach (DijkstraType type in mGrids.Keys)
      {
        DijkstraNode[,] grid = mGrids[type];
        int cols = grid.GetLength(0);
        int rows = grid.GetLength(1);

        for (int c = 0; c < cols; ++c)
        {
          for (int r = 0; r < rows; ++r)
          {
            grid[c, r].Reset();
          }
        }
      }
    }

    /// <summary>
    /// Solves the specific grid using Dijkstra's algorithm.
    /// </summary>
    /// <param name="type">The type of grid to solve.</param>
    public void Solve(DijkstraType type)
    {
      Dijkstra.SolveGrid(mGrids[type], mGoal[type]);
    }

    /// <summary>
    /// Sets the goal cell for a specific grid type. Note that the cell must be
    /// created and initialized before this can be called.
    /// </summary>
    /// <param name="type">The type of grid of to set the goal cell for.</param>
    /// <param name="c">The column index of the goal cell.</param>
    /// <param name="r">The row index of the goal cell.</param>
    public void SetGoal(DijkstraType type, int c, int r)
    {
      mGoal[type] = mGrids[type][c, r];
    }
  }
}
