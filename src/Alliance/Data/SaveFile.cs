using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Alliance.Objects;

namespace Alliance.Data
{
  public static class SaveFile
  {
    const string SaveFilePrefix = "DE7978AE8645488a90EB337DE751B137SLT";
    const string SaveFileExtension = ".alliance";

    private static string mDirectory;
    public static List<SaveData> Saves { get; private set; }

    static SaveFile()
    {
      mDirectory = string.Format("{0}\\Alliance\\", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
      Saves = new List<SaveData>();

      if (!Directory.Exists(mDirectory))
        Directory.CreateDirectory(mDirectory);
    }

    public static void Load()
    {
      Saves.Clear();

      DirectoryInfo info = new DirectoryInfo(mDirectory);
      FileInfo[] files = info.GetFiles(string.Format("*{0}", SaveFileExtension), SearchOption.AllDirectories);

      BinaryFormatter formatter = new BinaryFormatter();
      foreach (FileInfo file in files)
      {
        bool delete = false;
        using (Stream stream = file.OpenRead())
        {
          try
          {
            SaveData data = (SaveData)formatter.Deserialize(stream);
            Saves.Add(data);
          }
          catch (Exception)
          {
            delete = true;
          }
        }

        if (delete)
        {
          file.Delete();
        }
      }
    }

    public static void LoadSaveData(int slotIndex)
    {
      SaveData data = Saves.Find(s => s.Index == slotIndex);
      Player.Load(data);
      AllianceGame.CurrentGrid.LoadSaveData(data.GridComponent);
    }

    public static void Save(int slotIndex)
    {
      SaveData data = new SaveData
      {
        Cash = Player.Cash,
        Civilians = Player.Civilians,
        DateTime = DateTime.Now,
        GridComponent = AllianceGame.CurrentGrid.GetSaveData(),
        Index = slotIndex,
        PlayerState = Player.State,
        Level = Player.Level,
        Experience = Player.Experience,
        ExperienceNeeded = Player.ExperienceNeeded,
        TimeUntilInvadersArrive = Player.TimeUntilInvadersArrive,
      };

      string filepath = string.Format("{0}\\{1}{2}{3}", mDirectory, SaveFilePrefix, slotIndex, SaveFileExtension);
      using (Stream stream = File.Open(filepath, FileMode.Create))
      {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, data);
      }

      Saves.RemoveAll(s => s.Index == slotIndex);
      Saves.Add(data);
    }

    public static void ClearSaveData(int slotIndex)
    {
      Saves.RemoveAll(s => s.Index == slotIndex);
      string filepath = string.Format("{0}\\{1}{2}{3}", mDirectory, SaveFilePrefix, slotIndex, SaveFileExtension);
      File.Delete(filepath);
    }
  }
}
