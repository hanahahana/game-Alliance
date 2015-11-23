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
    private int mCash;

    public override float MaximumLife
    {
      get { return mMaximumLife; }
    }

    public override InvaderAttributes Attributes
    {
      get { return mAttributes; }
      set { mAttributes = value; }
    }

    public override int Cash
    {
      get { return mCash; }
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
      mLevel = RandomHelper.Next(MaxEntityLevel) + 1;

      // interpolate the varaibles
      if (mLevel > MinEntityLevel)
      {
        mMaximumLife = Utils.SpLine(LvlLfeSamples, mLevel);
        mMPS = Utils.SpLine(LvlMpsSamples, mLevel);
      }
      else
      {
        mMaximumLife = MinEntityLife;
        mMPS = MinMovementPerSecond;
      }

      // set the remaining variables
      mCurrentLife = MaximumLife;
      mCash = (int)Math.Round((float)mLevel * 1.5f);
    }
  }
}
