using System;

namespace Alliance
{
  /// <summary>
  /// Holds data specific to a single player.
  /// </summary>
  public static class Player
  {
    /// <summary>
    /// This is the experience needed to level up.
    /// </summary>
    public const int ExpFactor = 100;

    /// <summary>
    /// This is the maximum level that the player can reach.
    /// </summary>
    public const int MaximumLevel = 100;

    /// <summary>
    /// Gets the amount of money that the player has.
    /// </summary>
    public static float Cash { get; private set; }

    /// <summary>
    /// Gets the amount of experience that the player has.
    /// </summary>
    public static float Experience { get; private set; }

    /// <summary>
    /// Gets the amount of experience that the player needs to level up.
    /// </summary>
    public static float ExperienceNeeded { get; private set; }

    /// <summary>
    /// Gets the civilians that the player has.
    /// </summary>
    public static int Civilians { get; private set; }

    /// <summary>
    /// Gets the current state of the player.
    /// </summary>
    public static PlayerState State { get; private set; }

    /// <summary>
    /// Gets the current level of the player.
    /// </summary>
    public static int Level { get; private set; }

    /// <summary>
    /// Gets the current time until the invaders arrive
    /// </summary>
    public static TimeSpan TimeUntilInvadersArrive { get; set; }

    /// <summary>
    /// Gets a value indicating if the player is alive.
    /// </summary>
    public static bool IsAlive { get { return Civilians > 0; } }

    public static void InitializePlayer(int initialCash, int civilians)
    {
      Cash = initialCash;
      Civilians = civilians;
      State = PlayerState.Designing;
      Level = 1;
      Experience = 0;
      ExperienceNeeded = (Level * Level * ExpFactor);

      // at first, the invaders take 2 minutes to arrive
      TimeUntilInvadersArrive = TimeSpan.FromMinutes(2.0);
    }

    public static bool PurchasePiece(Piece piece)
    {
      float newCash = Cash - piece.Price;
      if (newCash >= 0)
      {
        Cash = newCash;
      }
      return newCash >= 0;
    }

    public static void SellPiece(Piece piece)
    {
      float cash = piece.Price;
      if (State == PlayerState.Designing)
        cash = piece.GetLifetimePrice();
      Cash += cash;
    }

    public static void InvaderGotThrough(Invader invader)
    {
      --Civilians;
    }

    public static bool EnoughCashFor(Piece piece)
    {
      return (Cash - piece.Price) >= 0;
    }

    public static void TriggerDesignPhaseOver()
    {
      State = PlayerState.Running;
    }

    public static void CollectSpoils(Invader invader)
    {
      // update the cash and the invaders
      Cash += invader.Value;
      Experience += invader.Experience;

      // if we have enough experience and we aren't at the maximum level
      if (Experience >= ExperienceNeeded && Level < MaximumLevel)
      {
        // level up
        ++Level;

        // set the experience needed
        ExperienceNeeded = Level * Level * ExpFactor;
      }
    }
  }
}
