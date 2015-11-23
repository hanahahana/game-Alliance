using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alliance.Data
{
  public struct SizeF
  {
    public static readonly SizeF Empty;
    private float width;
    private float height;

    public SizeF(SizeF size)
    {
      this.width = size.width;
      this.height = size.height;
    }

    public SizeF(Vector2 pt)
    {
      this.width = pt.X;
      this.height = pt.Y;
    }

    public SizeF(float width, float height)
    {
      this.width = width;
      this.height = height;
    }

    public static SizeF operator +(SizeF sz1, SizeF sz2)
    {
      return Add(sz1, sz2);
    }

    public static SizeF operator -(SizeF sz1, SizeF sz2)
    {
      return Subtract(sz1, sz2);
    }

    public static bool operator ==(SizeF sz1, SizeF sz2)
    {
      return ((sz1.Width == sz2.Width) && (sz1.Height == sz2.Height));
    }

    public static bool operator !=(SizeF sz1, SizeF sz2)
    {
      return !(sz1 == sz2);
    }

    public static explicit operator Vector2(SizeF size)
    {
      return new Vector2(size.Width, size.Height);
    }

    public bool IsEmpty
    {
      get
      {
        return ((this.width == 0f) && (this.height == 0f));
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

    public static SizeF Add(SizeF sz1, SizeF sz2)
    {
      return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
    }

    public static SizeF Subtract(SizeF sz1, SizeF sz2)
    {
      return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is SizeF))
      {
        return false;
      }
      SizeF ef = (SizeF)obj;
      return (((ef.Width == this.Width) && (ef.Height == this.Height)) && ef.GetType().Equals(base.GetType()));
    }

    public override int GetHashCode()
    {
      return string.Concat(this.width, ":", this.height).GetHashCode();
    }

    public Vector2 ToVector2()
    {
      return (Vector2)this;
    }

    public Size ToSize()
    {
      return Size.Truncate(this);
    }

    public override string ToString()
    {
      return ("{Width=" + this.width.ToString() + ", Height=" + this.height.ToString() + "}");
    }

    static SizeF()
    {
      Empty = new SizeF();
    }
  }
}
