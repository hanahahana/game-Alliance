using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public struct AColor
  {
    public static AColor Yellow = new AColor(255, 255, 0);
    public static AColor DarkGreen = new AColor(0, 100, 0);
    public static AColor Purple = new AColor(128, 0, 128);
    public static AColor Red = new AColor(255, 0, 0);
    public static AColor White = new AColor(255, 255, 255);
    public static AColor Blue = new AColor(0, 0, 255);

    public byte A, R, G, B;

    public AColor(byte r, byte g, byte b)
      : this(255, r, g, b)
    {

    }

    public AColor(byte a, byte r, byte g, byte b)
    {
      A = a;
      R = r;
      G = g;
      B = b;
    }
  }
}
