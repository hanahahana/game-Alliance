using System;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the shockwave tower.
  /// </summary>
  [Serializable]
  public class ShockwaveProjectile : Projectile
  {
    private const float ScalesPerSecond = 1.5f;
    private const float RotationsPerSecond = 34.567f;

    private BoxF mOwnerBounds;

    public ShockwaveProjectile(Piece parent, BoxF ownerBounds, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      mOwnerBounds = ownerBounds;
      Color = Color.Gray;
      ImageKey = "wave";
      StayAlive = true;
    }

    public override void Update(GameTime gameTime)
    {
      // update the time to live
      base.UpdateTimeToLive(gameTime);
     
      // if we're still alive, then update out
      if (IsAlive)
      {
        UpdateVariables((float)gameTime.ElapsedGameTime.TotalSeconds);
      }
    }

    protected void UpdateVariables(float elapsedSeconds)
    {
      //mScale += elapsedSeconds * ScalesPerSecond;
      //Size = mOwnerBounds.Size * .5f * mScale;

      // spin the projectile
      Orientation += elapsedSeconds * RotationsPerSecond;
      Orientation = MathHelper.WrapAngle(Orientation);
    }

    public override void UpdateByFrameCount(GameTime gameTime, int frameCount)
    {
      float time = (float)(gameTime.ElapsedGameTime.TotalSeconds * (frameCount + 1.0) * 10.0);
      UpdateVariables(time);
    }

    protected override TextureDrawData GetTextureDrawData(Vector2 offset)
    {
      AImage projectile = GetImage();
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
