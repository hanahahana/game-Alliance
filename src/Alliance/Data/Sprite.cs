using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public abstract class Sprite
  {
    private BoxF previousBounds;
    private Polygon hullCache;

    /// <summary>
    /// Gets or sets the color to draw the sprite.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets the orientation of the sprite (the angle).
    /// </summary>
    public float Orientation { get; set; }

    /// <summary>
    /// Gets or sets the upper-left x coordinate of the sprite.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Gets or sets the upper-left y coordinate of the sprite.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Gets or sets the width of the sprite.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the sprite.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets the position of the sprite (X,Y)
    /// </summary>
    public Vector2 Position { get { return new Vector2(X, Y); } set { X = value.X; Y = value.Y; } }

    /// <summary>
    /// Gets or sets the size of the sprite (Width,Height)
    /// </summary>
    public SizeF Size { get { return new SizeF(Width, Height); } set { Width = value.Width; Height = value.Height; } }

    /// <summary>
    /// Gets or sets the bounds of the sprite (X,Y,Width,Height) or (Position,Size)
    /// </summary>
    public BoxF Bounds { get { return new BoxF(Position, Size); } set { Position = value.Location; Size = value.Size; } }

    /// <summary>
    /// Gets or sets the velocity of the sprite (how fast and in what direction). This is usually multipled by a scalar value reprsenting
    /// the speed.
    /// </summary>
    /// <example>X += Velocity * Scalar * elapsedSeconds.</example>
    public Vector2 Velocity { get; set; }

    /// <summary>
    /// Gets or sets the factor to apply to the velocity.
    /// </summary>
    public Vector2 VelocityFactor { get; set; }

    /// <summary>
    /// Gets the image key to use when retrieving the image for this sprite.
    /// </summary>
    public string ImageKey { get; protected set; }

    /// <summary>
    /// Gets the origin to use when drawing the sprite
    /// </summary>
    public Vector2 Origin { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public Sprite()
    {
      Orientation = 0f;
      Bounds = BoxF.Empty;
      previousBounds = Bounds;
      hullCache = null;
      VelocityFactor = Vector2.One;
      Origin = Vector2.Zero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual Texture2D GetImage()
    {
      return Program.Resources.Images[ImageKey].Texture;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual Texture2D GetDisplayImage()
    {
      return GetImage();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual Vector2[] GetImageHull()
    {
      return Program.Resources.Images[ImageKey].Hull;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected virtual Vector2 GetCenter(Vector2 offset)
    {
      // get the drawing data
      TextureDrawData data = GetTextureDrawData(offset);

      // get the center of the image
      Vector2 center = (data.TextureSize / 2f).ToVector2();

      // compute the transform
      Matrix transform = CreateTransform(data);

      // return the center transformated
      return AllianceUtilities.Transform(center, transform);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected virtual TextureDrawData GetTextureDrawData(Vector2 offset)
    {
      Texture2D image = GetImage();
      SizeF imgSize = new SizeF(image.Width, image.Height);
      Vector2 scale = MathHelper.ComputeScale(imgSize, Size);
      return new TextureDrawData(image, imgSize, Position + offset, Origin, scale);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected virtual Matrix CreateTransform(TextureDrawData data)
    {
      // create the matrix for transforming the center
      Matrix transform =
        Matrix.Translation(-data.Origin.X, -data.Origin.Y, 0) *
        Matrix.RotationZ(Orientation) *
        Matrix.Scaling(data.Scale.X, data.Scale.Y, 1f) *
        Matrix.Translation(data.Position.X, data.Position.Y, 0);

      // return the transform
      return transform;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public Polygon GetHull(Vector2 offset)
    {
      Vector2[] polygon = GetImageHull();
      if (hullCache == null || Bounds != previousBounds)
      {
        Matrix transform = CreateTransform(GetTextureDrawData(offset));
        hullCache = new Polygon(polygon, transform);
        previousBounds = Bounds;
      }
      return hullCache;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return Bounds.GetHashCode();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      Sprite polygon = obj as Sprite;
      if (polygon == null) return false;
      return polygon.GetHashCode().Equals(this.GetHashCode());
    }
  }
}
