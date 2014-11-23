using System;
using System.Collections.Generic;

namespace Alliance
{
  /// <summary>
  /// Simple Alliance application using SharpDX.Toolkit.
  /// </summary>
  class Program
  {
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      using (var program = new AllianceGame())
      {
        program.Run();
      }
    }
  }
}