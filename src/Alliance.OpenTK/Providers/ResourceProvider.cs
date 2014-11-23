using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public class ResourceProvider : IFontProvider, IImageProvider
  {
    private readonly Font font;
    private readonly Dictionary<string, GsImage> textures;
    private readonly Dictionary<string, FramedImage> images;

    public ResourceProvider()
    {
      FontProvider.Register(this);
      ImageProvider.Register(this);

      font = new Font("Consolas", 8.75f);
      textures = new Dictionary<string, GsImage>();
      images = new Dictionary<string, FramedImage>();

      LoadTextures();
      LoadImages();
    }

    private GsImage CreateGsImage(string path)
    {
      var bmp = Texture.LoadAsBitmap(path);
      return CreateGsImage(bmp);
    }

    private GsImage CreateGsImage(Bitmap bmp)
    {
      return new GsImage
      {
        Data = new TextureData
        {
          Bitmap = bmp,
          ID = Texture.BindImage(bmp),
        },
      };
    }

    private void LoadTextures()
    {
      textures["background"] = CreateGsImage(@"Textures\background.png");
      textures["blank"] = CreateGsImage(@"Textures\blank.png");
      textures["gradient"] = CreateGsImage(@"Textures\gradient.png");
    }

    private void LoadImages()
    {
      FramedImage[] arr = new FramedImage[]
      {
        // bases
        new FramedImage(CreateGsImage(@"Images\Bases\towerBase.png"), "towerBase", false, 0, 0),

        // enemies ( all enemies are animated to avoid exceptions )
        new FramedImage(CreateGsImage(@"Images\Enemies\walker.png"), "walker", true, 4, 1),
        new FramedImage(CreateGsImage(@"Images\Enemies\biker.png"), "biker", true, 4, 1),
        new FramedImage(CreateGsImage(@"Images\Enemies\flapper.png"), "flapper", true, 5, 1),
        new FramedImage(CreateGsImage(@"Images\Enemies\glider.png"), "glider", true, 4, 1),

        // towers
        new FramedImage(CreateGsImage(@"Images\Towers\railgun.png"), "railgun", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Towers\turret.png"), "turret", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Towers\missileLauncher.png"), "missileLauncher", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Towers\shockwaveGenerator.png"), "shockwaveGenerator", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Towers\speedbump.png"), "speedbump", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Towers\sprinkler.png"), "sprinkler", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Towers\teslaCoil.png"), "teslaCoil", true, 5, 1),
        new FramedImage(CreateGsImage(@"Images\Towers\machinegun.png"), "machinegun", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Towers\flamethrower.png"), "flamethrower", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Towers\seeker.png"), "seeker", false, 0, 0),

        // projectiles
        new FramedImage(CreateGsImage(@"Images\Projectiles\rocket.png"), "rocket", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\bullet.png"), "bullet", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\pulse.png"), "pulse", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\debri.png"), "debri", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\fragment.png"), "fragment", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\lightning.png"), "lightning", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\flame.png"), "flame", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\flamewave.png"), "flamewave", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\wave.png"), "wave", false, 0, 0),
        new FramedImage(CreateGsImage(@"Images\Projectiles\lockmissile.png"), "lockmissile", false, 0, 0),
      };

      foreach (var d in arr)
        images[d.Key] = d;
    }

    public GsFont GetButtonFont()
    {
      return new GsFont { Data = font };
    }

    public GsFont GetPieceLevelFont()
    {
      return new GsFont { Data = font };
    }

    public GsFont GetInvaderLevelFont()
    {
      return new GsFont { Data = font };
    }

    public GsFont GetDefaultFont()
    {
      return new GsFont { Data = font };
    }

    public float GetLineSpacing(GsFont font)
    {
      return (font.Data as Font).GetHeight();
    }

    public FramedImage GetFramedImage(string key)
    {
      return images[key];
    }

    public GsSize GetSize(GsImage image)
    {
      var data = image.Data as TextureData;
      return new GsSize(data.Bitmap.Width, data.Bitmap.Height);
    }

    public GsVector[] CreateConvexHull(GsImage image)
    {
      var data = image.Data as TextureData;
      return data.Bitmap.CreateConvexHull();
    }

    public GsColor[,] ToColorData(GsImage image)
    {
      var data = image.Data as TextureData;
      var bmp = data.Bitmap;

      var pixels = bmp.GetPixels();
      return pixels
        .Select(OpenGLExtensions.ToGsColor)
        .ToArray()
        .To2D(bmp.Width, bmp.Height);
    }

    public GsImage FromColorData(GsColor[] data, int width, int height)
    {
      Bitmap bitmap = new Bitmap(width, height);
      bitmap.SetPixels(data.Select(OpenGLExtensions.ToPixelData).ToArray());
      return CreateGsImage(bitmap);
    }
  }
}
