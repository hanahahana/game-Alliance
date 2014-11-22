using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public struct ASize
  {
    public float Width, Height;

    public ASize(float size)
      : this(size, size)
    {

    }

    public ASize(float width, float height)
    {
      Width = width;
      Height = height;
    }
  }
}
