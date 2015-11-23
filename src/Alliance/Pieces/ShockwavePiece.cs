using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Data;
using Alliance.Utilities;
using Alliance.Projectiles;

namespace Alliance.Pieces
{
  public class ShockwavePiece : Piece
  {
    private const string ShockwaveName = "Shockwave";
    private string mDescription;
    private float mRadius;

    public ShockwavePiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Creates a shockwave to send at the enemy!");
      mDescription = sb.ToString();
      mRadius = 50;
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return ShockwaveName; }
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
      get { return 10000f; }
    }

    public override bool FaceTarget
    {
      get { return false; }
    }

    public override float ProjectilesPerSecond
    {
      get { return .25f; }
    }

    public override int NumberProjectilesToFire
    {
      get { return 3; }
    }

    protected override Piece CreatePiece(Cell[] cells)
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

    protected override Texture2D GetTowerImage()
    {
      return AllianceGame.Textures["shockwaveGenerator"];
    }

    protected override void DrawWeaponBase(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      // don't draw the weapon base
    }

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D tower = GetTowerImage();
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
