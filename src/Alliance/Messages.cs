using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Utilities;
using Alliance.Data;

namespace Alliance
{
  /// <summary>
  /// TODO: Implement Singleton
  /// </summary>
  public class Messages : DrawableGameComponent
  {
    private enum MessageState { FadeIn, Displayed, FadeOut, Complete };
    private class Message
    {
      public const float DefaultFadeTimeMs = 1500f;

      private Vector2 mPosition;
      public Vector2 Position
      {
        get { return mPosition; }
      }

      private SizeF mSize;
      public SizeF Size
      {
        get { return mSize; }
      }

      private string mText;
      public string Text
      {
        get { return mText; }
      }

      private float mRemainingTime;
      public float RemainingTime
      {
        get { return mRemainingTime; }
      }

      private MessageState mState;
      public MessageState State
      {
        get { return mState; }
      }

      private Color mBackColor;
      public Color BackColor
      {
        get { return mBackColor; }
      }

      private Color mForeColor;
      public Color ForeColor
      {
        get { return mForeColor; }
      }

      private bool mFadeEffects;
      public bool FadeEffects
      {
        get { return mFadeEffects; }
      }

      private SpriteFont mFont;
      public SpriteFont Font
      {
        get { return mFont; }
      }

      private float mAlpha;
      public float Alpha
      {
        get { return mAlpha; }
      }

      private float mFadeTimeMs;
      public float FadeTimeMs
      {
        get { return mFadeTimeMs; }
      }

      public Message(string text, Vector2 position, float aliveTimeMs, float fadeTimeMs, Color backColor, Color foreColor, SpriteFont font, bool useFadeEffects)
      {
        mText = text;
        mPosition = position;
        mRemainingTime = aliveTimeMs;
        mBackColor = backColor;
        mForeColor = foreColor;
        mFont = font;
        mFadeEffects = useFadeEffects;
        mFadeTimeMs = fadeTimeMs;
        mState = (mFadeEffects ? MessageState.FadeIn : MessageState.Displayed);
        mSize = new SizeF(mFont.MeasureString(mText));
        mAlpha = (mFadeEffects ? 0 : 1f);
      }

      public void Update(GameTime gameTime)
      {
        switch (mState)
        {
          case MessageState.FadeIn:
            {
              AdjustAlpha((float)gameTime.ElapsedGameTime.TotalMilliseconds);
              if (mAlpha >= 1f)
              {
                mState = MessageState.Displayed;
                mAlpha = 1f;
              }
              break;
            }
          case MessageState.FadeOut:
            {
              AdjustAlpha(-(float)gameTime.ElapsedGameTime.TotalMilliseconds);
              if (mAlpha <= 0f)
              {
                mState = MessageState.Complete;
                mAlpha = 0f;
              }
              break;
            }
          case MessageState.Displayed:
            {
              mRemainingTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
              if (mRemainingTime <= 0f)
              {
                mState = (mFadeEffects ? MessageState.FadeOut : MessageState.Complete);
                mRemainingTime = 0f;
              }
              break;
            }
          default:
            { break; }
        }
      }

      private void AdjustAlpha(float elapsedMilliseconds)
      {
        mAlpha += (mFadeTimeMs * elapsedMilliseconds);
      }

      public void Draw(SpriteBatch spriteBatch)
      {
        if (mState != MessageState.Complete)
        {
          mBackColor = Utils.NewAlpha(mBackColor, mAlpha);
          mForeColor = Utils.NewAlpha(mForeColor, mAlpha);

          BoxF box = new BoxF(mPosition.X - 3, mPosition.Y - 2, mSize.Width + 6, mSize.Height + 4);
          Shapes.FillRectangle(spriteBatch, box, mBackColor);
          Shapes.DrawRectangle(spriteBatch, box, mForeColor);
          spriteBatch.DrawString(mFont, mText, mPosition, mForeColor);
        }
      }

      public void Readjust()
      {
        mPosition -= (new Vector2(mSize.Width / 2f, mSize.Height / 2f));
      }
    }

    private static Color mDefaultForeColor;
    public static Color DefaultForeColor
    {
      get { return mDefaultForeColor; }
    }

    private static Vector2 mDefaultPosition;
    public static Vector2 DefaultPosition
    {
      get { return mDefaultPosition; }
    }

    private static Color mDefaultBackColor;
    public static Color DefaultBackColor
    {
      get { return mDefaultBackColor; }
    }

    private static float mDefaultAliveTime;
    public static float DefaultAliveTime
    {
      get { return mDefaultAliveTime; }
    }

    private static bool mDefaultUseFadeEffects;
    public static bool DefaultUseFadeEffects
    {
      get { return mDefaultUseFadeEffects; }
    }

    private static SpriteFont mDefaultFont;
    public static SpriteFont DefaultFont
    {
      get { return mDefaultFont; }
    }

    private static SpriteBatch mSpriteBatch;
    public static SpriteBatch SpriteBatch
    {
      get { return mSpriteBatch; }
    }

    private static List<Message> mMessages = new List<Message>();
    static Messages()
    {
      mDefaultForeColor = Color.Black;
      mDefaultBackColor = Color.Goldenrod;
      mDefaultAliveTime = 1500f;
      mDefaultUseFadeEffects = true;
    }

    public Messages(Game game)
      : base(game)
    {
    }

    protected override void LoadContent()
    {
      LoadStaticContent(Game);
    }

    private static void LoadStaticContent(Game game)
    {
      if (mDefaultFont == null)
      {
        mDefaultFont = AllianceGame.Fonts["Tahoma"];
      }

      if (mSpriteBatch == null)
      {
        mSpriteBatch = new SpriteBatch(game.GraphicsDevice);
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
          mMessages.RemoveAt(i);
      }
      base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
      foreach (Message msg in mMessages)
      {
        msg.Draw(mSpriteBatch);
      }
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

    public static void AddMessage(string text, Vector2? position, float? aliveTimeMs, float? fadeTimeMs, Color? backColor, Color? foreColor, SpriteFont font, bool? useFadeEffects, bool adjustPosition)
    {
      Message message = new Message(
        text,
        position.HasValue ? position.Value : mDefaultPosition,
        aliveTimeMs.HasValue ? aliveTimeMs.Value : mDefaultAliveTime,
        fadeTimeMs.HasValue ? fadeTimeMs.Value : Message.DefaultFadeTimeMs,
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
