using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] lanes; // 레인 위치 (ex. 4레인)
    [SerializeField] private float noteSpeed = 5f;

    InputAction key0Action;
    InputAction key1Action;
    InputAction key2Action;
    InputAction key3Action;
    InputAction key4Action;
    InputAction key5Action;
    InputAction key6Action;
    InputAction key7Action;


    private void Awake()
    {
        
    }

    private void Start()
    {
        key0Action = InputSystem.actions.FindAction("Key0");
        key1Action = InputSystem.actions.FindAction("Key1");
        key2Action = InputSystem.actions.FindAction("Key2");
        key3Action = InputSystem.actions.FindAction("Key3");
        key4Action = InputSystem.actions.FindAction("Key4");
        key5Action = InputSystem.actions.FindAction("Key5");
        key6Action = InputSystem.actions.FindAction("Key6");
        key7Action = InputSystem.actions.FindAction("Key7");
    }
    void Update()
    {
        bool Key0Value = key0Action.WasPressedThisFrame(); // isPressed() 를 사용하면 키다운 유지도 인식하여 노트가 이어짐
        bool Key1Value = key1Action.WasPressedThisFrame();
        bool Key2Value = key2Action.WasPressedThisFrame();
        bool Key3Value = key3Action.WasPressedThisFrame();
        bool Key4Value = key4Action.WasPressedThisFrame();
        bool Key5Value = key5Action.WasPressedThisFrame();
        bool Key6Value = key6Action.WasPressedThisFrame();
        bool Key7Value = key7Action.WasPressedThisFrame();
        // 테스트용: 각 키 누르면 노트 스폰
        if (Key0Value)
        {
            NotePool.instance.JudgeNoteOnKey(0, DSPTimer.instance.getTime());
        }
        if (Key1Value)
        {
            NotePool.instance.JudgeNoteOnKey(1, DSPTimer.instance.getTime());
        }
        if (Key2Value)
        {
            NotePool.instance.JudgeNoteOnKey(2, DSPTimer.instance.getTime());
        }
        if (Key3Value)
        {
            NotePool.instance.JudgeNoteOnKey(3, DSPTimer.instance.getTime());
        }
        if (Key4Value)
        {
            NotePool.instance.JudgeNoteOnKey(4, DSPTimer.instance.getTime());
        }
        if (Key5Value)
        {
            NotePool.instance.JudgeNoteOnKey(5, DSPTimer.instance.getTime());
        }   
        if (Key6Value)
        {
            NotePool.instance.JudgeNoteOnKey(6, DSPTimer.instance.getTime());
        }
        if (Key7Value)
        {
            NotePool.instance.JudgeNoteOnKey(7, DSPTimer.instance.getTime());
        }
    }
}