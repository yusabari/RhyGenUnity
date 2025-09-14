using FMOD;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DSPTimer : MonoBehaviour
{
    public static DSPTimer instance;

    ChannelGroup channelGroup;

    ulong nextDSPClock;
    ulong musicPlayDSPClock;

    public Queue<List<NoteData>> noteQueue = new Queue<List<NoteData>>();
    bool musicPlaying = false;
    static double bpm;
    static int sampleRate = 48000;
    int seqLength;
    int seqCount;
    double tickRate;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 이벤트 생성
        RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);

        bpm = FileImportManager.Instance.bpm;
        tickRate = sampleRate * 4 * (double)(60.0 / bpm);

        ulong dspClock;
        ulong parentClock;
        channelGroup.getDSPClock(out dspClock, out parentClock);

        UnityEngine.Debug.Log($"ChannelGroup DSP Clock: {dspClock}");

        UnityEngine.Debug.Log($"Event DSP Clock: {dspClock}");

        NotePool.instance.Init();
        ChartParser.instance.Parse();

        seqLength = noteQueue.Count;
        seqCount = 0;

        // 2초 뒤부터 재생 시작 (lookahead)
        nextDSPClock = parentClock + (ulong)(sampleRate * 2); // 48kHz 기준
        musicPlayDSPClock = parentClock + (ulong)(sampleRate * 2);
        for (int i = 0; i < 4; i++)
        {
            poolNoteSeq();
        }
    }

    private void FixedUpdate()
    {
        ProcessDSPTicks();
    }

    void ProcessDSPTicks()
    {
        ulong currentClock;
        channelGroup.getDSPClock(out currentClock, out _);

        while (!musicPlaying & currentClock > musicPlayDSPClock)
        {
            musicPlaying = true;
            AudioManager.instance.PlayMusic();
        }
        while (currentClock > nextDSPClock)
        {
            // AudioManager.instance.PlaySFX(SFX.HitSound);
            poolNoteSeq();
            seqCount++;
            nextDSPClock += (ulong)(tickRate);
        }
        if (musicPlaying && seqCount > seqLength)
        {
            SceneManager.LoadScene("SongEndScene");
        }
    }

    public ulong getTime()
    {
        ulong currentClock;
        channelGroup.getDSPClock(out currentClock, out _);
        return currentClock;
    }

    void poolNoteSeq()
    {
        if (noteQueue.Count == 0)
        {

        }
        else
        {
            List<NoteData> noteList = noteQueue.Dequeue();

            for (int i = 0; i < noteList.Count; i++)
            {
                NoteData data = noteList[i];
                NotePool.instance.GetNote(data.line, (ulong)(musicPlayDSPClock + tickRate * (data.node) + tickRate * ((double)data.nodeSeq / (double)data.nodeLength)));
            }
        }
    }
}
