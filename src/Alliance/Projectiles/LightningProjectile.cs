using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the tesla coil tower.
  /// </summary>
  [Serializable]
  public class LightningProjectile : Projectile
  {
    private const float SecondsPerFrame = 1f / 22.3456789f;

    private Invader mTarget;
    private float mSecondsSinceUpdate = 0;
    private ImageFlip mFlipImage = ImageFlip.None;

    public LightningProjectile(Piece parent, double timeToLiveInSeconds, Invader target)
      : base(parent, timeToLiveInSeconds)
    {
      mTarget = target;
      Size = new GsSize(48f, 12f);
      ImageKey = "lightning";
    }

    public override void Update(TimeSpan elapsed)
    {
      base.Update(elapsed);
      mSecondsSinceUpdate += (float)elapsed.TotalSeconds;
      if (mSecondsSinceUpdate >= SecondsPerFrame)
      {
        mSecondsSinceUpdate -= SecondsPerFrame;
        mFlipImage = (mFlipImage == ImageFlip.None) ? ImageFlip.Horizontal : ImageFlip.None;
      }
    }

    public override void Draw(DrawParams dparams)
    {
      var graphics = dparams.Graphics;
      var offset = dparams.Offset;
      TextureParams data = GetTextureDrawData(offset);
      graphics.DrawImage(data, Color, offset, Orientation, mFlipImage);
    }
  }
}
