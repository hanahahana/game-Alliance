using SharpDX;
using SharpDX.Toolkit.Graphics;
namespace Alliance
{
  public class TextureDrawData
  {
    public Texture2D Texture { get; private set; }
    public SizeF TextureSize { get; private set; }
    public Vector2 Position { get; private set; }
    public Vector2 Origin { get; private set; }
    public Vector2 Scale { get; private set; }

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
