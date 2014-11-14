namespace Alliance.Data
{
  /// <summary>
  /// 
  /// </summary>
  public class TextureDrawData
  {
    /// <summary>
    /// 
    /// </summary>
    public Texture2D Texture { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public SizeF TextureSize { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public Vector2 Origin { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public Vector2 Scale { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="textureSize"></param>
    /// <param name="position"></param>
    /// <param name="origin"></param>
    /// <param name="scale"></param>
    public TextureDrawData(Texture2D texture, SizeF textureSize, Vector2 position, Vector2 origin, Vector2 scale)
    {
      Texture = texture;
      TextureSize = textureSize;
      Position = position;
      Origin = origin;
      Scale = scale;
    }
  }
}
