using System.Collections.Generic;
using System.Linq;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// A class representing a selected piece. The selected piece can be drawn over the grid.
  /// </summary>
  public class Selection
  {
    private static readonly GsColor ValidSelectionColor = new GsColor(GsColor.Green, 128);
    private static readonly GsColor InvalidSelectionColor = new GsColor(GsColor.Red, 128);

    private Dictionary<GridCellChunk, byte> chunksBeingEdited = new Dictionary<GridCellChunk, byte>();

    public GridCellChunk Chunk { get; set; }
    public GridCellChunk StartingChunk { get; set; }
    public GsRectangle Bounds { get; set; }

    public IEnumerable<GridCellChunk> Edits 
    {
      get
      {
        List<GridCellChunk> edits = new List<GridCellChunk>();
        if (StartingChunk != null)
          edits.Add(StartingChunk);

        edits.AddRange(chunksBeingEdited.Keys.ToArray());
        edits.Sort((a, b) => a.Location.CompareTo(b.Location));
        return edits;
      }
    }

    public void Draw(DrawParams dparams)
    {
      if (Chunk != null)
      {
        GsColor color = Chunk.Valid ? GsColor.Green : GsColor.Red;
        GsRectangle bounds = new GsRectangle(Chunk.Location + dparams.Offset, Chunk.Size);

        dparams.Graphics.FillRectangle(new GsColor(color, 100), bounds);
        dparams.Graphics.DrawRectangle(color, bounds);

        // draw the chunks being edited
        foreach (GridCellChunk item in chunksBeingEdited.Keys)
        {
          GsColor itemClr = item.Valid ? GsColor.Green : GsColor.Red;
          GsRectangle itemBounds = new GsRectangle(item.Location + dparams.Offset, item.Size);

          dparams.Graphics.FillRectangle(new GsColor(itemClr, 100), itemBounds);
          dparams.Graphics.DrawRectangle(itemClr, itemBounds);
        }
      }
    }

    public void ClearEdits()
    {
      // clear away the chunks being edited
      chunksBeingEdited.Clear();
    }

    public void EndPlaceEdits()
    {
      // clear the chunks being edited
      chunksBeingEdited.Clear();

      // there is no starting chunk
      StartingChunk = null;
    }

    public void AddChunk(GridCellChunk item)
    {
      chunksBeingEdited.Add(item, 0x00);
    }
  }
}
