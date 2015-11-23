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

    public static Dictionary<string, Texture2D> Textures = null;
    public static Dictionary<string, Vector2[]> TextureHulls = null;
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
      Textures = new Dictionary<string, Texture2D>();
      TextureHulls = new Dictionary<string, Vector2[]>();
      Fonts = new Dictionary<string, SpriteFont>();

      // load all of the fonts
      LoadFonts();

      // load all of the images
      LoadImages();

      // load all of the image data
      LoadImageData();

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

    private void LoadImageData()
    {
      foreach (string key in Textures.Keys)
      {
        Texture2D texture = Textures[key];
        Color[] colorData = new Color[texture.Width * texture.Height];
        texture.GetData<Color>(colorData);

        List<Vector2> pixels = new List<Vector2>(colorData.Length);
        for (int x = 0; x < texture.Width; ++x)
        {
          for (int y = 0; y < texture.Height; ++y)
          {
            Color color = colorData[x + (y * texture.Width)];
            if (color.A > 250)
            {
              pixels.Add(new Vector2(x, y));
            }
          }
        }

        Vector2[] polygon = pixels.ToArray();
        Vector2[] H = new Vector2[polygon.Length];
        int n = Utils.Chain2DConvexHull(polygon, polygon.Length, ref H);

        Vector2[] values = new Vector2[n];
        Array.Copy(H, values, n);

        // store the values
        TextureHulls[key] = values;
      }
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
      // load the bases
      Textures.Add("towerBase", LoadImage("towerBase"));

      // load the enemies
      Textures.Add("tank", LoadImage("tank"));
      Textures.Add("mouse", LoadImage("mouse"));

      // load the towers
      Textures.Add("railgun", LoadImage("railgun"));
      Textures.Add("turret", LoadImage("turret"));
      Textures.Add("missileLauncher", LoadImage("missileLauncher"));
      Textures.Add("shockwaveGenerator", LoadImage("shockwaveGenerator"));
      Textures.Add("speedbump", LoadImage("speedbump"));
      Textures.Add("sprinkler", LoadImage("sprinkler"));
      Textures.Add("teslaCoil", LoadImage("teslaCoil"));
      Textures.Add("machinegun", LoadImage("machinegun"));
      Textures.Add("flamethrower", LoadImage("flamethrower"));

      // load the projectiles
      Textures.Add("rocket", LoadImage("rocket"));
      Textures.Add("bullet", LoadImage("bullet"));
      Textures.Add("wave", LoadImage("wave"));
      Textures.Add("pulse", LoadImage("pulse"));
      Textures.Add("debri", LoadImage("debri"));
      Textures.Add("fragment", LoadImage("fragment"));
      Textures.Add("lightning", LoadImage("lightning"));
      Textures.Add("flame", LoadImage("flame"));
      Textures.Add("flamewave", LoadImage("flamewave"));
    }

    private Texture2D LoadImage(string name)
    {
      return contentManager.Load<Texture2D>(string.Format("{0}", name));
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
