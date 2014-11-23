using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public static class ElementComparer
  {
    public static sbyte CompareElements(Element a, Element b)
    {
      // Fire > Air > Earth > Water > (Fire & Electricity)
      const sbyte equal = 0;
      const sbyte greater = 1;
      const sbyte less = -1;
      const sbyte invalid = sbyte.MaxValue;

      sbyte retval = invalid;
      switch (a)
      {
        case Element.Air:
          {
            switch (b)
            {
              case Element.Air: retval = equal; break;
              case Element.Earth: retval = greater; break;
              case Element.Fire: retval = less; break;
            }
            break;
          }
        case Element.Earth:
          {
            switch (b)
            {
              case Element.Earth: retval = equal; break;
              case Element.Water: retval = greater; break;
              case Element.Air: retval = less; break;
            }
            break;
          }
        case Element.Electricity:
          {
            switch (b)
            {
              case Element.Electricity: retval = equal; break;
              case Element.Water: retval = less; break;
            }
            break;
          }
        case Element.Fire:
          {
            switch (b)
            {
              case Element.Fire: retval = equal; break;
              case Element.Air: retval = greater; break;
              case Element.Water: retval = less; break;
            }
            break;
          }
        case Element.Water:
          {
            switch (b)
            {
              case Element.Water: retval = equal; break;
              case Element.Fire: retval = greater; break;
              case Element.Earth: retval = less; break;
            }
            break;
          }
      }

      return retval;
    }
  }
}
