using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


public static class Settings
{
    private static Texture2D closeSettings;
    private static Texture2D turnUp;
    private static Texture2D turnDown;
    private static Texture2D soundOff;
    private static Texture2D sound20per;
    private static Texture2D sound40per;
    private static Texture2D sound60per;
    private static Texture2D sound80per;
    private static Texture2D sound100per;
    private static SoundEffect clickSound;
    public static int soundValue = 20;
    private static float elapsedTime;

    public static void LoadContent(ContentManager content)
    {
        closeSettings = content.Load<Texture2D>("CloseSettings");
        turnUp = content.Load<Texture2D>("TurnUp");
        turnDown = content.Load<Texture2D>("TurnDown");
        soundOff = content.Load<Texture2D>("SoundOff");
        sound20per = content.Load<Texture2D>("Sound20per");
        sound40per = content.Load<Texture2D>("Sound40per");
        sound60per = content.Load<Texture2D>("Sound60per");
        sound80per = content.Load<Texture2D>("Sound80per");
        sound100per = content.Load<Texture2D>("Sound100per");
        clickSound = content.Load<SoundEffect>("ClickSound");
    }

    public static void Update(GameTime gameTime, MouseState mouseState)
    {
        elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            var turnUpRect = new Rectangle(1150, 820, 100, 100);
            if (turnUpRect.Contains(mouseState.Position))
                if (elapsedTime >= 100)
                {
                    clickSound.Play();
                    soundValue = Math.Min(soundValue + 20, 100);
                    elapsedTime = 0;
                }

            var turnDownRect = new Rectangle(645, 820, 100, 100);
            if (turnDownRect.Contains(mouseState.Position))
                if (elapsedTime >= 100)
                {
                    clickSound.Play();
                    soundValue = Math.Max(soundValue - 20, 0);
                    elapsedTime = 0;
                }
        }
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        switch (soundValue)
        {
            case 0:
                spriteBatch.Draw(soundOff, Vector2.Zero, Color.White);
                break;
            case 20:
                spriteBatch.Draw(sound20per, Vector2.Zero, Color.White);
                break;
            case 40:
                spriteBatch.Draw(sound40per, Vector2.Zero, Color.White);
                break;
            case 60:
                spriteBatch.Draw(sound60per, Vector2.Zero, Color.White);
                break;
            case 80:
                spriteBatch.Draw(sound80per, Vector2.Zero, Color.White);
                break;
            case 100:
                spriteBatch.Draw(sound100per, Vector2.Zero, Color.White);
                break;
            default:
                break;
        }
        var mouseState = Mouse.GetState();
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, closeSettings, new Vector2(420, 190), mouseState, 100, 100);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, turnUp, new Vector2(1150, 820), mouseState, 100, 100);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, turnDown, new Vector2(645, 820), mouseState, 100, 100);
    }
}

