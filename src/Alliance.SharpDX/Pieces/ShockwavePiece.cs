using System;
using System.Text;
using SharpDX;

namespace Alliance
{
  /// <summary>
  /// The shockwave tower. It's meant to emulate sending out a shockwave when invaders are in range.
  /// </summary>
  [Serializable]
  public class ShockwavePiece : Piece
  {
    private const string ShockwaveName = "Shockwave";
    private const string UltimateShockwaveName = "Earthquake";

    public ShockwavePiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Creates a shockwave to send at the enemy! The shockwave can't hit flying invaders!");

      // set the properties of the piece
      Attack = 2000;
      Price = 600;
      Radius = 100;
      UpgradePercent = 15;
      LevelVisibility = 20;

      Description = sb.ToString();
      ProjectilesPerSecond = .1f;
      NumberProjectilesToFire = 1;
      FaceTarget = false;
      Name = ShockwaveName;
      UltimateName = UltimateShockwaveName;
      ImageKey = "shockwaveGenerator";
      ProjectileLifeInSeconds = .8f;
      Element = Element.Earth;
      Specialty = PieceSpecialty.Ground;
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      ShockwavePiece piece = new ShockwavePiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      BoxF bounds = new BoxF(this.Position, this.Size);
      ShockwaveProjectile projectile = new ShockwaveProjectile(this, bounds, ProjectileLifeInSeconds);
      //projectile.Size = new SizeF(Width * .25f, Height * .25f);
      projectile.Size = new SizeF(Radius * 2f, Radius * 2f);
      return projectile;
    }

    protected override float UpgradeRadius(float factor)
    {
      // no need to upgrade the radius
      return Radius;
    }

    protected override void DrawWeaponBase(DrawParams dparams, BoxF bounds, BoxF inside)
    {
      // don't draw the weapon base
    }

    protected override TextureDrawData GetTextureDrawData(Vector2 offset)
    {
      Tuple<BoxF, BoxF> outin = GetOutsideInsideBounds(offset);
      BoxF bounds = outin.First;
      BoxF inside = outin.Second;

      TextureDrawData data = base.GetTextureDrawData(offset);
      Vector2 scale = MathHelper.ComputeScale(data.TextureSize, bounds.Size);
      return new TextureDrawData(data.Texture, data.TextureSize, bounds.Location, Vector2.Zero, scale);
    }
  }
}
