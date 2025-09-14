using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    public static NotePool instance;

    [SerializeField] private GameNote NotePrefab;
    [SerializeField] private int poolSize = 100;

    private List<GameNote> pool = new List<GameNote>();

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameNote gameNote = Instantiate(NotePrefab, transform);
            gameNote.gameObject.SetActive(false);
            pool.Add(gameNote);
        }
    }

    public GameNote GetNote(int spawnPos, ulong startTime)
    {
        for (int i = 0;i < pool.Count;i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
            {
                pool[i].InitNote(spawnPos, startTime);
                return pool[i];
            }
        }

        GameNote newNote = Instantiate (NotePrefab, transform);
        newNote.gameObject.SetActive(false);
        pool.Add (newNote);

        newNote.InitNote(spawnPos, startTime);
        return newNote;
    }

    /// <summary>
    /// 해당 라인에서 활성화된 가장 아래(가장 먼저 생성된) 노트 반환
    /// </summary>
    public GameNote GetOldestActiveNote(int line)
    {
        GameNote oldest = null;
        ulong minStartTime = ulong.MaxValue;

        foreach (var note in pool)
        {
            if (note.gameObject.activeInHierarchy && note.line == line)
            {
                // startTime이 가장 작은(가장 오래된) 노트 선택
                var noteStartTime = typeof(GameNote).GetField("startTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(note);
                ulong st = (ulong)noteStartTime;
                if (st < minStartTime)
                {
                    minStartTime = st;
                    oldest = note;
                }
            }
        }
        return oldest;
    }

    /// <summary>
    /// 키 입력 시 호출: 해당 라인에서 가장 아래의 활성화된 노트에 판정 처리
    /// </summary>
    public void JudgeNoteOnKey(int line, ulong currentTime)
    {
        GameNote note = GetOldestActiveNote(line);
        if (note != null)
        {
            if (note.endTime - (double)currentTime <= 480)
            {
                AudioManager.instance.PlaySFX(SFX.HitSound);
                note.ReturnToPool();
            }
        }
    }

    private void Start()
    {
        
    }
}
