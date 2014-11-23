using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the flame thrower tower.
  /// </summary>
  [Serializable]
  public class FlameProjectile : Projectile
  {
    private const float SecondsPerFrame = 1.0f / 12.3456789f;

    private float mSecondsSinceUpdate = 0;
    private GsImageFlip effects = GsImageFlip.None;

    public FlameProjectile(Piece parent, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      Size = new GsSize(20f, 80f);
      ImageKey = "flame";

      var size = ImageProvider.GetSize(GetImage());
      Origin = new GsVector(0, size.Height / 2);
    }

    public override void Update(TimeSpan elapsed)
    {
      this.UpdateTimeToLive(elapsed);

      mSecondsSinceUpdate += (float)elapsed.TotalSeconds;
      if (mSecondsSinceUpdate >= SecondsPerFrame)
      {
        mSecondsSinceUpdate -= SecondsPerFrame;
        effects = (effects == GsImageFlip.None ? GsImageFlip.Horizontal : GsImageFlip.None);
      }
    }

    protected override ImageParams GetTextureDrawData(GsVector offset)
    {
      var image = GetImage();
      var imgSize = ImageProvider.GetSize(image);
      var scale = Calculator.ComputeScale(imgSize, Size);
      return new ImageParams(image, imgSize, Position + offset, Origin, scale);
    }

    public override void UpdateByFrameCount(TimeSpan elapsed, int frameCount)
    {
      float time = (float)(elapsed.TotalSeconds * ((frameCount + 1.0) * 20.0));
      Position += (time * VelocityFactor * VelocityFactor);
    }

    public override void OnCollidedWithInvader(Invader invader)
    {
      // we don't care!
    }

    public override void Draw(DrawParams dparams)
    {
      var graphics = dparams.Graphics;
      var offset = dparams.Offset;
      ImageParams data = GetTextureDrawData(offset);
      graphics.DrawImage(data, Color, Orientation);
    }
  }
}
