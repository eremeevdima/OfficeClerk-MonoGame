using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OfficeClerk
{
    enum Stat
    {
        Menu,
        Settings,
        GameAudio,
        VideoScreensaver,
        LevelSelection,
        InfoBoard,
        SecondLevel,
        FirstLevel,
        GameOver,
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Stat Stat = Stat.Menu;
        private static SoundEffect clickSound;
        private static Texture2D cursorTexture;
        private static Texture2D bigCursorTexture;
        private MouseState previousMouseState;
        public static bool flag;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            clickSound = Content.Load<SoundEffect>("ClickSound");
            cursorTexture = Content.Load<Texture2D>("Cursor");
            bigCursorTexture = Content.Load<Texture2D>("BigCursor");
            Mouse.SetCursor(MouseCursor.FromTexture2D(cursorTexture, 0, 0));
            LevelSelection.LoadContent(Content);
            GameAudio.LoadContent(Content);
            InfoBoard.LoadContent(Content);
            Settings.LoadContent(Content);
            Menu.LoadContent(Content);
            VideoScreensaver.LoadContent(Content);
            FirstLevel.LoadContent(Content);
            SecondLevel.LoadContent(Content);
            GameOver.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = HandleMouseCursorState(gameTime);

            switch (Stat)
            {
                case Stat.Menu:
                    CheckLevelChange(mouseState, new Rectangle(980, 370, 300, 80), Stat.VideoScreensaver);
                    CheckLevelChange(mouseState, new Rectangle(170, 916, 100, 100), Stat.Settings);
                    if (mouseState.LeftButton == ButtonState.Pressed && new Rectangle(44, 916, 100, 100).Contains(mouseState.Position))
                        Exit();
                    break;

                case Stat.Settings:
                    Settings.Update(gameTime, mouseState);
                    CheckLevelChange(mouseState, new Rectangle(420, 190, 100, 100), Stat.Menu);
                    break;

                case Stat.VideoScreensaver:
                    VideoScreensaver.Update();
                    if (VideoScreensaver.hasPlayed == 2)
                        Stat = Stat.LevelSelection;
                    break;

                case Stat.LevelSelection:
                    CheckLevelChange(mouseState, new Rectangle(280, 395, 370, 345), Stat.FirstLevel);
                    CheckLevelChange(mouseState, new Rectangle(1260, 395, 370, 345), Stat.SecondLevel);
                    CheckLevelChange(mouseState, new Rectangle(1500, 40, 350, 120), Stat.InfoBoard);
                    CheckLevelChange(mouseState, new Rectangle(35, 43, 170, 112), Stat.Menu);
                    break;

                case Stat.InfoBoard:
                    CheckLevelChange(mouseState, new Rectangle(420, 190, 100, 100), Stat.LevelSelection);
                    break;

                case Stat.SecondLevel:
                    SecondLevel.Update(gameTime);
                    if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                        AuxiliaryMethods.OnMouseDown(new Vector2(mouseState.X, mouseState.Y));
                    else if (mouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                        AuxiliaryMethods.OnMouseUp();
                    else if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
                        AuxiliaryMethods.OnMouseMove();
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        SecondLevel.Reset();
                        SecondLevel.score = 0;
                        SecondLevel.healthСount = 3;
                        SecondLevel.xBoss.Clear();
                        SecondLevel.papers.Clear();
                        clickSound.Play();
                        Stat = Stat.LevelSelection;
                    }
                    if (SecondLevel.healthСount == 0)
                    {
                        SecondLevel.Reset();
                        SecondLevel.score = 0;
                        SecondLevel.healthСount = 3;
                        SecondLevel.xBoss.Clear();
                        SecondLevel.papers.Clear();
                        flag = false;
                        Stat = Stat.GameOver;
                    }
                    break;

                case Stat.FirstLevel:
                    FirstLevel.Update(gameTime);

                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        FirstLevel.Reset();
                        clickSound.Play();
                        Stat = Stat.LevelSelection;
                    }
                    if (FirstLevel.healthCount == 0)
                    {
                        FirstLevel.Reset();
                        flag = true;
                        Stat = Stat.GameOver;
                    }
                    break;

                case Stat.GameOver:
                    if (flag)
                        CheckLevelChange(mouseState, new Rectangle(780, 950, 350, 50), Stat.FirstLevel);
                    else
                        CheckLevelChange(mouseState, new Rectangle(780, 950, 350, 50), Stat.SecondLevel);
                    GameOver.alpha = 0f;
                    break;
            }
            previousMouseState = mouseState;
            base.Update(gameTime);
        }

        private MouseState HandleMouseCursorState(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                Mouse.SetCursor(MouseCursor.FromTexture2D(bigCursorTexture, 0, 0));
            else if (mouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                Mouse.SetCursor(MouseCursor.FromTexture2D(cursorTexture, 0, 0));

            GameAudio.Update(gameTime);
            GameAudio.SetBackgroundMusicVolume(Settings.soundValue / 100f);
            return mouseState;
        }

        private void CheckLevelChange(MouseState mouseState, Rectangle rectangle, Stat status)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && rectangle.Contains(mouseState.Position))
            {
                VideoScreensaver.hasPlayed = 0;
                clickSound.Play();
                Stat = status;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            switch (Stat)
            {
                case Stat.Menu:
                    spriteBatch.Draw(cursorTexture, Vector2.Zero, Color.White);
                    Menu.Draw(spriteBatch);
                    break;
                case Stat.Settings:
                    Settings.Draw(spriteBatch);
                    break;
                case Stat.VideoScreensaver:
                    VideoScreensaver.Draw(spriteBatch);
                    break;
                case Stat.LevelSelection:
                    LevelSelection.Draw(spriteBatch);
                    break;
                case Stat.InfoBoard:
                    InfoBoard.Draw(spriteBatch);
                    break;
                case Stat.SecondLevel:
                    SecondLevel.Draw(spriteBatch);
                    break;
                case Stat.FirstLevel:
                    FirstLevel.Draw(spriteBatch);
                    break;
                case Stat.GameOver:
                    GameOver.Draw(spriteBatch);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}





