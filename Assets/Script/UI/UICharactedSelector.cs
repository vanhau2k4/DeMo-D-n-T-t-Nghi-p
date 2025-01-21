using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UICharactedSelector : MonoBehaviour
{
    public CharacterData defaultCharacter;
    public static CharacterData selected;
    public UiStatsDisplay statsUI;

    [Header("Template")]
    public Toggle toggleTemplate;
    public string characterNamePath = "Character Name";
    public string weaponIconPath = "Weapon Icon";
    public string characterIconPath = "Character Icon";
    public List<Toggle> selectableToggles = new List<Toggle>();

    [Header("DescriptionBox")]
    public TMP_Text characterFullName;
    public TMP_Text characterDescription;
    public Image selectedCharacterIcon;
    public Image selectedCharacterWeapon;

    private void Start()
    {
        if(defaultCharacter) Select(defaultCharacter);
    }
    public static CharacterData[] GetAllCharacterDataAssets()
    {
        List<CharacterData> characters = new List<CharacterData>();
#if UNITY_EDITOR
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            if (assetPath.EndsWith(".asset"))
            {
                CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath); 
                if(characterData != null) characters.Add(characterData);
            }
        }
#else
        Debug.LogWarning("This function cannot bo called on builds");
#endif
        return characters.ToArray();
    }
    public static CharacterData GetData()
    {
        if(selected)
        {
            return selected;
        }
        else
        {
            CharacterData[] characters = GetAllCharacterDataAssets();
            if(characters.Length > 0) return characters[Random.Range(0,characters.Length)];
        }
        return null;
    }
    public void Select(CharacterData character)
    {
        selected = statsUI.character = character;
        statsUI.UpdataStatFields();

        characterFullName.text = character.FullName;
        characterDescription.text = character.CharacterDescription;
        selectedCharacterIcon.sprite = character.Icon;
        selectedCharacterWeapon.sprite = character.StartingWeapon.icon;
    }
}
