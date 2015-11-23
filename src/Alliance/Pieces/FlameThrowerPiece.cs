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

namespace Alliance.Pieces
{
  public class FlameThrowerPiece : Piece
  {
    private const string FlameThrowerPieceName = "Flame Thrower";
    private const string UltimateFlameThrowerPieceName = "Flamewave";

    public FlameThrowerPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.Append("Ignites gasoline to produce a constant flame. Careful, some enemies can't be burned.");

      // set the properties of the piece
      mDescription = sb.ToString();
      mRadius = 70;
      mAttack = 9000000;
      mNumberProjectilesToFire = 1;
      mUpgradePercent = 20;
      mPrice = 10;
      mProjectilesPerSecond = 20;
      mProjectileLifeInSeconds = .1876f;
      mName = FlameThrowerPieceName;
      mUltimateName = UltimateFlameThrowerPieceName;
      mGrouping = PieceGrouping.Four;
    }

    protected override string ImageKey
    {
      get { return "flamethrower"; }
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      if (mLevel == MaxLevel)
      {
        mOrientation = Environment.TickCount / 1000f;
        mOrientation = Utils.WrapAngle(mOrientation);
      }
    }

    protected override void FinalizeUpgrade()
    {
      base.FinalizeUpgrade();
      if (mLevel == MaxLevel)
      {
        mOrientation = 0;
        mFaceTarget = false;

        mNumberProjectilesToFire = 1;
        mProjectileLifeInSeconds = DefaultProjectileLifeInSeconds * 5;
        mRadius = 100;

        mProjectilesPerSecond = 10;
        mProjectileLifeInSeconds = 1.1676f;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Rotates to gain energy and releases a VERY powerful flame wave. Careful, some enemies can't be burned.");
        mDescription = sb.ToString();
      }
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      // don't uprade the variables
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      FlameThrowerPiece piece = new FlameThrowerPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      Projectile projectile = null;
      if (mLevel == MaxLevel)
      {
        BoxF bounds = new BoxF(this.Position, this.Size);
        projectile = new FlamewaveProjectile(bounds, mProjectileLifeInSeconds);
        projectile.Size = new SizeF(Width, Height);
      }
      else
      {
        float extra = mRadius / 2f;
        projectile = new FlameProjectile(mProjectileLifeInSeconds);
        projectile.Size = new SizeF(projectile.Width + extra, projectile.Height + extra);
      }
      return projectile;
    }
  }
}
