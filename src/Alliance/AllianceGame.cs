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

    private MessageComponent messages;
    private GridComponent grid;
    private InputProvider input;
    private PlayerHudComponent player;

    public static Dictionary<string, Texture2D> Textures = null;
    public static Dictionary<string, Color[,]> TextureData = null;
    public static Dictionary<string, SpriteFont> Fonts = null;
    public static SoundBank Sounds = null;

    private AudioEngine audioEngine;
    private WaveBank waveBank;

    public AllianceGame()
    {
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
      TextureData = new Dictionary<string, Color[,]>();
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

      audioEngine = new AudioEngine("Content\\Sounds\\SoundsProj.xgs");
      waveBank = new WaveBank(audioEngine, "Content\\Sounds\\Wave Bank.xwb");
      Sounds = new SoundBank(audioEngine, "Content\\Sounds\\Sound Bank.xsb");

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

    private void LoadImageData()
    {
      foreach (string key in Textures.Keys)
      {
        Texture2D texture = Textures[key];
        TextureData[key] = Utils.TextureTo2DArray(texture);
      }
    }

    private void LoadImages()
    {
      // load the bases
      Textures.Add("towerBase", LoadImage("Bases\\towerBase"));

      // load the enemies
      Textures.Add("tank", LoadImage("Enemies\\tank"));
      Textures.Add("mouse", LoadImage("Enemies\\mouse"));

      // load the towers
      Textures.Add("railgun", LoadImage("Towers\\railgun"));
      Textures.Add("turret", LoadImage("Towers\\turret"));
      Textures.Add("missileLauncher", LoadImage("Towers\\missileLauncher"));
      Textures.Add("shockwaveGenerator", LoadImage("Towers\\shockwaveGenerator"));
      Textures.Add("speedbump", LoadImage("Towers\\speedbump"));
      Textures.Add("sprinkler", LoadImage("Towers\\sprinkler"));

      // load the projectiles
      Textures.Add("rocket", LoadImage("Projectiles\\rocket"));
      Textures.Add("bullet", LoadImage("Projectiles\\bullet"));
      Textures.Add("wave", LoadImage("Projectiles\\wave"));
      Textures.Add("pulse", LoadImage("Projectiles\\pulse"));
      Textures.Add("debri", LoadImage("Projectiles\\debri"));
      Textures.Add("fragment", LoadImage("Projectiles\\fragment"));
    }

    private Texture2D LoadImage(string name)
    {
      return Content.Load<Texture2D>(string.Format("Images\\{0}", name));
    }

    private SpriteFont LoadFont(string name)
    {
      return Content.Load<SpriteFont>(string.Format("Fonts\\{0}", name));
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
      audioEngine.Update();
      base.Update(gameTime);
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
