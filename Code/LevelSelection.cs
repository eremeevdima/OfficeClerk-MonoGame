using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
public static class LevelSelection
{
    private static Texture2D levelSelection;
    private static Texture2D levelSelectionBack;
    private static Texture2D firstLevel;
    private static Texture2D secondLevel;
    private static Texture2D info;

    public static void LoadContent(ContentManager content)
    {
        levelSelection = content.Load<Texture2D>("LevelSelection");
        levelSelectionBack = content.Load<Texture2D>("LevelSelectionBack");
        firstLevel = content.Load<Texture2D>("FirstLevel");
        secondLevel = content.Load<Texture2D>("SecondLevel");
        info = content.Load<Texture2D>("Info");
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        var mouseState = Mouse.GetState();
        spriteBatch.Draw(levelSelection, Vector2.Zero, Color.White);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, levelSelectionBack, new Vector2(35, 43), mouseState, 170, 112);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, firstLevel, new Vector2(280, 395), mouseState, 370, 345);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, secondLevel, new Vector2(1260, 395), mouseState, 370, 345);
        AuxiliaryMethods.DrawTextureWithHover(spriteBatch, info, new Vector2(1500, 40), mouseState, 350, 120);
    }
}

