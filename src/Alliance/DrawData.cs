using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Alliance.Data;

namespace Alliance
{
  public class DrawData
  {
    private Texture2D mTexture;
    public Texture2D Texture
    {
      get { return mTexture; }
    }

    private SizeF mTextureSize;
    public SizeF TextureSize
    {
      get { return mTextureSize; }
    }

    private Vector2 mPosition;
    public Vector2 Position
    {
      get { return mPosition; }
    }

    private Vector2 mOrigin;
    public Vector2 Origin
    {
      get { return mOrigin; }
    }

    private Vector2 mScale;
    public Vector2 Scale
    {
      get { return mScale; }
    }

    public DrawData(Texture2D texture, SizeF textureSize, Vector2 position, Vector2 origin, Vector2 scale)
    {
      mTexture = texture;
      mTextureSize = textureSize;
      mPosition = position;
      mOrigin = origin;
      mScale = scale;
    }
  }
}
