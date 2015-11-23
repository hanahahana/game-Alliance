using System;
using System.Collections.Generic;
using System.Text;

namespace Alliance.Pieces
{
  public class SpeedBumpPiece : Piece
  {
    private const string SpeedBumpName = "Speed Bump";
    private string mDescription;
    private float mRadius;

    public SpeedBumpPiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Slows the enemy down by threatening to destroy their transportation.");
      sb.AppendLine(" Careful, some enemies can roll right over them without feeling anything!");
      mDescription = sb.ToString();
      mLevel = Piece.MaxLevel;
      mRadius = 20;
    }

    public override bool IsBlocking
    {
      get { return false; }
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return SpeedBumpName; }
    }

    public override PieceGrouping Grouping
    {
      get { return PieceGrouping.One; }
    }

    public override float Radius
    {
      get { return mRadius; }
    }

    protected override Piece CreatePiece(Cell[] cells)
    {
      SpeedBumpPiece piece = new SpeedBumpPiece();
      return piece;
    }
  }
}
