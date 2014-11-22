using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public struct ARect
  {
    public static readonly ARect Empty = new ARect();
    public float X, Y, Width, Height;

    public APoint Location
    {
      get { return new APoint(X, Y); }
      set
      {
        X = value.X;
        Y = value.Y;
      }
    }

    public ASize Size
    {
      get
      {
        return new ASize(this.Width, this.Height);
      }
      set
      {
        Width = value.Width;
        Height = value.Height;
      }
    }

    public float Left { get { return X; } }
    public float Top { get { return Y; } }
    public float Right { get { return X + Width; } }
    public float Bottom { get { return Y + Height; } }
    public APoint Center
    {
      get { return new APoint((X + (Width/2), Y + (Height/2)); }
      set { Location += value - Center; }
    }
    public bool IsEmpty { get { return Width <= 0f || Height <= 0f; } }

    public ARect(APoint location, ASize size)
      : this(location.X, location.Y, size.Width, size.Height)
    {
    }

    public ARect(float x, float y, float width, float height)
    {
      X = x;
      Y = y;
      Width = width;
      Height = height;
    }

    public static ARect FromLTRB(float left, float top, float right, float bottom)
    {
      return new ARect(left, top, right - left, bottom - top);
    }

    public static ARect FromCenterAndSize(APoint center, ASize size)
    {
      return FromCenterAndSize(center.X, center.Y, size.Width, size.Height);
    }

    public static ARect FromCenterAndSize(float cx, float cy, ASize size)
    {
      return FromCenterAndSize(cx, cy, size.Width, size.Height);
    }

    public static ARect FromCenterAndSize(APoint center, float width, float height)
    {
      return FromCenterAndSize(center.X, center.Y, width, height);
    }

    public static ARect FromCenterAndSize(float cx, float cy, float width, float height)
    {
      return new ARect(0f, 0f, width, height)
      {
        Center = new APoint(cx, cy),
      };
    }

    public override bool Equals(object obj)
    {
      if (!(obj is ARect))
      {
        return false;
      }
      ARect ef = (ARect)obj;
      return ef.X == this.X && ef.Y == this.Y && ef.Width == this.Width && ef.Height == this.Height;
    }

    public static bool operator ==(ARect left, ARect right)
    {
      return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
    }

    public static bool operator !=(ARect left, ARect right)
    {
      return !(left == right);
    }

    public bool Contains(float x, float y)
    {
      return this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;
    }

    public bool Contains(APoint pt)
    {
      return this.Contains(pt.X, pt.Y);
    }

    public bool Contains(ARect rect)
    {
      return this.X <= rect.X && rect.X + rect.Width <= this.X + this.Width && this.Y <= rect.Y && rect.Y + rect.Height <= this.Y + this.Height;
    }

    public override int GetHashCode()
    {
      return (int)((uint)this.X ^ ((uint)this.Y << 13 | (uint)this.Y >> 19) ^ ((uint)this.Width << 26 | (uint)this.Width >> 6) ^ ((uint)this.Height << 7 | (uint)this.Height >> 25));
    }

    public void Inflate(float x, float y)
    {
      X -= x;
      Y -= y;
      Width += 2f * x;
      Height += 2f * y;
    }

    public void Inflate(ASize size)
    {
      this.Inflate(size.Width, size.Height);
    }

    public static ARect Inflate(ARect box, float x, float y)
    {
      ARect ef = box;
      ef.Inflate(x, y);
      return ef;
    }

    public void Intersect(ARect box)
    {
      ARect ef = ARect.Intersect(box, this);
      this.X = ef.X;
      this.Y = ef.Y;
      this.Width = ef.Width;
      this.Height = ef.Height;
    }

    public static ARect Intersect(ARect a, ARect b)
    {
      float x = Math.Max(a.X, b.X);
      float num2 = Math.Min(a.X + a.Width, b.X + b.Width);
      float y = Math.Max(a.Y, b.Y);
      float num3 = Math.Min(a.Y + a.Height, b.Y + b.Height);
      if (num2 >= x && num3 >= y)
      {
        return new ARect(x, y, num2 - x, num3 - y);
      }
      return ARect.Empty;
    }

    public bool IntersectsWith(ARect box)
    {
      return box.X < this.X + this.Width && this.X < box.X + box.Width && box.Y < this.Y + this.Height && this.Y < box.Y + box.Height;
    }

    public static ARect Union(ARect a, ARect b)
    {
      float x = Math.Min(a.X, b.X);
      float num2 = Math.Max(a.X + a.Width, b.X + b.Width);
      float y = Math.Min(a.Y, b.Y);
      float num3 = Math.Max(a.Y + a.Height, b.Y + b.Height);
      return new ARect(x, y, num2 - x, num3 - y);
    }

    public void Offset(APoint pos)
    {
      this.Offset(pos.X, pos.Y);
    }

    public void Offset(float x, float y)
    {
      this.X += x;
      this.Y += y;
    }

    public static ARect Offset(ARect box, float dx, float dy)
    {
      return new ARect(box.X + dx, box.Y + dy, box.Width, box.Height);
    }

    public static ARect Offset(ARect box, APoint offset)
    {
      return new ARect(box.Location + offset, box.Size);
    }

    public override string ToString()
    {
      return string.Format("{{X={0},Y={1},Width={2},Height={3}}}",
        X, Y, Width, Height);
    }
  }
}
