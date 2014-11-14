using System;
using System.Collections.Generic;

namespace Alliance
{
  /// <summary>
  /// Simple Alliance application using SharpDX.Toolkit.
  /// </summary>
  class Program
  {
    static Lazy<ResourceCache> resources;
    public static ResourceCache Resources { get { return resources.Value; } }

    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      using (var program = new AllianceGame())
      {
        resources = new Lazy<ResourceCache>(() => 
          new ResourceCache(program));
        program.Run();
      }
    }
  }
}