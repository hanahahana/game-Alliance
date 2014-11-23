using System;
using System.Collections.Generic;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuple<T1>
  {
    /// <summary></summary>
    protected T1 mFirst;

    /// <summary>
    /// Gets or sets the first value.
    /// </summary>
    public T1 First
    {
      get { return mFirst; }
      set { mFirst = value; }
    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value">The value to store inside the tuple.</param>
    public Tuple(T1 value)
    {
      mFirst = value;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuple<T1, T2> : Tuple<T1>
  {
    /// <summary></summary>
    protected T2 mSecond;

    /// <summary>
    /// Gets or sets the second value.
    /// </summary>
    public T2 Second
    {
      get { return mSecond; }
      set { mSecond = value; }
    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="first">The value of the first object.</param>
    /// <param name="second">The value of the second object.</param>
    public Tuple(T1 first, T2 second)
      : base(first)
    {
      mSecond = second;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuple<T1, T2, T3> : Tuple<T1, T2>
  {
    /// <summary></summary>
    protected T3 mThird;

    /// <summary>
    /// Gets or sets the third value.
    /// </summary>
    public T3 Third
    {
      get { return mThird; }
      set { mThird = value; }
    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="first">The value of the first object.</param>
    /// <param name="second">The value of the second object.</param>
    /// <param name="third">The value of the third object.</param>
    public Tuple(T1 first, T2 second, T3 third)
      : base(first, second)
    {
      mThird = third;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
  {
    /// <summary></summary>
    protected T4 mFourth;

    /// <summary>
    /// Gets or sets the fourth value.
    /// </summary>
    public T4 Fourth
    {
      get { return mFourth; }
      set { mFourth = value; }
    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="first">The value of the first object.</param>
    /// <param name="second">The value of the second object.</param>
    /// <param name="third">The value of the third object.</param>
    /// <param name="fourth">The value of the fourth object.</param>
    public Tuple(T1 first, T2 second, T3 third, T4 fourth)
      : base(first, second, third)
    {
      mFourth = fourth;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuple<T1, T2, T3, T4, T5> : Tuple<T1, T2, T3, T4>
  {
    /// <summary></summary>
    protected T5 mFifth;

    /// <summary>
    /// Gets or sets the fifth value.
    /// </summary>
    public T5 Fifth
    {
      get { return mFifth; }
      set { mFifth = value; }
    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="first">The value of the first object.</param>
    /// <param name="second">The value of the second object.</param>
    /// <param name="third">The value of the third object.</param>
    /// <param name="fourth">The value of the fourth object.</param>
    /// <param name="fifth">The value of the fifth object.</param>
    public Tuple(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
      : base(first, second, third, fourth)
    {
      mFifth = fifth;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuple<T1, T2, T3, T4, T5, T6> : Tuple<T1, T2, T3, T4, T5>
  {
    /// <summary></summary>
    protected T6 mSixth;

    /// <summary>
    /// Gets or sets the sixth value.
    /// </summary>
    public T6 Sixth
    {
      get { return mSixth; }
      set { mSixth = value; }
    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="first">The value of the first object.</param>
    /// <param name="second">The value of the second object.</param>
    /// <param name="third">The value of the third object.</param>
    /// <param name="fourth">The value of the fourth object.</param>
    /// <param name="fifth">The value of the fifth object.</param>
    /// <param name="sixth">The value of the sixth object.</param>
    public Tuple(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth)
      : base(first, second, third, fourth, fifth)
    {
      mSixth = sixth;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuple<T1, T2, T3, T4, T5, T6, T7> : Tuple<T1, T2, T3, T4, T5, T6>
  {
    /// <summary></summary>
    protected T7 mSeventh;

    /// <summary>
    /// Gets or sets the sixth value.
    /// </summary>
    public T7 Seventh
    {
      get { return mSeventh; }
      set { mSeventh = value; }
    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="first">The value of the first object.</param>
    /// <param name="second">The value of the second object.</param>
    /// <param name="third">The value of the third object.</param>
    /// <param name="fourth">The value of the fourth object.</param>
    /// <param name="fifth">The value of the fifth object.</param>
    /// <param name="sixth">The value of the sixth object.</param>
    /// <param name="seventh">The value of the seventh obhect.</param>
    public Tuple(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth, T7 seventh)
      : base(first, second, third, fourth, fifth, sixth)
    {
      mSeventh = seventh;
    }
  }
}
