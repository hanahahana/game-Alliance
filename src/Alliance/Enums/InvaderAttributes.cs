using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// 
  /// </summary>
  public enum InvaderAttributes
  {
    /// <summary>
    /// the greater the speed, the faster the invader goes, but also, the more a "propulsion" is drawn
    /// </summary>
    Speed,

    /// <summary>
    /// the greater the skill, the more abilities that the invader will get to choose from. Abilities like "teleportation"
    /// </summary>
    Skill,

    /// <summary>
    /// the greater the defense, the more hits the invader can taken and the more bulk the invader will be (this affects the scale). 
    /// (if EASY, 75% of the defense is subtracted from the attach, hard 50%, difficult 25%)
    /// </summary>
    Defense,
  };
}
