using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Alliance.Components;

namespace Alliance.Parameters
{
  public class UpdateParams
  {
    private GameTime mGameTime;
    public GameTime GameTime
    {
      get { return mGameTime; }
    }

    private InputProvider mInput;
    public InputProvider Input
    {
      get { return mInput; }
    }

    private Vector2 mOffset;
    public Vector2 Offset
    {
      get { return mOffset; }
    }

    public UpdateParams(GameTime gameTime, InputProvider input, Vector2 offset)
    {
      mGameTime = gameTime;
      mInput = input;
      mOffset = offset;
    }
  }
}
