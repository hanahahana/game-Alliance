using System;
using System.Collections.Generic;
using System.Text;
using Alliance.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Utilities;
using Alliance.Pieces;
using MLA.Utilities.Helpers;

namespace Alliance.Projectiles
{
  public class ShockwaveProjectile : Projectile
  {
    const float ScalesPerSecond = 1.5f;
    const float RotationsPerSecond = 34.567f;

    private BoxF mOwnerBounds;
    private float mScale = 1.0f;

    public override bool IsAlive
    {
      get { return base.IsAlive; }
      set { base.IsAlive = true; }
    }

    public ShockwaveProjectile(BoxF ownerBounds, double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {
      mOwnerBounds = ownerBounds;
      mColor = Utils.RandomColor();
    }

    public override void Update(GameTime gameTime)
    {
      // update the time to live
      base.UpdateTimeToLive(gameTime);
     
      // if we're still alive, then update out
      if (IsAlive)
      {
        UpdateScaleAndOrientation((float)gameTime.ElapsedGameTime.TotalSeconds);
      }
    }

    protected void UpdateScaleAndOrientation(float elapsedSeconds)
    {
      mScale += elapsedSeconds * ScalesPerSecond;
      mOrientation += elapsedSeconds * RotationsPerSecond;
      Size = mOwnerBounds.Size * mScale;
    }

    public override void UpdateOut(int frames)
    {
      UpdateScaleAndOrientation(frames * .1f);
    }

    public override Texture2D GetProjectileImage()
    {
      return AllianceGame.Textures["wave"];
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
      Texture2D projectile = GetProjectileImage();
      SizeF projectileSize = new SizeF(projectile.Width, projectile.Height);

      Vector2 origin = new Vector2(projectileSize.Width / 2, projectileSize.Height / 2);
      Vector2 scale = Utils.ComputeScale(projectileSize, Size);

      Vector2 position = mOwnerBounds.Location;
      position += ((mOwnerBounds.Size.ToVector2() * .5f));

      spriteBatch.Draw(
          projectile,
          position + offset,
          null,
          mColor,
          mOrientation,
          origin,
          scale,
          SpriteEffects.None,
          0);
    }

    public override Vector2 GetCenter(Vector2 offset)
    {
      Texture2D projectile = GetProjectileImage();
      SizeF projectileSize = new SizeF(projectile.Width, projectile.Height);

      Vector2 origin = new Vector2(0, projectileSize.Height / 2);
      Vector2 scale = Utils.ComputeScale(projectileSize, Size);

      // get the center of the original image
      Vector2 center = new Vector2(projectileSize.Width / 2f, 0);

      // create the matrix for transforming the center
      Matrix transform =
        Matrix.CreateTranslation(-origin.X, -origin.Y, 0) *
        Matrix.CreateRotationZ(mOrientation) *
        Matrix.CreateScale(scale.X, scale.Y, 1f) *
        Matrix.CreateTranslation(X + offset.X, Y + offset.Y, 0);

      // return the center transformated
      Vector2 result;
      Vector2.Transform(ref center, ref transform, out result);
      return result;
    }
  }
}
