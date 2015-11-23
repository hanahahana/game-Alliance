using System;
using System.Text;
using Alliance.Data;
using Alliance.Enums;
using Alliance.Objects;
using Alliance.Parameters;
using Alliance.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities;
using MLA.Utilities.Xna;
using MLA.Utilities.Xna.Helpers;

namespace Alliance.Pieces
{
  /// <summary>
  /// The tesla coil tower. It's meant to emulate firing a charged blast of electricity.
  /// </summary>
  [Serializable]
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
      Attack = 9000;
      Price = 2500;
      Radius = 250;
      UpgradePercent = 45;
      LevelVisibility = 75;

      Description = sb.ToString();
      ProjectileLifeInSeconds = 6.7f;
      CanFireProjectiles = false;
      Name = TeslaCoilName;
      UltimateName = UltimateTeslaCoilName;
      Grouping = PieceGrouping.Three;
      ImageKey = "teslaCoil";
      Element = Element.Electricity;
      Specialty = PieceSpecialty.Both;

      // set the properties of the piece
      mIndex = 0;
      mAggregateTimeSinceUpdate = 0;
      mTeslaState = TeslaState.Idle;
      Color = Color.White;

      // get the image
      Texture2D image = GetImage();
      FrameSize = new Size(image.Width / (NumberIndexFrames + 1), image.Height);
      LightningColor = ColorHelper.Blend(
        new Color[] { Color.Purple, Color.DarkBlue, Color.Red },
        new float[] { .5f, .75f });
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      TeslaCoilPiece piece = new TeslaCoilPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      LightningProjectile projectile = new LightningProjectile(this, ProjectileLifeInSeconds, Target);
      projectile.Color = LightningColor;
      return projectile;
    }

    public override Texture2D GetDisplayImage()
    {
      return AllianceGame.Images[ImageKey][0].Texture;
    }

    protected override Vector2[] GetImageHull()
    {
      return AllianceGame.Images[ImageKey][0].Hull;
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      base.UpgradeProjectileVariables(factor);
      ++ProjectilesPerSecond;

      ProjectileSpeed = DefaultProjectileSpeed;
      ++NumberProjectilesToFire;
    }

    public override void Update(GameTime gameTime)
    {
      // let the base update
      base.Update(gameTime);

      // if we're idle, then we COULD be firing
      if (State == PieceState.Idle)
      {
        UpdateTeslaState(gameTime);
      }
      else
      {
        mTeslaState = TeslaState.Idle;
        mIndex = 0;
      }

      float factor = ((float)mIndex) / ((float)NumberIndexFrames);
      Color = ColorHelper.Blend(Color.Beige, LightningColor, factor);
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
          FireProjectile(gameTime);
          mTeslaState = TeslaState.Firing;
        }
      }
    }

    private void UpdateTeslaIdleState(GameTime gameTime)
    {
      if (Target != null)
      {
        mTeslaState = TeslaState.Charging;
      }
    }

    protected override TextureDrawData GetTextureDrawData(Vector2 offset)
    {
      Tuple<BoxF, BoxF> outin = GetOutsideInsideBounds(offset);
      BoxF bounds = outin.First;
      BoxF inside = outin.Second;

      Texture2D wtower = GetImage();
      SizeF imgSize = new SizeF(FrameSize.Width, FrameSize.Height);
      SizeF actSize = new SizeF(bounds.Width, bounds.Height);

      Vector2 scale = MathematicsHelper.ComputeScale(imgSize, actSize);
      Vector2 origin = imgSize.ToVector2() * .5f;
      Vector2 center = actSize.ToVector2() * .5f;

      return new TextureDrawData(wtower, imgSize, bounds.Location + center, origin, scale);
    }

    protected override void DrawWeaponTower(DrawParams dparams, Vector2 offset)
    {
      TextureDrawData data = GetTextureDrawData(offset);
      Rectangle source = new Rectangle(
        mIndex * FrameSize.Width, 0,
        FrameSize.Width, FrameSize.Height);

      SpriteBatch spriteBatch = dparams.SpriteBatch;
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
