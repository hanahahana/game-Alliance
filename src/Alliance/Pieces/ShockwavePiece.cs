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

    public override bool FaceTarget
    {
      get { return false; }
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      ShockwavePiece piece = new ShockwavePiece();
      return piece;
    }

    protected override Texture2D GetWeaponTower()
    {
      return AllianceGame.Textures["shockwaveGenerator"];
    }

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D wtower = GetWeaponTower();
      SizeF imgSize = new SizeF(wtower.Width, wtower.Height);
      SizeF actSize = new SizeF(bounds.Width, bounds.Height);

      Vector2 scale = Utils.ComputeScale(imgSize, actSize);
      Vector2 imgCenter = imgSize.ToVector2() * .5f;
      Vector2 myCenter = actSize.ToVector2() * .5f;

      if (!FaceTarget)
        mOrientation = 0;

      Color color = Color.Gray;
      spriteBatch.Draw(
        wtower,
        bounds.Location + myCenter,
        null,
        color,
        mOrientation,
        imgCenter,
        scale,
        SpriteEffects.None,
        0f);
    }

    protected override void DrawWeaponBase(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      // don't draw the weapon base
    }

    protected override Projectile CreateProjectile()
    {
      Projectile projectile = new Projectile(0.85);
      return projectile;
    }
  }
}
