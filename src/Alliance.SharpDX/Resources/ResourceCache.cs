using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;
using SharpDX;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public class ResourceCache : IFontProvider, IImageProvider
  {
    public AllianceGame Owner { get; private set; }
    public ContentManager Content { get { return Owner.Content; } }

    public Dictionary<string, FramedImage> Images { get; private set; }
    public Dictionary<string, SpriteFont> Fonts { get; private set; }
    public Dictionary<string, Texture2D> Textures { get; private set; }
    public GridComponent CurrentGrid { get; set; }

    public ResourceCache(AllianceGame owner)
    {
      FontProvider.Register(this);
      ImageProvider.Register(this);

      Owner = owner;
      LoadContent();
    }

    private T Load<T>(string p)
    {
      return Content.Load<T>(p);
    }

    private GsImage LoadTexture(string path)
    {
      return new GsImage { Data = Load<Texture2D>(path) };
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
        new FramedImage(LoadTexture(@"Images\Bases\towerBase"), "towerBase", false, 0, 0),

        // enemies ( all enemies are animated to avoid exceptions )
        new FramedImage(LoadTexture(@"Images\Enemies\walker"), "walker", true, 4, 1),
        new FramedImage(LoadTexture(@"Images\Enemies\biker"), "biker", true, 4, 1),
        new FramedImage(LoadTexture(@"Images\Enemies\flapper"), "flapper", true, 5, 1),
        new FramedImage(LoadTexture(@"Images\Enemies\glider"), "glider", true, 4, 1),

        // towers
        new FramedImage(LoadTexture(@"Images\Towers\railgun"), "railgun", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Towers\turret"), "turret", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Towers\missileLauncher"), "missileLauncher", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Towers\shockwaveGenerator"), "shockwaveGenerator", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Towers\speedbump"), "speedbump", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Towers\sprinkler"), "sprinkler", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Towers\teslaCoil"), "teslaCoil", true, 5, 1),
        new FramedImage(LoadTexture(@"Images\Towers\machinegun"), "machinegun", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Towers\flamethrower"), "flamethrower", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Towers\seeker"), "seeker", false, 0, 0),

        // projectiles
        new FramedImage(LoadTexture(@"Images\Projectiles\rocket"), "rocket", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\bullet"), "bullet", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\pulse"), "pulse", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\debri"), "debri", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\fragment"), "fragment", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\lightning"), "lightning", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\flame"), "flame", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\flamewave"), "flamewave", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\wave"), "wave", false, 0, 0),
        new FramedImage(LoadTexture(@"Images\Projectiles\lockmissile"), "lockmissile", false, 0, 0),
      };

      Images = images.ToDictionary(k => k.Key, v => v);
    }

    GsFont IFontProvider.GetButtonFont()
    {
      return new GsFont { Data = Fonts["Tahoma"] };
    }

    GsFont IFontProvider.GetPieceLevelFont()
    {
      return new GsFont { Data = Fonts["ComicSans"] };
    }

    GsFont IFontProvider.GetInvaderLevelFont()
    {
      return new GsFont { Data = Fonts["BookmanOldStyle"] };
    }

    GsFont IFontProvider.GetDefaultFont()
    {
      return new GsFont { Data = Fonts["Rockwell"] };
    }

    float IFontProvider.GetLineSpacing(GsFont font)
    {
      var f = font.Data as SpriteFont;
      return f.LineSpacing;
    }

    FramedImage IImageProvider.GetFramedImage(string key)
    {
      return Images[key];
    }

    GsSize IImageProvider.GetSize(GsImage image)
    {
      var tex = image.Data as Texture2D;
      return new GsSize(tex.Width, tex.Height);
    }

    GsVector[] IImageProvider.CreateConvexHull(GsImage image)
    {
      var tex = image.Data as Texture2D;
      return tex.CreateConvexHull();
    }

    GsColor[,] IImageProvider.ToColorData(GsImage image)
    {
      var tex = image.Data as Texture2D;
      var data = tex.GetData<Color>();

      return data
        .Select(SharpDXExtensions.ToGsColor)
        .ToArray()
        .To2D(tex.Width, tex.Height);
    }

    GsImage IImageProvider.FromColorData(GsColor[] data, int width, int height)
    {
      var frame = data.Select(SharpDXExtensions.ToColor).ToArray();
      Texture2D tex = Texture2D.New<Color>(Owner.GraphicsDevice,
        width, height,
        PixelFormat.R8G8B8A8.UNorm,
        frame);
      return new GsImage { Data = tex, Tag = "FromColorData" };
    }
  }
}
