using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(VerticalLayoutGroup))]
public class UiUpgradeWindown : MonoBehaviour
{
    VerticalLayoutGroup verticalLayout;

    public RectTransform upgradeOptionTemplate;
    public TMP_Text tooltipTemplate;

    [Header("Setting")]
    public int maxOptions = 4;
    public string newText = "new:";

    public Color newTextColor = Color.yellow,levelTextColor = Color.white;

    [Header("Paths")]
    public string icomPath = "Icon/Item Icom";
    public string namePath = "name", descriptionPath = "Description", buttonPath = "Button", levelPath = "Level";

    RectTransform rectTransform;
    float optionHeight;
    int activeOptons;

    List<RectTransform> upgradeOptions = new List<RectTransform>();

    Vector2 lastScreen;

    public void SetUpgrades(PlayerInventory inventory, List<ItemData> possibleUpgrades, int pick = 3, string tooltip ="")
    {
        pick = Mathf.Min(maxOptions, pick);
        if(maxOptions > upgradeOptions.Count)
        {
            for (int i = upgradeOptions.Count; i < pick; i++)
            {
                GameObject go = Instantiate(upgradeOptionTemplate.gameObject,transform);
                upgradeOptions.Add((RectTransform)go.transform);
            }
        }

        tooltipTemplate.text = tooltip;
        tooltipTemplate.gameObject.SetActive(tooltip.Trim() != "");

        activeOptons = 0;
        int totalPositonUpgrades = possibleUpgrades.Count;
        foreach(RectTransform r in upgradeOptions)
        {
            if(activeOptons < pick && activeOptons < totalPositonUpgrades)
            {
                r.gameObject.SetActive(true);

                ItemData selected = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
                possibleUpgrades.Remove(selected);
                Item item = inventory.Get(selected);

                TMP_Text name = r.Find(namePath).GetComponent<TMP_Text>();
                if (name)
                {
                    name.text = selected.name;
                }
                TMP_Text level = r.Find(levelPath).GetComponent<TMP_Text>();
                if(level)
                {
                    if (item)
                    {
                        if(item.currentLevel >= item.maxLevel)
                        {
                            level.text = "Max !";
                            level.color = newTextColor;
                        }
                        else
                        {
                            level.text = selected.GetLevelData(item.currentLevel + 1).name;
                            level.color = levelTextColor;
                        }
                    }
                    else
                    {
                        level.text = newText;
                        level.color = newTextColor;
                    }
                }
                TMP_Text desc = r.Find(descriptionPath).GetComponent<TMP_Text>();
                if (desc)
                {
                    if (item)
                    {
                        desc.text = selected.GetLevelData(item.currentLevel + 1).description;
                    }
                    else
                    {
                        desc.text = selected.GetLevelData(1).description;
                    }
                }
                Image icon = r.Find(icomPath).GetComponent<Image>();
                if(icon)
                {
                    icon.sprite = selected.icon;
                }
                Button b = r.Find(buttonPath).GetComponent<Button>();
                if (b)
                {
                    b.onClick.RemoveAllListeners();
                    if (item)
                    {
                        b.onClick.AddListener(() => inventory.LevelUp(item));
                    }
                    else
                    {
                        b.onClick.AddListener(() => inventory.Add(selected));
                    }
                }
                activeOptons++;
            }
            else r.gameObject.SetActive(false);
        }
        RecalculateLayout();
    }
    void RecalculateLayout()
    {
        optionHeight = (rectTransform.rect.height - verticalLayout.padding.top 
            - verticalLayout.padding.bottom - (maxOptions - 1) * verticalLayout.spacing);
        if(activeOptons == maxOptions && tooltipTemplate.gameObject.activeSelf)
        {
            optionHeight /= maxOptions + 1;
        }
        else
        {
            optionHeight /= maxOptions;
        }
        if(tooltipTemplate.gameObject.activeSelf)
        {
            RectTransform tooltipRect = (RectTransform)tooltipTemplate.transform;
            tooltipTemplate.gameObject.SetActive(true);
            tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x,optionHeight);
            tooltipTemplate.transform.SetAsLastSibling();
        }
        foreach(RectTransform r in upgradeOptions)
        {
            if(!r.gameObject.activeSelf) continue;
            r.sizeDelta = new Vector2(r.sizeDelta.x,optionHeight);
        }
    }
    private void Update()
    {
        if(lastScreen.x != Screen.width || lastScreen.y != Screen.height)
        {
            RecalculateLayout();
            lastScreen = new Vector2 (Screen.width, Screen.height);
        }
    }
    private void Awake()
    {
        verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();
        if(tooltipTemplate) tooltipTemplate.gameObject.SetActive(false);
        if(upgradeOptionTemplate) upgradeOptions.Add(upgradeOptionTemplate);

        rectTransform = (RectTransform)transform;
    }
    private void Reset()
    {
        upgradeOptionTemplate = (RectTransform)transform.Find("Upgrade Option");
        tooltipTemplate = transform.Find("Tooltip").GetComponentInChildren<TMP_Text>(); 
    }
}
