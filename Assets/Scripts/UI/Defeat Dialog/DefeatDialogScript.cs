using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefeatDialogScript : MonoBehaviour {

    public Text msgTxt;
    public ScrollRect msgScrollRect;
    public Text killedByTxt;

    private void Start()
    {
        msgTxt.text = GameManager.instance.msgLog;
        msgScrollRect.verticalNormalizedPosition = 0;
        killedByTxt.text = GameManager.instance.player.killedBy;
    }

    public void GoToMainMenu()
    {
        UIManager.instance.ExitToMainMenu();
    }
}
