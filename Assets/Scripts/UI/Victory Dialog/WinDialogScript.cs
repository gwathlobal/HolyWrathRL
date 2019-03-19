using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinDialogScript : MonoBehaviour {

	public void GoToMainMenu()
    {
        UIManager.instance.ExitToMainMenu();
    }
}
