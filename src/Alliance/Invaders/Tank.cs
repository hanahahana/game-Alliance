using System;
using System.Collections.Generic;
using System.Text;

using MLA.Utilities.Helpers;

using Microsoft.Xna.Framework;

using Alliance.Utilities;
using Alliance.Objects;

namespace Alliance.Invaders
{
  public class Tank : Invader
  {
    private InvaderAttributes mAttributes;
    private readonly float mMaximumLife;
    private double mValue;

    public override float MaximumLife
    {
      get { return mMaximumLife; }
    }

    public override InvaderAttributes Attributes
    {
      get { return mAttributes; }
      set { mAttributes = value; }
    }

    public override double Value
    {
      get { return mValue; }
    }

    public Tank(GridCell startCell, GridCell goalCell, DijkstraType dijkstraType)
      : base(startCell, goalCell, dijkstraType)
    {
      // set the attributes
      mAttributes = InvaderAttributes.None;
      if (RandomHelper.NextRareBool())
      {
        mAttributes |= InvaderAttributes.SpeedBumpResistant;
      }
      if (RandomHelper.NextRareBool())
      {
        mAttributes |= InvaderAttributes.FireResistant;
      }

      // determine the level
      mLevel = RandomHelper.Next(MaxInvaderLevel) + 1;

      // interpolate the varaibles
      float mu = ((float)mLevel / (float)MaxInvaderLevel);
      mMaximumLife = (float)Math.Round(MathHelper.SmoothStep(MinInvaderLife, MaxInvaderLife, mu));
      mMPS = (float)Math.Round(MathHelper.SmoothStep(MinMovementPerSecond, MaxMovementPerSecond, mu));

      // set the remaining variables
      mCurrentLife = MaximumLife;
      mValue = Math.Round((double)mLevel * 1.23456789);
    }
  }
}
