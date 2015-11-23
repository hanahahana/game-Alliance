using System;
using System.Collections.Generic;
using System.Text;
using Alliance.Data;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Alliance.Parameters;
using Alliance.Invaders;

namespace Alliance.Projectiles
{
  public class FlameProjectile : Projectile
  {
    private const float SecondsPerFrame = 1f / 12.3456789f;
    private float mSecondsSinceUpdate = 0;
    private SpriteEffects effects = SpriteEffects.None;

    public FlameProjectile(double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {
      Size = new SizeF(40f, 40f);
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

    public override void UpdateByFrameCount(int frameCount)
    {
      Position += ((frameCount + 1) * Velocity * MovementPerSecond * 20f);
    }

    protected override string ImageKey
    {
      get { return "flame"; }
    }

    public override void OnCollidedWithInvader(Invader invader)
    {
      // we don't care!
    }

    public override void Draw(DrawParams dparams)
    {
      SpriteBatch spriteBatch = dparams.SpriteBatch;
      Vector2 offset = dparams.Offset;

      DrawData data = GetDrawData(offset);
      spriteBatch.Draw(
          data.Texture,
          data.Position,
          null,
          Color,
          mOrientation,
          data.Origin,
          data.Scale,
          effects,
          0);
    }
  }
}
