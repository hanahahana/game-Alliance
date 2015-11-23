using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Alliance.Data;
using Alliance.Utilities;
using Alliance.Pieces;

namespace Alliance.Projectiles
{
  public class RailgunProjectile : Projectile
  {
    const float PixelsPerSecond = 150f;
    const float FactorOfWidth = 6f;
    const float SecondsBeforeUpdate = 1f / PixelsPerSecond;

    Point sourcePt;
    RailgunPiece mOwner;
    float totalElapsedSeconds;

    public RailgunProjectile(RailgunPiece owner, double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {
      sourcePt = Point.Zero;
      mColor = Utils.GetIntermediateColor(Color.Yellow, Color.Red, .55f, 0f, 1f);
      mOwner = owner;
      totalElapsedSeconds = 0;
    }

    protected override Texture2D GetProjectileImage()
    {
      return AllianceGame.Textures["pulse"];
    }

    public override void Update(GameTime gameTime)
    {
      // update the time to live
      base.UpdateTimeToLive(gameTime);

      if (IsAlive)
      {
        totalElapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
        while (totalElapsedSeconds >= SecondsBeforeUpdate)
        {
          totalElapsedSeconds -= SecondsBeforeUpdate;
          ++sourcePt.X;
        }

        float totalWidth = GetProjectileImage().Width;
        float width = totalWidth / FactorOfWidth;

        if ((sourcePt.X + width) >= totalWidth)
          sourcePt.X = 0;
        Position += (Velocity * MovementPerSecond * .3f);
      }
    }

    public override void UpdateOut(int frames)
    {
      Position += (Velocity * MovementPerSecond * frames);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
      Texture2D projectile = GetProjectileImage();
      SizeF projectileSize = new SizeF(projectile.Width, projectile.Height);

      float width = (projectileSize.Width / FactorOfWidth);
      Vector2 origin = new Vector2(0, projectile.Height / 2);

      Vector2 scale = Utils.ComputeScale(new SizeF(width, projectileSize.Height), Size);
      Rectangle source = new Rectangle(sourcePt.X, sourcePt.Y, (int)width, projectile.Height);

      spriteBatch.Draw(
          projectile,
          Position + offset,
          source,
          mColor,
          mOrientation,
          origin,
          scale,
          SpriteEffects.None,
          0);
    }
  }
}
