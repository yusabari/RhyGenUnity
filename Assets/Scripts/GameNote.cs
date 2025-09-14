using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;

public class GameNote : MonoBehaviour
{
    public bool isActive = false;
    ulong startTime;
    public ulong endTime;
    ulong time;
    float speed = 1;
    public int line; // 노트가 속한 라인 정보 추가

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //time = DSPTimer.instance.getTime();
        //startTime = 48000;
        //endTime = startTime + 96000;
        //isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        time = DSPTimer.instance.getTime();
        if (isActive && time >= startTime && time <= endTime)
        {
            float ypos;
            ypos = -4 + (float)((double)(endTime - time) / 4800 * speed);
            transform.position = new Vector3(transform.position.x, ypos, transform.position.z);
        }
        else if (isActive && time > endTime && time <= (endTime + 24000))
        {
            float ypos;
            ypos = -4 - (float)((double)(time - endTime) / 4800 * speed);
            transform.position = new Vector3(transform.position.x, ypos, transform.position.z);
        }
        else if (isActive && time > (endTime + 4800/ speed))
        {
            ReturnToPool();
        }
    }

    public void InitNote(int _line, ulong _endTime)
    {
        line = _line; // 라인 정보 저장
        endTime = _endTime;
        startTime = endTime - 48000;

        transform.position = new Vector3((float)(_line - 3.5), 6, 0);
        isActive = true;
        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
