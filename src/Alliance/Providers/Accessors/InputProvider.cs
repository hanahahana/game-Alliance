using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public static class InputProvider
  {
    private static IInputProvider sProvider;

    public static void Register(IInputProvider provider)
    {
      sProvider = provider;
    }

    public static IAllianceInputState GetState()
    {
      return sProvider.GetState();
    }
  }
}
