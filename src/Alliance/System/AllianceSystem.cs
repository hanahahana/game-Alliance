using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public static class AllianceSystem
  {
    private static readonly Lazy<Random> random = new Lazy<Random>();
    public static Random Random { get { return random.Value; } }

    private static readonly Dictionary<string, AImage> images;
    public static Dictionary<string, AImage> Images { get { return images; } }
  }
}
