using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Components;

namespace Alliance.Parameters
{
  public class DrawParams
  {
    private GameTime mGameTime;
    public GameTime GameTime
    {
      get { return mGameTime; }
    }

    private Vector2 mOffset;
    public Vector2 Offset
    {
      get { return mOffset; }
    }

    private SpriteBatch mSpriteBatch;
    public SpriteBatch SpriteBatch
    {
      get { return mSpriteBatch; }
    }

    private GridFillMode mFillMode;
    public GridFillMode FillMode
    {
      get { return mFillMode; }
    }

    public DrawParams(GameTime gameTime, Vector2 offset, SpriteBatch spriteBatch, GridFillMode gridFillMode)
    {
      mGameTime = gameTime;
      mOffset = offset;
      mSpriteBatch = spriteBatch;
      mFillMode = gridFillMode;
    }
  }
}
