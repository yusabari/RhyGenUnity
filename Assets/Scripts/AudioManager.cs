using System.IO;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Instance")]
    public static AudioManager instance;

    [Header("SFX")]
    FMOD.ChannelGroup sfxChannelGroup;
    FMOD.Sound[] sfxs;
    FMOD.Channel[] sfxChannels;

    [Header("Music")]
    FMOD.ChannelGroup musicChannelGroup;
    FMOD.Sound music;
    FMOD.Channel musicChannel;

    private void Awake()
    {
        instance = this;
        LoadSFX();
        LoadMUSIC();
    }

    void LoadSFX() // 효과음 로드 함수
    {
        int count = System.Enum.GetValues(typeof(SFX)).Length;

        sfxChannelGroup = new FMOD.ChannelGroup();
        sfxChannels = new FMOD.Channel[count];
        sfxs = new FMOD.Sound[count];

        for(int i = 0; i < count; i++)
        {
            string sfxFileName = System.Enum.GetName(typeof(SFX), i);
            string audioType = "mp3";

            FMODUnity.RuntimeManager.CoreSystem.createSound(Path.Combine(Application.streamingAssetsPath, "SFXS", sfxFileName + "." + audioType), FMOD.MODE.CREATESAMPLE, out sfxs[i]);
        }

        for (int i = 0; i < count; i++)
        {
            sfxChannels[i].setChannelGroup(sfxChannelGroup);
        }
    }

    public void PlaySFX(SFX _sfx, float _volume = 1)
    {
        sfxChannels[(int)_sfx].stop();

        FMODUnity.RuntimeManager.CoreSystem.playSound(sfxs[(int)_sfx], sfxChannelGroup, false, out sfxChannels[(int)_sfx]);

        sfxChannels[(int)_sfx].setPaused(true);
        sfxChannels[(int)_sfx].setVolume((_volume) * 100);
        sfxChannels[(int)_sfx].setPaused(false);
    }

    void LoadMUSIC()
    {
        musicChannelGroup = new FMOD.ChannelGroup();
        musicChannel = new FMOD.Channel();
        music = new FMOD.Sound();

        FMODUnity.RuntimeManager.CoreSystem.createSound(FileImportManager.Instance.filePath, FMOD.MODE.CREATESAMPLE, out music);

        musicChannel.setChannelGroup(musicChannelGroup);
    }

    public void PlayMusic()
    {
        musicChannel.stop();

        FMODUnity.RuntimeManager.CoreSystem.playSound(music, musicChannelGroup, false, out musicChannel);
        musicChannel.setPaused(false);
    }
}
