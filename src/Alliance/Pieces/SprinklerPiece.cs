using System;
using System.Text;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The sprinkler tower. It's meant to emulate firing a round of bullets in a circle.
  /// </summary>
  [Serializable]
  public class SprinklerPiece : Piece
  {
    private const string SprinklerName = "Sprinkler";
    private const string UltimateSprinklerName = "Sprayer";

    private const float MinVelocity = DefaultProjectileSpeed;
    private const float MaxVelocity = DefaultProjectileSpeed * 3;

    private const float MinPieSlices = 2f;
    private const float MaxPieSlices = 16f;

    private const float RadiansPerSecond = 5.5f;
    private const float TotalSeconds = GsMath.TwoPi / RadiansPerSecond;

    private float mPiePieces;

    public SprinklerPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Sprinkles projectiles around (without aiming). Good for closing up holes in your maze.");

      // set the properties of the piece
      Attack = 1000;
      Price = 600;
      Radius = 100;
      UpgradePercent = 50;
      LevelVisibility = 50;

      mPiePieces = MinPieSlices;
      Description = sb.ToString();
      ProjectilesPerSecond = TotalSeconds * mPiePieces;
      NumberProjectilesToFire = 1;
      FaceTarget = false;
      Name = SprinklerName;
      UltimateName = UltimateSprinklerName;
      Grouping = PieceGrouping.One;
      ImageKey = "sprinkler";
      Specialty = PieceSpecialty.Both;
    }

    public override void Update(TimeSpan elapsed)
    {
      // let the base do it's thing
      base.Update(elapsed);

      // spin around in a circle
      Orientation += ((float)elapsed.TotalSeconds * RadiansPerSecond);
      Orientation = GsMath.WrapAngle(Orientation);
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      base.UpgradeProjectileVariables(factor);
      float mu = (float)Level / (float)MaxLevel;

      ProjectileSpeed = GsMath.Lerp(MinVelocity, MaxVelocity, mu);
      mPiePieces = GsMath.Lerp(MinPieSlices, MaxPieSlices, mu);

      NumberProjectilesToFire = 1;
      ProjectilesPerSecond = TotalSeconds * mPiePieces;
    }

    protected override void FinalizeUpgrade()
    {
      base.FinalizeUpgrade();
      if (Level == MaxLevel)
      {
        // set the description
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Extremely powerful and fast sprayer. Extremely expensive too...nice job!");
        Description = sb.ToString();
      }
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      SprinklerPiece piece = new SprinklerPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      SprinklerProjectile projectile = new SprinklerProjectile(this, ProjectileLifeInSeconds);
      return projectile;
    }

    protected override ImageParams GetTextureDrawData(GsVector offset)
    {
      var outin = GetOutsideInsideBounds(offset);
      var bounds = outin.Outside;

      var wtower = GetImage();
      var imgSize = ImageProvider.GetSize(wtower);
      var actSize = new GsSize(bounds.Width, bounds.Height);

      GsVector scale = Calculator.ComputeScale(imgSize, actSize);
      GsVector origin = imgSize.ToVector() * .5f;
      GsVector center = actSize.ToVector() * .5f;

      return new ImageParams(wtower, imgSize, bounds.Location + center, origin, scale);
    }
  }
}
