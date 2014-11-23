using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  /// <summary>
  /// Defines methods used for displaying a header and then a body of text. This is used
  /// when displaying information about a piece, a projectile, a tower; etc.
  /// </summary>
  public interface ITextDisplay
  {
    /// <summary>
    /// Get the header text. This will be used as the caption of a specific item.
    /// </summary>
    /// <returns>The header text.</returns>
    string GetHeader();

    /// <summary>
    /// Get the body text. This will be used as the description of a specific item.
    /// </summary>
    /// <returns>The body text.</returns>
    string GetText();
  }
}
