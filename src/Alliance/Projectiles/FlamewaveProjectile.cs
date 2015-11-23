using System;
using System.Collections.Generic;
using System.Text;

using MLA.Utilities.Helpers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Pieces;
using Alliance.Parameters;

namespace Alliance.Projectiles
{
  public class FlamewaveProjectile : Projectile
  {
    const float ScalesPerSecond = 1.5f;
    const float RotationsPerSecond = 24.567f;

    private BoxF mOwnerBounds;
    private float mScale = 1.0f;

    public override bool IsAlive
    {
      get { return base.IsAlive; }
      set { base.IsAlive = true; }
    }

    public FlamewaveProjectile(BoxF ownerBounds, double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {
      mOwnerBounds = ownerBounds;
      Color = Color.White;
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

    public override void UpdateByFrameCount(int frameCount)
    {
      UpdateScaleAndOrientation(frameCount * .3f);
    }

    protected override string ImageKey
    {
      get { return "flamewave"; }
    }

    protected override DrawData GetDrawData(Vector2 offset)
    {
      Texture2D projectile = GetImage();
      SizeF projectileSize = new SizeF(projectile.Width, projectile.Height);

      Vector2 origin = projectileSize.ToVector2() * .5f;
      Vector2 scale = Utils.ComputeScale(projectileSize, Size);

      Vector2 position = mOwnerBounds.Location;
      position += ((mOwnerBounds.Size.ToVector2() * .5f));

      // return the data
      return new DrawData(projectile, projectileSize, position + offset, origin, scale);
    }
  }
}
