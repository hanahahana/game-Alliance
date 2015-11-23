using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Alliance.Helpers;

namespace Alliance.Components
{
  public class PlayerHudComponent : DrawableGameComponent
  {
    private SpriteBatch spriteBatch;
    private SpriteFont spriteFont;

    public ContentManager Content
    {
      get { return Game.Content; }
    }

    public PlayerHudComponent(Game game)
      : base(game)
    {

    }

    protected override void LoadContent()
    {      
      spriteBatch = new SpriteBatch(GraphicsDevice);
      spriteFont = AllianceGame.Fonts["Verdana"];
    }

    public override void Draw(GameTime gameTime)
    {
      string cashText = string.Format("Cash: {0:c2}", Player.Cash);
      Vector2 cashVec = new Vector2(5, 5);
      Color cashColor = Color.Blue;

      string lifeText = string.Format("Civilians: {0}", Player.Life);
      Vector2 lifeVec = new Vector2(cashVec.X + spriteFont.MeasureString(cashText).X + 60f, cashVec.Y);
      Color lifeColor = Color.DarkGreen;

      spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

      spriteBatch.DrawString(spriteFont, cashText, cashVec, cashColor);
      spriteBatch.DrawString(spriteFont, lifeText, lifeVec, lifeColor);

      spriteBatch.End();
      base.Draw(gameTime);
    }
  }
}
