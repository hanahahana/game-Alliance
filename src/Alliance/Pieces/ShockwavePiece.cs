using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alliance.Pieces
{
  public class ShockwavePiece : Piece
  {
    private const string ShockwaveName = "Shockwave";
    private string mDescription;
    private float mRadius;

    public ShockwavePiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Creates a shockwave to send at the enemy!");
      mDescription = sb.ToString();
      mRadius = 50;
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return ShockwaveName; }
    }

    public override PieceGrouping Grouping
    {
      get { return PieceGrouping.Two; }
    }

    public override float Radius
    {
      get { return mRadius; }
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      ShockwavePiece piece = new ShockwavePiece();
      return piece;
    }
  }
}
