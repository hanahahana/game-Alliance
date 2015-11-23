using System;
using System.Collections.Generic;
using System.Text;

using Alliance.Helpers;

namespace Alliance.Data
{
  public class DijkstraData
  {
    private int mDistance = int.MaxValue;
    private Cell mParent = null;

    public int Distance
    {
      get { return mDistance; }
      set { mDistance = value; }
    }

    public Cell Parent
    {
      get { return mParent; }
      set { mParent = value; }
    }

    public void Reset()
    {
      mDistance = int.MaxValue;
      mParent = null;
    }
  }
}
