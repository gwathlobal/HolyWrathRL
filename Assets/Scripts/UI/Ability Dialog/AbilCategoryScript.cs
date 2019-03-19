using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilCategoryScript : MonoBehaviour, IPointerClickHandler {

    public AbilityPlayerCategory category;
    public AbilityDialogScript AbilityDialog;

    public void OnPointerClick(PointerEventData eventData)
    {

        AbilityDialog.curCategory = category;

        AbilityDialog.SetUpScrollablePanels(AbilityDialog.curCategory);
    }
}
