using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilCategoryScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    public AbilityPlayerCategoryEnum category;
    public AbilityDialogScript AbilityDialog;
    public Text txt; 

    private void Start()
    {
        txt.text = AbilityCategoryTypes.categories[category].name;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AbilityDialog.DescrText.text = AbilityCategoryTypes.categories[category].descr;
        txt.color = new Color32(255, 255, 0, 255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityDialog.DescrText.text = "";
        txt.color = new Color32(255, 255, 255, 255);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        AbilityDialog.curCategory = category;

        AbilityDialog.SetUpScrollablePanels(AbilityDialog.curCategory);
    }


}
