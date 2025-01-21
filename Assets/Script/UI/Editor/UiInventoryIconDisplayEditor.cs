using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(UiInventoryIconDisplay))]
public class UiInventoryIconDisplayEditor : Editor
{
    UiInventoryIconDisplay display;
    int targetedItemListIndex = 0;
    string[] itemListOptions;

    private void OnEnable()
    {
        display = target as UiInventoryIconDisplay;

        Type playerInventoryType = typeof(PlayerInventory);

        FieldInfo[] fields = playerInventoryType.GetFields(BindingFlags.Public 
            | BindingFlags.NonPublic | BindingFlags.Instance);

        List<string> slotListName = fields
            .Where(field => field.FieldType.IsGenericType && 
            field.FieldType.GetGenericTypeDefinition() == typeof(List<>) && 
            field.FieldType.GetGenericArguments()[0] == typeof(PlayerInventory.Slot))
            .Select(field => field.Name).ToList();
        slotListName.Insert(0, "None");
        itemListOptions = slotListName.ToArray();

        targetedItemListIndex = Mathf.Max(0,Array.IndexOf(itemListOptions, display.targetedItemList));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        targetedItemListIndex = EditorGUILayout.Popup("targetes item list", Mathf.Max(0, targetedItemListIndex), itemListOptions);
        if(EditorGUI.EndChangeCheck())
        {
            display.targetedItemList = itemListOptions[targetedItemListIndex].ToString();
            EditorUtility.SetDirty(display);
        }
        if (GUILayout.Button("Generate Icons")) RegenerateIcons();
    }
    void RegenerateIcons()
    {
        display = target as UiInventoryIconDisplay;

        Undo.RegisterCompleteObjectUndo(display, "Regenerate Icons");

        if(display.slots.Length > 0 )
        {
            foreach(GameObject g in display.slots)
            {
                if(!g) continue;
                if(g != display.slotTemplate)
                    Undo.DestroyObjectImmediate(g);
            }
        }
        for(int i = 0; i < display.transform.childCount; i++)
        {
            if (display.transform.GetChild(i).gameObject == display.slotTemplate) continue;
            Undo.DestroyObjectImmediate(display.transform.GetChild(i).gameObject);
            i--;
        }
        if (display.maxSlots <= 0) return;
        display.slots = new GameObject[display.maxSlots];
        display.slots[0] = display.slotTemplate;
        for(int i = 0;i < display.slots.Length; i++)
        {
            display.slots[i] = Instantiate(display.slotTemplate, display.transform);
            display.slots[i].name = display.slotTemplate.name;
        }
    }
}
