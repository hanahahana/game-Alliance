using System;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// The speed bump tower. It's meant to emulate a speed bump found in real life. TODO: This could be updated
  /// to make the invaders move when they go over it.
  /// </summary>
  [Serializable]
  public class SpeedBumpPiece : Piece
  {
    private const string SpeedBumpName = "Speed Bump";

    public SpeedBumpPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Slows the enemy down all while chucking up debris. Careful, some enemies aren't affected!");

      // set the properties of the piece
      Attack = 5;
      Price = 1;
      Radius = 0;
      UpgradePercent = 0;

      Description = sb.ToString();
      Level = Piece.MaxLevel;
      FaceTarget = false;
      IsBlocking = false;
      CanFireProjectiles = false;
      Name = SpeedBumpName;
      UltimateName = SpeedBumpName;
      Grouping = PieceGrouping.One;
      ImageKey = "speedbump";
      Specialty = PieceSpecialty.Ground;

      // set the price info
      mPriceAtLevels[Piece.MaxLevel - 1] = Price;
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      SpeedBumpPiece piece = new SpeedBumpPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      return null;
    }

    protected override void DrawBackground(DrawParams dparams, BoxF bounds, BoxF inside)
    {
      Color color = Selected ? Color.DarkGreen : Color.Beige;
      dparams.Graphics.FillRectangle(bounds, color);
    }

    protected override void DrawWeaponBase(DrawParams dparams, BoxF bounds, BoxF inside)
    {
      // draw a speed bump!
      AImage speedbump = GetImage();
      SizeF speedbumpSize = new SizeF(speedbump.Width, speedbump.Height);

      Vector2 scale = MathematicsHelper.ComputeScale(speedbumpSize, bounds.Size);
      Color color = Color.White;

      SpriteBatch spriteBatch = dparams.SpriteBatch;
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

    protected override void DrawWeaponTower(DrawParams dparams, Vector2 offset)
    {
      // don't draw a weapon tower
    }
  }
}
