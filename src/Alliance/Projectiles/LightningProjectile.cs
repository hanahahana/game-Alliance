using System;

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
    private SpriteEffects effects = SpriteEffects.None;
    private float mSecondsSinceUpdate = 0;

    public LightningProjectile(Piece parent, double timeToLiveInSeconds, Invader target)
      : base(parent, timeToLiveInSeconds)
    {
      mTarget = target;
      Size = new SizeF(48f, 12f);
      ImageKey = "lightning";
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      mSecondsSinceUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (mSecondsSinceUpdate >= SecondsPerFrame)
      {
        mSecondsSinceUpdate -= SecondsPerFrame;
        effects = (effects == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
      }
    }

    public override void Draw(DrawParams dparams)
    {
      SpriteBatch spriteBatch = dparams.SpriteBatch;
      Vector2 offset = dparams.Offset;

      TextureDrawData data = GetTextureDrawData(offset);
      spriteBatch.Draw(
          data.Texture,
          data.Position,
          null,
          Color,
          Orientation,
          data.Origin,
          data.Scale,
          effects,
          0);
    }
  }
}
