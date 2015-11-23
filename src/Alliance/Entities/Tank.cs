using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MLA.Utilities.Helpers;

namespace Alliance.Entities
{
  public class Tank : Entity
  {
    private EntityAttributes mAttributes;
    private readonly float mMaximumLife;

    public override float MaximumLife
    {
      get { return mMaximumLife; }
    }

    public override EntityAttributes Attributes
    {
      get { return mAttributes; }
      set { mAttributes = value; }
    }

    public Tank(Cell startCell, Cell goalCell)
      : base(startCell, goalCell)
    {
      mMPS = MathHelper.Lerp(MinMovementPerSecond, MaxMovementPerSecond, RandomHelper.NextSingle());
      mAttributes = RandomHelper.NextRareBool() ? EntityAttributes.SpeedBumpNoAffect : EntityAttributes.None;
      mMaximumLife = MathHelper.Lerp(5000f, 5000000f, RandomHelper.NextSingle());
      mCurrentLife = MaximumLife;
    }
  }
}
