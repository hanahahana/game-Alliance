using System.Collections.Generic;
using System.Threading;
using SharpDX.Toolkit;

namespace Alliance
{
  /// <summary>
  /// 
  /// </summary>
  public sealed class MessageComponent : GameSystem
  {
    public const float DefaultFadeTimeMs = 2500f;
    public const float MessageMargin = 5;
    public const float TwoMessageMargin = MessageMargin * 2;

    #region Private Message Class

    private enum MessageState { FadeIn, Displayed, FadeOut, Complete };
    private class Message
    {
      public Vector2 Position { get; private set; }
      public SizeF Size { get; private set; }
      public string Text { get; private set; }
      public float RemainingTime { get; private set; }
      public MessageState State { get; private set; }
      public Color BackColor { get; private set; }
      public Color ForeColor { get; private set; }
      public bool FadeEffects { get; private set; }
      public SpriteFont Font { get; private set; }
      public float Alpha { get; private set; }
      public float FadeTimeMs { get; private set; }

      public Message(string text, Vector2 position, float aliveTimeMs, float fadeTimeMs, Color backColor, Color foreColor, SpriteFont font, bool useFadeEffects)
      {
        Text = text;
        Position = position;
        RemainingTime = aliveTimeMs;
        BackColor = backColor;
        ForeColor = foreColor;
        Font = font;
        FadeEffects = useFadeEffects;
        FadeTimeMs = fadeTimeMs;
        State = (FadeEffects ? MessageState.FadeIn : MessageState.Displayed);
        Size = new SizeF(Font.MeasureString(Text));
        Alpha = (FadeEffects ? 0 : 1f);
      }

      public void Update(GameTime gameTime)
      {
        switch (State)
        {
          case MessageState.FadeIn:
            {
              AdjustAlpha((float)gameTime.ElapsedGameTime.TotalMilliseconds);
              if (Alpha >= 1f)
              {
                State = MessageState.Displayed;
                Alpha = 1f;
              }
              break;
            }
          case MessageState.FadeOut:
            {
              AdjustAlpha(-(float)gameTime.ElapsedGameTime.TotalMilliseconds);
              if (Alpha <= 0f)
              {
                State = MessageState.Complete;
                Alpha = 0f;
              }
              break;
            }
          case MessageState.Displayed:
            {
              RemainingTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
              if (RemainingTime <= 0f)
              {
                State = (FadeEffects ? MessageState.FadeOut : MessageState.Complete);
                RemainingTime = 0f;
              }
              break;
            }
          default:
            { break; }
        }
      }

      private void AdjustAlpha(float elapsedMilliseconds)
      {
        Alpha += (FadeTimeMs * elapsedMilliseconds);
      }

      public void Draw(DrawParams data)
      {
        if (State != MessageState.Complete)
        {
          BackColor = ColorHelper.NewAlpha(BackColor, Alpha);
          ForeColor = ColorHelper.NewAlpha(ForeColor, Alpha);
          BoxF box = new BoxF
          {
            X = Position.X - MessageMargin,
            Y = Position.Y - MessageMargin,
            Width = Size.Width + TwoMessageMargin,
            Height = Size.Height + TwoMessageMargin
          };

          data.Graphics.FillRectangle(box, BackColor);
          data.Graphics.DrawRectangle(box, ForeColor);
          data.SpriteBatch.DrawString(Font, Text, Position, ForeColor);
        }
      }

      public void Readjust()
      {
        Position -= (new Vector2(Size.Width / 2f, Size.Height / 2f));
      }
    }
    #endregion

    private static Color mDefaultForeColor;
    private static Vector2 mDefaultPosition;
    private static Color mDefaultBackColor;
    private static float mDefaultAliveTime;
    private static bool mDefaultUseFadeEffects;
    private static SpriteFont mDefaultFont;
    private static SpriteBatch mSpriteBatch;
    private static GraphicsBase mGraphics;
    private static int creationRequests;
    private static List<Message> mMessages = new List<Message>();
    private static MessageComponent mComponentInstance;

    static MessageComponent()
    {
      mDefaultForeColor = Color.Black;
      mDefaultBackColor = Color.Goldenrod;
      mDefaultAliveTime = 1500f;
      mDefaultUseFadeEffects = true;
    }

    private MessageComponent(Game game)
      : base(game)
    {
    }

    public static MessageComponent CreateInstance(Game game)
    {
      if (Interlocked.Increment(ref creationRequests) == 1)
      {
        mComponentInstance = new MessageComponent(game);
      }
      return mComponentInstance;
    }

    public override void LoadContent()
    {
      LoadStaticContent(Game);
    }

    private static void LoadStaticContent(Game game)
    {
      if (mDefaultFont == null)
      {
        mDefaultFont = AllianceGame.Fonts["TimesNewRoman"];
      }

      if (mSpriteBatch == null)
      {
        mSpriteBatch = new SpriteBatch(game.GraphicsDevice);
      }

      if (mGraphics == null)
      {
        mGraphics = new PrimitiveGraphics(mSpriteBatch);
      }
    }

    public override void Update(GameTime gameTime)
    {
      mDefaultPosition = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
      for (int i = 0; i < mMessages.Count; ++i)
      {
        Message msg = mMessages[i];
        msg.Update(gameTime);

        if (msg.State == MessageState.Complete)
        {
          mMessages.RemoveAt(i);
          --i;
        }
      }
      base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      DrawParams data = new DrawParams(gameTime, Vector2.Zero, GridFillMode.Solid, mSpriteBatch, mGraphics);
      mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

      mMessages.ForEach(msg => msg.Draw(data));

      mSpriteBatch.End();
      base.Draw(gameTime);
    }

    public static void AddMessage(string text, float aliveTimeMs, bool useFadeEffects)
    {
      AddMessage(text, null, aliveTimeMs, null, null, null, null, useFadeEffects, true);
    }

    public static void AddMessage(string text, Vector2 position, float aliveTimeMs, bool useFadeEffects)
    {
      AddMessage(text, position, aliveTimeMs, null, null, null, null, useFadeEffects, true);
    }

    public static void AddMessage(string text, Vector2 position, float aliveTimeMs, SpriteFont font, bool useFadeEffects)
    {
      AddMessage(text, position, aliveTimeMs, null, null, null, font, useFadeEffects, true);
    }

    public static void AddMessage(string text, Vector2? position, float? aliveTimeMs, float? fadeTimeMs, Color? backColor, Color? foreColor, SpriteFont font, bool? useFadeEffects, bool adjustPosition)
    {
      Message message = new Message(
        text,
        position.HasValue ? position.Value : mDefaultPosition,
        aliveTimeMs.HasValue ? aliveTimeMs.Value : mDefaultAliveTime,
        fadeTimeMs.HasValue ? fadeTimeMs.Value : DefaultFadeTimeMs,
        backColor.HasValue ? backColor.Value : mDefaultBackColor,
        foreColor.HasValue ? foreColor.Value : mDefaultForeColor,
        font != null ? font : mDefaultFont,
        useFadeEffects.HasValue ? useFadeEffects.Value : mDefaultUseFadeEffects);

      if (adjustPosition)
        message.Readjust();
      mMessages.Add(message);
    }
  }
}
