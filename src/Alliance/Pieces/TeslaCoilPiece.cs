using System;
using System.Text;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The tesla coil tower. It's meant to emulate firing a charged blast of electricity.
  /// </summary>
  [Serializable]
  public class TeslaCoilPiece : Piece
  {
    private const string TeslaCoilName = "Tesla Coil";
    private const string UltimateTeslaCoilName = "Discharger";

    private const int NumberIndexFrames = 4;
    private const float SecondsPerFrame = 1f / 5.3456789f;
    private const float SecondsToStayOpen = 1f / 2.5f;

    private readonly GsSize FrameSize;
    private readonly GsColor LightningColor;

    private int mIndex;
    private float mAggregateTimeSinceUpdate;
    private TeslaState mTeslaState;

    public TeslaCoilPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Fires a charged burst of electricity. The burst is very effective against ground units.");

      // set the properties needed
      Attack = 9000;
      Price = 2500;
      Radius = 250;
      UpgradePercent = 45;
      LevelVisibility = 75;

      Description = sb.ToString();
      ProjectileLifeInSeconds = 6.7f;
      CanFireProjectiles = false;
      Name = TeslaCoilName;
      UltimateName = UltimateTeslaCoilName;
      Grouping = PieceGrouping.Three;
      ImageKey = "teslaCoil";
      Element = Element.Electricity;
      Specialty = PieceSpecialty.Both;

      // set the properties of the piece
      mIndex = 0;
      mAggregateTimeSinceUpdate = 0;
      mTeslaState = TeslaState.Idle;
      Color = GsColor.White;

      // get the image
      var image = GetImage();
      var imageSize = ImageProvider.GetSize(image);
      FrameSize = new GsSize(imageSize.Width / (NumberIndexFrames + 1), imageSize.Height);
      LightningColor = GsMath.Lerp(GsColor.Purple, GsColor.DarkBlue, .5f);
      LightningColor = GsMath.Lerp(LightningColor, GsColor.Red, .75f);
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      TeslaCoilPiece piece = new TeslaCoilPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      LightningProjectile projectile = new LightningProjectile(this, ProjectileLifeInSeconds, Target);
      projectile.Color = LightningColor;
      return projectile;
    }

    public override GsImage GetDisplayImage()
    {
      return ImageProvider.GetAnimatedImage(ImageKey)[0].Texture;
    }

    protected override GsVector[] GetImageHull()
    {
      return ImageProvider.GetAnimatedImage(ImageKey)[0].Hull;
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      base.UpgradeProjectileVariables(factor);
      ++ProjectilesPerSecond;

      ProjectileSpeed = DefaultProjectileSpeed;
      ++NumberProjectilesToFire;
    }

    public override void Update(TimeSpan elapsed)
    {
      // let the base update
      base.Update(elapsed);

      // if we're idle, then we COULD be firing
      if (State == PieceState.Idle)
      {
        UpdateTeslaState(elapsed);
      }
      else
      {
        mTeslaState = TeslaState.Idle;
        mIndex = 0;
      }

      float factor = ((float)mIndex) / ((float)NumberIndexFrames);
      Color = GsMath.Lerp(GsColor.Beige, LightningColor, factor);
    }

    private void UpdateTeslaState(TimeSpan elapsed)
    {
      switch (mTeslaState)
      {
        case TeslaState.Idle:
          {
            UpdateTeslaIdleState(elapsed);
            break;
          }
        case TeslaState.Charging:
          {
            UpdateTeslaChargingState(elapsed);
            break;
          }
        case TeslaState.Firing:
          {
            UpdateTeslaFiringState(elapsed);
            break;
          }
        case TeslaState.Discharging:
          {
            UpdateTeslaDischaringState(elapsed);
            break;
          }
      }
    }

    private void UpdateTeslaDischaringState(TimeSpan elapsed)
    {
      // here, we backtrack until we get to the beginning
      mAggregateTimeSinceUpdate += (float)elapsed.TotalSeconds;
      if (mAggregateTimeSinceUpdate >= SecondsPerFrame)
      {
        mAggregateTimeSinceUpdate -= SecondsPerFrame;
        --mIndex;

        if (mIndex == 0)
        {
          mTeslaState = TeslaState.Idle;
        }
      }
    }

    private void UpdateTeslaFiringState(TimeSpan elapsed)
    {
      // we stay open for a certain amount of time before discharging.
      mAggregateTimeSinceUpdate += (float)elapsed.TotalSeconds;
      if (mAggregateTimeSinceUpdate >= SecondsToStayOpen)
      {
        mAggregateTimeSinceUpdate -= SecondsToStayOpen;
        mTeslaState = TeslaState.Discharging;
      }
    }

    private void UpdateTeslaChargingState(TimeSpan elapsed)
    {
      mAggregateTimeSinceUpdate += (float)elapsed.TotalSeconds;
      if (mAggregateTimeSinceUpdate >= SecondsPerFrame)
      {
        mAggregateTimeSinceUpdate -= SecondsPerFrame;
        ++mIndex;

        if (mIndex == NumberIndexFrames)
        {
          FireProjectile(elapsed);
          mTeslaState = TeslaState.Firing;
        }
      }
    }

    private void UpdateTeslaIdleState(TimeSpan elapsed)
    {
      if (Target != null)
      {
        mTeslaState = TeslaState.Charging;
      }
    }

    protected override TextureParams GetTextureDrawData(GsVector offset)
    {
      var outin = GetOutsideInsideBounds(offset);
      var bounds = outin.Outside;
      var inside = outin.Inside;

      var wtower = GetImage();
      GsSize imgSize = new GsSize(FrameSize.Width, FrameSize.Height);
      GsSize actSize = new GsSize(bounds.Width, bounds.Height);

      GsVector scale = Calculator.ComputeScale(imgSize, actSize);
      GsVector origin = imgSize.ToVector() * .5f;
      GsVector center = actSize.ToVector() * .5f;

      return new TextureParams(wtower, imgSize, bounds.Location + center, origin, scale);
    }

    protected override void DrawWeaponTower(DrawParams dparams, GsVector offset)
    {
      TextureParams data = GetTextureDrawData(offset);
      GsRectangle source = new GsRectangle(
        mIndex * FrameSize.Width, 0,
        FrameSize.Width, FrameSize.Height);
      var graphics = dparams.Graphics;
      graphics.DrawImage(data, Color, source);
    }
  }
}
