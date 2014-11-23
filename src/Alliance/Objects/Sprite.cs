using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public abstract class Sprite
  {
    private GsRectangle previousBounds;
    private GsPolygon hullCache;

    /// <summary>
    /// Gets or sets the color to draw the sprite.
    /// </summary>
    public GsColor Color { get; set; }

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
    public GsVector Position { get { return new GsVector(X, Y); } set { X = value.X; Y = value.Y; } }

    /// <summary>
    /// Gets or sets the size of the sprite (Width,Height)
    /// </summary>
    public GsSize Size { get { return new GsSize(Width, Height); } set { Width = value.Width; Height = value.Height; } }

    /// <summary>
    /// Gets or sets the bounds of the sprite (X,Y,Width,Height) or (Position,Size)
    /// </summary>
    public GsRectangle Bounds { get { return new GsRectangle(Position, Size); } set { Position = value.Location; Size = value.Size; } }

    /// <summary>
    /// Gets or sets the velocity of the sprite (how fast and in what direction). This is usually multipled by a scalar value reprsenting
    /// the speed.
    /// </summary>
    /// <example>X += Velocity * Scalar * elapsedSeconds.</example>
    public GsVector Velocity { get; set; }

    /// <summary>
    /// Gets or sets the factor to apply to the velocity.
    /// </summary>
    public GsVector VelocityFactor { get; set; }

    /// <summary>
    /// Gets the image key to use when retrieving the image for this sprite.
    /// </summary>
    public string ImageKey { get; protected set; }

    /// <summary>
    /// Gets the origin to use when drawing the sprite
    /// </summary>
    public GsVector Origin { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public Sprite()
    {
      Orientation = 0f;
      Bounds = GsRectangle.Empty;
      previousBounds = Bounds;
      hullCache = null;
      VelocityFactor = GsVector.One;
      Origin = GsVector.Zero;
    }

    /// <summary></summary>
    public virtual GsImage GetImage()
    {
      return ImageProvider.GetFramedImage(ImageKey).Image;
    }

    /// <summary></summary>
    public virtual GsImage GetDisplayImage()
    {
      return GetImage();
    }

    /// <summary></summary>
    protected virtual GsVector[] GetImageHull()
    {
      return ImageProvider.GetFramedImage(ImageKey).Hull;
    }

    /// <summary></summary>
    public virtual GsRectangle GetBoundingBox(GsVector offset)
    {
      // get the center of the projectile
      GsVector center = GetCenter(offset);

      // create a rough box that has the projectile inside of it
      float dW = Width * .5f;
      float dH = Height * .5f;
      return new GsRectangle(
        center.X - dW,
        center.Y - dH,
        dW * 2f,
        dH * 2f);
    }

    /// <summary></summary>
    protected virtual GsVector GetCenter(GsVector offset)
    {
      // get the drawing data
      ImageParams data = GetTextureDrawData(offset);

      // get the center of the image
      GsVector center = data.ImageSize.ToVector() / 2f;

      // compute the transform
      GsMatrix transform = CreateTransform(data);

      // return the center transformated
      return GsVector.Transform(center, transform);
    }

    /// <summary></summary>
    protected virtual ImageParams GetTextureDrawData(GsVector offset)
    {
      var image = GetImage();
      var imgSize = ImageProvider.GetSize(image);
      var scale = Calculator.ComputeScale(imgSize, Size);
      return new ImageParams(image, imgSize, Position + offset, Origin, scale);
    }

    /// <summary></summary>
    protected virtual GsMatrix CreateTransform(ImageParams data)
    {
      // create the matrix for transforming the center
      GsMatrix transform =
        GsMatrix.CreateTranslation(-data.Origin.X, -data.Origin.Y, 0) *
        GsMatrix.CreateRotationZ(Orientation) *
        GsMatrix.CreateScale(data.Scale.X, data.Scale.Y, 1f) *
        GsMatrix.CreateTranslation(data.Position.X, data.Position.Y, 0);

      // return the transform
      return transform;
    }

    /// <summary></summary>
    public GsPolygon GetHull(GsVector offset)
    {
      GsVector[] polygon = GetImageHull();
      if (hullCache == null || Bounds != previousBounds)
      {
        GsMatrix transform = CreateTransform(GetTextureDrawData(offset));
        hullCache = new GsPolygon(polygon, transform);
        previousBounds = Bounds;
      }
      return hullCache;
    }

    /// <summary></summary>
    public override int GetHashCode()
    {
      return Bounds.GetHashCode();
    }

    /// <summary></summary>
    public override bool Equals(object obj)
    {
      Sprite polygon = obj as Sprite;
      if (polygon == null) return false;
      return polygon.GetHashCode().Equals(this.GetHashCode());
    }
  }
}
