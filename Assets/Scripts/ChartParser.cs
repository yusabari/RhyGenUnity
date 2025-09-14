using System;
using System.Collections.Generic;
using UnityEngine;

public class NoteData : MonoBehaviour
{
    public void setData(int _node, int _nodeLength, int _nodeSeq, int _line)
    {
        node = _node;
        nodeLength = _nodeLength;
        nodeSeq = _nodeSeq;
        line = _line;
    }
    public int node;
    public int nodeLength;
    public int nodeSeq;
    public int line;

}

public class ChartParser : MonoBehaviour
{
    public static ChartParser instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    public void Parse(string name = "_a")
    {
        int[] lineSeq = { 1, 2, 3, 4, 5, 6, 8, 9 };

        if (!(FileImportManager.Instance.chart == null))
        {
            string[] lines = FileImportManager.Instance.chart.Split("n");
            UnityEngine.Debug.Log(lines);
            List<NoteData> noteList = new List<NoteData>();
            foreach (string line in lines)
            {
                if (line.StartsWith("#")) {
                    UnityEngine.Debug.Log(line);
                    string[] parts = line.Split(':');
                    int lane = int.Parse(parts[0].Substring(5, 1));
                    
                    if (lane == 1)
                    {
                        noteList = new List<NoteData>();
                    }
                    UnityEngine.Debug.Log(parts[0].Substring(1, 3) + "마디: " + parts[0].Substring(5, 1) + "라인" + parts[1]);

                    for (int i = 0; i < parts[1].Length / 2; i++)
                    {
                        if (parts[1].Substring(i * 2 + 1, 1) == "1")
                        {
                            NoteData newNote = new NoteData();
                            newNote.setData(int.Parse(parts[0].Substring(1, 3)), parts[1].Length / 2, i, Array.IndexOf(lineSeq, lane));
                            noteList.Add(newNote);
                        }
                    }

                    if (lane == 9)
                    {
                        DSPTimer.instance.noteQueue.Enqueue(noteList);
                    }
                    
                }
            }
            UnityEngine.Debug.Log(DSPTimer.instance.noteQueue.Count);

        }
    }
}
