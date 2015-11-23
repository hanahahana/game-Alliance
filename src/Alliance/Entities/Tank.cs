using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MLA.Utilities.Helpers;
using Alliance.Utilities;

namespace Alliance.Entities
{
  public class Tank : Entity
  {
    private EntityAttributes mAttributes;
    private readonly float mMaximumLife;
    private int mCash;

    public override float MaximumLife
    {
      get { return mMaximumLife; }
    }

    public override EntityAttributes Attributes
    {
      get { return mAttributes; }
      set { mAttributes = value; }
    }

    public override int Cash
    {
      get { return mCash; }
    }

    public Tank(Cell startCell, Cell goalCell)
      : base(startCell, goalCell)
    {
      // set the attributes
      mAttributes = RandomHelper.NextRareBool() ? EntityAttributes.SpeedBumpNoAffect : EntityAttributes.None;

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
