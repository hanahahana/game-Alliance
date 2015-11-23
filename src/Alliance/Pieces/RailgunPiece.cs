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
  public class RailgunPiece : Piece
  {
    private const string RailgunName = "Rail Gun";
    private const string UltimateRailgunName = "Annihilator";

    private string mDescription;
    private float mRadius;
    private float mAttack;
    private int mPrice;
    private int mUpgradePercent;

    public RailgunPiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Fires an electrical pulse at the enemy. This can shoot very far, and is extremely powerful, but isn't very fast.");
      mDescription = sb.ToString();

      mRadius = 180;
      mAttack = 2500f;
      mProjectilesPerSecond = .5f;
      mNumberProjectilesToFire = 3;
      mProjectileLifeInSeconds = 5.5f;
      mUpgradePercent = 15;
      mPrice = 250;
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return RailgunName; }
    }

    public override string UltimateName
    {
      get { return UltimateRailgunName; }
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

    protected override Piece CreatePiece(Cell[] cells)
    {
      RailgunPiece piece = new RailgunPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      RailgunProjectile projectile = new RailgunProjectile(ProjectileLifeSeconds);
      return projectile;
    }

    protected override Texture2D GetTowerImage()
    {
      return AllianceGame.Textures["railgun"];
    }
  }
}
