using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);
    }
	
    public void ExitToMainMenu()
    {
        UIManager.instance.ExitToMainMenu();
    }

    public void QuitToDesktop()
    {
        UIManager.instance.QuitToDesktop();
    }
}
