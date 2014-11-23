using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public class GridDestination
  {
    public GridCell Start, End;
    public GridDestination() { }
    public GridDestination(GridCell start, GridCell end)
    {
      Start = start;
      End = end;
    }
  }
}
