using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Utilities;
using Alliance.Data;
using Alliance.Projectiles;
using Alliance.Helpers;

namespace Alliance.Pieces
{
  public class SprinklerPiece : Piece
  {
    const float RadiansPerSecond = 5.5f;
    const float TotalSeconds = MathHelper.TwoPi / RadiansPerSecond;

    private const string SprinklerName = "Sprinkler";
    private const string UltimateSprinklerName = "Sprayer";

    private string mDescription;
    private float mRadius;
    private float mAttack;
    private int mPrice;
    private int mUpgradePercent;
    private float mPiePieces;

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return SprinklerName; }
    }

    public override string UltimateName
    {
      get { return UltimateSprinklerName; }
    }

    public override PieceGrouping Grouping
    {
      get { return PieceGrouping.One; }
    }

    public override float Radius
    {
      get { return mRadius; }
      protected set { mRadius = value; }
    }

    public override float Attack
    {
      get { return mAttack; }
      protected set { mAttack = value; }
    }

    public override int Price
    {
      get { return mPrice; }
      protected set { mPrice = value; }
    }

    public override int UpgradePercent
    {
      get { return mUpgradePercent; }
    }

    public override bool FaceTarget
    {
      get { return false; }
    }

    public SprinklerPiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Sprinkles projectiles around. This tower doesn't aim, just shoots. Good for closing up loose holes in paths.");
      mDescription = sb.ToString();

      mRadius = 20;
      mAttack = 25;

      mPiePieces = 2;
      mProjectilesPerSecond = TotalSeconds * mPiePieces;
      mNumberProjectilesToFire = 1;
      mUpgradePercent = 15;
      mPrice = 100;
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

    protected override Piece CreatePiece(Cell[] cells)
    {
      SprinklerPiece piece = new SprinklerPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      SprinklerProjectile projectile = new SprinklerProjectile(mProjectileLifeInSeconds);
      return projectile;
    }

    protected override Texture2D GetTowerImage()
    {
      return AllianceGame.Textures["sprinkler"];
    }

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D wtower = GetTowerImage();
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
