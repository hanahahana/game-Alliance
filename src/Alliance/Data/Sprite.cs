using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Utilities;
using Alliance.Parameters;
using Alliance.Projectiles;

namespace Alliance.Data
{
  public abstract class Sprite
  {
    private BoxF mBounds;
    private Polygon hull;

    protected float mOrientation;
    protected Color mColor;

    public Color Color
    {
      get { return mColor; }
      set { mColor = value; }
    }

    public float Orientation
    {
      get { return mOrientation; }
      protected set { mOrientation = value; }
    }

    public BoxF Bounds
    {
      get { return mBounds; }
      set 
      {
        if (value != mBounds) hull = null;
        mBounds = value; 
      }
    }

    public Vector2 Position
    {
      get { return mBounds.Location; }
      set 
      {
        if (value != mBounds.Location) hull = null;
        mBounds.Location = value; 
      }
    }

    public float X
    {
      get { return mBounds.X; }
      set 
      {
        if (value != mBounds.X) hull = null;
        mBounds.X = value; 
      }
    }

    public float Y
    {
      get { return mBounds.Y; }
      set 
      {
        if (value != mBounds.Y) hull = null;
        mBounds.Y = value; 
      }
    }

    public SizeF Size
    {
      get { return mBounds.Size; }
      set 
      {
        if (value != mBounds.Size) hull = null;
        mBounds.Size = value; 
      }
    }

    public float Width
    {
      get { return mBounds.Width; }
      set 
      {
        if (value != mBounds.Width) hull = null;
        mBounds.Width = value; 
      }
    }

    public float Height
    {
      get { return mBounds.Height; }
      set 
      {
        if (value != mBounds.Height) hull = null;
        mBounds.Height = value; 
      }
    }

    protected abstract string ImageKey { get; }
    protected abstract Vector2 Origin { get; }

    public Sprite()
    {
      mBounds = BoxF.Empty;
      mOrientation = 0f;
      hull = null;
    }

    public virtual Texture2D GetImage()
    {
      return AllianceGame.Images[ImageKey].Texture;
    }

    public virtual Texture2D GetDisplayImage()
    {
      return GetImage();
    }

    protected virtual Vector2[] GetImageHull()
    {
      return AllianceGame.Images[ImageKey].Hull;
    }

    public virtual BoxF GetBoundingBox(Vector2 offset)
    {
      // get the center of the projectile
      Vector2 center = GetCenter(offset);

      // create a rough box that has the projectile inside of it
      float dW = Width * .5f;
      float dH = Height * .5f;
      return new BoxF(
        center.X - dW,
        center.Y - dH,
        dW * 2f,
        dH * 2f);
    }

    protected virtual Vector2 GetCenter(Vector2 offset)
    {
      // get the drawing data
      DrawData data = GetDrawData(offset);

      // get the center of the image
      Vector2 center = (data.TextureSize / 2f).ToVector2();

      // compute the transform
      Matrix transform = CreateTransform(data);

      // return the center transformated
      Vector2 result;
      Vector2.Transform(ref center, ref transform, out result);
      return result;
    }

    protected virtual DrawData GetDrawData(Vector2 offset)
    {
      Texture2D image = GetImage();
      SizeF imgSize = new SizeF(image.Width, image.Height);
      Vector2 scale = Utils.ComputeScale(imgSize, Size);
      return new DrawData(image, imgSize, Position + offset, Origin, scale);
    }

    protected virtual Matrix CreateTransform(DrawData data)
    {
      // create the matrix for transforming the center
      Matrix transform =
        Matrix.CreateTranslation(-data.Origin.X, -data.Origin.Y, 0) *
        Matrix.CreateRotationZ(mOrientation) *
        Matrix.CreateScale(data.Scale.X, data.Scale.Y, 1f) *
        Matrix.CreateTranslation(data.Position.X, data.Position.Y, 0);

      // return the transform
      return transform;
    }

    public Polygon GetHull(Vector2 offset)
    {
      Vector2[] polygon = GetImageHull();
      if (hull == null)
      {
        Matrix transform = CreateTransform(GetDrawData(offset));
        hull = new Polygon(polygon, transform);
      }
      return hull;
    }

    public override int GetHashCode()
    {
      return mBounds.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Sprite polygon = obj as Sprite;
      if (polygon == null) return false;
      return polygon.GetHashCode().Equals(this.GetHashCode());
    }
  }
}
