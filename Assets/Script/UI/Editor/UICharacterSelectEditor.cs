using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;

[DisallowMultipleComponent]
[CustomEditor(typeof(UICharactedSelector))]
public class UICharacterSelectEditor : Editor
{
    UICharactedSelector selector;

    private void OnEnable()
    {
        selector = target as UICharactedSelector;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Selectable Character"))
        {
            CreateTogglesForCharacterData();
        }
    }

    public void CreateTogglesForCharacterData()
    {
        if(!selector.toggleTemplate)
        {
            Debug.LogWarning("Please assign a toggle Template for the ui Character Selector first.");
            return;   
        }
        for(int i = selector.toggleTemplate.transform.parent.childCount - 1; i >= 0; i--)
        {
            Toggle tog = selector.toggleTemplate.transform.parent.GetChild(i).GetComponent<Toggle>();
            if (tog == selector.toggleTemplate) continue;

            Undo.DestroyObjectImmediate(tog.gameObject);
        }

        Undo.RecordObject(selector, "Updates to UICharacterSelect. ");
        selector.selectableToggles.Clear();
        CharacterData[] characters = UICharactedSelector.GetAllCharacterDataAssets();

        for(int i = 0; i < characters.Length; i++)
        {
            Toggle tog;
            if(i == 0)
            {
                tog = selector.toggleTemplate;
                Undo.RecordObject(tog, "Modifying the template. ");
            }
            else
            {
                tog = Instantiate(selector.toggleTemplate,selector.toggleTemplate.transform.parent);
                Undo.RegisterCreatedObjectUndo(tog.gameObject, "Created a new toggle. ");
            }

            Transform characterName = tog.transform.Find(selector.characterNamePath);
            if(characterName && characterName.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.text = tog.gameObject.name = characters[i].Name;
            }

            Transform characterIcon = tog.transform.Find(selector.characterIconPath);
            if (characterIcon && characterIcon.TryGetComponent(out Image chrIcon))
            {
                chrIcon.sprite = characters[i].Icon;
            }

            Transform weaponIcon = tog.transform.Find(selector.weaponIconPath);
            if (weaponIcon && weaponIcon.TryGetComponent(out Image wpnIcon))
            {
                wpnIcon.sprite = characters[i].StartingWeapon.icon;
            }

            selector.selectableToggles.Add(tog);

            for(int j = 0; j < tog.onValueChanged.GetPersistentEventCount(); j++)
            {
                if(tog.onValueChanged.GetPersistentMethodName(j) == "Select")
                {
                    UnityEventTools.RemovePersistentListener(tog.onValueChanged, j);
                }
            }
            UnityEventTools.AddObjectPersistentListener(tog.onValueChanged, selector.Select, characters[i]);
        }
        EditorUtility.SetDirty(selector);
    }
}
