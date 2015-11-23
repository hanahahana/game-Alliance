using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alliance.Pieces
{
  public class ShockwavePiece : Piece
  {
    const string ShockwaveName = "Shockwave";
    private string mDescription;

    public ShockwavePiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Creates a shockwave to send at the enemy!");
      mDescription = sb.ToString();
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return ShockwaveName; }
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      ShockwavePiece piece = new ShockwavePiece();
      return piece;
    }
  }
}
