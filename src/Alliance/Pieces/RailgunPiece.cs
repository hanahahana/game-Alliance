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

    protected override float ProjectilesPerSecond
    {
      get { return .4f; }
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      RailgunPiece piece = new RailgunPiece();
      return piece;
    }

    protected override Texture2D GetWeaponTower()
    {
      return AllianceGame.Textures["railgun"];
    }

    protected override Projectile CreateProjectile()
    {
      SizeF size = new SizeF(30, 4);
      Projectile projectile = new Projectile(5.85);
      projectile.Bounds = new BoxF(mPosition + (mSize.ToVector2() * .5f) - (size.ToVector2() * .5f), size);
      projectile.Color = Color.DarkRed;
      return projectile;
    }
  }
}
