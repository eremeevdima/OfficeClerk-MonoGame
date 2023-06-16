using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public static class InfoBoard
{
    private static Texture2D closeSettings;
    private static Texture2D gameplay;

    public static void LoadContent(ContentManager content)
    {
        closeSettings = content.Load<Texture2D>("CloseSettings");
        gameplay = content.Load<Texture2D>("Gameplay");
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(gameplay, Vector2.Zero, Color.White);
        var mouseState = Mouse.GetState();
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, closeSettings, new Vector2(420, 190), mouseState, 100, 100);
    }
}