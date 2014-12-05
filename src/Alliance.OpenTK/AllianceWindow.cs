using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;
using GuiSystem;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Alliance
{
  public class AllianceWindow : GameWindow, IGuiSurface
  {
    private OpenGLGraphics graphics;
    private GridComponent grid;
    private ResourceProvider resources;
    private KeyMouseProvider input;

    public AllianceWindow()
    {
      resources = new ResourceProvider();
      input = new KeyMouseProvider(this);
    }

    protected override void OnLoad(EventArgs e)
    {
      // setup settings, load textures, sounds
      VSync = VSyncMode.On;
      Title = "Alliance";
      Width = 800;
      Height = 600;

      graphics = new OpenGLGraphics();
      grid = new GridComponent(this);
      grid.Initialize();

      base.OnLoad(e);
    }

    protected override void OnResize(EventArgs e)
    {
      GL.Viewport(0, 0, Width, Height);
      base.OnResize(e);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      input.Update();

      var state = new GuiInputState
      {
        Point = input.CursorPosition,
        Pressed = input.SelectPressed,
        Released = !input.SelectPressed,
      };

      grid.Update(state, TimeSpan.FromSeconds(e.Time));
      base.OnUpdateFrame(e);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      // render graphics
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.ClearColor(Color.CornflowerBlue);

      graphics.Begin();
      grid.Draw(graphics);
      graphics.End();

      SwapBuffers();
      base.OnRenderFrame(e);
    }
  }
}
