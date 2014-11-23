using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Alliance
{
  /// <summary>
  /// Represents the movement allowed in an A* search.
  /// </summary>
  public enum AStarMovement
  {
    /// <summary>Movement only in the standard straight directions (N, S, E and W).</summary>
    Orthogonal = 2,

    /// <summary>Movement only in the standard diagonal directions (NE, NW, SE and SW).</summary>
    Diagonal = 4,

    /// <summary>Movement in both orthogonal and diagonal directions.</summary>
    Both = Orthogonal | Diagonal,
  };

  /// <summary>
  /// Static class for performing a best-first graph search algorithm that finds the least-cost path from a given initial node to one goal node 
  /// (out of one or more possible goals). It uses a distance-plus-cost heuristic function to determine the order in which 
  /// the search visits nodes in the tree.
  /// </summary>
  public static class AStar
  {
    /// <summary>
    /// Solves an A* grid and determines if a path exists. A path exists if the search returns successful, and the path
    /// returned actually contains nodes.
    /// </summary>
    /// <param name="grid">The grid of nodes to test.</param>
    /// <param name="allowedMovement">The movement allowed.</param>
    /// <param name="start">The starting node.</param>
    /// <param name="end">The ending (or goal) node.</param>
    /// <returns>A value indicating if a path exists.</returns>
    public static bool PathExists(AStarNode[,] grid, AStarMovement allowedMovement, AStarNode start, AStarNode end)
    {
      AStarNode[] path;
      bool exists = SolveGrid(grid, allowedMovement, start, end, out path);
      return exists && path.Length > 0;
    }

    /// <summary>
    /// Performs the A* searching algorithm to find the best path to the end (or goal) node.
    /// </summary>
    /// <param name="grid">The grid of nodes to solve (or search).</param>
    /// <param name="allowedMovement">The movement allowed.</param>
    /// <param name="start">The starting node.</param>
    /// <param name="end">The ending (or goal) node.</param>
    /// <param name="path">
    /// The collection of nodes that form the path. This can also be obtained by checking the parent of the end (or goal)
    /// node until it is null (no path found) or it equals the start node. (end.Parent.Parent.Parent.Parent.Parent ..etc).
    /// </param>
    /// <returns>A boolean value indicating if the grid has been solved (e.g. a path was successfully found from start to end).</returns>
    public static bool SolveGrid(AStarNode[,] grid, AStarMovement allowedMovement, AStarNode start, AStarNode end, out AStarNode[] path)
    {
      #region old code
      //// set the return value
      //bool solved = true;

      //// add it to an "open list" of squares to be considered
      //Dictionary<AStarNode, Index> openList = new Dictionary<AStarNode, Index>(grid.Length);
      //openList.Add(start, start.Index);

      //Dictionary<AStarNode, Index> closedList = new Dictionary<AStarNode, Index>(grid.Length);
      //// Look at all the reachable or walkable squares adjacent to the starting point, 
      //// ignoring squares with walls, water, or other illegal terrain. Add them to the open list, too
      //int width = grid.GetLength(0);
      //int height = grid.GetLength(1);
      //Index[] indices = start.GetAdjacentPoints(allowedMovement);
      //foreach (Index index in indices)
      //{
      //  if (Range.IsInRange(index.C, 0, width - 1) && Range.IsInRange(index.R, 0, height - 1))
      //  {
      //    AStarNode neighbor = grid[index.C, index.R];
      //    if (neighbor.IsWalkable)
      //    {
      //      // For each of these squares, save point A as its "parent square". This parent square stuff 
      //      // is important when we want to trace our path.
      //      neighbor.Parent = start;
      //      openList.Add(neighbor, index);
      //    }
      //  }
      //}

      //// Drop the starting square A from your open list, and add it to a "closed list" of squares that 
      //// you don’t need to look at again for now
      //openList.Remove(start);
      //closedList.Add(start, start.Index);

      //// We repeat this process until we add the target square to the closed list
      //while (!closedList.ContainsKey(end))
      //{
      //  // The key to determining which squares to use when figuring out the path is the following equation: 
      //  // F = G + H
      //  // * G = the movement cost to move from the starting point A to a given square on the grid, following 
      //  //   the path generated to get there.
      //  // * H = the estimated movement cost to move from that given square on the grid to the final destination, 
      //  //   point B. This is often referred to as the heuristic, which can be a bit confusing. The reason why 
      //  //   it is called that is because it is a guess. We really don’t know the actual distance until we find the 
      //  //   path, because all sorts of things can be in the way (walls, water, etc.). You are given one way to calculate 
      //  //   H in this tutorial, but there are many others that you can find in other articles on the web.
      //  List<AStarNode> nodes = new List<AStarNode>(openList.Keys);
      //  foreach (AStarNode node in nodes)
      //  {
      //    // if both the x and the y are different, then this is a diagonal movement
      //    float movementCost = AlgorithmConstants.OrthogonalCost;
      //    if (node.X != node.Parent.X && node.Y != node.Parent.Y)
      //    {
      //      movementCost = AlgorithmConstants.DiagonalCost;
      //    }

      //    // Since we are calculating the G cost along a specific path to a given square, the way to figure out the G 
      //    // cost of that square is to take the G cost of its parent, and then add 10 or 14 depending on whether it is 
      //    // diagonal or orthogonal (non-diagonal) from that parent square.
      //    float h = Hueristic(node, end);
      //    float g = node.Parent.Cost + movementCost;
      //    node.Cost = g;

      //    // F is calculated by adding G and H
      //    node.F = h + g;
      //  }

      //  if (nodes.Count == 0)
      //  {
      //    solved = false;
      //    break;
      //  }

      //  // Choose the lowest F score square from all those that are on the open list
      //  AStarNode lowest = CollectionHelper.FindMinimumValue<AStarNode>(nodes);

      //  // Drop it from the open list and add it to the closed list.
      //  Index curIdx = openList[lowest];
      //  openList.Remove(lowest);
      //  closedList.Add(lowest, curIdx);

      //  // Check all of the adjacent squares. Ignoring those that are on the closed list or unwalkable (terrain with walls, water, 
      //  // or other illegal terrain), add squares to the open list if they are not on the open list already. 
      //  indices = lowest.GetAdjacentPoints(allowedMovement);
      //  foreach (Index index in indices)
      //  {
      //    if (index.IsValid(height, width))
      //    {
      //      AStarNode neighbor = grid[index.C, index.R];
      //      if (neighbor.IsWalkable && !closedList.ContainsKey(neighbor))
      //      {
      //        if (openList.ContainsKey(neighbor))
      //        {
      //          // If an adjacent square is already on the open list, check to see if this path to that square is a better one. 
      //          // In other words, check to see if the G score for that square is lower if we use the current square to get there. 
      //          // If not, don’t do anything.

      //          // if both the x and the y are different, then this is a diagonal movement
      //          float movementCost = AlgorithmConstants.OrthogonalCost;
      //          if (neighbor.X != lowest.X && neighbor.Y != lowest.Y)
      //          {
      //            movementCost = AlgorithmConstants.DiagonalCost;
      //          }

      //          float costOfNeighbor = neighbor.Cost;
      //          float costToGetToNeighbor = lowest.Cost + movementCost;

      //          // On the other hand, if the G cost of the new path is lower, change the parent of the 
      //          // adjacent square to the selected square.
      //          if (costToGetToNeighbor < costOfNeighbor)
      //          {
      //            neighbor.Parent = lowest;
      //          }
      //        }
      //        else
      //        {
      //          // Make the selected square is the "parent" of the new squares. 
      //          neighbor.Parent = lowest;
      //          openList.Add(neighbor, index);
      //        }
      //      }
      //    }
      //  }
      //}

      //List<AStarNode> outParam = new List<AStarNode>();
      //if (solved)
      //{
      //  AStarNode trace = end;
      //  while (trace.Parent != null)
      //  {
      //    outParam.Insert(0, trace);
      //    trace = trace.Parent;
      //  }
      //  outParam.Insert(0, trace);
      //}
      //else
      //{
      //  outParam.Clear();
      //}

      //path = outParam.ToArray();
      //return solved;
      #endregion

      // determine the width and height to aid in path finding
      int width = grid.GetLength(0);
      int height = grid.GetLength(1);

      // closedset := the empty set % The set of nodes already evaluated.     
      List<AStarNode> closedset = new List<AStarNode>(grid.Length);

      // openset := set containing the initial node % The set of tentative nodes to be evaluated.
      List<AStarNode> openset = new List<AStarNode>(grid.Length);
      openset.Add(start);

      // create a list to hold the path
      List<AStarNode> pathList = new List<AStarNode>(grid.Length);

      // g_score[start] := 0 % Distance from start along optimal path.
      // h_score[start] := heuristic_estimate_of_distance(start, goal)
      // f_score[start] := h_score[start] % Estimated total distance from start to goal through y.
      start.G = 0;
      start.H = Hueristic(start, end);
      start.F = start.H;

      // while openset is not empty
      while (openset.Count > 0)
      {
        // x := the node in openset having the lowest f_score[] value
        int idx = FindMinIndex(openset);
        AStarNode x = openset[idx];

        // if x = goal
        if (x.Equals(end))
        {
          // return reconstruct_path(came_from,goal)
          pathList = ReconstructPath(end);
          break;
        }

        // remove x from openset
        openset.RemoveAt(idx);

        // add x to closedset
        closedset.Add(x);

        // foreach y in neighbor_nodes(x)
        Index[] neighborsIndex = x.GetAdjacentPoints(allowedMovement);
        foreach (Index index in neighborsIndex)
        {
          // if the index isn't valid, then keep going
          if (!index.IsValid(height, width))
            continue;

          // other, get the node
          AStarNode y = grid[index.C, index.R];

          // if y in closedset or it isn't walkable, then keep going
          if (closedset.Contains(y) || !y.IsWalkable)
            continue;

          // tentative_g_score := g_score[x] + dist_between(x,y)
          float tentative_g_score = x.G + Distance(x, y);
          bool tentative_is_better = false;

          // if y not in openset
          if (!openset.Contains(y))
          {
            // add y to openset
            openset.Add(y);

            // tentative_is_better := true
            tentative_is_better = true;
          }
          // elseif tentative_g_score < g_score[y]
          else if (tentative_g_score < y.G)
          {
            // tentative_is_better := true
            tentative_is_better = true;
          }

          // if tentative_is_better = true
          if (tentative_is_better)
          {
            // came_from[y] := x
            y.Parent = x;

            // g_score[y] := tentative_g_score
            y.G = tentative_g_score;

            // h_score[y] := heuristic_estimate_of_distance(y, goal)
            y.H = Hueristic(y, end);

            // f_score[y] := g_score[y] + h_score[y]
            y.F = y.G + y.H;
          }
        }
      }

      // set the path
      path = pathList.ToArray();

      // return if the pathlist was empty
      return pathList.Count > 0;
    }

    private static List<AStarNode> ReconstructPath(AStarNode endOfPath)
    {
      List<AStarNode> retval = new List<AStarNode>();
      AStarNode trace = endOfPath;
      while (trace.Parent != null)
      {
        retval.Insert(0, trace);
        trace = trace.Parent;
      }
      retval.Insert(0, trace);
      return retval;
    }

    private static float Distance(AStarNode a, AStarNode b)
    {
      float dx = a.X - b.X;
      float dy = a.Y - b.Y;
      return (dx * dx) + (dy * dy);
    }

    private static int FindMinIndex(List<AStarNode> lst)
    {
      // find the index of the item with the lowest f score
      int idx = 0;
      float lowest = lst[idx].F;

      // cycle through the items
      for (int i = 1; i < lst.Count; ++i)
      {
        if (lst[i].F < lowest)
        {
          idx = i;
          lowest = lst[i].F;
        }
      }

      // return the index
      return idx;
    }

    private static float Hueristic(AStarNode current, AStarNode target)
    {
      return PathfindingConstants.OrthogonalCost * (Math.Abs(current.X - target.X) + Math.Abs(current.Y - target.Y));
    }
  }
}
