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
  public class MachineGunPiece : Piece
  {
    private const string MachineGunName = "Machine Gun";
    private const string UltimateMachineGunName = "Destroyer";

    public MachineGunPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.Append("Fires a constant stream of bullets at the enemy. These are very weak, but very fast. These also turn very slow...");

      // set the properties of the piece
      mDescription = sb.ToString();
      mRadius = 100;
      mAttack = 20;
      mNumberProjectilesToFire = 2;
      mUpgradePercent = 20;
      mPrice = 10;
      mProjectilesPerSecond = 15;
      mProjectileLifeInSeconds = 3.4567f;
      mName = MachineGunName;
      mUltimateName = UltimateMachineGunName;
    }

    protected override void FinalizeUpgrade()
    {
      base.FinalizeUpgrade();
      if (mLevel == MaxLevel - 1)
      {
        mUpgradePercent = 20000;
      }

      if (mLevel == MaxLevel)
      {
        mRadius = 250;
      }
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      // don't upgrade the projectile variables
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      MachineGunPiece piece = new MachineGunPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      BulletProjectile projectile = new BulletProjectile(mProjectileLifeInSeconds);
      return projectile;
    }

    protected override string ImageKey
    {
      get { return "machinegun"; }
    }
  }
}
