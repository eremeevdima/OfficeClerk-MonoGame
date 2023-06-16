using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

public class Paper
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Rotation;
    public Texture2D Texture;
    public bool ThrowFlag;

    public Paper(bool throwFlag, Vector2 position, float rotation, Texture2D texture, Vector2 velocity)
    {
        ThrowFlag = throwFlag;
        Position = position;
        Rotation = rotation;
        Texture = texture;
        Velocity = velocity;
    }
}

public static class FirstLevel
{
    private static Texture2D levelTexture1;
    private static Texture2D levelTexture2;
    private static Texture2D levelTexture3;

    private static Texture2D dustBinTexture;
    private static Texture2D dustBinUpperTexture;
    private static Vector2 dustBinPosition;
    private static SoundEffect dustBinSound;

    private static Texture2D paperTexture;
    public static List<Paper> papers = new List<Paper>();

    private static SpriteFont scoreFont;
    public static int score;
    private static int bestScore;

    public static int healthCount = 3;
    private static float addTimer;

    public static void LoadContent(ContentManager content)
    {
        dustBinTexture = content.Load<Texture2D>("DustBin");
        dustBinUpperTexture = content.Load<Texture2D>("DustBinUpper");
        dustBinPosition = new Vector2(850, 800);
        dustBinSound = content.Load<SoundEffect>("TrashCanSound");

        paperTexture = content.Load<Texture2D>("Paper");
        scoreFont = content.Load<SpriteFont>("ScoreFont");

        levelTexture1 = content.Load<Texture2D>("Level (1)");
        levelTexture2 = content.Load<Texture2D>("Level (2)");
        levelTexture3 = content.Load<Texture2D>("Level (3)");
    }

    public static void Reset()
    {
        dustBinPosition = new Vector2(850, 800);
        addTimer = 0;
        score = 0;
        healthCount = 3;
        papers.Clear();
    }

    public static void Update(GameTime gameTime)
    {
        addTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (addTimer <= 0)
        {
            var rnd = new Random();
            var x = rnd.Next(50 + paperTexture.Width, 1870 - paperTexture.Width);
            var y = 0;
            var position = new Vector2(x, y);
            var rotation = 0;
            var velocity = new Vector2(0, 300);
            papers.Add(new Paper(false, position, rotation, paperTexture, velocity));
            addTimer = 1.5f;
        }
        KeyboardState keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Left) && dustBinPosition.X > 0)
            dustBinPosition.X -= 20;
        else if (keyboardState.IsKeyDown(Keys.Right) && (dustBinPosition.X + dustBinTexture.Width) < 1920)
            dustBinPosition.X += 20;
        foreach (var paper in papers.ToList())
        {
            var dustBinBounds = new Rectangle((int)dustBinPosition.X + paperTexture.Height, (int)dustBinPosition.Y + paperTexture.Height, dustBinTexture.Width - 2 * paperTexture.Height, dustBinTexture.Height - 2 * paperTexture.Height);
            var leftBoundary = new Rectangle((int)dustBinPosition.X, (int)dustBinPosition.Y + paperTexture.Height / 2, 1, dustBinTexture.Height + paperTexture.Height / 2);
            var rightBoundary = new Rectangle((int)(dustBinPosition.X + dustBinTexture.Width), (int)dustBinPosition.Y + paperTexture.Height / 2, 1, dustBinTexture.Height + paperTexture.Height / 2);
            var bottomBoundary = new Rectangle((int)dustBinPosition.X, (int)(dustBinPosition.Y + dustBinTexture.Height * 0.8), dustBinTexture.Width, 1);
            var paperBounds = new Rectangle((int)paper.Position.X, (int)paper.Position.Y, paper.Texture.Width, paper.Texture.Height);
            var rnd = new Random();
            var flag = true;
            if (paperBounds.Intersects(dustBinBounds))
            {
                if (keyboardState.IsKeyDown(Keys.Left) && dustBinPosition.X > 0)
                    paper.Position.X -= 20;
                else if (keyboardState.IsKeyDown(Keys.Right) && (dustBinPosition.X + dustBinTexture.Width) < 1920)
                    paper.Position.X += 20;
                flag = false;
                var movementSpeedY = 150f;
                var deltaY = movementSpeedY * (float)gameTime.ElapsedGameTime.TotalSeconds;
                paper.Position.Y += deltaY;

                if (paperBounds.Intersects(bottomBoundary))
                {
                    papers.Remove(paper);
                    score++;
                    flag = true;
                    dustBinSound.Play();
                    if (score > bestScore)
                        bestScore = score;
                    break;
                }
            }

            if (flag)
            {
                if (paperBounds.Intersects(leftBoundary))
                    HandlePaperBoundaryCollision(paper);
                else if (paperBounds.Intersects(rightBoundary))
                    HandlePaperBoundaryCollision(paper);
                paper.Velocity += new Vector2(0, (float)gameTime.ElapsedGameTime.TotalSeconds);
                paper.Position += paper.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                paper.Rotation += 0.04f;
            }

            CheckPaperBounds(paper);
        }
    }

    private static void CheckPaperBounds(Paper paper)
    {
        var paperCenterX = paper.Position.X + paperTexture.Width / 2;
        var paperCenterY = paper.Position.Y + paperTexture.Height / 2;
        Action<float, float> adjustVelocityAndPositionX = (absVelocityX, positionX) =>
        {
            paper.Velocity.X = Math.Abs(paper.Velocity.X) * absVelocityX;
            paper.Position.X = positionX;
        };

        if (paperCenterX + 300 < 0)
            adjustVelocityAndPositionX(1, paperTexture.Width / 2);
        else if (paperCenterX - 300 > 1920)
            adjustVelocityAndPositionX(-1, 1920 - paperTexture.Width / 2);

        if (paperCenterY + 300 < 0)
        {
            paper.Velocity.Y = Math.Abs(paper.Velocity.Y);
            paper.Position.Y = paperTexture.Height / 2;
        }

        if (paper.Position.Y + paper.Texture.Height > 1080)
        {
            papers.Remove(paper);
            healthCount--;
        }
    }

    private static void HandlePaperBoundaryCollision(Paper paper)
    {
        var direction = paper.Position - dustBinPosition;
        direction.Normalize();
        paper.Velocity = direction * 400f;
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        switch (healthCount)
        {
            case 1:
                spriteBatch.Draw(levelTexture1, Vector2.Zero, Color.White);
                break;
            case 2:
                spriteBatch.Draw(levelTexture2, Vector2.Zero, Color.White);
                break;
            case 3:
                spriteBatch.Draw(levelTexture3, Vector2.Zero, Color.White);
                break;
        }

        spriteBatch.Draw(dustBinUpperTexture, dustBinPosition, Color.White);
        foreach (var paper in papers)
            spriteBatch.Draw(paper.Texture, paper.Position, null, Color.White, paper.Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        spriteBatch.Draw(dustBinTexture, dustBinPosition, Color.White);
        spriteBatch.DrawString(scoreFont, "Score: " + score, new Vector2(1600, 60), Color.AntiqueWhite);
        spriteBatch.DrawString(scoreFont, "Best Score: " + bestScore, new Vector2(1600, 10), Color.AntiqueWhite);
    }
}