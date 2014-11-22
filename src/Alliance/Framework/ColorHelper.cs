using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Alliance
{
  internal struct HLSColor
  {
    private const int ShadowAdj = -333;
    private const int HilightAdj = 500;
    private const int WatermarkAdj = -50;
    private const int Range = 240;
    private const int HLSMax = 240;
    private const int RGBMax = 255;
    private const int Undefined = 160;
    private int hue;
    private int saturation;
    private int luminosity;
    public int Luminosity
    {
      get
      {
        return this.luminosity;
      }
    }
    public HLSColor(Color color)
    {
      int r = (int)color.R;
      int g = (int)color.G;
      int b = (int)color.B;
      int num4 = Math.Max(Math.Max(r, g), b);
      int num5 = Math.Min(Math.Min(r, g), b);
      int num6 = num4 + num5;
      this.luminosity = (num6 * 240 + 255) / 510;
      int num7 = num4 - num5;
      if (num7 == 0)
      {
        this.saturation = 0;
        this.hue = 160;
        return;
      }
      if (this.luminosity <= 120)
      {
        this.saturation = (num7 * 240 + num6 / 2) / num6;
      }
      else
      {
        this.saturation = (num7 * 240 + (510 - num6) / 2) / (510 - num6);
      }
      int num8 = ((num4 - r) * 40 + num7 / 2) / num7;
      int num9 = ((num4 - g) * 40 + num7 / 2) / num7;
      int num10 = ((num4 - b) * 40 + num7 / 2) / num7;
      if (r == num4)
      {
        this.hue = num10 - num9;
      }
      else
      {
        if (g == num4)
        {
          this.hue = 80 + num8 - num10;
        }
        else
        {
          this.hue = 160 + num9 - num8;
        }
      }
      if (this.hue < 0)
      {
        this.hue += 240;
      }
      if (this.hue > 240)
      {
        this.hue -= 240;
      }
    }
    public Color Darker(float percDarker)
    {
      int num4 = 0;
      int num5 = this.NewLuma(-333, true);
      return this.ColorFromHLS(this.hue, num5 - (int)((float)(num5 - num4) * percDarker), this.saturation);
    }
    public override bool Equals(object o)
    {
      if (!(o is HLSColor))
      {
        return false;
      }
      HLSColor color = (HLSColor)o;
      return this.hue == color.hue && this.saturation == color.saturation && this.luminosity == color.luminosity;
    }
    public static bool operator ==(HLSColor a, HLSColor b)
    {
      return a.Equals(b);
    }
    public static bool operator !=(HLSColor a, HLSColor b)
    {
      return !a.Equals(b);
    }
    public override int GetHashCode()
    {
      return this.hue << 6 | this.saturation << 2 | this.luminosity;
    }
    public Color Lighter(float percLighter)
    {
      int luminosity = this.luminosity;
      int num5 = this.NewLuma(500, true);
      return this.ColorFromHLS(this.hue, luminosity + (int)((float)(num5 - luminosity) * percLighter), this.saturation);
    }
    private int NewLuma(int n, bool scale)
    {
      return this.NewLuma(this.luminosity, n, scale);
    }
    private int NewLuma(int luminosity, int n, bool scale)
    {
      if (n == 0)
      {
        return luminosity;
      }
      if (!scale)
      {
        int num = luminosity + (int)((long)n * 240L / 1000L);
        if (num < 0)
        {
          num = 0;
        }
        if (num > 240)
        {
          num = 240;
        }
        return num;
      }
      if (n > 0)
      {
        return (int)(((long)(luminosity * (1000 - n)) + 241L * (long)n) / 1000L);
      }
      return luminosity * (n + 1000) / 1000;
    }
    private Color ColorFromHLS(int hue, int luminosity, int saturation)
    {
      byte num4;
      byte num3;
      byte num2;
      if (saturation == 0)
      {
        num2 = (num3 = (num4 = (byte)(luminosity * 255 / 240)));
        if (hue == 160)
        {
        }
      }
      else
      {
        int num5;
        if (luminosity <= 120)
        {
          num5 = (luminosity * (240 + saturation) + 120) / 240;
        }
        else
        {
          num5 = luminosity + saturation - (luminosity * saturation + 120) / 240;
        }
        int num6 = 2 * luminosity - num5;
        num3 = (byte)((this.HueToRGB(num6, num5, hue + 80) * 255 + 120) / 240);
        num2 = (byte)((this.HueToRGB(num6, num5, hue) * 255 + 120) / 240);
        num4 = (byte)((this.HueToRGB(num6, num5, hue - 80) * 255 + 120) / 240);
      }
      return new Color(num3, num2, num4);
    }
    private int HueToRGB(int n1, int n2, int hue)
    {
      if (hue < 0)
      {
        hue += 240;
      }
      if (hue > 240)
      {
        hue -= 240;
      }
      if (hue < 40)
      {
        return n1 + ((n2 - n1) * hue + 20) / 40;
      }
      if (hue < 120)
      {
        return n2;
      }
      if (hue < 160)
      {
        return n1 + ((n2 - n1) * (160 - hue) + 20) / 40;
      }
      return n1;
    }
  }

  public static class ColorHelper
  {
    public static Color NewAlpha(Color baseColor, byte alpha)
    {
      return ColorHelper.NewAlpha(baseColor, (float)alpha / 255f);
    }
    public static Color NewAlpha(Color baseColor, float alpha)
    {
      return new Color(new Vector4(baseColor.ToVector3(), MathHelper.Clamp(alpha, 0f, 1f)));
    }
    public static Color TransitionColors(Color start, Color end, float pc)
    {
      int ca = (int)start.A;
      int cr = (int)start.R;
      int cg = (int)start.G;
      int cb = (int)start.B;
      int c2a = (int)end.A;
      int c2r = (int)end.R;
      int c2g = (int)end.G;
      int c2b = (int)end.B;
      int a = (int)Math.Abs((float)ca + (float)(ca - c2a) * pc);
      int r = (int)Math.Abs((float)cr - (float)(cr - c2r) * pc);
      int g = (int)Math.Abs((float)cg - (float)(cg - c2g) * pc);
      int b = (int)Math.Abs((float)cb - (float)(cb - c2b) * pc);
      a = (int)MathHelper.Clamp((float)a, 0f, 255f);
      r = (int)MathHelper.Clamp((float)r, 0f, 255f);
      g = (int)MathHelper.Clamp((float)g, 0f, 255f);
      b = (int)MathHelper.Clamp((float)b, 0f, 255f);
      return ColorHelper.FromArgb(a, r, g, b);
    }
    public static Color Blend(Color[] colors, float[] factors)
    {
      Color retval = colors[0];
      for (int i = 1; i < colors.Length; i++)
      {
        retval = ColorHelper.Blend(retval, colors[i], factors[i - 1]);
      }
      return retval;
    }
    public static Color Blend(Color color1, Color color2, float factor)
    {
      Color c = color1;
      Color c2 = color2;
      float pc = MathHelper.Clamp(factor, 0f, 1f);
      int ca = (int)c.A;
      int cr = (int)c.R;
      int cg = (int)c.G;
      int cb = (int)c.B;
      int c2a = (int)c2.A;
      int c2r = (int)c2.R;
      int c2g = (int)c2.G;
      int c2b = (int)c2.B;
      int a = (int)Math.Abs((float)ca - (float)(ca - c2a) * pc);
      int r = (int)Math.Abs((float)cr - (float)(cr - c2r) * pc);
      int g = (int)Math.Abs((float)cg - (float)(cg - c2g) * pc);
      int b = (int)Math.Abs((float)cb - (float)(cb - c2b) * pc);
      if (a > 255)
      {
        a = 255;
      }
      if (r > 255)
      {
        r = 255;
      }
      if (g > 255)
      {
        g = 255;
      }
      if (b > 255)
      {
        b = 255;
      }
      return ColorHelper.FromArgb(a, r, g, b);
    }
    public static Color FromArgb(int a, int r, int g, int b)
    {
      return new Color((byte)r, (byte)g, (byte)b, (byte)a);
    }
    public static Color OppositeColor(Color color)
    {
      return new Color((float)(255 - color.R), (float)(255 - color.G), (float)(255 - color.B), (float)color.A);
    }
    public static Color Light(Color baseColor)
    {
      HLSColor color = new HLSColor(baseColor);
      return color.Lighter(0.5f);
    }
    public static Color Dark(Color baseColor)
    {
      HLSColor color = new HLSColor(baseColor);
      return color.Darker(0.5f);
    }
  }
}
