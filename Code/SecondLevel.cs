using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;


public class Boss
{
    public Vector2 Position;
    public float Rotation;
    public Texture2D Texture;
    public bool IsPaperDropped;

    public Boss(Vector2 position, float rotation, Texture2D texture)
    {
        Position = position;
        Rotation = rotation;
        Texture = texture;
    }
}

public static class SecondLevel
{
    private struct PixelColor
    {
        public byte R, G, B, A;

        public PixelColor(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }
    }

    private static Texture2D levelTexture1;
    private static Texture2D levelTexture2;
    private static Texture2D levelTexture3;

    private static Texture2D paperTexture;
    public static List<(Stopwatch, Paper)> papers = new List<(Stopwatch, Paper)>();
    public static List<Vector2> trajectoryPoints = new List<Vector2>();

    private static SoundEffect wallSound;
    private static Texture2D[] wallTextures = new Texture2D[4];
    private static float[] highlightTimes = new float[4];
    private static Rectangle[] wallBounds = new Rectangle[4];

    private static Texture2D boss;
    private static SoundEffect bossSound;
    private static Texture2D circleBoss;
    public static List<(Boss, Stopwatch)> xBoss = new List<(Boss, Stopwatch)>();

    private static SpriteFont scoreFont;
    private static int bestStore;
    public static int score;

    public static bool isMouseDragging = false;
    public static Vector2 mouseStartPosition;

    private static float addTimer;
    public static int healthСount = 3;

    public static void LoadContent(ContentManager content)
    {
        paperTexture = content.Load<Texture2D>("Paper");

        scoreFont = content.Load<SpriteFont>("ScoreFont");

        bossSound = content.Load<SoundEffect>("TrashCanSound");
        boss = content.Load<Texture2D>("Boss");
        circleBoss = content.Load<Texture2D>("CircleBoss");

        levelTexture1 = content.Load<Texture2D>("Level (1)");
        levelTexture2 = content.Load<Texture2D>("Level (2)");
        levelTexture3 = content.Load<Texture2D>("Level (3)");

        wallSound = content.Load<SoundEffect>("WallSound");
        wallTextures[0] = content.Load<Texture2D>("Right");
        wallTextures[1] = content.Load<Texture2D>("Bottom");
        wallTextures[2] = content.Load<Texture2D>("Left");
        wallTextures[3] = content.Load<Texture2D>("Upper");
        wallBounds[0] = new Rectangle(1920, 0, 0, 1080);
        wallBounds[1] = new Rectangle(0, 1080, 1920, 0);
        wallBounds[2] = new Rectangle(paperTexture.Width, 0, 0, 1080);
        wallBounds[3] = new Rectangle(0, paperTexture.Height, 1920, 0);
    }

    public static void Reset()
    {
        isMouseDragging = false;
        mouseStartPosition = Vector2.Zero;
        trajectoryPoints.Clear();
    }

    public static void Update(GameTime gameTime)
    {
        var stopwatch = Stopwatch.StartNew();
        if (papers.Count == 0)
            papers.Add((stopwatch, new Paper(false, new Vector2(960, 540), 0f, paperTexture, Vector2.Zero)));
        var flag = 0;
        foreach (var paper in papers)
            if (paper.Item2.Position == new Vector2(960, 540))
                flag++;
        if (flag == 1)
            papers.Add((stopwatch, new Paper(false, new Vector2(960, 540), 0f, paperTexture, Vector2.Zero)));

        CreateBoss(gameTime);

        List<(Stopwatch, Paper)> papersToRemove = new List<(Stopwatch, Paper)>();

        CreatingRemovingCollection(gameTime, papersToRemove);

        foreach (var paperToRemove in papersToRemove)
        {
            papers.Remove(paperToRemove);
        }
    }

    private static void CreatingRemovingCollection(GameTime gameTime, List<(Stopwatch, Paper)> papersToRemove)
    {
        foreach (var paper in papers)
        {
            var paperBounds = new Rectangle((int)paper.Item2.Position.X, (int)paper.Item2.Position.Y, paperTexture.Width, paperTexture.Height);

            UpdateScore(paperBounds, paper, papersToRemove);

            BounceOffTheWall(gameTime, paperBounds);

            if (paper.Item2.Position != new Vector2(960, 540))
            {
                if (!paper.Item2.ThrowFlag)
                    paper.Item1.Restart();
                paper.Item2.ThrowFlag = true;
            }

            if (paper.Item2.ThrowFlag && paper.Item1.ElapsedMilliseconds > 3000)
                papersToRemove.Add(paper);

            if (!isMouseDragging)
            {
                var paperCenterX = paper.Item2.Position.X + paperTexture.Width / 2;
                var paperCenterY = paper.Item2.Position.Y + paperTexture.Height / 2;

                CheckWallPosition(paper, paperCenterX, paperCenterY);
                if (paper.Item2.Position == new Vector2(960, 540))
                    paper.Item2.Position += paper.Item2.Velocity;
            }
            if (paper.Item2.Position != new Vector2(960, 540))
            {
                paper.Item2.Velocity.Y += 1f / 60f;
                paper.Item2.Rotation += 0.1f;
                paper.Item2.Position += paper.Item2.Velocity;
            }
        }
    }

    private static void UpdateScore(Rectangle paperBounds, (Stopwatch, Paper) paper, List<(Stopwatch, Paper)> papersToRemove)
    {
        foreach (var boss in xBoss)
        {
            var bossRect = new Rectangle((int)boss.Item1.Position.X, (int)boss.Item1.Position.Y, boss.Item1.Texture.Width, boss.Item1.Texture.Height);
            if (bossRect.Intersects(paperBounds))
            {
                xBoss.Remove(boss);
                score++;
                bossSound.Play();
                if (score > bestStore)
                    bestStore = score;
                Reset();
                papersToRemove.Add(paper);
                break;
            }
        }
    }



    private static void CheckWallPosition((Stopwatch, Paper) paper, float paperCenterX, float paperCenterY)
    {
        if (paperCenterX - paperTexture.Width < 0)
        {
            paper.Item2.Velocity.X = Math.Abs(paper.Item2.Velocity.X);
            paper.Item2.Position.X = paperTexture.Width / 2;
            wallSound.Play();
        }
        else if (paperCenterX > 1920)
        {
            paper.Item2.Velocity.X = -Math.Abs(paper.Item2.Velocity.X);
            paper.Item2.Position.X = 1920 - paperTexture.Width / 2;
            wallSound.Play();
        }

        if (paperCenterY - paperTexture.Height < 0)
        {
            paper.Item2.Velocity.Y = Math.Abs(paper.Item2.Velocity.Y);
            paper.Item2.Position.Y = paperTexture.Height / 2;
            wallSound.Play();
        }
        else if (paperCenterY > 1080)
        {
            paper.Item2.Velocity.Y = -Math.Abs(paper.Item2.Velocity.Y);
            paper.Item2.Position.Y = 1080 - paperTexture.Height / 2;
            wallSound.Play();
        }
    }

    private static void BounceOffTheWall(GameTime gameTime, Rectangle paperBounds)
    {
        for (int i = 0; i < 4; i++)
            if (paperBounds.Intersects(wallBounds[i]))
                highlightTimes[i] = 0.5f;
        for (int i = 0; i < 4; i++)
            if (highlightTimes[i] > 0)
                highlightTimes[i] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    private static void CreateBoss(GameTime gameTime)
    {
        addTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (addTimer <= 0)
        {
            var stopwatch = Stopwatch.StartNew();
            var rnd = new Random();

            int[,] positions = new int[,] { { 480, 270 }, { 1440, 270 }, { 1440, 810 }, { 480, 810 } };
            var x = positions[rnd.Next(0, 3), 0] + rnd.Next(-280, 280);
            var y = positions[rnd.Next(0, 3), 1] + rnd.Next(-120, 120);
            var position = new Vector2(x, y);
            var rotation = rnd.Next(0, 8) * 15;

            var isOverlapping = false;
            var firstBounds = new Rectangle(x + 50, y + 50, boss.Width + 50, boss.Height + 50);
            foreach (var boss in xBoss)
            {
                var secondBounds = new Rectangle((int)boss.Item1.Position.X + 50, (int)boss.Item1.Position.Y + 50, boss.Item1.Texture.Width + 50, boss.Item1.Texture.Height + 50);
                if (firstBounds.Intersects(secondBounds))
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (!isOverlapping)
            {
                xBoss.Add((new Boss(position, rotation, SecondLevel.boss), stopwatch));
                addTimer = 2f;
            }
        }
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        switch (healthСount)
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
        spriteBatch.DrawString(scoreFont, "Score: " + score, new Vector2(1600, 60), Color.AntiqueWhite);
        spriteBatch.DrawString(scoreFont, "Best Score: " + bestStore, new Vector2(1600, 10), Color.AntiqueWhite);
        foreach (var paper in papers)
            spriteBatch.Draw(paperTexture, paper.Item2.Position, null, Color.White, paper.Item2.Rotation, new Vector2(paperTexture.Width / 2, paperTexture.Height / 2), 1f, SpriteEffects.None, 0f);
        CheckMouse(spriteBatch);
        foreach (var boss in xBoss)
        {
            Vector2 circleCenter = boss.Item1.Position;
            spriteBatch.Draw(boss.Item1.Texture, boss.Item1.Position, null, Color.White, boss.Item1.Rotation, new Vector2(boss.Item1.Texture.Width / 2, boss.Item1.Texture.Height / 2), 1f, SpriteEffects.None, 0f);

            if (boss.Item2.ElapsedMilliseconds > 3000 && boss.Item2.ElapsedMilliseconds < 5000)
            {
                float alpha = MathHelper.Lerp(1f, 0f, boss.Item2.ElapsedMilliseconds / 5000);
                spriteBatch.Draw(boss.Item1.Texture, boss.Item1.Position, null, Color.Red * alpha, boss.Item1.Rotation, new Vector2(boss.Item1.Texture.Width / 2, boss.Item1.Texture.Height / 2), 1f, SpriteEffects.None, 0f);
            }

            if (boss.Item2.ElapsedMilliseconds >= 5000)
            {
                xBoss.Remove(boss);
                healthСount--;
                break;
            }
            CheckColor(boss);
            spriteBatch.Draw(circleBoss, circleCenter, null, Color.White, 0, new Vector2(circleBoss.Width / 2, circleBoss.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }

    private static void CheckMouse(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < 4; i++)
            if (highlightTimes[i] > 0)
            {
                var alpha = MathHelper.Lerp(1f, 0f, highlightTimes[i] / 0.5f);
                spriteBatch.Draw(wallTextures[i], Vector2.Zero, new Color(1f, 1f, 1f, alpha));
            }

        if (isMouseDragging)
        {
            var mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            var direction = mousePosition - mouseStartPosition;
            var tempPaperPosition = new Vector2(960, 540);
            var tempPaperVelocity = direction / 10f;
            for (int i = 0; i < 60; i++)
            {
                spriteBatch.Draw(paperTexture, tempPaperPosition, null, Color.Black, 0f, new Vector2(paperTexture.Width / 2, paperTexture.Height / 2), 0.1f, SpriteEffects.None, 0f);
                tempPaperVelocity.Y += 1f / 60f;
                tempPaperPosition += tempPaperVelocity;
            }
        }
    }

    private static void CheckColor((Boss, Stopwatch) boss)
    {
        foreach (var paper in papers)
        {
            Color[] circleData = new Color[circleBoss.Width * circleBoss.Height];
            circleBoss.GetData(circleData);
            var paperBounds = new Rectangle((int)paper.Item2.Position.X, (int)paper.Item2.Position.Y, paperTexture.Width, paperTexture.Height);
            var circleBounds = new Rectangle((int)boss.Item1.Position.X - circleBoss.Width / 2, (int)boss.Item1.Position.Y - circleBoss.Height / 2, circleBoss.Width, circleBoss.Height);
            if (paperBounds.Intersects(circleBounds))
            {
                int offsetX = (int)(paper.Item2.Position.X - boss.Item1.Position.X + circleBoss.Width / 2);
                int offsetY = (int)(paper.Item2.Position.Y - boss.Item1.Position.Y + circleBoss.Height / 2);
                if (offsetX >= 0 && offsetX <= circleBoss.Width && offsetY >= 0 && offsetY <= circleBoss.Height)
                {
                    PixelColor pixelColor = new PixelColor(circleData[offsetY * circleBoss.Width + offsetX]);
                    if (pixelColor.A > 0)
                    {
                        var direction = paper.Item2.Position - boss.Item1.Position;
                        direction.Normalize();
                        paper.Item2.Velocity = direction * 5f;
                    }
                }
            }
        }
    }
}
