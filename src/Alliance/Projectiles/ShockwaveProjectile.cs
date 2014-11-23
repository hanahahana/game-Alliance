using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the shockwave tower.
  /// </summary>
  [Serializable]
  public class ShockwaveProjectile : Projectile
  {
    private const float ScalesPerSecond = 1.5f;
    private const float RotationsPerSecond = 34.567f;

    private GsRectangle mOwnerBounds;

    public ShockwaveProjectile(Piece parent, GsRectangle ownerBounds, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      mOwnerBounds = ownerBounds;
      Color = GsColor.Gray;
      ImageKey = "wave";
      StayAlive = true;
    }

    public override void Update(TimeSpan elapsed)
    {
      // update the time to live
      base.UpdateTimeToLive(elapsed);
     
      // if we're still alive, then update out
      if (IsAlive)
      {
        UpdateVariables((float)elapsed.TotalSeconds);
      }
    }

    protected void UpdateVariables(float elapsedSeconds)
    {
      //mScale += elapsedSeconds * ScalesPerSecond;
      //Size = mOwnerBounds.Size * .5f * mScale;

      // spin the projectile
      Orientation += elapsedSeconds * RotationsPerSecond;
      Orientation = GsMath.WrapAngle(Orientation);
    }

    public override void UpdateByFrameCount(TimeSpan elapsed, int frameCount)
    {
      float time = (float)(elapsed.TotalSeconds * (frameCount + 1.0) * 10.0);
      UpdateVariables(time);
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
