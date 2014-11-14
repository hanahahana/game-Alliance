using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Alliance
{
  public static class ColorExtensions
  {
    public static Color NewAlpha(this Color c, int alpha)
    {
      return new Color(c.R, c.G, c.B, alpha);
    }
  }
}
