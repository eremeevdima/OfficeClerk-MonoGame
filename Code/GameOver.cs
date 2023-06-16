using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

public static class GameOver
{
    private static Texture2D gameOver;
    private static Texture2D gameOverRestart;
    public static float alpha = 0f;

    public static void LoadContent(ContentManager content)
    {
        gameOver = content.Load<Texture2D>("GameOver");
        gameOverRestart = content.Load<Texture2D>("GameOverRestart");
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        var mouseState = Mouse.GetState();
        alpha += 0.15f;
        spriteBatch.Draw(gameOver, Vector2.Zero, Color.White * alpha);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, gameOverRestart, new Vector2(780, 950), mouseState, 350, 50);
    }
}

