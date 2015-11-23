using System;
using Alliance.Data;
using Alliance.Invaders;
using Alliance.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities.Xna;
using MLA.Utilities.Xna.Helpers;
using Alliance.Pieces;

namespace Alliance.Projectiles
{
  /// <summary>
  /// The projectile fired by the flame thrower tower.
  /// </summary>
  [Serializable]
  public class FlameProjectile : Projectile
  {
    private const float SecondsPerFrame = 1.0f / 12.3456789f;

    private float mSecondsSinceUpdate = 0;
    private SpriteEffects effects = SpriteEffects.None;

    public FlameProjectile(Piece parent, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      Size = new SizeF(20f, 80f);
      ImageKey = "flame";
      Origin = new Vector2(0, GetImage().Height / 2);
    }

    public override void Update(GameTime gameTime)
    {
      this.UpdateTimeToLive(gameTime);

      mSecondsSinceUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (mSecondsSinceUpdate >= SecondsPerFrame)
      {
        mSecondsSinceUpdate -= SecondsPerFrame;
        effects = (effects == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
      }
    }

    protected override TextureDrawData GetTextureDrawData(Vector2 offset)
    {
      Texture2D image = GetImage();
      SizeF imgSize = new SizeF(image.Width, image.Height);
      Vector2 scale = MathematicsHelper.ComputeScale(imgSize, Size);
      return new TextureDrawData(image, imgSize, Position + offset, Origin, scale);
    }

    public override void UpdateByFrameCount(GameTime gameTime, int frameCount)
    {
      float time = (float)(gameTime.ElapsedGameTime.TotalSeconds * ((frameCount + 1.0) * 20.0));
      Position += (time * VelocityFactor * VelocityFactor);
    }

    public override void OnCollidedWithInvader(Invader invader)
    {
      // we don't care!
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
          0f);
    }
  }
}
