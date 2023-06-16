using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;


public static class GameAudio
{
    private static SoundEffect backgroundMusic;
    private static SoundEffectInstance backgroundMusicInstance;
    private static bool isPlayingBackgroundMusic = false;
    private static float backgroundMusicVolume = 0.6f;

    public static void LoadContent(ContentManager content)
    {
        backgroundMusic = content.Load<SoundEffect>("BackgroundMusic");
        backgroundMusicInstance = backgroundMusic.CreateInstance();
        backgroundMusicInstance.IsLooped = true;
        backgroundMusicInstance.Volume = backgroundMusicVolume;
    }

    public static void Update(GameTime gameTime)
    {
        backgroundMusicInstance.Play();
    }

    public static void PlayBackgroundMusic()
    {
        if (!isPlayingBackgroundMusic)
        {
            backgroundMusicInstance.Play();
            isPlayingBackgroundMusic = true;
        }
    }

    public static void PauseBackgroundMusic()
    {
        if (isPlayingBackgroundMusic)
        {
            backgroundMusicInstance.Pause();
            isPlayingBackgroundMusic = false;
        }
    }

    public static void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicVolume = MathHelper.Clamp(volume, 0f, 1f);
        backgroundMusicInstance.Volume = backgroundMusicVolume;
    }

    public static float GetBackgroundMusicVolume()
    {
        return backgroundMusicVolume;
    }
}

