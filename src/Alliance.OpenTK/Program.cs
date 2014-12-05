using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  class Progam
  {
    [STAThread]
    public static void Main()
    {
      using (var game = new AllianceWindow())
      {
        // Run the game at 60 updates per second
        game.Run(60.0);
      }
    }
  }
}
