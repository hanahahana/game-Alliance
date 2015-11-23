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

    private Messages messages;
    private GridComponent grid;
    private InputProvider input;

    public static Dictionary<string, Texture2D> Textures = null;
    public static SoundBank Sounds = null;

    private AudioEngine audioEngine;
    private WaveBank waveBank;

    public AllianceGame()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      grid = new GridComponent(this);
      Components.Add(grid);

      messages = new Messages(this);
      Components.Add(messages);

      input = new InputProvider(this);
      input.UpdateOrder = 0;
      Components.Add(input);

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
      // TODO: Add your initialization logic here
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

      Textures = new Dictionary<string, Texture2D>();

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

      // load the projectiles
      Textures.Add("rocket", LoadImage("Projectiles\\rocket"));
      Textures.Add("bullet", LoadImage("Projectiles\\bullet"));
      Textures.Add("wave", LoadImage("Projectiles\\wave"));
      Textures.Add("pulse", LoadImage("Projectiles\\pulse"));
    }

    private Texture2D LoadImage(string name)
    {
      return Content.Load<Texture2D>(string.Format("Images\\{0}", name));
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
