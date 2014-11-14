using System.Collections.Generic;
using System.Linq;

namespace Alliance
{
  /// <summary>
  /// A class representing a selected piece. The selected piece can be drawn over the grid.
  /// </summary>
  public class Selection
  {
    private static readonly Color ValidSelectionColor = ColorHelper.NewAlpha(Color.Green, .5f);
    private static readonly Color InvalidSelectionColor = ColorHelper.NewAlpha(Color.Red, .5f);

    private Dictionary<GridCellChunk, byte> chunksBeingEdited = new Dictionary<GridCellChunk, byte>();

    public GridCellChunk Chunk { get; set; }
    public GridCellChunk StartingChunk { get; set; }
    public BoxF Bounds { get; set; }

    public IEnumerable<GridCellChunk> Edits 
    {
      get
      {
        List<GridCellChunk> edits = new List<GridCellChunk>();
        if (StartingChunk != null)
          edits.Add(StartingChunk);

        edits.AddRange(chunksBeingEdited.Keys.ToArray());
        edits.Sort((a, b) => AllianceUtilities.CompareVector2(a.Location, b.Location));
        return edits;
      }
    }

    public void Draw(DrawParams dparams)
    {
      if (Chunk != null)
      {
        Color color = Chunk.Valid ? Color.Green : Color.Red;
        BoxF bounds = new BoxF(Chunk.Location + dparams.Offset, Chunk.Size);

        dparams.Graphics.FillRectangle(bounds, ColorHelper.NewAlpha(color, 100));
        dparams.Graphics.DrawRectangle(bounds, color);

        // draw the chunks being edited
        foreach (GridCellChunk item in chunksBeingEdited.Keys)
        {
          Color itemClr = item.Valid ? Color.Green : Color.Red;
          BoxF itemBounds = new BoxF(item.Location + dparams.Offset, item.Size);

          dparams.Graphics.FillRectangle(itemBounds, ColorHelper.NewAlpha(itemClr, 100));
          dparams.Graphics.DrawRectangle(itemBounds, itemClr);
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
