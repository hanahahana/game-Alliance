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
    public int C, R;

    public Index(int col, int row)
    {
      C = col;
      R = row;
    }

    public override bool Equals(object obj)
    {
      var idx = obj as Index?;
      if (idx == null)
      {
        return false;
      }

      Index index = idx.Value;
      return index.C == C && index.R == R;
    }

    public override int GetHashCode()
    {
      return C ^ R;
    }

    public bool IsValid(int rowCount, int columnCount)
    {
      if (C < 0) return false;
      if (R < 0) return false;

      if (C >= columnCount) return false;
      if (R >= rowCount) return false;

      return true;
    }
  }
}
