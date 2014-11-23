using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public interface IAllianceInputState
  {
    bool SelectObject { get; }
    bool SellRequested { get; }
    bool UpgradeRequested { get; }
    bool ClearSelections { get; }
    bool SelectPressed { get; }
    bool EctoplasTransmaterPort { get; }
    GsVector CursorPosition { get; }
  }
}
