using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// Represents parameters passed to methods to perform updates.
  /// </summary>
  public class UpdateParams
  {
    /// <summary>
    /// Gets the game time coming to apply to each item being updated.
    /// </summary>
    public TimeSpan Elapsed { get; private set; }

    /// <summary>
    /// Gets the input provider service.
    /// </summary>
    public IAllianceInputState Input { get; private set; }

    /// <summary>
    /// Gets the offset to use when updating the position of objects.
    /// </summary>
    public GsVector Offset { get; private set; }

    /// <summary>
    /// Creates update parameters containing the necessary objects.
    /// </summary>
    /// <param name="elapsed">The game time to pass on.</param>
    /// <param name="input">The input provider to pass on.</param>
    /// <param name="offset">The global offset pass on.</param>
    public UpdateParams(TimeSpan elapsed, IAllianceInputState input, GsVector offset)
    {
      Elapsed = elapsed;
      Input = input;
      Offset = offset;
    }
  }
}
