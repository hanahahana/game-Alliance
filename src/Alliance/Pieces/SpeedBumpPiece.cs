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

namespace Alliance.Pieces
{
  public class SpeedBumpPiece : Piece
  {
    private const string SpeedBumpName = "Speed Bump";

    protected override string ImageKey
    {
      get { return "speedbump"; }
    }

    public SpeedBumpPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Slows the enemy down all while chucking up debri. Careful, some enemies aren't affected!");

      // set the properties of the piece
      mDescription = sb.ToString();
      mPrice = 1;
      mLevel = Piece.MaxLevel;
      mAttack = 2;
      mPriceAtLevels[Piece.MaxLevel - 1] = mPrice;
      mUpgradePercent = 0;
      mFaceTarget = false;
      mIsBlocking = false;
      mCanFireProjectiles = false;
      mName = SpeedBumpName;
      mUltimateName = SpeedBumpName;
      mGrouping = PieceGrouping.One;
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      SpeedBumpPiece piece = new SpeedBumpPiece();
      return piece;
    }

    protected override void DrawBackground(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Color color = mSelected ? Color.DarkGreen : Color.Beige;
      Shapes.FillRectangle(spriteBatch, bounds, color);
    }

    protected override void DrawWeaponBase(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      // draw a speed bump!
      Texture2D speedbump = GetImage();
      SizeF speedbumpSize = new SizeF(speedbump.Width, speedbump.Height);

      Vector2 scale = Utils.ComputeScale(speedbumpSize, bounds.Size);
      Color color = Color.White;

      spriteBatch.Draw(
        speedbump,
        bounds.Location,
        null,
        color,
        0f,
        Vector2.Zero,
        scale,
        SpriteEffects.None,
        0f);
    }

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, Vector2 offset)
    {
      // don't draw a weapon tower
    }
  }
}
