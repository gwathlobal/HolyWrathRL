using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalDialogScript : MonoBehaviour {

    public GameObject NemesisPanelPrefab;
    public Image NemesisScrollPanel;
    public Text DescrText;

    public List<GameObject> nemesisPanels;

    // Use this for initialization
    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);

        nemesisPanels = new List<GameObject>();
    }

    public void InitializeNemesis()
    {
        foreach (GameObject go in nemesisPanels)
        {
            Destroy(go);
        }
        nemesisPanels = new List<GameObject>();

        DescrText.text = "";

        int i = 0;
        float w = NemesisPanelPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float h = NemesisPanelPrefab.GetComponent<RectTransform>().sizeDelta.y;

        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            GameObject nemsisPanel = GameObject.Instantiate(NemesisPanelPrefab);
            nemsisPanel.transform.SetParent(NemesisScrollPanel.transform, false);
            nemsisPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0 + i * h * -1);

            nemsisPanel.GetComponent<NemesisPanelScript>().InitializeUI(this, nemesis.mob.GetFullName() + "\n" + MobTypes.mobTypes[nemesis.mob.idType].name,
                nemesis.GetNemesisDescription());
            i++;
            nemesisPanels.Add(nemsisPanel);
        }
        NemesisScrollPanel.rectTransform.sizeDelta = new Vector2(w, i * h);
    }
}
