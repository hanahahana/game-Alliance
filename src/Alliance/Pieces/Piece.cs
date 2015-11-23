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
  public abstract partial class Piece : Sprite, ITextDisplay
  {
    public const int MaxLevel = 5;
    public const float MaxProgress = 100f;
    public const float ProgressPerSecond = 50f;
    public const float Delta = 5f;
    public const float Delta2 = Delta * 2;

    protected const float DefaultProjectilesPerSecond = 2.5f;
    protected const float DefaultProjectileVelocity = .5f;
    protected const float DefaultProjectileLifeInSeconds = 1f;
    protected const float DefaultTurnSpeed = 1.5f;
    protected const int DefaultNumberProjectilesToFire = 1;

    protected int[] mPriceAtLevels = new int[MaxLevel + 1];
    protected GridCell[] mCells;
    protected bool mSelected;
    protected PieceState mState;
    protected float mProgress = 0;
    protected int mLevel = 0;
    protected Entity mTarget;
    protected float mElapsedProjectileSeconds;
    protected List<Projectile> mQueuedProjectiles = new List<Projectile>(50);

    protected string mDescription = string.Empty;
    protected float mRadius = 15f;
    protected float mAttack = 15f;
    protected int mPrice = 15;
    protected int mUpgradePercent = 15;
    protected bool mFaceTarget = true;
    protected bool mIsBlocking = true;
    protected bool mCanFireProjectiles = true;
    protected float mTurnSpeed = DefaultTurnSpeed;
    protected string mName = string.Empty;
    protected string mUltimateName = string.Empty;
    protected PieceGrouping mGrouping = PieceGrouping.Two;
    protected float mProjectilesPerSecond = DefaultProjectilesPerSecond;
    protected float mProjectileVelocity = DefaultProjectileVelocity;
    protected float mProjectileLifeInSeconds = DefaultProjectileLifeInSeconds;
    protected int mNumberProjectilesToFire = DefaultNumberProjectilesToFire;

    protected abstract Piece CreatePiece(GridCell[] cells);

    protected void SavePriceInfo()
    {
      if (mLevel < mPriceAtLevels.Length)
        mPriceAtLevels[mLevel] = Price;
    }

    private void TurnToFaceTarget()
    {
      // if we have a target and we're supposed to be facing the target
      if (mTarget != null && FaceTarget)
      {
        Vector2 targetCenter = mTarget.Position + (mTarget.Size.ToVector2() * .5f);
        Vector2 myCenter = Position + (Size.ToVector2() * .5f);

        float dx = targetCenter.X - myCenter.X;
        float dy = targetCenter.Y - myCenter.Y;

        float desiredAngle = (float)Math.Atan2(dy, dx);
        float difference = Utils.WrapAngle(desiredAngle - mOrientation);

        difference = MathHelper.Clamp(difference, -mTurnSpeed, mTurnSpeed);
        mOrientation = Utils.WrapAngle(mOrientation + difference);
      }
    }

    protected void FireProjectile()
    {
      if (mTarget != null)
      {
        BoxF inside = new BoxF(X + Delta, Y + Delta, Width - Delta2, Height - Delta2);
        SizeF actSize = new SizeF(Width - Delta, inside.Height);

        Vector2 myCenter = actSize.ToVector2() * .5f;
        for (int i = 0; i < NumberProjectilesToFire; ++i)
        {
          Projectile projectile = CreateProjectile();
          projectile.Velocity = Utils.ComputeProjectileDirection(mOrientation) * ProjectileVelocity;
          projectile.Position = inside.Location + myCenter;
          projectile.Orientation = mOrientation;
          projectile.Attack = Attack;
          projectile.UpdateByFrameCount(i * NumberProjectilesToFire);
          mQueuedProjectiles.Add(projectile);
        }
      }
    }

    protected virtual Projectile CreateProjectile()
    {
      Projectile projectile = new Projectile(ProjectileLifeInSeconds);
      return projectile;
    }

    protected virtual void FinalizeSell()
    {
      mState = PieceState.Sold;
      Player.SellPiece(this);
      Clear();
    }

    protected int UpgradePrice(float factor)
    {
      return (int)Math.Round(Price * factor);
    }

    protected float UpgradeAttack(float factor)
    {
      return (float)Math.Round(Attack * factor);
    }

    protected float ComputeUpgradeFactor()
    {
      return 1f + ((float)UpgradePercent / 100f);
    }

    protected float UpgradeRadius(float factor)
    {
      return (Radius * factor);
    }

    protected virtual void FinalizeUpgrade()
    {
      mState = PieceState.Upgraded;
      ++mLevel;

      // determine the factor to multiply by
      float factor = ComputeUpgradeFactor();

      // upgrade the attack, price and the radius
      mAttack = UpgradeAttack(factor);
      mRadius = UpgradeRadius(factor);
      mPrice = UpgradePrice(factor);

      // set the price
      SavePriceInfo();

      // upgrade the projectile variables
      UpgradeProjectileVariables(factor);
    }

    protected virtual void UpgradeProjectileVariables(float factor)
    {
      mProjectilesPerSecond *= factor;
      mProjectileVelocity *= factor;
      mProjectileLifeInSeconds *= (1.001f);
      mNumberProjectilesToFire += (mLevel / 4);
    }

    public virtual void Update(GameTime gameTime)
    {
      // if we're not idle, then clear the projectiles
      if (mState != PieceState.Idle)
      {
        // clear away the projectiles
        mQueuedProjectiles.Clear();
      }

      // once we've upgrade, set this back to idle
      if (mState == PieceState.Upgraded)
      {
        mState = PieceState.Idle;
      }

      // if we're selling or upgrading
      if (mState == PieceState.Selling || mState == PieceState.Upgrading)
      {
        UpdateSellingUpgrading(gameTime);
      }

      // if we're just idle
      if (mState == PieceState.Idle)
      {
        UpdateIdle(gameTime);
      }
    }

    private void UpdateIdle(GameTime gameTime)
    {
      // face the target
      TurnToFaceTarget();

      // if we can fire projectiles
      if (mCanFireProjectiles)
      {
        // update the projectiles per second
        mElapsedProjectileSeconds += (float)(ProjectilesPerSecond * gameTime.ElapsedGameTime.TotalSeconds);
        if (mElapsedProjectileSeconds >= 1.0f)
        {
          // decrement the value
          mElapsedProjectileSeconds -= 1.0f;

          // fire a projectile
          FireProjectile();
        }
      }
    }

    private void UpdateSellingUpgrading(GameTime gameTime)
    {
      if (Player.State == PlayerState.Designing)
      {
        // if we're designing, then the sell/upgrade is instant
        mProgress = MaxProgress;
      }
      else
      {
        // update the current progress
        mProgress += (float)(ProgressPerSecond * gameTime.ElapsedGameTime.TotalSeconds);
      }

      // if we've exceeded the max progress
      if (mProgress >= MaxProgress)
      {
        // reset the progress
        mProgress = 0;

        // check the current state to determine the finalize action
        if (mState == PieceState.Selling)
        {
          // if we were selling the piece, then finalize the sell
          FinalizeSell();
        }
        else
        {
          // otherwise, finalize the upgrading.
          FinalizeUpgrade();
        }
      }
    }

    public void Draw(DrawParams dparams)
    {
      SpriteBatch spriteBatch = dparams.SpriteBatch;
      Vector2 offset = dparams.Offset;

      BoxF bounds = new BoxF(X + offset.X, Y + offset.Y, Width, Height);
      BoxF inside = new BoxF(bounds.X + Delta, bounds.Y + Delta, bounds.Width - Delta2, bounds.Height - Delta2);

      DrawBackground(spriteBatch, bounds, inside);
      if (mState == PieceState.Idle)
      {
        DrawWeaponBase(spriteBatch, bounds, inside);
        DrawWeaponTower(spriteBatch, bounds, inside);
        DrawCurrentLevel(spriteBatch, bounds, inside);
      }
      else if (mState == PieceState.Selling || mState == PieceState.Upgrading)
      {
        DrawProgressState(spriteBatch, bounds, inside);
      }
    }

    protected virtual void DrawBackground(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Color bgColor = (mLevel == MaxLevel ? Utils.GetIntermediateColor(Color.Beige, Color.SkyBlue, .5f, 0, 1f) : Color.Beige);
      Color color = mSelected ? Color.DarkGreen : bgColor;
      Shapes.FillRectangle(spriteBatch, bounds, color);
    }

    protected virtual void DrawWeaponBase(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D wbase = AllianceGame.Textures["towerBase"];
      Vector2 scale = Utils.ComputeScale(new SizeF(wbase.Width, wbase.Height), bounds.Size);

      Color color = Utils.NewAlpha(Color.Gray, .5f);
      spriteBatch.Draw(
        wbase,
        bounds.Location,
        null,
        color,
        0f,
        Vector2.Zero,
        scale,
        SpriteEffects.None,
        0f);
    }

    protected virtual void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D wtower = GetImage();
      SizeF imgSize = new SizeF(wtower.Width, wtower.Height);
      SizeF actSize = new SizeF(bounds.Width - Delta, inside.Height);

      Vector2 scale = Utils.ComputeScale(imgSize, actSize);
      Vector2 imgCenter = imgSize.ToVector2() * .5f;
      Vector2 myCenter = actSize.ToVector2() * .5f;

      Color color = Color.Gray;
      spriteBatch.Draw(
        wtower,
        inside.Location + myCenter,
        null,
        color,
        mOrientation,
        imgCenter,
        scale,
        SpriteEffects.None,
        0f);
    }

    protected virtual void DrawCurrentLevel(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      if (mLevel < MaxLevel)
      {
        float spacing = 2f;
        float dimension = (bounds.Width - (spacing * 5f)) / 8f;

        float x = bounds.X + spacing;
        float y = bounds.Bottom - (spacing + dimension);

        for (int i = 0; i < mLevel; ++i, x += (spacing + dimension))
        {
          BoxF box = new BoxF(x, y, dimension, dimension);
          Shapes.FillRectangle(spriteBatch, box, Color.Black);
        }
      }
    }

    private void DrawProgressState(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      float width = inside.Width;
      float height = 4f;

      float x = inside.X + ((inside.Width / 2f) - (width / 2f));
      float y = inside.Y + ((inside.Height / 2f) - (height / 2f));

      float progressWidth = width * Utils.CalculatePercent(mProgress, 0, MaxProgress);
      Color progressColor = Utils.GetIntermediateColor(Color.Red, Color.DarkGreen, mProgress, 0, MaxProgress);

      Shapes.FillRectangle(spriteBatch, x, y, progressWidth, height, progressColor);
      Shapes.DrawRectangle(spriteBatch, x, y, width, height, Color.Black);
    }
  }
}
