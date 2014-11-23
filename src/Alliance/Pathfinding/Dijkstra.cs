using System;
using System.Collections.Generic;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// Static class for performing a graph search that solves the single-source shortest path problem for a graph 
  /// with nonnegative edge path costs, producing a shortest path tree. Each path can be looked up by checking the
  /// parent of any node until the parent equals the goal.
  /// </summary>
  public static class Dijkstra
  {
    /// <summary>
    /// Performs the Dijkstra searching algorithm to find the best paths to the goal node from any node.
    /// </summary>
    /// <param name="grid">The grid of nodes to solve (or search).</param>
    /// <param name="goal">
    /// The goal node. Note that there is no need for a starting node because this algorithm solves the
    /// shortest path from any node to the goal node.
    /// </param>
    public static void SolveGrid(DijkstraNode[,] grid, DijkstraNode goal)
    {
      List<DijkstraNode> Q = new List<DijkstraNode>(grid.Length);
      int cols = grid.GetLength(0);
      int rows = grid.GetLength(1);

      for (int c = 0; c < cols; ++c)
      {
        for (int r = 0; r < rows; ++r)
        {
          DijkstraNode v = grid[c, r];
          if (!v.IsIllegal)
          {
            v.Reset();
            Q.Add(v);
          }
        }
      }

      goal.Reset();
      goal.Distance = 0;

      while (Q.Count > 0)
      {
        // extra the minimum value
        DijkstraNode u = ExtractMin(ref Q);

        // relax each of the neighbors
        foreach (DijkstraNode v in GetNeighbors(u.GetAdjacentPoints(), grid))
        {
          if (v == null || v.IsIllegal) continue;
          Relax(v, u);
        }
      }
    }

    private static IEnumerable<DijkstraNode> GetNeighbors(Index[] indices, DijkstraNode[,] cells)
    {
      int cols = cells.GetLength(0);
      int rows = cells.GetLength(1);

      foreach (Index index in indices)
      {
        if (index.IsValid(rows, cols))
          yield return cells[index.C, index.R];
      }
    }

    private static DijkstraNode ExtractMin(ref List<DijkstraNode> Q)
    {
      int index = 0;
      DijkstraNode min = Q[0];
      for (int i = 1; i < Q.Count; ++i)
      {
        var curr = Q[i];

        int comparison = curr.CompareTo(min);
        if (comparison < 0)
        {
          min = curr;
          index = i;
        }
      }

      Q.RemoveAt(index);
      return min;
    }

    private static void Relax(DijkstraNode v, DijkstraNode u)
    {
      int alt = u.Distance + PathfindingConstants.OrthogonalCost;
      if (alt < v.Distance)
      {
        v.Distance = alt;
        v.Parent = u;
      }
    }
  }
}
