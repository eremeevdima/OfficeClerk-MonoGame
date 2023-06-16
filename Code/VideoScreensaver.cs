using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

public static class VideoScreensaver
{
    private static VideoPlayer videoPlayer;
    private static Video currentVideo;
    public static int hasPlayed = 0;


    public static void LoadContent(ContentManager content)
    {
        currentVideo = content.Load<Video>("Clipchamp");
    }
    public static void Update()
    {
        if (videoPlayer == null)
            videoPlayer = new VideoPlayer();
        if(!IsPlaying())
        {
            videoPlayer.Volume = 1.0f;
            videoPlayer.Play(currentVideo);
            hasPlayed += 1;
            if (hasPlayed == 2)
                videoPlayer.Stop();
        }
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        if (IsPlaying())
            spriteBatch.Draw(GetTexture(), new Rectangle(0, 0, 1920, 1080), Color.White);
    }

    public static Texture2D GetTexture()
    {
        return videoPlayer.GetTexture();
    }

    public static bool IsPlaying()
    {
        return videoPlayer != null && videoPlayer.State == MediaState.Playing;
    }
}
