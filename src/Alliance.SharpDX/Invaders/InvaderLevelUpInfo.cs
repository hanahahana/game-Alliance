using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alliance
{
  public class InvaderLevelUpInfo
  {
    public IDictionary<Element, int> ElementCounts { get; private set; }
    public int TotalPieceCount { get; private set; }

    public int InvadersMadeItCount { get; private set; }
    public int InvadersDestroyedCount { get; private set; }
    public int TotalInvadersSentCount { get; private set; }

    public InvaderLevelUpInfo(IEnumerable<Piece> pieces, int madeItCount, int destroyedCount)
    {
      InvadersMadeItCount = madeItCount;
      InvadersDestroyedCount = destroyedCount;
      TotalInvadersSentCount = madeItCount + destroyedCount;

      ElementCounts = new Dictionary<Element, int>();
      ElementCounts[Element.Fire] = pieces.Count(piece => piece.IsFire);
      ElementCounts[Element.Water] = pieces.Count(piece => piece.IsWater);
      ElementCounts[Element.Earth] = pieces.Count(piece => piece.IsEarth);
      ElementCounts[Element.Air] = pieces.Count(piece => piece.IsAir);
      ElementCounts[Element.Electricity] = pieces.Count(piece => piece.IsElectricity);
      TotalPieceCount = pieces.Count();
    }
  }
}
