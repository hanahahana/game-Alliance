using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Projectiles;
using Alliance.Data;
using Microsoft.Xna.Framework;

namespace Alliance.Pieces
{
  public class RailgunPiece : Piece
  {
    private const string MissileName = "Rail Gun";
    private string mDescription;
    private float mRadius;

    public RailgunPiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Fires an electrical pulse at the enemy. This can shoot very far, and is extremely powerful, but isn't very fast.");
      mDescription = sb.ToString();
      mRadius = 180;
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return MissileName; }
    }

    public override PieceGrouping Grouping
    {
      get { return PieceGrouping.Two; }
    }

    public override float Radius
    {
      get { return mRadius; }
    }

    public override float Attack
    {
      get { return 2500f; }
    }

    public override float ProjectilesPerSecond
    {
      get { return .09f; }
    }

    public override float ProjectileLifeSeconds
    {
      get { return 5.5f; }
    }

    public override int NumberProjectilesToFire
    {
      get { return 3; }
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      RailgunPiece piece = new RailgunPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      RailgunProjectile projectile = new RailgunProjectile(this, ProjectileLifeSeconds);
      return projectile;
    }

    protected override Texture2D GetTowerImage()
    {
      return AllianceGame.Textures["railgun"];
    }
  }
}
