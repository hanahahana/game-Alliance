using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Invaders;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Objects;
using MLA.Utilities;

namespace Alliance.Pieces
{
  public class SprinklerPiece : Piece
  {
    private const string SprinklerName = "Sprinkler";
    private const string UltimateSprinklerName = "Sprayer";

    private const float MinVelocity = DefaultProjectileVelocity;
    private const float MaxVelocity = DefaultProjectileVelocity * 3;

    private const float MinPieSlices = 2f;
    private const float MaxPieSlices = 16f;

    private const float RadiansPerSecond = 5.5f;
    private const float TotalSeconds = MathHelper.TwoPi / RadiansPerSecond;

    private float mPiePieces;

    public SprinklerPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Sprinkles projectiles around (without aiming). Good for closing up holes in your maze.");

      // set the properties of the piece
      mDescription = sb.ToString();
      mRadius = 20;
      mAttack = 25;
      mPiePieces = MinPieSlices;
      mProjectilesPerSecond = TotalSeconds * mPiePieces;
      mNumberProjectilesToFire = 1;
      mUpgradePercent = 15;
      mPrice = 5;
      mFaceTarget = false;
      mName = SprinklerName;
      mUltimateName = UltimateSprinklerName;
      mGrouping = PieceGrouping.One;
    }

    public override void Update(GameTime gameTime)
    {
      // let the base do it's thing
      base.Update(gameTime);

      // spin around in a circle
      mOrientation += ((float)gameTime.ElapsedGameTime.TotalSeconds * RadiansPerSecond);
      Utils.WrapAngle(mOrientation);
    }

    protected override float UpgradeAttack(float factor)
    {
      if (mLevel == MaxLevel)
      {
        return Invader.MaxInvaderLife * 5f;
      }
      else
      {
        return base.UpgradeAttack(factor);
      }
    }

    protected override double UpgradePrice(float factor)
    {
      if (mLevel == MaxLevel - 1)
      {
        // set the price to be outrageous
        return int.MaxValue;
      }
      else
      {
        return base.UpgradePrice(factor);
      }
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      base.UpgradeProjectileVariables(factor);

      float mu = (float)mLevel / (float)MaxLevel;
      mUpgradePercent = (int)Math.Round(mUpgradePercent * factor * 2f);

      mProjectileVelocity = MathHelper.Lerp(MinVelocity, MaxVelocity, mu);
      mPiePieces = MathHelper.Lerp(MinPieSlices, MaxPieSlices, mu);

      mNumberProjectilesToFire = 1;
      mProjectilesPerSecond = TotalSeconds * mPiePieces;
    }

    protected override void FinalizeUpgrade()
    {
      base.FinalizeUpgrade();
      if (mLevel == MaxLevel)
      {
        // set the description
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Extremely powerful and fast sprayer. Extremely expensive too...nice job!");
        mDescription = sb.ToString();
      }
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      SprinklerPiece piece = new SprinklerPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      SprinklerProjectile projectile = new SprinklerProjectile(mProjectileLifeInSeconds);
      return projectile;
    }

    protected override string ImageKey
    {
      get { return "sprinkler"; }
    }

    protected override DrawData GetDrawData(Vector2 offset)
    {
      Tuple<BoxF, BoxF> outin = GetOutsideInsideBounds(offset);
      BoxF bounds = outin.First;

      Texture2D wtower = GetImage();
      SizeF imgSize = new SizeF(wtower.Width, wtower.Height);
      SizeF actSize = new SizeF(bounds.Width, bounds.Height);

      Vector2 scale = Utils.ComputeScale(imgSize, actSize);
      Vector2 origin = imgSize.ToVector2() * .5f;
      Vector2 center = actSize.ToVector2() * .5f;

      return new DrawData(wtower, imgSize, bounds.Location + center, origin, scale);
    }
  }
}
