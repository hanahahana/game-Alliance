using System;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the flame thrower tower.
  /// </summary>
  [Serializable]
  public class FlamewaveProjectile : Projectile
  {
    const float ScalesPerSecond = 1.5f;
    const float RotationsPerSecond = 24.567f;

    private BoxF mOwnerBounds;
    private float mScale = 1.0f;

    public FlamewaveProjectile(Piece parent, BoxF ownerBounds, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      mOwnerBounds = ownerBounds;
      Color = Color.White;
      ImageKey = "flamewave";
      StayAlive = true;
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
      Orientation += elapsedSeconds * RotationsPerSecond;
      Size = mOwnerBounds.Size * mScale;
    }

    public override void UpdateByFrameCount(GameTime gameTime, int frameCount)
    {
      float time = (float)(gameTime.ElapsedGameTime.TotalSeconds * ((frameCount + 1.0) * 4.0) * .3);
      UpdateScaleAndOrientation(time);
    }

    protected override TextureDrawData GetTextureDrawData(Vector2 offset)
    {
      Texture2D projectile = GetImage();
      SizeF projectileSize = new SizeF(projectile.Width, projectile.Height);

      Vector2 origin = projectileSize.ToVector2() * .5f;
      Vector2 scale = MathematicsHelper.ComputeScale(projectileSize, Size);

      Vector2 position = mOwnerBounds.Location;
      position += ((mOwnerBounds.Size.ToVector2() * .5f));

      // return the data
      return new TextureDrawData(projectile, projectileSize, position + offset, origin, scale);
    }
  }
}
