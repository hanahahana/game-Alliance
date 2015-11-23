using System;
using System.Collections.Generic;
using System.Text;

namespace Alliance.Pieces
{
  public class MissilePiece : Piece
  {
    private const string MissileName = "Missile";
    private string mDescription;
    private float mRadius;

    public MissilePiece()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Slams multiple missiles into the enemy. They explode on contact and affect the surrounding pieces.");
      sb.AppendLine(" Missiles are best with flying enemies but can sometimes damage friendly units.");
      mDescription = sb.ToString();
      mRadius = 80;
    }

    public override string Description
    {
      get { return mDescription; }
    }

    public override string Name
    {
      get { return MissileName; }
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
      MissilePiece piece = new MissilePiece();
      return piece;
    }
  }
}
