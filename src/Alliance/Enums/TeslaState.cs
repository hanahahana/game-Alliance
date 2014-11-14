
namespace Alliance
{
  /// <summary>
  /// The state of the tesla coil. This is public in-case the code needs to be extended to
  /// use this enumeration.
  /// </summary>
  public enum TeslaState
  {
    /// <summary></summary>
    Idle,

    /// <summary></summary>
    Charging,

    /// <summary></summary>
    Firing,

    /// <summary></summary>
    Discharging,
  }
}
