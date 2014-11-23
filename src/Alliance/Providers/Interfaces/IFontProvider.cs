using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public interface IFontProvider
  {
    GsFont GetButtonFont();
    GsFont GetPieceLevelFont();
    GsFont GetInvaderLevelFont();
    GsFont GetDefaultFont();
    float GetLineSpacing(GsFont font);
  }
}
