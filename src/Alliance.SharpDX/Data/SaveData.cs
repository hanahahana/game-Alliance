using System;
using System.Collections.Generic;

namespace Alliance
{
  [Serializable]
  public class SaveData
  {
    public int Index { get; set; }
    public DateTime DateTime { get; set; }
    public float Cash { get; set; }
    public float Experience { get; set; }
    public float ExperienceNeeded { get; set; }
    public int Civilians { get; set; }
    public PlayerState PlayerState { get; set; }
    public int Level { get; set; }
    public TimeSpan TimeUntilInvadersArrive { get; set; }
    public Dictionary<string, object> GridComponent { get; set; }

    public override string ToString()
    {
      return string.Format("Save File {0}: {1}\r\nLvl {2} - {3:c2} - {4} - {5:mm:ss}", 
        Index + 1, 
        DateTime, 
        Level, 
        Cash, 
        Civilians,
        new DateTime(TimeUntilInvadersArrive.Ticks));
    }
  }
}
