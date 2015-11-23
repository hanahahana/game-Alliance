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
      mAttack = 800;
      mProjectilesPerSecond = .25f;
      mNumberProjectilesToFire = 3;
      mUpgradePercent = 5;
      mPrice = 50;
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

    protected override DrawData GetDrawData(Vector2 offset)
    {
      Tuple<BoxF, BoxF> outin = GetOutsideInsideBounds(offset);
      BoxF bounds = outin.First;
      BoxF inside = outin.Second;

      DrawData data = base.GetDrawData(offset);
      Vector2 scale = Utils.ComputeScale(data.TextureSize, bounds.Size);
      return new DrawData(data.Texture, data.TextureSize, bounds.Location, Vector2.Zero, scale);
    }
  }
}
