using System.Collections.Generic;
using Alliance.Components;
using Alliance.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alliance
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class AllianceGame : Microsoft.Xna.Framework.Game
  {
    private GraphicsDeviceManager graphics;
    private GraphicsDevice device;
    private ResourceContentManager contentManager;
    private ScreenComponent screenManager;

    public static Dictionary<string, Image> Images = null;
    public static Dictionary<string, SpriteFont> Fonts = null;
    public static Dictionary<string, List<string>> Strings = null;
    public static Dictionary<string, Texture2D> Textures = null;
    public static GridComponent CurrentGrid = null;

    public AllianceGame()
    {
      contentManager = new ResourceContentManager(Services, ContentResources.ResourceManager);
      contentManager.RootDirectory = "Resources";
      graphics = new GraphicsDeviceManager(this);

      screenManager = new ScreenComponent(this);
      Components.Add(screenManager);

      screenManager.AddScreen(new MainMenuScreen());
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      // the mouse is visible
      IsMouseVisible = true;
      graphics.PreferredBackBufferWidth = 800;
      graphics.PreferredBackBufferHeight = 600;
      graphics.ApplyChanges();

      // initialize the dictionaries
      Images = new Dictionary<string, Image>();
      Fonts = new Dictionary<string, SpriteFont>();
      Strings = new Dictionary<string, List<string>>();
      Textures = new Dictionary<string, Texture2D>();

      // load all of the fonts
      LoadFonts();

      // load all of the images
      LoadImages();

      // load all of the strings
      LoadStrings();

      // load all of the textures
      LoadTextures();

      // enumerate through the components
      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      device = GraphicsDevice;

      // run through the components
      base.LoadContent();
    }

    private void LoadTextures()
    {
      Textures["background"] = Load<Texture2D>("background");
      Textures["blank"] = Load<Texture2D>("blank");
      Textures["gradient"] = Load<Texture2D>("gradient");
    }

    private void LoadStrings()
    {
      Strings["story"] = Load<List<string>>("story");
      Strings["wizard"] = Load<List<string>>("wizard");
    }

    private void LoadFonts()
    {
      Fonts["ComicSans"] = Load<SpriteFont>("ComicSans");
      Fonts["Tahoma"] = Load<SpriteFont>("Tahoma");
      Fonts["Verdana"] = Load<SpriteFont>("Verdana");
      Fonts["Georgia"] = Load<SpriteFont>("Georgia");
      Fonts["Arial"] = Load<SpriteFont>("Arial");
      Fonts["BookmanOldStyle"] = Load<SpriteFont>("BookmanOldStyle");
      Fonts["TimesNewRoman"] = Load<SpriteFont>("TimesNewRoman");
    }

    private void LoadImages()
    {
      Image[] images = new Image[]
      {
        // bases
        new Image(contentManager, "towerBase", false, 0, 0),

        // enemies ( all enemies are animated to avoid exceptions )
        new Image(contentManager, "walker", true, 4, 1),
        new Image(contentManager, "biker", true, 4, 1),
        new Image(contentManager, "flapper", true, 5, 1),
        new Image(contentManager, "glider", true, 4, 1),

        // towers
        new Image(contentManager, "railgun", false, 0, 0),
        new Image(contentManager, "turret", false, 0, 0),
        new Image(contentManager, "missileLauncher", false, 0, 0),
        new Image(contentManager, "shockwaveGenerator", false, 0, 0),
        new Image(contentManager, "speedbump", false, 0, 0),
        new Image(contentManager, "sprinkler", false, 0, 0),
        new Image(contentManager, "teslaCoil", true, 5, 1),
        new Image(contentManager, "machinegun", false, 0, 0),
        new Image(contentManager, "flamethrower", false, 0, 0),
        new Image(contentManager, "seeker", false, 0, 0),

        // projectiles
        new Image(contentManager, "rocket", false, 0, 0),
        new Image(contentManager, "bullet", false, 0, 0),
        new Image(contentManager, "pulse", false, 0, 0),
        new Image(contentManager, "debri", false, 0, 0),
        new Image(contentManager, "fragment", false, 0, 0),
        new Image(contentManager, "lightning", false, 0, 0),
        new Image(contentManager, "flame", false, 0, 0),
        new Image(contentManager, "flamewave", false, 0, 0),
        new Image(contentManager, "wave", false, 0, 0),
        new Image(contentManager, "lockmissile", false, 0, 0),
      };

      foreach (Image image in images)
      {
        Images[image.Key] = image;
      }
    }

    private T Load<T>(string p)
    {
      return contentManager.Load<T>(p);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
      base.Draw(gameTime);
    }
  }
}
