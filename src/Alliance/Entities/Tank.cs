using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MLA.Utilities.Helpers;

namespace Alliance.Entities
{
  public class Tank : Entity
  {
    public Tank(Cell startCell, Cell goalCell)
      : base(startCell, goalCell)
    {
      mMPS = MathHelper.Lerp(MinMovementPerSecond, MaxMovementPerSecond, RandomHelper.NextSingle());
    }
  }
}
