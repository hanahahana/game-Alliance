using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Invaders;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Objects;
using MLA.Utilities;

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

    private int mIndex;
    private float mAggregateTimeSinceUpdate;
    private TeslaState mTeslaState;

    public TeslaCoilPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Fires a charged burst of electricity. The burst is very effective against ground units.");

      // set the properties needed
      mDescription = sb.ToString();
      mRadius = 75;
      mAttack = 9000;
      mPrice = 100;
      mUpgradePercent = 45;
      mProjectileLifeInSeconds = 6.7f;
      mCanFireProjectiles = false;
      mName = TeslaCoilName;
      mUltimateName = UltimateTeslaCoilName;
      mGrouping = PieceGrouping.Three;

      // set the properties of the piece
      mIndex = 0;
      mAggregateTimeSinceUpdate = 0;
      mTeslaState = TeslaState.Idle;
      Color = Color.White;
      FrameSize = new Size(GetImage().Width / (NumberIndexFrames + 1), GetImage().Height);
      LightningColor = Utils.GetIntermediateColor(
        Utils.GetIntermediateColor(Color.Purple, Color.DarkBlue, .5f, 0, 1),
        Color.Red, .75f, 0, 1);
    }

    protected override Piece CreatePiece(GridCell[] cells)
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

    protected override string ImageKey
    {
      get { return "teslaCoil"; }
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

      Color = Utils.GetIntermediateColor(Color.Beige, LightningColor, mIndex, 0, NumberIndexFrames);
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

    protected override DrawData GetDrawData(Vector2 offset)
    {
      Tuple<BoxF, BoxF> outin = GetOutsideInsideBounds(offset);
      BoxF bounds = outin.First;
      BoxF inside = outin.Second;

      Texture2D wtower = GetImage();
      SizeF imgSize = new SizeF(FrameSize.Width, FrameSize.Height);
      SizeF actSize = new SizeF(bounds.Width, bounds.Height);

      Vector2 scale = Utils.ComputeScale(imgSize, actSize);
      Vector2 origin = imgSize.ToVector2() * .5f;
      Vector2 center = actSize.ToVector2() * .5f;

      return new DrawData(wtower, imgSize, bounds.Location + center, origin, scale);
    }

    protected override void DrawWeaponTower(SpriteBatch spriteBatch, Vector2 offset)
    {
      DrawData data = GetDrawData(offset);
      Rectangle source = new Rectangle(
        mIndex * FrameSize.Width, 0,
        FrameSize.Width, FrameSize.Height);

      spriteBatch.Draw(
        data.Texture,
        data.Position,
        source,
        Color,
        0,
        data.Origin,
        data.Scale,
        SpriteEffects.None,
        0f);
    }
  }
}
