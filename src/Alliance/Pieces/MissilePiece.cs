using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Projectiles;
using Alliance.Data;
using Microsoft.Xna.Framework;

namespace Alliance.Pieces
{
  public class MissilePiece : Piece
  {
    private const string MissileName = "Missile";
    private const string UltimateMissileName = "Missile Blaster";

    private string mDescription;
    private float mRadius;
    private float mAttack;
    private int mUpgradePercent;

    public MissilePiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Slams multiple missiles into the enemy. They explode on contact and affect the surrounding pieces.");
      sb.AppendLine(" Missiles are best with flying enemies.");
      mDescription = sb.ToString();

      mRadius = 80;
      mAttack = 50f;

      mNumberProjectilesToFire = 5;
      mUpgradePercent = 15;
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return MissileName; }
    }

    public override string UltimateName
    {
      get { return UltimateMissileName; }
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

    public override int UpgradePercent
    {
      get { return mUpgradePercent; }
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      MissilePiece piece = new MissilePiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      MissileProjectile projectile = new MissileProjectile(base.ProjectileLifeSeconds);
      return projectile;
    }

    protected override Texture2D GetTowerImage()
    {
      return AllianceGame.Textures["missileLauncher"];
    }
  }
}
