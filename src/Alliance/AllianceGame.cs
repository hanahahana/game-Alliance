using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Alliance.Utilities;
using Alliance.Components;

namespace Alliance
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class AllianceGame : Microsoft.Xna.Framework.Game
  {
    private GraphicsDeviceManager graphics;
    private GraphicsDevice device;
    private SpriteBatch spriteBatch;
    private ResourceContentManager contentManager;

    private MessageComponent messages;
    private GridComponent grid;
    private InputProvider input;
    private PlayerHudComponent player;

    public static Dictionary<string, Image> Images = null;
    public static Dictionary<string, SpriteFont> Fonts = null;

    public AllianceGame()
    {
      contentManager = new ResourceContentManager(Services, ContentResources.ResourceManager);
      contentManager.RootDirectory = "Resources";

      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      grid = new GridComponent(this);
      Components.Add(grid);

      messages = MessageComponent.CreateInstance(this);
      Components.Add(messages);

      input = new InputProvider(this);
      Components.Add(input);

      player = new PlayerHudComponent(this);
      Components.Add(player);

      Services.AddService(typeof(InputProvider), input);
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      // initialize the dictionaries
      Images = new Dictionary<string, Image>();
      Fonts = new Dictionary<string, SpriteFont>();

      // load all of the fonts
      LoadFonts();

      // load all of the images
      LoadImages();

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
      spriteBatch = new SpriteBatch(device);

      // run through the components
      base.LoadContent();
    }

    private void LoadFonts()
    {
      Fonts["ComicSans"] = LoadFont("ComicSans");
      Fonts["Tahoma"] = LoadFont("Tahoma");
      Fonts["Verdana"] = LoadFont("Verdana");
      Fonts["Georgia"] = LoadFont("Georgia");
    }

    private void LoadImages()
    {
      Image[] images = new Image[]
      {
        new Image(contentManager, "towerBase", false, 0, 0),
        new Image(contentManager, "tank", false, 0, 0),
        new Image(contentManager, "mouse", false, 0, 0),
        new Image(contentManager, "railgun", false, 0, 0),
        new Image(contentManager, "turret", false, 0, 0),
        new Image(contentManager, "missileLauncher", false, 0, 0),
        new Image(contentManager, "shockwaveGenerator", false, 0, 0),
        new Image(contentManager, "speedbump", false, 0, 0),
        new Image(contentManager, "sprinkler", false, 0, 0),
        new Image(contentManager, "teslaCoil", true, 5, 1),
        new Image(contentManager, "machinegun", false, 0, 0),
        new Image(contentManager, "flamethrower", false, 0, 0),
        new Image(contentManager, "rocket", false, 0, 0),
        new Image(contentManager, "bullet", false, 0, 0),
        new Image(contentManager, "pulse", false, 0, 0),
        new Image(contentManager, "debri", false, 0, 0),
        new Image(contentManager, "fragment", false, 0, 0),
        new Image(contentManager, "lightning", false, 0, 0),
        new Image(contentManager, "flame", false, 0, 0),
        new Image(contentManager, "flamewave", false, 0, 0),
        new Image(contentManager, "wave", false, 0, 0),
      };

      foreach (Image image in images)
      {
        Images[image.Key] = image;
      }
    }

    private SpriteFont LoadFont(string name)
    {
      return contentManager.Load<SpriteFont>(string.Format("{0}", name));
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      if (IsActive)
      {
        grid.FillMode = GridFillMode.Solid;
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
          grid.FillMode = GridFillMode.Polygons;
        }
        base.Update(gameTime);
      }
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
