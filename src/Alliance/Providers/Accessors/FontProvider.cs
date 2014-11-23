using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public static class FontProvider
  {
    private static IFontProvider sProvider;

    public static void Register(IFontProvider provider)
    {
      sProvider = provider;
    }

    public static GsFont ButtonFont
    {
      get { return sProvider.GetButtonFont(); }
    }

    public static GsFont PieceLevelFont
    {
      get { return sProvider.GetPieceLevelFont(); }
    }

    public static GsFont InvaderLevelFont
    {
      get { return sProvider.GetInvaderLevelFont(); }
    }

    public static GsFont DefaultFont
    {
      get { return sProvider.GetDefaultFont(); }
    }

    public static float GetLineSpacing(GsFont font)
    {
      return sProvider.GetLineSpacing(font);
    }
  }
}
