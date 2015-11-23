using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Projectiles;
using Alliance.Data;
using Alliance.Helpers;
using Alliance.Utilities;

namespace Alliance.Pieces
{
  public enum TeslaState
  {
    Idle,
    Charging,
    Firing,
    Discharging,
  }

  public class TeslaCoilPiece : Piece
  {
    private const string TeslaCoilName = "Tesla Coil";
    private const string UltimateTeslaCoilName = "Discharger";

    private const int NumberIndexFrames = 4;
    private const float SecondsPerFrame = 1f / 5.3456789f;
    private const float SecondsToStayOpen = 1f / 2.5f;

    private readonly Size FrameSize;
    private readonly Color LightningColor;

    private string mDescription;
    private float mRadius;
    private float mAttack;
    private int mPrice;
    private int mUpgradePercent;
    private int mIndex;
    private float mAggregateTimeSinceUpdate;
    private TeslaState mTeslaState;
    private Color mColor;

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return TeslaCoilName; }
    }

    public override string UltimateName
    {
      get { return UltimateTeslaCoilName; }
    }

    public override PieceGrouping Grouping
    {
      get { return PieceGrouping.Three; }
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

    protected override bool CanFireProjectiles
    {
      get { return false; }
    }

    public TeslaCoilPiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Fires a charged burst of electricity. The burst is very effective against ground units.");
      mDescription = sb.ToString();

      mRadius = 75;
      mAttack = 9000;
      mProjectileLifeInSeconds = 6.7f;

      mNumberProjectilesToFire = 1;
      mUpgradePercent = 45;
      mPrice = 100;

      mIndex = 0;
      mAggregateTimeSinceUpdate = 0;
      mTeslaState = TeslaState.Idle;
      mColor = Color.White;
      LightningColor = Utils.GetIntermediateColor(
        Utils.GetIntermediateColor(Color.Purple, Color.DarkBlue, .5f, 0, 1),
        Color.Red, .75f, 0, 1);

      FrameSize = new Size(GetTowerImage().Width / (NumberIndexFrames + 1), GetTowerImage().Height);
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      TeslaCoilPiece piece = new TeslaCoilPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      LightningProjectile projectile = new LightningProjectile(mProjectileLifeInSeconds, mTarget);
      projectile.Color = LightningColor;
      return projectile;
    }

    protected override Texture2D GetTowerImage()
    {
      return AllianceGame.Textures["teslaCoil"];
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      base.UpgradeProjectileVariables(factor);
      ++mProjectilesPerSecond;

      mProjectileVelocity = DefaultProjectileVelocity;
      ++mNumberProjectilesToFire;
    }

    public override void Update(GameTime gameTime)
    {
      // let the base update
      base.Update(gameTime);

      // if we're idle, then we COULD be firing
      if (mState == PieceState.Idle)
      {
        UpdateTeslaState(gameTime);
      }
      else
      {
        mTeslaState = TeslaState.Idle;
        mIndex = 0;
      }

      mColor = Utils.GetIntermediateColor(Color.Beige, LightningColor, mIndex, 0, NumberIndexFrames);
    }

    private void UpdateTeslaState(GameTime gameTime)
    {
      switch (mTeslaState)
      {
        case TeslaState.Idle:
          {
            UpdateTeslaIdleState(gameTime);
            break;
          }
        case TeslaState.Charging:
          {
            UpdateTeslaChargingState(gameTime);
            break;
          }
        case TeslaState.Firing:
          {
            UpdateTeslaFiringState(gameTime);
            break;
          }
        case TeslaState.Discharging:
          {
            UpdateTeslaDischaringState(gameTime);
            break;
          }
      }
    }

    private void UpdateTeslaDischaringState(GameTime gameTime)
    {
      // here, we backtrack until we get to the beginning
      mAggregateTimeSinceUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (mAggregateTimeSinceUpdate >= SecondsPerFrame)
      {
        mAggregateTimeSinceUpdate -= SecondsPerFrame;
        --mIndex;

        if (mIndex == 0)
        {
          mTeslaState = TeslaState.Idle;
        }
      }
    }

    private void UpdateTeslaFiringState(GameTime gameTime)
    {
      // we stay open for a certain amount of time before discharging.
      mAggregateTimeSinceUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (mAggregateTimeSinceUpdate >= SecondsToStayOpen)
      {
        mAggregateTimeSinceUpdate -= SecondsToStayOpen;
        mTeslaState = TeslaState.Discharging;
      }
    }

    private void UpdateTeslaChargingState(GameTime gameTime)
    {
      mAggregateTimeSinceUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
      if (mAggregateTimeSinceUpdate >= SecondsPerFrame)
      {
        mAggregateTimeSinceUpdate -= SecondsPerFrame;
        ++mIndex;

        if (mIndex == NumberIndexFrames)
        {
          FireProjectile();
          mTeslaState = TeslaState.Firing;
        }
      }
    }

    private void UpdateTeslaIdleState(GameTime gameTime)
    {
      if (mTarget != null)
      {
        mTeslaState = TeslaState.Charging;
      }
    }

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D wtower = GetTowerImage();
      SizeF imgSize = new SizeF(FrameSize.Width, FrameSize.Height);
      SizeF actSize = new SizeF(bounds.Width, bounds.Height);

      Vector2 scale = Utils.ComputeScale(imgSize, actSize);
      Vector2 origin = imgSize.ToVector2() * .5f;
      Vector2 center = actSize.ToVector2() * .5f;

      Rectangle source = new Rectangle(
        mIndex * FrameSize.Width, 0,
        FrameSize.Width, FrameSize.Height);

      spriteBatch.Draw(
        wtower,
        bounds.Location + center,
        source,
        mColor,
        0,
        origin,
        scale,
        SpriteEffects.None,
        0f);
    }
  }
}
