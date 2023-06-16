using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

public static class Menu
{
    private static Texture2D menu;
    private static Texture2D settingsButton;
    private static Texture2D exitButton;
    private static Texture2D play;

    public static void LoadContent(ContentManager content)
    {
        menu = content.Load<Texture2D>("Menu");
        settingsButton = content.Load<Texture2D>("SettingsButton");
        exitButton = content.Load<Texture2D>("ExitButton");
        play = content.Load<Texture2D>("Play");
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        var mouseState = Mouse.GetState();
        spriteBatch.Draw(menu, Vector2.Zero, Color.White);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, play, new Vector2(980, 370), mouseState, 300, 80);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, exitButton, new Vector2(44, 916), mouseState, 100, 100);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, settingsButton, new Vector2(170, 916), mouseState, 100, 100);
    }
}

