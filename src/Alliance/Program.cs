
namespace Alliance
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main(string[] args)
    {
      using (AllianceGame game = new AllianceGame())
      {
        game.Run();
      }
    }
  }
}

