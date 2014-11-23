using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public interface IInputProvider
  {
    IAllianceInputState GetState();
  }
}
