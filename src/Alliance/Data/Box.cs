using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace Alliance.Data
{
  public struct Box
  {
    public static readonly Box Empty;
    private int x;
    private int y;
    private int width;
    private int height;

    public Box(int x, int y, int width, int height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    public Box(Point location, Size size)
    {
      this.x = location.X;
      this.y = location.Y;
      this.width = size.Width;
      this.height = size.Height;
    }

    public static Box FromLTRB(int left, int top, int right, int bottom)
    {
      return new Box(left, top, right - left, bottom - top);
    }

    public Point Location
    {
      get
      {
        return new Point(this.X, this.Y);
      }
      set
      {
        this.X = value.X;
        this.Y = value.Y;
      }
    }
    public Size Size
    {
      get
      {
        return new Size(this.Width, this.Height);
      }
      set
      {
        this.Width = value.Width;
        this.Height = value.Height;
      }
    }
    public int X
    {
      get
      {
        return this.x;
      }
      set
      {
        this.x = value;
      }
    }
    public int Y
    {
      get
      {
        return this.y;
      }
      set
      {
        this.y = value;
      }
    }
    public int Width
    {
      get
      {
        return this.width;
      }
      set
      {
        this.width = value;
      }
    }
    public int Height
    {
      get
      {
        return this.height;
      }
      set
      {
        this.height = value;
      }
    }

    public int Left
    {
      get
      {
        return this.X;
      }
    }

    public int Top
    {
      get
      {
        return this.Y;
      }
    }

    public int Right
    {
      get
      {
        return (this.X + this.Width);
      }
    }

    public int Bottom
    {
      get
      {
        return (this.Y + this.Height);
      }
    }

    public bool IsEmpty
    {
      get
      {
        return ((((this.height == 0) && (this.width == 0)) && (this.x == 0)) && (this.y == 0));
      }
    }

    public override bool Equals(object obj)
    {
      if (!(obj is Box))
      {
        return false;
      }
      Box rectangle = (Box)obj;
      return ((((rectangle.X == this.X) && (rectangle.Y == this.Y)) && (rectangle.Width == this.Width)) && (rectangle.Height == this.Height));
    }

    public static bool operator ==(Box left, Box right)
    {
      return ((((left.X == right.X) && (left.Y == right.Y)) && (left.Width == right.Width)) && (left.Height == right.Height));
    }

    public static bool operator !=(Box left, Box right)
    {
      return !(left == right);
    }

    public static Box Ceiling(BoxF value)
    {
      return new Box((int)Math.Ceiling((double)value.X), (int)Math.Ceiling((double)value.Y), (int)Math.Ceiling((double)value.Width), (int)Math.Ceiling((double)value.Height));
    }

    public static Box Truncate(BoxF value)
    {
      return new Box((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
    }

    public static Box Round(BoxF value)
    {
      return new Box((int)Math.Round((double)value.X), (int)Math.Round((double)value.Y), (int)Math.Round((double)value.Width), (int)Math.Round((double)value.Height));
    }

    public bool Contains(int x, int y)
    {
      return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
    }

    public bool Contains(Point pt)
    {
      return this.Contains(pt.X, pt.Y);
    }

    public bool Contains(Box box)
    {
      return ((((this.X <= box.X) && ((box.X + box.Width) <= (this.X + this.Width))) && (this.Y <= box.Y)) && ((box.Y + box.Height) <= (this.Y + this.Height)));
    }

    public override int GetHashCode()
    {
      return (((this.X ^ ((this.Y << 13) | (this.Y >> 0x13))) ^ ((this.Width << 0x1a) | (this.Width >> 6))) ^ ((this.Height << 7) | (this.Height >> 0x19)));
    }

    public void Inflate(int width, int height)
    {
      this.X -= width;
      this.Y -= height;
      this.Width += 2 * width;
      this.Height += 2 * height;
    }

    public void Inflate(Size size)
    {
      this.Inflate(size.Width, size.Height);
    }

    public static Box Inflate(Box box, int x, int y)
    {
      Box rectangle = box;
      rectangle.Inflate(x, y);
      return rectangle;
    }

    public void Intersect(Box box)
    {
      Box rectangle = Intersect(box, this);
      this.X = rectangle.X;
      this.Y = rectangle.Y;
      this.Width = rectangle.Width;
      this.Height = rectangle.Height;
    }

    public static Box Intersect(Box a, Box b)
    {
      int x = Math.Max(a.X, b.X);
      int num2 = Math.Min((int)(a.X + a.Width), (int)(b.X + b.Width));
      int y = Math.Max(a.Y, b.Y);
      int num4 = Math.Min((int)(a.Y + a.Height), (int)(b.Y + b.Height));
      if ((num2 >= x) && (num4 >= y))
      {
        return new Box(x, y, num2 - x, num4 - y);
      }
      return Empty;
    }

    public bool IntersectsWith(Box rect)
    {
      return ((((rect.X < (this.X + this.Width)) && (this.X < (rect.X + rect.Width))) && (rect.Y < (this.Y + this.Height))) && (this.Y < (rect.Y + rect.Height)));
    }

    public static Box Union(Box a, Box b)
    {
      int x = Math.Min(a.X, b.X);
      int num2 = Math.Max((int)(a.X + a.Width), (int)(b.X + b.Width));
      int y = Math.Min(a.Y, b.Y);
      int num4 = Math.Max((int)(a.Y + a.Height), (int)(b.Y + b.Height));
      return new Box(x, y, num2 - x, num4 - y);
    }

    public void Offset(Point pos)
    {
      this.Offset(pos.X, pos.Y);
    }

    public void Offset(int x, int y)
    {
      this.X += x;
      this.Y += y;
    }

    public override string ToString()
    {
      return ("{X=" + this.X.ToString() + ",Y=" + this.Y.ToString() + ",Width=" + this.Width.ToString() + ",Height=" + this.Height.ToString() + "}");
    }

    static Box()
    {
      Empty = new Box();
    }
  }
}
