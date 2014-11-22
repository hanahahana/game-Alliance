using System;

namespace Alliance
{
  public abstract class Sprite
  {
    private ARect previousBounds;
    private Polygon hullCache;

    /// <summary>
    /// Gets or sets the color to draw the sprite.
    /// </summary>
    public AColor Color { get; set; }

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
    public APoint Position { get { return new APoint(X, Y); } set { X = value.X; Y = value.Y; } }

    /// <summary>
    /// Gets or sets the size of the sprite (Width,Height)
    /// </summary>
    public ASize Size { get { return new ASize(Width, Height); } set { Width = value.Width; Height = value.Height; } }

    /// <summary>
    /// Gets or sets the bounds of the sprite (X,Y,Width,Height) or (Position,Size)
    /// </summary>
    public ARect Bounds { get { return new ARect(Position, Size); } set { Position = value.Location; Size = value.Size; } }

    /// <summary>
    /// Gets or sets the velocity of the sprite (how fast and in what direction). This is usually multipled by a scalar value reprsenting
    /// the speed.
    /// </summary>
    /// <example>X += Velocity * Scalar * elapsedSeconds.</example>
    public APoint Velocity { get; set; }

    /// <summary>
    /// Gets or sets the factor to apply to the velocity.
    /// </summary>
    public APoint VelocityFactor { get; set; }

    /// <summary>
    /// Gets the image key to use when retrieving the image for this sprite.
    /// </summary>
    public string ImageKey { get; protected set; }

    /// <summary>
    /// Gets the origin to use when drawing the sprite
    /// </summary>
    public APoint Origin { get; protected set; }

    public Sprite()
    {
      Orientation = 0f;
      Bounds = ARect.Empty;
      previousBounds = Bounds;
      hullCache = null;
      VelocityFactor = APoint.One;
      Origin = APoint.Zero;
    }

    public virtual AImage GetImage()
    {
      return AllianceSystem.Images[ImageKey];
    }

    public virtual AImage GetDisplayImage()
    {
      return GetImage();
    }

    protected virtual APoint[] GetImageHull()
    {
      return AllianceSystem.Images[ImageKey].Hull;
    }

    public virtual ARect GetBoundingBox(APoint offset)
    {
      // get the center of the projectile
      APoint center = GetCenter(offset);

      // create a rough box that has the projectile inside of it
      float dW = Width * .5f;
      float dH = Height * .5f;
      return new ARect(
        center.X - dW,
        center.Y - dH,
        dW * 2f,
        dH * 2f);
    }

    protected virtual APoint GetCenter(APoint offset)
    {
      // get the drawing data
      TextureDrawData data = GetTextureDrawData(offset);

      // get the center of the image
      var center = (data.TextureSize / 2f);

      // compute the transform
      AMatrix transform = CreateTransform(data);

      // return the center transformated
      return APoint.Transform(center, transform);
    }

    protected virtual TextureDrawData GetTextureDrawData(APoint offset)
    {
      AImage image = GetImage();
      var imgSize = new APoint(image.Width, image.Height);
      var scale = AMath.ComputeScale(imgSize, Size);
      return new TextureDrawData(image, imgSize, Position + offset, Origin, scale);
    }

    protected virtual AMatrix CreateTransform(TextureDrawData data)
    {
      // create the matrix for transforming the center
      AMatrix transform =
        AMatrix.CreateTranslation(-data.Origin.X, -data.Origin.Y, 0) *
        AMatrix.CreateRotationZ(Orientation) *
        AMatrix.CreateScale(data.Scale.X, data.Scale.Y, 1f) *
        AMatrix.CreateTranslation(data.Position.X, data.Position.Y, 0);

      // return the transform
      return transform;
    }

    public Polygon GetHull(APoint offset)
    {
      var polygon = GetImageHull();
      if (hullCache == null || Bounds != previousBounds)
      {
        AMatrix transform = CreateTransform(GetTextureDrawData(offset));
        hullCache = new Polygon(polygon, transform);
        previousBounds = Bounds;
      }
      return hullCache;
    }

    public override int GetHashCode()
    {
      return Bounds.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Sprite polygon = obj as Sprite;
      if (polygon == null) return false;
      return polygon.GetHashCode().Equals(this.GetHashCode());
    }
  }
}
