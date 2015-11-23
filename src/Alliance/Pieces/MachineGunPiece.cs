using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Projectiles;
using Alliance.Data;
using Alliance.Helpers;

namespace Alliance.Pieces
{
  public class MachineGunPiece : Piece
  {
    private const string MachineGunName = "Machine Gun";
    private const string UltimateMachineGunName = "Destroyer";

    private string mDescription;
    private float mRadius;
    private float mAttack;
    private int mPrice;
    private int mUpgradePercent;

    public MachineGunPiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Fires a constant stream of bullets at the enemy. These are very very weak, but very very fast.");
      mDescription = sb.ToString();

      mRadius = 100;
      mAttack = 20;

      mNumberProjectilesToFire = 2;
      mUpgradePercent = 20;
      mPrice = 10;

      mProjectilesPerSecond = 15;
      mProjectileLifeInSeconds = 3.4567f;
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return MachineGunName; }
    }

    public override string UltimateName
    {
      get { return UltimateMachineGunName; }
    }

    public override PieceGrouping Grouping
    {
      get { return PieceGrouping.Two; }
    }

    public override float Radius
    {
      get { return mRadius; }
      protected set { mRadius = value; }
    }

    public override float Attack
    {
      get { return mAttack; }
      protected set { mAttack = value; }
    }

    public override int Price
    {
      get { return mPrice; }
      protected set { mPrice = value; }
    }

    public override int UpgradePercent
    {
      get { return mUpgradePercent; }
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

    protected override Piece CreatePiece(Cell[] cells)
    {
      MachineGunPiece piece = new MachineGunPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      BulletProjectile projectile = new BulletProjectile(mProjectileLifeInSeconds);
      return projectile;
    }

    protected override Texture2D GetTowerImage()
    {
      return AllianceGame.Textures["machinegun"];
    }
  }
}
