using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatDialogScript : MonoBehaviour {

	public void GoToMainMenu()
    {
        UIManager.instance.ExitToMainMenu();
    }
}
