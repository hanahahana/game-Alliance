using System;
using System.Text;
using GraphicsSystem;

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

    protected override void DrawBackground(DrawParams dparams, GsRectangle bounds, GsRectangle inside)
    {
      GsColor color = Selected ? GsColor.DarkGreen : GsColor.Beige;
      dparams.Graphics.FillRectangle(color, bounds);
    }

    protected override void DrawWeaponBase(DrawParams dparams, GsRectangle bounds, GsRectangle inside)
    {
      // draw a speed bump!
      var speedbump = GetImage();
      var speedbumpSize = ImageProvider.GetSize(speedbump);

      var scale = Calculator.ComputeScale(speedbumpSize, bounds.Size);
      GsColor color = GsColor.White;
      var graphics = dparams.Graphics;
      graphics.DrawImage(speedbump, color, bounds.Location, scale);
    }

    protected override void DrawWeaponTower(DrawParams dparams, GsVector offset)
    {
      // don't draw a weapon tower
    }
  }
}
