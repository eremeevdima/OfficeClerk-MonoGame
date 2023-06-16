using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using static SecondLevel;


public static class AuxiliaryMethods
{
    public static void DrawTextureWithHover(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, MouseState mouseState, int a, int b)
    {
        var textureRectangle = new Rectangle((int)position.X, (int)position.Y, a, b);
        var isHovering = textureRectangle.Contains(mouseState.Position);
        if (isHovering)
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }
    public static void OnMouseDown(Vector2 position)
    {
        if (!isMouseDragging && papers[papers.Count - 1].Item2.Position.X == 960 && papers[papers.Count - 1].Item2.Position.Y == 540)
        {
            isMouseDragging = true;
            mouseStartPosition = position;
        }
    }

    public static void OnMouseUp()
    {
        if (isMouseDragging && papers[papers.Count - 1].Item2.Position.X == 960 && papers[papers.Count - 1].Item2.Position.Y == 540)
        {
            var mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            var direction = mousePosition - mouseStartPosition;
            papers[papers.Count - 1].Item2.Velocity = direction / 10f;
            isMouseDragging = false;
            CalculatePaperTrajectory(papers[papers.Count - 1].Item2.Position);
        }
    }

    public static void OnMouseMove()
    {
        if (isMouseDragging && papers[papers.Count - 1].Item2.Position.X == 960 && papers[papers.Count - 1].Item2.Position.Y == 540)
        {
            var mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            var direction = mousePosition - mouseStartPosition;
            var distance = direction.Length();
            var angle = (float)Math.Atan2(direction.Y, direction.X);
            var speed = distance;
            papers[papers.Count - 1].Item2.Velocity = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
        }
    }

    private static void CalculatePaperTrajectory(Vector2 startPosition)
    {
        var trajectoryPoints = new List<Vector2>();
        var currentPosition = startPosition;
        var currentVelocity = papers[papers.Count - 1].Item2.Velocity;
        var gravity = 9.81f;
        var timeStep = 0.1f;
        var maxTime = 5f;
        var currentTime = 0f;

        while (currentTime <= maxTime)
        {
            trajectoryPoints.Add(currentPosition);
            currentPosition += currentVelocity * timeStep;
            currentVelocity += new Vector2(0, gravity) * timeStep;
            currentTime += timeStep;
        }
        SecondLevel.trajectoryPoints = trajectoryPoints;
    }
}
