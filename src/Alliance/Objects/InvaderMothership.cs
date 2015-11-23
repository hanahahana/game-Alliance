using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alliance.Enums;
using Alliance.Invaders;
using Alliance.Pathfinding;
using Alliance.Pieces;
using Microsoft.Xna.Framework;
using MLA.Utilities;
using MLA.Utilities.Helpers;
using MLA.Utilities.Collections;

using Destination = MLA.Utilities.Tuple<Alliance.Objects.GridCell,Alliance.Objects.GridCell>;

namespace Alliance.Objects
{
  [Serializable]
  public class InvaderMothership
  {
    private int mMadeItCount = 0;
    private int mDestroyedCount = 0;

    public float InvaderLevel { get; private set; }
    public bool SentAllInvaders { get; private set; }

    public InvaderMothership()
    {
      // initialize the level to 1
      InvaderLevel = 1;
      
      // initialize the sent to false
      SentAllInvaders = false;
    }

    public void ReportBackIn(Invader invader)
    {
      // here, if the invader made it, then add it's experience to the experience pool
      if (invader.State == InvaderState.MadeIt)
      {
        ++mMadeItCount;
      }
      else if (invader.State == InvaderState.Dead)
      {
        ++mDestroyedCount;
      }
    }

    public IEnumerable<Invader> GenerateWave(Destination horizontal, Destination vertical, IEnumerable<Piece> pieces, GridCell[,] grid)
    {
      // create a list to store the invaders created
      List<Invader> retval = new List<Invader>();

      // if the invader level is past the maximum, then return an empty invader list
      if (SentAllInvaders)
        return retval;

      // TODO: This could be something to adjust based on the difficulty
      // decide how many invaders we're going to send
      int invaderCount = 10;

      // create a class that holds the default weights of everything
      InvaderLevelUpInfo info = new InvaderLevelUpInfo(pieces, mMadeItCount, mDestroyedCount);

      // now then, create a loop to generate the invaders
      for (int i = 0; i < invaderCount; ++i)
      {
        // determine the boolean properties for the invader
        bool flying = RandomHelper.NextBool();
        bool leftRight = RandomHelper.NextBool();

        // get the start/goal and key
        GridCell start = leftRight ? horizontal.First : vertical.First;
        GridCell goal = leftRight ? horizontal.Second : vertical.Second;
        DijkstraType key = leftRight ? DijkstraType.LeftToRight : DijkstraType.TopToBottom;

        // create a base invader. This invader will have a 50% chance of flying.
        Invader invader = new Invader(this, flying);

        // set the experience of the invader given the weights
        invader.LevelUp((InvaderLevel * 10), InvaderLevel, info);

        // send the invader to the map
        invader.BriefOnMission(start, goal, key);

        // next, add the invader to the list
        retval.Add(invader);
      }

      // increase the level
      ++InvaderLevel;

      // if we increased the level to the maximum invader level, then we've sent all of the invaders
      if (InvaderLevel > Invader.MaxInvaderLevel)
      {
        SentAllInvaders = true;
        InvaderLevel = Invader.MaxInvaderLevel;
      }

      // return the invaders
      return retval;
    }
  }
}
