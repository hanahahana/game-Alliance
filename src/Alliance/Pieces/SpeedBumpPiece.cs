using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Data;
using Alliance.Projectiles;
using Microsoft.Xna.Framework;
using Alliance.Utilities;

namespace Alliance.Pieces
{
  public class SpeedBumpPiece : Piece
  {
    private const string SpeedBumpName = "Speed Bump";
    private string mDescription;
    private float mRadius;

    public SpeedBumpPiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Slows the enemy down by threatening to destroy their transportation.");
      sb.AppendLine(" Careful, some enemies can roll right over them without feeling anything!");
      mDescription = sb.ToString();
      mLevel = Piece.MaxLevel;
      mRadius = 20;
    }

    public override bool IsBlocking
    {
      get { return false; }
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return SpeedBumpName; }
    }

    public override PieceGrouping Grouping
    {
      get { return PieceGrouping.One; }
    }

    public override float Radius
    {
      get { return mRadius; }
    }

    public override float Attack
    {
      get { return .5f; }
    }

    public override bool FaceTarget
    {
      get { return false; }
    }

    protected override bool CanFireProjectiles
    {
      get { return false; }
    }

    protected override Piece CreatePiece(Cell[] cells)
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
      Texture2D speedbump = AllianceGame.Textures["speedbump"];
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

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      // don't draw a weapon tower
    }
  }
}
