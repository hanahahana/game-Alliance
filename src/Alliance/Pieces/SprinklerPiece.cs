using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Entities;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Objects;

namespace Alliance.Pieces
{
  public class SprinklerPiece : Piece
  {
    private const string SprinklerName = "Sprinkler";
    private const string UltimateSprinklerName = "Sprayer";

    private const float RadiansPerSecond = 5.5f;
    private const float TotalSeconds = MathHelper.TwoPi / RadiansPerSecond;

    private float mPiePieces;

    public SprinklerPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Sprinkles projectiles around. This tower doesn't aim, just shoots. Good for closing up loose holes in paths.");

      // set the properties of the piece
      mDescription = sb.ToString();
      mRadius = 20;
      mAttack = 25;
      mPiePieces = 2;
      mProjectilesPerSecond = TotalSeconds * mPiePieces;
      mNumberProjectilesToFire = 1;
      mUpgradePercent = 15;
      mPrice = 100;
      mFaceTarget = false;
      mName = SprinklerName;
      mUltimateName = UltimateSprinklerName;
      mGrouping = PieceGrouping.One;
    }

    public override void Update(GameTime gameTime)
    {
      // let the base do it's thing
      base.Update(gameTime);

      // spin around in a circle
      mOrientation += ((float)gameTime.ElapsedGameTime.TotalSeconds * RadiansPerSecond);
      Utils.WrapAngle(mOrientation);
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      base.UpgradeProjectileVariables(factor);

      float mu = (float)mLevel / (float)MaxLevel;
      mUpgradePercent = (int)Math.Round(mUpgradePercent * factor * 3.5f);

      mProjectileVelocity = MathHelper.Lerp(DefaultProjectileVelocity, DefaultProjectileVelocity * 3f, mu);
      mPiePieces = MathHelper.Lerp(2f, 16f, mu);

      mNumberProjectilesToFire = 1;
      mProjectilesPerSecond = TotalSeconds * mPiePieces;
    }

    protected override void FinalizeUpgrade()
    {
      base.FinalizeUpgrade();
      if (mLevel == MaxLevel)
      {
        StringBuilder sb = new StringBuilder(mDescription);
        sb.AppendLine();
        sb.AppendLine("Not sure how you got this uber tower but congratulations!");
        mDescription = sb.ToString();
      }
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      SprinklerPiece piece = new SprinklerPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      SprinklerProjectile projectile = new SprinklerProjectile(mProjectileLifeInSeconds);
      return projectile;
    }

    protected override string ImageKey
    {
      get { return "sprinkler"; }
    }

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D wtower = GetImage();
      SizeF imgSize = new SizeF(wtower.Width, wtower.Height);
      SizeF actSize = new SizeF(bounds.Width, bounds.Height);

      Vector2 scale = Utils.ComputeScale(imgSize, actSize);
      Vector2 origin = imgSize.ToVector2() * .5f;
      Vector2 center = actSize.ToVector2() * .5f;

      Color color = Color.Gray;
      spriteBatch.Draw(
        wtower,
        bounds.Location + center,
        null,
        color,
        mOrientation,
        origin,
        scale,
        SpriteEffects.None,
        0f);
    }
  }
}
