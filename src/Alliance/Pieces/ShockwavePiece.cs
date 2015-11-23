using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Entities;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Objects;

namespace Alliance.Pieces
{
  public class ShockwavePiece : Piece
  {
    private const string ShockwaveName = "Shockwave";
    private const string UltimateShockwaveName = "Earthquake";

    public ShockwavePiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Creates a shockwave to send at the enemy! The shockwave grows as time passes.");

      // set the properties of the piece
      mDescription = sb.ToString();
      mRadius = 50;
      mAttack = 10000f;
      mProjectilesPerSecond = .25f;
      mNumberProjectilesToFire = 3;
      mUpgradePercent = 15;
      mPrice = 540;
      mFaceTarget = false;
      mName = ShockwaveName;
      mUltimateName = UltimateShockwaveName;
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      ShockwavePiece piece = new ShockwavePiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      BoxF bounds = new BoxF(this.Position, this.Size);
      ShockwaveProjectile projectile = new ShockwaveProjectile(bounds, 1.0);
      projectile.Size = new SizeF(Width * .25f, Height * .25f);
      return projectile;
    }

    protected override string ImageKey
    {
      get { return "shockwaveGenerator"; }
    }

    protected override void DrawWeaponBase(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      // don't draw the weapon base
    }

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D tower = GetImage();
      Vector2 scale = Utils.ComputeScale(new SizeF(tower.Width, tower.Height), bounds.Size);

      spriteBatch.Draw(
        tower,
        bounds.Location,
        null,
        Color.White,
        0f,
        Vector2.Zero,
        scale,
        SpriteEffects.None,
        0f);
    }
  }
}
