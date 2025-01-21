using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    void OnEnable()
    {
        weaponData = (WeaponData)target;

        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();
        List<string> subTypeString = subTypes.Select(t => t.Name).ToList();
        subTypeString.Insert(0, "None");
        weaponSubtypes = subTypeString.ToArray();

        selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubtypes,weaponData.behaviour));
    }
    public override void OnInspectorGUI()
    {
        selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Mathf.Max(0, selectedWeaponSubtype), weaponSubtypes);
        if(selectedWeaponSubtype > 0)
        {
            weaponData.behaviour = weaponSubtypes[selectedWeaponSubtype].ToString();
            EditorUtility.SetDirty(weaponData);
            DrawDefaultInspector();
        }
    }
}
