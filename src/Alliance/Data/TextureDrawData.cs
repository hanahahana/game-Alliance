namespace Alliance
{
  public class TextureDrawData
  {
    public AImage Texture;
    public APoint TextureSize;
    public APoint Position;
    public APoint Origin;
    public APoint Scale;

    public TextureDrawData(AImage texture, APoint textureSize, APoint position, APoint origin, APoint scale)
    {
      Texture = texture;
      TextureSize = textureSize;
      Position = position;
      Origin = origin;
      Scale = scale;
    }
  }
}
