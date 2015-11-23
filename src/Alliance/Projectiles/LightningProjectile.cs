using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Utilities;
using Alliance.Data;
using Alliance.Invaders;
using Alliance.Parameters;

namespace Alliance.Projectiles
{
  public class LightningProjectile : Projectile
  {
    private const float SecondsPerFrame = 1f / 22.3456789f;
    private float mSecondsSinceUpdate = 0;

    private Invader mTarget;
    private SpriteEffects effects = SpriteEffects.None;

    public LightningProjectile(double timeToLiveInSeconds, Invader target)
      : base(timeToLiveInSeconds)
    {
      mTarget = target;
      Size = new SizeF(48f, 12f);
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

    protected override string ImageKey
    {
      get { return "lightning"; }
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
