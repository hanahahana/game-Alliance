using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public class AStarNode : IComparable<AStarNode>
  {
    private int mHashCode;
    private string mKey;

    public float X
    {
      get;
      protected set;
    }

    public float Y
    {
      get;
      protected set;
    }

    public Index Index
    {
      get;
      protected set;
    }

    public virtual bool IsWalkable
    {
      get;
      protected set;
    }

    public AStarNode Parent
    {
      get;
      internal set;
    }

    internal float G;
    internal float H;
    internal float F;

    public AStarNode(float x, float y, int column, int row, bool walkable)
      : this(x, y, new Index(column, row), walkable)
    {
    }

    public AStarNode(float x, float y, Index index, bool walkable)
    {
      this.X = x;
      this.Y = y;
      this.IsWalkable = walkable;
      this.Index = index;
      this.mKey = string.Format("X={0}, Y={1}", this.X, this.Y);
      this.mHashCode = this.mKey.GetHashCode();
      this.Reset();
    }

    public void Reset()
    {
      this.F = 0f;
      this.G = 0f;
      this.H = 0f;
      this.Parent = null;
    }

    public virtual Index[] GetAdjacentPoints(AStarMovement allowedMovement)
    {
      List<Index> retval = new List<Index>(8);
      Index pt = this.Index;
      if ((allowedMovement & AStarMovement.Orthogonal) != (AStarMovement)0)
      {
        retval.Add(new Index(pt.C - 1, pt.R));
        retval.Add(new Index(pt.C + 1, pt.R));
        retval.Add(new Index(pt.C, pt.R - 1));
        retval.Add(new Index(pt.C, pt.R + 1));
      }
      if ((allowedMovement & AStarMovement.Diagonal) != (AStarMovement)0)
      {
        retval.Add(new Index(pt.C + 1, pt.R - 1));
        retval.Add(new Index(pt.C - 1, pt.R - 1));
        retval.Add(new Index(pt.C - 1, pt.R + 1));
        retval.Add(new Index(pt.C + 1, pt.R + 1));
      }
      return retval.ToArray();
    }
    public override int GetHashCode()
    {
      return this.mHashCode;
    }

    public override bool Equals(object obj)
    {
      AStarNode node = obj as AStarNode;
      return node != null && node.mHashCode.Equals(this.mHashCode);
    }

    public int CompareTo(AStarNode other)
    {
      return this.F.CompareTo(other.F);
    }
  }
}
