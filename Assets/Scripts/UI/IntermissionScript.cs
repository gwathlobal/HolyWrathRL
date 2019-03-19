using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermissionScript : MonoBehaviour {

    enum IntermissionDialogStatus
    {
        nemesis, ability
    }

    IntermissionDialogStatus status;
    public GameObject nemesisPanel;
    public GameObject abilityPanel;
    public NextLocFunc func;

    // Use this for initialization
    void Start () {
        ShowNemesisDialog();
	}

    public void ShowNemesisDialog()
    {
        status = IntermissionDialogStatus.nemesis;
        nemesisPanel.SetActive(true);
        abilityPanel.SetActive(false);
    }

    public void ShowAbilityDialog()
    {
        status = IntermissionDialogStatus.ability;
        nemesisPanel.SetActive(false);
        abilityPanel.SetActive(true);
    }
}
