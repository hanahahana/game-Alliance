using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  [Serializable]
  public struct Index
  {
    public static readonly Index Empty;
    private int mCol;
    private int mRow;
    public bool IsEmpty
    {
      get
      {
        return this.mCol == 0 && this.mRow == 0;
      }
    }

    public int C
    {
      get
      {
        return this.mCol;
      }
      set
      {
        this.mCol = value;
      }
    }

    public int R
    {
      get
      {
        return this.mRow;
      }
      set
      {
        this.mRow = value;
      }
    }

    public Index(int col, int row)
    {
      this.mCol = col;
      this.mRow = row;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is Index))
      {
        return false;
      }
      Index index = (Index)obj;
      return index.C == this.C && index.R == this.R;
    }

    public override int GetHashCode()
    {
      return this.mCol ^ this.mRow;
    }

    public bool IsValid(int rowCount, int columnCount)
    {
      return ArithmeticHelper.InRange(this.C, 0, columnCount - 1) && ArithmeticHelper.InRange(this.R, 0, rowCount - 1);
    }

    static Index()
    {
      Index.Empty = default(Index);
    }
  }
}
