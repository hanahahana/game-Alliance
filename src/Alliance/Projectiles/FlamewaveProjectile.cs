using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the flame thrower tower.
  /// </summary>
  [Serializable]
  public class FlamewaveProjectile : Projectile
  {
    const float ScalesPerSecond = 1.5f;
    const float RotationsPerSecond = 24.567f;

    private GsRectangle mOwnerBounds;
    private float mScale = 1.0f;

    public FlamewaveProjectile(Piece parent, GsRectangle ownerBounds, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      mOwnerBounds = ownerBounds;
      Color = GsColor.White;
      ImageKey = "flamewave";
      StayAlive = true;
    }

    public override void Update(TimeSpan elapsed)
    {
      // update the time to live
      base.UpdateTimeToLive(elapsed);

      // if we're still alive, then update out
      if (IsAlive)
      {
        UpdateScaleAndOrientation((float)elapsed.TotalSeconds);
      }
    }

    protected void UpdateScaleAndOrientation(float elapsedSeconds)
    {
      mScale += elapsedSeconds * ScalesPerSecond;
      Orientation += elapsedSeconds * RotationsPerSecond;
      Size = new GsSize(mOwnerBounds.Width * mScale, mOwnerBounds.Height * mScale);
    }

    public override void UpdateByFrameCount(TimeSpan elapsed, int frameCount)
    {
      float time = (float)(elapsed.TotalSeconds * ((frameCount + 1.0) * 4.0) * .3);
      UpdateScaleAndOrientation(time);
    }

    protected override ImageParams GetTextureDrawData(GsVector offset)
    {
      var projectile = GetImage();
      var projectileSize = ImageProvider.GetSize(projectile);

      var origin = projectileSize.ToVector() * .5f;
      var scale = Calculator.ComputeScale(projectileSize, Size);

      var position = mOwnerBounds.Location;
      position += ((mOwnerBounds.Size.ToVector() * .5f));

      // return the data
      return new ImageParams(projectile, projectileSize, position + offset, origin, scale);
    }
  }
}
