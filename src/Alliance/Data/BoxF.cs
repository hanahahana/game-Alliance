using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alliance.Data
{
  public struct BoxF
  {
    public static readonly BoxF Empty;
    private float x;
    private float y;
    private float width;
    private float height;

    public BoxF(float x, float y, float width, float height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    public BoxF(Vector2 location, SizeF size)
    {
      this.x = location.X;
      this.y = location.Y;
      this.width = size.Width;
      this.height = size.Height;
    }

    public static BoxF FromLTRB(float left, float top, float right, float bottom)
    {
      return new BoxF(left, top, right - left, bottom - top);
    }

    public Vector2 Location
    {
      get
      {
        return new Vector2(this.X, this.Y);
      }
      set
      {
        this.X = value.X;
        this.Y = value.Y;
      }
    }

    public SizeF Size
    {
      get
      {
        return new SizeF(this.Width, this.Height);
      }
      set
      {
        this.Width = value.Width;
        this.Height = value.Height;
      }
    }

    public float X
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

    public float Y
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

    public float Width
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

    public float Height
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

    public float Left
    {
      get
      {
        return this.X;
      }
    }

    public float Top
    {
      get
      {
        return this.Y;
      }
    }

    public float Right
    {
      get
      {
        return (this.X + this.Width);
      }
    }

    public float Bottom
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
        if (this.Width > 0f)
        {
          return (this.Height <= 0f);
        }
        return true;
      }
    }

    public override bool Equals(object obj)
    {
      if (!(obj is BoxF))
      {
        return false;
      }
      BoxF ef = (BoxF)obj;
      return ((((ef.X == this.X) && (ef.Y == this.Y)) && (ef.Width == this.Width)) && (ef.Height == this.Height));
    }

    public static bool operator ==(BoxF left, BoxF right)
    {
      return ((((left.X == right.X) && (left.Y == right.Y)) && (left.Width == right.Width)) && (left.Height == right.Height));
    }

    public static bool operator !=(BoxF left, BoxF right)
    {
      return !(left == right);
    }

    public bool Contains(float x, float y)
    {
      return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
    }

    public bool Contains(Vector2 pt)
    {
      return this.Contains(pt.X, pt.Y);
    }

    public bool Contains(BoxF rect)
    {
      return ((((this.X <= rect.X) && ((rect.X + rect.Width) <= (this.X + this.Width))) && (this.Y <= rect.Y)) && ((rect.Y + rect.Height) <= (this.Y + this.Height)));
    }

    public override int GetHashCode()
    {
      return (int)(((((uint)this.X) ^ ((((uint)this.Y) << 13) | (((uint)this.Y) >> 0x13))) ^ ((((uint)this.Width) << 0x1a) | (((uint)this.Width) >> 6))) ^ ((((uint)this.Height) << 7) | (((uint)this.Height) >> 0x19)));
    }

    public void Inflate(float x, float y)
    {
      this.X -= x;
      this.Y -= y;
      this.Width += 2f * x;
      this.Height += 2f * y;
    }

    public void Inflate(SizeF size)
    {
      this.Inflate(size.Width, size.Height);
    }

    public static BoxF Inflate(BoxF rect, float x, float y)
    {
      BoxF ef = rect;
      ef.Inflate(x, y);
      return ef;
    }

    public void Intersect(BoxF rect)
    {
      BoxF ef = Intersect(rect, this);
      this.X = ef.X;
      this.Y = ef.Y;
      this.Width = ef.Width;
      this.Height = ef.Height;
    }

    public static BoxF Intersect(BoxF a, BoxF b)
    {
      float x = Math.Max(a.X, b.X);
      float num2 = Math.Min((float)(a.X + a.Width), (float)(b.X + b.Width));
      float y = Math.Max(a.Y, b.Y);
      float num4 = Math.Min((float)(a.Y + a.Height), (float)(b.Y + b.Height));
      if ((num2 >= x) && (num4 >= y))
      {
        return new BoxF(x, y, num2 - x, num4 - y);
      }
      return Empty;
    }

    public bool IntersectsWith(BoxF rect)
    {
      return ((((rect.X < (this.X + this.Width)) && (this.X < (rect.X + rect.Width))) && (rect.Y < (this.Y + this.Height))) && (this.Y < (rect.Y + rect.Height)));
    }

    public static BoxF Union(BoxF a, BoxF b)
    {
      float x = Math.Min(a.X, b.X);
      float num2 = Math.Max((float)(a.X + a.Width), (float)(b.X + b.Width));
      float y = Math.Min(a.Y, b.Y);
      float num4 = Math.Max((float)(a.Y + a.Height), (float)(b.Y + b.Height));
      return new BoxF(x, y, num2 - x, num4 - y);
    }

    public void Offset(Vector2 pos)
    {
      this.Offset(pos.X, pos.Y);
    }

    public void Offset(float x, float y)
    {
      this.X += x;
      this.Y += y;
    }

    public static implicit operator BoxF(Rectangle r)
    {
      return new BoxF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
    }

    public override string ToString()
    {
      return ("{X=" + this.X.ToString() + ",Y=" + this.Y.ToString() + ",Width=" + this.Width.ToString() + ",Height=" + this.Height.ToString() + "}");
    }

    static BoxF()
    {
      Empty = new BoxF();
    }
  }

}
