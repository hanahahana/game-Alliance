using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Alliance.Data;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Utilities;
using Alliance.Entities;
using Alliance.Projectiles;

namespace Alliance.Pieces
{
  public enum PieceState
  {
    Idle,
    Upgrading,
    Upgraded,
    Selling,
    Sold,
  };

  public enum PieceGrouping
  {
    /// <summary>
    /// 1x1
    /// </summary>
    One = 1,

    /// <summary>
    /// 2x2
    /// </summary>
    Two,

    /// <summary>
    /// 3x3
    /// </summary>
    Three,

    /// <summary>
    /// 4x4
    /// </summary>
    Four,
  }

  public abstract class Piece
  {
    public const int MaxLevel = 5;
    public const float MaxProgress = 100f;
    public const float ProgressPerSecond = 50f;
    public const float Delta = 5f;
    public const float Delta2 = Delta * 2;
    public const float DefaultProjectilesPerSecond = 1.5f;

    public abstract string Description { get; }
    public abstract string Name { get; }
    public abstract PieceGrouping Grouping { get; }
    public abstract float Radius { get; }

    protected abstract Piece CreatePiece(Cell[] cells);
    protected abstract Projectile CreateProjectile();

    protected Vector2 mPosition;
    protected Cell[] mCells;
    protected SizeF mSize;
    protected bool mSelected;
    protected PieceState mState;
    protected float mProgress = 0;
    protected int mLevel;
    protected Entity mTarget;
    protected float mOrientation;
    protected float mElapsedProjectileSeconds;
    protected List<Projectile> mQueuedProjectiles = new List<Projectile>(50);

    protected virtual float ProjectilesPerSecond { get { return DefaultProjectilesPerSecond; } }

    public Vector2 Position { get { return mPosition; } }
    public float X {get { return mPosition.X; }}
    public float Y { get { return mPosition.Y; } }
    public Cell[] Cells { get { return mCells; } }
    public SizeF Size { get { return mSize; } }
    public float Width {get { return mSize.Width; }}
    public float Height { get { return mSize.Height; } }
    public float Progress { get { return mProgress; } }
    public int Level { get { return mLevel; } }
    public bool CanUpgrade { get { return mLevel < MaxLevel; } }
    public PieceState State { get { return mState; } }
    public float Orientation { get { return mOrientation; } }

    public virtual bool IsBlocking { get { return true; } }
    public virtual bool FaceTarget { get { return true; } }

    public bool Selected
    {
      get { return mSelected; }
      set { mSelected = value; }
    }

    public Piece Place(Cell[] cells, SelectionPiece selection)
    {
      Piece piece = CreatePiece(cells);
      for (int i = 0; i < cells.Length; ++i)
      {
        Cell cell = cells[i];
        cell.SetPiece(piece);
      }

      piece.mCells = cells;
      piece.mPosition = selection.Bounds.Location;
      piece.mSize = selection.Bounds.Size;

      return piece;
    }

    public void Clear()
    {
      for (int i = 0; i < mCells.Length; ++i)
      {
        mCells[i].RemovePiece();
      }
      Array.Clear(mCells, 0, mCells.Length);
    }

    public override int GetHashCode()
    {
      return (new BoxF(mPosition, mSize)).GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Piece piece = obj as Piece;
      if (piece == null) return false;
      return piece.GetHashCode().Equals(this.GetHashCode());
    }

    public override string ToString()
    {
      return Name;
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
        // update the current progress
        mProgress += (float)(ProgressPerSecond * gameTime.ElapsedGameTime.TotalSeconds);

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

      // if we're just idle
      if (mState == PieceState.Idle)
      {
        // face the target
        TurnToFaceTarget();

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

    protected virtual void FireProjectile()
    {
      if (mTarget != null)
      {
        Projectile projectile = CreateProjectile();
        if (projectile != null)
        {
          projectile.Orientation = mOrientation;
          projectile.Velocity = new Vector2(
                        (float)Math.Cos(mOrientation),
                        (float)Math.Sin(mOrientation));
          projectile.IsAlive = true;
          mQueuedProjectiles.Add(projectile);
        }
      }
    }

    public Projectile[] GetProjectiles()
    {
      Projectile[] retval = mQueuedProjectiles.ToArray();
      mQueuedProjectiles.Clear();
      return retval;
    }

    private void TurnToFaceTarget()
    {
      if (mTarget != null)
      {
        float dx = mTarget.X - X;
        float dy = mTarget.Y - Y;

        float desiredAngle = (float)Math.Atan2(dy, dx);
        float difference = Utils.WrapAngle(desiredAngle - mOrientation);
        mOrientation = Utils.WrapAngle(mOrientation + difference);
      }
    }

    protected virtual void FinalizeSell()
    {
      mState = PieceState.Sold;
      Clear();
    }

    protected virtual void FinalizeUpgrade()
    {
      mState = PieceState.Upgraded;
      ++mLevel;
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
      BoxF bounds = new BoxF(X + offset.X, Y + offset.Y, Width, Height);
      BoxF inside = new BoxF(bounds.X + Delta, bounds.Y + Delta, bounds.Width - Delta2, bounds.Height - Delta2);

      DrawBackground(spriteBatch, bounds, inside);

      if (mState == PieceState.Idle)
      {
        DrawWeaponBase(spriteBatch, bounds, inside);
        DrawWeaponTower(spriteBatch, bounds, inside);
      }
      else if (mState == PieceState.Selling || mState == PieceState.Upgrading)
      {
        DrawProgressState(spriteBatch, bounds, inside);
      }
    }

    protected virtual void DrawProgressState(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
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

    protected virtual void DrawWeaponTower(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D wtower = GetWeaponTower();
      SizeF imgSize = new SizeF(wtower.Width, wtower.Height);
      SizeF actSize = new SizeF(bounds.Width - Delta, inside.Height);

      Vector2 scale = Utils.ComputeScale(imgSize, actSize);
      Vector2 imgCenter = imgSize.ToVector2() * .5f;
      Vector2 myCenter = actSize.ToVector2() * .5f;

      if (!FaceTarget)
        mOrientation = 0;

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

    protected virtual Texture2D GetWeaponTower()
    {
      return AllianceGame.Textures["turret"];
    }

    protected virtual void DrawWeaponBase(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Texture2D wbase = AllianceGame.Textures["towerBase"];
      Vector2 scale = Utils.ComputeScale(new SizeF(wbase.Width, wbase.Height), bounds.Size);
      Color color = Utils.NewAlpha(Color.Gray, .5f);
      spriteBatch.Draw(wbase, bounds.Location, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    protected virtual void DrawBackground(SpriteBatch spriteBatch, BoxF bounds, BoxF inside)
    {
      Color bgColor = (mLevel == MaxLevel ? Utils.GetIntermediateColor(Color.Beige, Color.SkyBlue, .5f, 0, 1f) : Color.Beige);
      Color color = mSelected ? Color.DarkGreen : bgColor;
      Shapes.FillRectangle(spriteBatch, bounds, color);
    }

    public void Sell()
    {
      if (mState == PieceState.Idle)
      {
        mProgress = 0;
        mState = PieceState.Selling;
      }
    }

    public void Upgrade()
    {
      if (mState == PieceState.Idle && CanUpgrade)
      {
        mProgress = 0;
        mState = PieceState.Upgrading;
      }
    }

    public static void FollowSelectionPiece(Piece piece, SelectionPiece mSelectionPiece)
    {
      if (piece != null && mSelectionPiece != null)
      {
        piece.mPosition = mSelectionPiece.Bounds.Location;
        piece.mSize = mSelectionPiece.Bounds.Size;
      }
    }

    public void SetTarget(Entity entity)
    {
      if (mTarget == null)
        mTarget = entity;
    }

    public void ClearTarget()
    {
      mTarget = null;
    }
  }
}
