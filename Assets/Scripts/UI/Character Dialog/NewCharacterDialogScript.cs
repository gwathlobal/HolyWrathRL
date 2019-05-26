using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterDialogScript : MonoBehaviour
{
    public Text DescrText;

    private bool allCheckboxActive;
    public Toggle allCheckbox;
    private bool terrainCheckboxActive;
    public Toggle terrainCheckbox;
    private bool featuresCheckboxActive;
    public Toggle featuresCheckbox;
    private bool mobCheckboxActive;
    public Toggle mobCheckbox;
    private bool itemsCheckboxActive;
    public Toggle itemsCheckbox;

    private TerrainTypeEnum terrainType;
    private List<Feature> featureList;
    private Mob mob;
    private List<Item> itemList;
    private bool showMobOnly;

    // Use this for initialization
    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);

        allCheckboxActive = true;
        terrainCheckboxActive = true;
        featuresCheckboxActive = true;
        mobCheckboxActive = true;
        itemsCheckboxActive = true;
    }

    public void InitializeUI(TerrainTypeEnum _terrainType, List<Feature> _featureList, Mob _mob, List<Item> _itemList, bool _showMobOnly = false)
    {
        terrainType = _terrainType;
        mob = _mob;
        featureList = _featureList;
        itemList = _itemList;
        showMobOnly = _showMobOnly;

        if (showMobOnly)
        {
            allCheckbox.gameObject.SetActive(false);
            terrainCheckbox.gameObject.SetActive(false);
            featuresCheckbox.gameObject.SetActive(false);
            mobCheckbox.gameObject.SetActive(false);
            itemsCheckbox.gameObject.SetActive(false);
        }
        else
        {
            allCheckbox.gameObject.SetActive(true);
            Vector2 pos = allCheckbox.transform.position;
            
            pos = new Vector2(pos.x + 5 + allCheckbox.GetComponent<RectTransform>().sizeDelta.x, pos.y);


            terrainCheckbox.gameObject.SetActive(true);
            terrainCheckbox.transform.position = pos;
            pos = new Vector2(pos.x + 5 + terrainCheckbox.GetComponent<RectTransform>().sizeDelta.x, pos.y);

            if (featureList != null && featureList.Count > 0)
            {
                featuresCheckbox.gameObject.SetActive(true);
                featuresCheckbox.transform.position = pos;
                pos = new Vector2(pos.x + 5 + featuresCheckbox.GetComponent<RectTransform>().sizeDelta.x, pos.y);
            }
            else
                featuresCheckbox.gameObject.SetActive(false);

            if (mob != null)
            {
                mobCheckbox.gameObject.SetActive(true);
                mobCheckbox.transform.position = pos;
                pos = new Vector2(pos.x + 5 + mobCheckbox.GetComponent<RectTransform>().sizeDelta.x, pos.y);
            }
            else
                mobCheckbox.gameObject.SetActive(false);

            if (itemList != null && itemList.Count > 0)
            {
                itemsCheckbox.gameObject.SetActive(true);
                itemsCheckbox.transform.position = pos;
                pos = new Vector2(pos.x + 5 + itemsCheckbox.GetComponent<RectTransform>().sizeDelta.x, pos.y);
            }
            else
                itemsCheckbox.gameObject.SetActive(false);

            allCheckbox.isOn = true;
        }

        SetDescriptionText();
    }

    public void SetDescriptionText()
    { 
        DescrText.text = "";
        bool insertReturns = false;

        if (showMobOnly)
        {
            DescrText.text += mob.Description();
        }
        else
        {
            if (terrainCheckbox.isOn)
            {
                DescrText.text += "<b>TERRAIN</b>\n\n";
                DescrText.text += TerrainTypes.terrainTypes[terrainType].Description();
                insertReturns = true;
            }

            if (featuresCheckbox.gameObject.activeSelf && featuresCheckbox.isOn)
            {
                if (insertReturns)
                    DescrText.text += "\n\n";

                if (featureList.Count > 1)
                    DescrText.text += "<b>FEATURES</b>\n\n";
                else
                    DescrText.text += "<b>FEATURE</b>\n\n";
                foreach (Feature feature in featureList)
                {
                    DescrText.text += feature.GetEffectFullLine() + "\n";
                    if (feature.Description() == "")
                        DescrText.text += "\n";
                    else
                        DescrText.text += feature.Description() + "\n\n";

                }
                DescrText.text += "\n";
                insertReturns = false;
            }

            if (mobCheckbox.gameObject.activeSelf && mobCheckbox.isOn)
            {
                if (insertReturns)
                    DescrText.text += "\n\n";

                DescrText.text += "<b>CREATURE</b>\n\n";
                DescrText.text += mob.Description();
                insertReturns = true;
            }

            if (itemsCheckbox.gameObject.activeSelf && itemsCheckbox.isOn)
            {
                if (insertReturns)
                    DescrText.text += "\n\n";

                if (itemList.Count > 1)
                    DescrText.text += "<b>ITEMS</b>\n\n";
                else
                    DescrText.text += "<b>ITEM</b>\n\n";
                foreach (Item item in itemList)
                {
                    DescrText.text += item.name + "\n";
                    if (item.Description() == "")
                        DescrText.text += "\n";
                    else
                        DescrText.text += item.Description() + "\n\n";

                }
            }
        }
    }

    public void ToggleAll()
    {
        if (!allCheckboxActive) return;

        if (allCheckbox.isOn)
        {
            terrainCheckbox.isOn = true;
            featuresCheckbox.isOn = true;
            mobCheckbox.isOn = true;
            itemsCheckbox.isOn = true;
        }
        else
        {
            terrainCheckbox.isOn = false;
            featuresCheckbox.isOn = false;
            mobCheckbox.isOn = false;
            itemsCheckbox.isOn = false;
        }
        SetDescriptionText();
    }

    public void ToggleTerrain()
    {
        if (!terrainCheckboxActive) return;

        if (terrainCheckbox.isOn)
        {
            if (terrainCheckbox.isOn && mobCheckbox.isOn && featuresCheckbox.isOn && itemsCheckbox.isOn)
            {
                allCheckboxActive = false;
                allCheckbox.isOn = true;
                allCheckboxActive = true;
            }
        }
        else
        {
            allCheckboxActive = false;
            allCheckbox.isOn = false;
            allCheckboxActive = true;
        }
        SetDescriptionText();
    }

    public void ToggleMob()
    {
        if (!mobCheckboxActive) return;

        if (mobCheckbox.isOn)
        {
            if (terrainCheckbox.isOn && mobCheckbox.isOn && featuresCheckbox.isOn && itemsCheckbox.isOn)
            {
                allCheckboxActive = false;
                allCheckbox.isOn = true;
                allCheckboxActive = true;
            }
        }
        else
        {
            allCheckboxActive = false;
            allCheckbox.isOn = false;
            allCheckboxActive = true;
        }
        SetDescriptionText();
    }

    public void ToggleFeatures()
    {
        if (!featuresCheckboxActive) return;

        if (featuresCheckbox.isOn)
        {
            if (terrainCheckbox.isOn && mobCheckbox.isOn && featuresCheckbox.isOn && itemsCheckbox.isOn)
            {
                allCheckboxActive = false;
                allCheckbox.isOn = true;
                allCheckboxActive = true;
            }
        }
        else
        {
            allCheckboxActive = false;
            allCheckbox.isOn = false;
            allCheckboxActive = true;
        }
        SetDescriptionText();
    }

    public void ToggleItems()
    {
        if (!itemsCheckboxActive) return;

        if (itemsCheckbox.isOn)
        {
            if (terrainCheckbox.isOn && mobCheckbox.isOn && featuresCheckbox.isOn && itemsCheckbox.isOn)
            {
                allCheckboxActive = false;
                allCheckbox.isOn = true;
                allCheckboxActive = true;
            }
        }
        else
        {
            allCheckboxActive = false;
            allCheckbox.isOn = false;
            allCheckboxActive = true;
        }
        SetDescriptionText();
    }
}
