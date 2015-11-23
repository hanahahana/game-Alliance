using System;
using System.Collections.Generic;
using System.Text;
using Alliance.Pieces;
using Alliance.Entities;

namespace Alliance
{
  public enum PlayerState
  {
    Designing,
    Running,
  };

  public static class Player
  {
    private static int mCash = 0;
    private static int mLife = 0;
    private static PlayerState mState;

    public static int Cash
    {
      get { return mCash; }
    }

    public static int Life
    {
      get { return mLife; }
    }

    public static bool IsAlive
    {
      get { return mLife > 0; }
    }

    public static PlayerState State
    {
      get { return mState; }
    }

    public static void InitializePlayer(int initialCash, int initialLife)
    {
      mCash = initialCash;
      mLife = initialLife;
      mState = PlayerState.Designing;
    }

    public static bool PurchasePiece(Piece piece)
    {
      int newCash = mCash - piece.Price;
      if (newCash >= 0)
      {
        mCash = newCash;
      }
      return newCash >= 0;
    }

    public static void SellPiece(Piece piece)
    {
      int cash = piece.Price;
      if (mState == PlayerState.Designing)
        cash = piece.GetLifetimePrice();
      mCash += cash;
    }

    public static void InvaderGotThrough(Entity entity)
    {
      --mLife;
    }

    public static bool EnoughCashFor(Piece piece)
    {
      return (mCash - piece.Price) >= 0;
    }

    public static void CollectSpoils(Entity entity)
    {
      mCash += entity.Cash;
    }
  }
}
