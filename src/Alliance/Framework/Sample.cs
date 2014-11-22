using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public class Sample : IComparable<Sample>, IComparable, IEquatable<Sample>
  {
    private double mX;
    private double mY;
    public double X
    {
      get
      {
        return this.mX;
      }
      set
      {
        this.mX = value;
      }
    }
    public double Y
    {
      get
      {
        return this.mY;
      }
      set
      {
        this.mY = value;
      }
    }
    public Sample()
      : this(0.0, 0.0)
    {
    }
    public Sample(double x, double y)
    {
      this.mX = x;
      this.mY = y;
    }
    public override int GetHashCode()
    {
      return this.X.GetHashCode();
    }
    public int CompareTo(Sample other)
    {
      return this.mX.CompareTo(other.mX);
    }
    public int CompareTo(object obj)
    {
      Sample sample = obj as Sample;
      if (sample == null)
      {
        return 0;
      }
      return this.CompareTo(sample);
    }
    public bool Equals(Sample other)
    {
      return this.X.Equals(other.X);
    }
    public override bool Equals(object obj)
    {
      Sample sample = obj as Sample;
      return sample != null && this.Equals(sample);
    }
  }
}
