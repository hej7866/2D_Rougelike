using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class UIManager : SingleTon<UIManager>
{
    [Header("View")]
    public PlayerView playerView;
    public AgumentView agumentView;
    public SkillRuntimeView skillRuntimeView;

    void OnEnable()  => HideCursor();
    void OnDisable() => ShowCursor();

    public void SwitchUI(GameObject ui)
    {
        bool b = ui.activeSelf ? false : true;
        ui.SetActive(b);
    }

    void HideCursor()
    {
        Cursor.visible = false;

    }
    
    void ShowCursor()
    {
        Cursor.visible = true;
    }
}
