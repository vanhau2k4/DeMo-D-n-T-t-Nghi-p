using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(LayoutGroup))]
public class UiInventoryIconDisplay : MonoBehaviour
{
    public GameObject slotTemplate;
    public uint maxSlots = 6;
    public bool showLevels = true;
    public PlayerInventory inventory;

    public GameObject[] slots;

    [Header("Paths")]
    public string iconPath;
    public string levelTextPath;
    [HideInInspector] public string targetedItemList;

    private void Reset()
    {
        slotTemplate = transform.GetChild(0).gameObject;
        inventory = FindObjectOfType<PlayerInventory>();
    }
    private void OnEnable()
    {
        Refresh();
    }
    public void Refresh()
    {
        if (!inventory) Debug.LogWarning("NO inventory");

        Type t = typeof(PlayerInventory);
        FieldInfo field = t.GetField(targetedItemList, BindingFlags.Public | BindingFlags.Instance);

        if(field == null )
        {
            Debug.LogWarning("the list in the inventory is nt found");
                return;
        }

        List<PlayerInventory.Slot> items = (List<PlayerInventory.Slot>)field.GetValue(inventory);
        
        for(int i = 0; i < items.Count; i++)
        {
            if(i >= slots.Length)
            {
                Debug.LogWarning(string.Format("  ", items.Count, slots.Length));
                break;
            }
            Item item = items[i].item;

            Transform iconObj = slots[i].transform.Find(iconPath);

            if (iconObj)
            {
                Image icon = iconObj.GetComponentInChildren<Image>();

                if(!item) icon.color = new Color(1,1,1,0);
                else
                {
                    icon.color = new Color(1,1,1,1);
                    if(icon) icon.sprite = item.data.icon;
                }
            }
            
            Transform levelObj = slots[i].transform.Find(levelTextPath);

            if (levelObj)
            {
                TMP_Text leveltxt = levelObj.GetComponentInChildren<TMP_Text>();

                if (!item || !showLevels) leveltxt.text = "";
                else leveltxt.text = item.currentLevel.ToString();
            }
        }
    }
}
