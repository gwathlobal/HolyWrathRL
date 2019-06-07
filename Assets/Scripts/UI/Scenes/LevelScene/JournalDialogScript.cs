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

        GameManager.instance.nemeses.Sort((a, b) => (a.deathStatus.CompareTo(b.deathStatus)));

        foreach (Nemesis nemesis in GameManager.instance.nemeses)
        {
            if (nemesis.personalStatus != Nemesis.PersonalStatusEnum.hidden)
            {
                string str = "";
                Color32 color = new Color32(255, 255, 255, 255);
                str = System.String.Format("{0}\n{1}", nemesis.mob.GetFullName(), MobTypes.mobTypes[nemesis.mob.idType].name);

                if (nemesis.deathStatus == Nemesis.DeathStatusEnum.deceased)
                    color = new Color32(135, 135, 135, 255);

                GameObject nemsisPanel = GameObject.Instantiate(NemesisPanelPrefab);
                nemsisPanel.transform.SetParent(NemesisScrollPanel.transform, false);
                nemsisPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0 + i * h * -1);

                nemsisPanel.GetComponent<NemesisPanelScript>().InitializeUI(this, str, nemesis.GetNemesisDescription(), color);
                i++;
                nemesisPanels.Add(nemsisPanel);
            }
        }
        if (nemesisPanels.Count > 0)
            nemesisPanels[0].GetComponent<NemesisPanelScript>().OnClick();

        NemesisScrollPanel.rectTransform.sizeDelta = new Vector2(w, i * h);
    }
}
