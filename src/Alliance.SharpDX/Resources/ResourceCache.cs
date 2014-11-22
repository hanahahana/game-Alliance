using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public class ResourceCache
  {
    public AllianceGame Owner { get; private set; }
    public ContentManager Content { get { return Owner.Content; } }

    public Dictionary<string, FramedImage> Images { get; private set; }
    public Dictionary<string, SpriteFont> Fonts { get; private set; }
    public Dictionary<string, Texture2D> Textures { get; private set; }
    //public static GridComponent CurrentGrid { get; private set; }

    public ResourceCache(AllianceGame owner)
    {
      Owner = owner;
      LoadContent();
    }

    private T Load<T>(string p)
    {
      return Content.Load<T>(p);
    }

    private void LoadContent()
    {
      LoadFonts();
      LoadImages();
      LoadTextures();
    }

    private void LoadTextures()
    {
      Textures = new Dictionary<string, Texture2D>();
      Textures["background"] = Load<Texture2D>(@"Textures\background");
      Textures["blank"] = Load<Texture2D>(@"Textures\blank");
      Textures["gradient"] = Load<Texture2D>(@"Textures\gradient");
    }

    private void LoadFonts()
    {
      Fonts = new Dictionary<string, SpriteFont>();
      Fonts["ComicSans"] = Load<SpriteFont>(@"Fonts\ComicSans");
      Fonts["Tahoma"] = Load<SpriteFont>(@"Fonts\Tahoma");
      Fonts["Verdana"] = Load<SpriteFont>(@"Fonts\Verdana");
      Fonts["Georgia"] = Load<SpriteFont>(@"Fonts\Georgia");
      Fonts["Arial"] = Load<SpriteFont>(@"Fonts\Arial");
      Fonts["BookmanOldStyle"] = Load<SpriteFont>(@"Fonts\BookmanOldStyle");
      Fonts["Rockwell"] = Load<SpriteFont>(@"Fonts\Rockwell");
    }

    private void LoadImages()
    {
      FramedImage[] images = new FramedImage[]
      {
        // bases
        new FramedImage(Content, @"Images\Bases", "towerBase", false, 0, 0),

        // enemies ( all enemies are animated to avoid exceptions )
        new FramedImage(Content, @"Images\Enemies", "walker", true, 4, 1),
        new FramedImage(Content, @"Images\Enemies", "biker", true, 4, 1),
        new FramedImage(Content, @"Images\Enemies", "flapper", true, 5, 1),
        new FramedImage(Content, @"Images\Enemies", "glider", true, 4, 1),

        // towers
        new FramedImage(Content, @"Images\Towers", "railgun", false, 0, 0),
        new FramedImage(Content, @"Images\Towers", "turret", false, 0, 0),
        new FramedImage(Content, @"Images\Towers", "missileLauncher", false, 0, 0),
        new FramedImage(Content, @"Images\Towers", "shockwaveGenerator", false, 0, 0),
        new FramedImage(Content, @"Images\Towers", "speedbump", false, 0, 0),
        new FramedImage(Content, @"Images\Towers", "sprinkler", false, 0, 0),
        new FramedImage(Content, @"Images\Towers", "teslaCoil", true, 5, 1),
        new FramedImage(Content, @"Images\Towers", "machinegun", false, 0, 0),
        new FramedImage(Content, @"Images\Towers", "flamethrower", false, 0, 0),
        new FramedImage(Content, @"Images\Towers", "seeker", false, 0, 0),

        // projectiles
        new FramedImage(Content, @"Images\Projectiles", "rocket", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "bullet", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "pulse", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "debri", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "fragment", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "lightning", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "flame", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "flamewave", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "wave", false, 0, 0),
        new FramedImage(Content, @"Images\Projectiles", "lockmissile", false, 0, 0),
      };

      Images = images.ToDictionary(k => k.Key, v => v);
    }
  }
}
