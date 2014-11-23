using SharpDX;
using SharpDX.Toolkit;
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
    public GameTime GameTime { get; private set; }

    /// <summary>
    /// Gets the input provider service.
    /// </summary>
    public InputState Input { get; private set; }

    /// <summary>
    /// Gets the offset to use when updating the position of objects.
    /// </summary>
    public Vector2 Offset { get; private set; }

    /// <summary>
    /// Creates update parameters containing the necessary objects.
    /// </summary>
    /// <param name="gameTime">The game time to pass on.</param>
    /// <param name="input">The input provider to pass on.</param>
    /// <param name="offset">The global offset pass on.</param>
    public UpdateParams(GameTime gameTime, InputState input, Vector2 offset)
    {
      GameTime = gameTime;
      Input = input;
      Offset = offset;
    }
  }
}
