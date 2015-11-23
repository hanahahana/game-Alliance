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
    private string mDescription;
    private float mRadius;

    public MissilePiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Slams multiple missiles into the enemy. They explode on contact and affect the surrounding pieces.");
      sb.AppendLine(" Missiles are best with flying enemies but can sometimes damage friendly units.");
      mDescription = sb.ToString();
      mRadius = 80;
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

    protected override Piece CreatePiece(Cell[] cells)
    {
      MissilePiece piece = new MissilePiece();
      return piece;
    }

    protected override Texture2D GetWeaponTower()
    {
      return AllianceGame.Textures["missileLauncher"];
    }

    protected override Projectile CreateProjectile()
    {
      SizeF size = new SizeF(8, 4);
      Projectile projectile = new Projectile(0.85);
      projectile.Bounds = new BoxF(mPosition + new Vector2(Delta, 0) + (mSize.ToVector2() * .5f), size);
      projectile.Color = Color.Gray;
      return projectile;
    }
  }
}
