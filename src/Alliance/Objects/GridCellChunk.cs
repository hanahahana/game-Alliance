
using Microsoft.Xna.Framework;
using MLA.Utilities.Xna;

namespace Alliance.Objects
{
  public class GridCellChunk
  {
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public bool Valid { get; set; }
    public GridCell[] Cells { get; set; }

    public Vector2 Location
    {
      get { return new Vector2(X, Y); }
      set { X = value.X; Y = value.Y; }
    }

    public SizeF Size
    {
      get { return new SizeF(Width, Height); }
      set { Width = value.Width; Height = value.Height; }
    }

    public BoxF Bounds
    {
      get { return new BoxF(Location, Size); }
      set { Location = value.Location; Size = value.Size; }
    }

    public override int GetHashCode()
    {
      string key = string.Format("{0}-{1}-{2}-{3}", X, Y, Width, Height);
      return key.GetHashCode();
    }
  }
}
