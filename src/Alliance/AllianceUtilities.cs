using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alliance.Invaders;
using Alliance.Objects;
using Alliance.Pieces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities;
using MLA.Utilities.Algorithms.ConvexHull;
using MLA.Utilities.Xna;
using Alliance.Enums;

namespace Alliance
{
  /// <summary>
  /// Contains various methods used solely for the AllianceGame.
  /// </summary>
  public static class AllianceUtilities
  {
    /// <summary>
    /// Retrieves a grid index corresponding to a point.
    /// </summary>
    /// <param name="position">The point to retrieve an index for.</param>
    /// <param name="cellWidth">The width of each cell in the grid.</param>
    /// <param name="cellHeight">The height of each cell in the grid.</param>
    /// <returns>The index corresponding to a point.</returns>
    public static Index GetIndexCorrespondingTo(Vector2 position, int cellWidth, int cellHeight)
    {
      return GetIndexCorrespondingTo(position, cellWidth, cellHeight, null);
    }

    /// <summary>
    /// Retrieves a grid index corresponding to a point.
    /// </summary>
    /// <param name="position">The point to retrieve an index for.</param>
    /// <param name="cellWidth">The width of each cell in the grid.</param>
    /// <param name="cellHeight">The height of each cell in the grid.</param>
    /// <param name="offset">The offset to apply to the point.</param>
    /// <returns>The index corresponding to a point.</returns>
    public static Index GetIndexCorrespondingTo(Vector2 position, int cellWidth, int cellHeight, Vector2? offset)
    {
      Vector2 pos = position - ((offset != null && offset.HasValue) ? offset.Value : Vector2.Zero);
      int c = (int)(pos.X / cellWidth);
      int r = (int)(pos.Y / cellHeight);
      return new Index(c, r);
    }

    /// <summary>
    /// Computes the direction of a projectile that starts out pointing to the right.
    /// </summary>
    /// <param name="angle">
    /// The angle of the projectile after being rotated from the starting point.
    /// </param>
    /// <returns>A vector containing the direction of the projectile.</returns>
    public static Vector2 ComputeProjectileDirection(float angle)
    {
      Vector2 v = new Vector2(1, 0);
      Matrix rotMatrix = Matrix.CreateRotationZ(angle);
      return Vector2.Transform(v, rotMatrix);
    }

    /// <summary>
    /// Creates an array of points corresponding to the bounding polygon of a texture.
    /// </summary>
    /// <param name="texture">The image to create a bounding polygon for.</param>
    /// <returns>An array of points surrounding the image.</returns>
    public static Vector2[] CreateConvexHull(Texture2D texture)
    {
      Color[] colorData = new Color[texture.Width * texture.Height];
      texture.GetData<Color>(colorData);

      List<Vector2> pixels = new List<Vector2>(colorData.Length);
      for (int x = 0; x < texture.Width; ++x)
      {
        for (int y = 0; y < texture.Height; ++y)
        {
          Color color = colorData[x + (y * texture.Width)];
          if (color.A > 250)
          {
            pixels.Add(new Vector2(x, y));
          }
        }
      }

      Vector[] polygon = pixels.Select(v => new Vector(v.X, v.Y)).ToArray();
      Vector[] H = new Vector[polygon.Length];

      int n = ChainConvexHull.ComputeHull(polygon, polygon.Length, ref H);
      Vector[] values = new Vector[n];

      Array.Copy(H, values, n);
      return values.Select(v => new Vector2(v.X, v.Y)).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="invader"></param>
    /// <returns></returns>
    public static Vector2 ComputeDirection(Piece piece, Invader invader)
    {
      float speed = piece.ProjectileSpeed;
      Vector2 position = invader.Bounds.Center - piece.Bounds.Center;
      Vector2 velocity = invader.Velocity * invader.VelocityFactor;

      float a = speed * speed - velocity.LengthSquared();
      float b = Vector2.Dot(position, velocity);
      float c = position.LengthSquared();
      // Cope with rare special case where bullet and target have same speed, to avoid dividing by zero  
      if (a == 0)
      {
        if (b < 0)
        {
          // Meet halfway...  
          float time = -0.5f * c / b;
          return (position + velocity * time) / (speed * time);
        }
        else
        {
          // Can't hit target
        }
      }
      else
      {
        float bSqPlusAC = b * b + a * c;
        // Can't take square root of negative number  
        if (bSqPlusAC >= 0)
        {
          float solution = (b + (float)Math.Sqrt(bSqPlusAC)) / a;
          if (solution >= 0)
          {
            float time = solution;
            return (position + velocity * time) / (speed * time);
          }
          else
          {
            // Can't hit target  
          }
        }
        else
        {
          // Can't hit target  
        }
      }

      return Vector2.Zero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="orientation"></param>
    /// <param name="direction"></param>
    /// <param name="turnSpeed"></param>
    /// <returns></returns>
    public static float ComputeOrientation(float orientation, Vector2 direction, float turnSpeed)
    {
      float desiredAngle = (float)Math.Atan2(direction.Y, direction.X);
      float difference = MathHelper.WrapAngle(desiredAngle - orientation);

      //difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
      return MathHelper.WrapAngle(orientation + difference);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pt"></param>
    /// <param name="offset"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static BoxF CreateBoxAroundPoint(Vector2 pt, Vector2 offset, float width, float height)
    {
      BoxF box = new BoxF(pt.X - (width / 2), pt.Y - (height / 2), width, height);
      box.Location += offset;
      return box;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="box"></param>
    /// <param name="cellOffset"></param>
    /// <returns></returns>
    public static GridCell[] GetCellsWithCenterContainedIn(GridCell[,] grid, BoxF box, Vector2 cellOffset)
    {
      int ColCount = grid.GetLength(0);
      int RowCount = grid.GetLength(1);

      List<GridCell> cells = new List<GridCell>(100);
      for (int c = 0; c < ColCount; ++c)
      {
        for (int r = 0; r < RowCount; ++r)
        {
          GridCell cell = grid[c, r];
          Vector2 center = cell.Bounds.Location + cellOffset;

          center.X += (cell.Width / 2f);
          center.Y += (cell.Height / 2f);

          if (box.Contains(center))
          {
            cells.Add(cell);
          }
        }
      }
      return cells.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cells"></param>
    /// <param name="cellWidth"></param>
    /// <param name="cellHeight"></param>
    /// <returns></returns>
    public static BoxF ComputeBounds(IEnumerable<GridCell> cells, float cellWidth, float cellHeight)
    {
      // create a copy of the cells
      List<GridCell> lst = new List<GridCell>(cells);

      // sort the list by Y, then X. This will make the cells go left to right and then top to bottom.
      lst.Sort();

      // get the first and last locations
      GridCell tl = lst[0];
      GridCell br = lst[lst.Count - 1];

      // finally, return the box
      return BoxF.FromLTRB(tl.X, tl.Y, br.X + cellWidth, br.Y + cellHeight);
    }

    /// <summary>
    /// Changes a value to the target value given the associated delta. This is the equivalent of running the code:
    /// 
    /// if (dA &gt; 0)
    /// {
    ///   a = (a &gt;= tA ? tA : a);
    /// }
    /// else if (da &lt; 0)
    /// {
    ///   a = (a &lt;= tA ? tA : a);
    /// }
    /// </summary>
    /// <param name="a">The value to change.</param>
    /// <param name="dA">The delta value.</param>
    /// <param name="tA">The target value.</param>
    /// <returns>The changed value.</returns>
    public static float StepToNext(float a, float dA, float tA)
    {
      return ((dA > 0 && a >= tA) || (dA < 0 && a <= tA)) ? tA : a;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="spaceWidth"></param>
    /// <param name="lineWidth"></param>
    /// <param name="font"></param>
    /// <returns></returns>
    public static string Wrap(string text, float spaceWidth, float lineWidth, SpriteFont font)
    {
      float spaceLeft = lineWidth;
      List<string> words = new List<string>(text.Split(' '));
      words.RemoveAll(w => string.IsNullOrEmpty(w.Trim()));
      for (int i = 0; i < words.Count; ++i)
      {
        string word = words[i];
        float wordWidth = font.MeasureString(word).X;

        if (wordWidth > spaceLeft)
        {
          words.Insert(i, "\r\n"); ++i;
          spaceLeft = lineWidth - wordWidth;
        }
        else
        {
          spaceLeft = spaceLeft - (wordWidth + spaceWidth);
        }
      }

      StringBuilder sb = new StringBuilder();
      foreach (string item in words)
      {
        sb.Append(item);
        if (!item.Equals("\r\n"))
          sb.Append(" ");
      }

      return sb.ToString();
    }

    public static int CompareVector2(Vector2 a, Vector2 b)
    {
      int compare = a.X.CompareTo(b.X);
      if (compare == 0)
      {
        compare = a.Y.CompareTo(b.Y);
      }
      return compare;
    }

    public static float GetPercent(int actualCount, int totalCount)
    {
      return ((float)actualCount) / ((float)totalCount);
    }

    public static sbyte CompareElements(Element a, Element b)
    {
      // Fire > Air > Earth > Water > (Fire & Electricity)
      const sbyte equal = 0;
      const sbyte greater = 1;
      const sbyte less = -1;
      const sbyte invalid = sbyte.MaxValue;

      sbyte retval = invalid;
      switch (a)
      {
        case Element.Air:
          {
            switch (b)
            {
              case Element.Air: retval = equal; break;
              case Element.Earth: retval = greater; break;
              case Element.Fire: retval = less; break;
            }
            break;
          }
        case Element.Earth:
          {
            switch (b)
            {
              case Element.Earth: retval = equal; break;
              case Element.Water: retval = greater; break;
              case Element.Air: retval = less; break;
            }
            break;
          }
        case Element.Electricity:
          {
            switch (b)
            {
              case Element.Electricity: retval = equal; break;
              case Element.Water: retval = less; break;
            }
            break;
          }
        case Element.Fire:
          {
            switch (b)
            {
              case Element.Fire: retval = equal; break;
              case Element.Air: retval = greater; break;
              case Element.Water: retval = less; break;
            }
            break;
          }
        case Element.Water:
          {
            switch (b)
            {
              case Element.Water: retval = equal; break;
              case Element.Fire: retval = greater; break;
              case Element.Earth: retval = less; break;
            }
            break;
          }
      }

      return retval;
    }
  }
}
