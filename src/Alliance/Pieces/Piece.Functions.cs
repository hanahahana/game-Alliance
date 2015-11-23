using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Alliance.Projectiles;
using Alliance.Data;
using Alliance.Entities;
using Alliance.Objects;

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
  };

  public partial class Piece
  {
    public GridCell[] Cells { get { return mCells; } }
    public float Progress { get { return mProgress; } }
    public int Level { get { return mLevel; } }
    public bool CanUpgrade { get { return mLevel < MaxLevel && Player.EnoughCashFor(this); } }
    public PieceState State { get { return mState; } }

    public virtual string Description { get { return mDescription; } }
    public virtual string Name { get { return mName; } }
    public virtual string UltimateName { get { return mUltimateName; } }
    public virtual PieceGrouping Grouping { get { return mGrouping; } }
    public virtual float Radius { get { return mRadius; } }
    public virtual float Attack { get { return mAttack; } }
    public virtual int Price { get { return mPrice; } }
    public virtual int UpgradePercent { get { return mUpgradePercent; } }
    public virtual bool IsBlocking { get { return mIsBlocking; } }
    public virtual bool FaceTarget { get { return mFaceTarget; } }
    public virtual float ProjectilesPerSecond { get { return mProjectilesPerSecond; } }
    public virtual float ProjectileVelocity { get { return mProjectileVelocity; } }
    public virtual float ProjectileLifeInSeconds { get { return mProjectileLifeInSeconds; } }
    public virtual int NumberProjectilesToFire { get { return mNumberProjectilesToFire; } }

    public bool Selected
    {
      get { return mSelected; }
      set { mSelected = value; }
    }

    public override string ToString()
    {
      return Name;
    }

    protected override string ImageKey { get { return "turret"; } }
    protected override Vector2 Origin { get { return Vector2.Zero; } }

    public int GetLifetimePrice()
    {
      // we get the price that it was to place the piece
      int sum = mPriceAtLevels[0];

      // cycle through the levels
      for (int i = 0; i < mLevel; ++i)
      {
        // add on the price to upgrade for that level
        sum += mPriceAtLevels[i];
      }

      // return the lifetime sum
      return sum;
    }

    public Piece Place(GridCell[] cells, SelectionPiece selection)
    {
      Piece piece = CreatePiece(cells);
      for (int i = 0; i < cells.Length; ++i)
      {
        GridCell cell = cells[i];
        cell.SetPiece(piece);
      }

      piece.mCells = cells;
      piece.Bounds = selection.Bounds;
      piece.SavePriceInfo();

      if (!Player.PurchasePiece(this))
        throw new Exception("Somehow you placed this without enough money!");
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

    public Projectile[] PopProjectiles()
    {
      Projectile[] retval = mQueuedProjectiles.ToArray();
      mQueuedProjectiles.Clear();
      return retval;
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
        if (!Player.PurchasePiece(this))
          throw new Exception("Somehow you upgraded this without enough money!");

        mProgress = 0;
        mState = PieceState.Upgrading;
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

    public static void FollowSelectionPiece(Piece piece, SelectionPiece mSelectionPiece)
    {
      if (piece != null && mSelectionPiece != null)
      {
        piece.Bounds = mSelectionPiece.Bounds;
      }
    }

    #region ITextProvider Members

    public string GetHeader()
    {
      string header = Name;
      if (0 < Level)
      {
        if (Level < Piece.MaxLevel)
        {
          header = string.Concat(header, " Lvl ", Level);
        }
        else if (!(this is SpeedBumpPiece))
        {
          header = UltimateName;
        }
      }
      return header;
    }

    public string GetText()
    {
      StringBuilder text = new StringBuilder();
      List<string> lines = new List<string>();

      text.AppendLine(Description);
      float factor = ComputeUpgradeFactor();

      float newAttack = UpgradeAttack(factor);
      int newPrice = UpgradePrice(factor);

      lines.Add(string.Format("Attack: {0}", Attack));
      lines.Add(string.Format("Cost: {0:c2}", Price));

      if (Level < MaxLevel)
      {
        lines[0] = string.Concat(lines[0], string.Format(" + {0}", newAttack - Attack));
        lines[1] = string.Concat(lines[1], string.Format(" + {0:c2}", newPrice - Price));
      }

      text.AppendLine(lines[0]);
      text.AppendLine(lines[1]);

      return text.ToString();
    }

    #endregion
  }
}
