using GraphicsSystem;

namespace Alliance
{
  public class GridCellChunk
  {
    public float X, Y, Width, Height;
    public bool Valid;

    public GridCell[] Cells { get; set; }

    public GsVector Location
    {
      get { return new GsVector(X, Y); }
      set { X = value.X; Y = value.Y; }
    }

    public GsSize Size
    {
      get { return new GsSize(Width, Height); }
      set { Width = value.Width; Height = value.Height; }
    }

    public GsRectangle Bounds
    {
      get { return new GsRectangle(Location, Size); }
      set { Location = value.Location; Size = value.Size; }
    }

    public override int GetHashCode()
    {
      return Calculator.CombineHashCodes(X, Y, Width, Height);
    }
  }
}
