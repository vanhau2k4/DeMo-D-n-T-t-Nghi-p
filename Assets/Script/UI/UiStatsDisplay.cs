using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class UiStatsDisplay : MonoBehaviour
{
    public PlayerStats player;
    public CharacterData character;
    public bool displayCurrentHealth = false;
    public bool updataInEditor = false;
    TMP_Text statNames, statValues;

    private void OnEnable()
    {
        UpdataStatFields();
    }
    private void OnDrawGizmosSelected()
    {
        if (updataInEditor) UpdataStatFields();
    }
    public CharacterData.Stats GetDisplayedStats()
    {
        if(player) return player.Stats;
        else if(character) return character.stats;

        return new CharacterData.Stats();
    }
    public void UpdataStatFields()
    {
        if(!player && !character) return;
        if(!statNames) statNames = transform.GetChild(0).GetComponent<TMP_Text>();
        if(!statValues) statValues = transform.GetChild(1).GetComponent<TMP_Text>();

        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();

        if (displayCurrentHealth)
        {
            names.AppendLine("Health");
            values.AppendLine(player.CurrentHealth.ToString());
        }

        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach(FieldInfo field in fields)
        {
            names.AppendLine(field.Name);

            object val = field.GetValue(GetDisplayedStats());
            float fva1 = val is int ? (int)val :(float)val;

            PropertyAttribute attribute = (PropertyAttribute)PropertyAttribute.
                GetCustomAttribute(field,typeof(PropertyAttribute));
            if (attribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fva1 * 100 - 100);
                if(Mathf.Approximately(percentage, 0))
                {
                    values.Append('-').Append('\n');
                }
                else
                {
                    if(percentage > 0)
                        values.Append('+');
                    else values.Append('-');
                        values.Append(percentage).Append('%').Append('\n');
                }

            }
            else
            {
                values.Append(fva1).Append('\n');
            }
            statNames.text = PrettifyName(names);
            statValues.text = values.ToString();
        }
    }
    public static string PrettifyName(StringBuilder input)
    {
        if(input.Length <= 0) return string.Empty;

        StringBuilder result = new StringBuilder();
        char last = '\0';
        for(int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (last == '\0' || char.IsWhiteSpace(last))
            {
                c = char.ToUpper(c);
            }else if (char.IsUpper(c))
            {
                result.Append(' ');
            }
            result.Append(c);

            last = c;
        }
        return result.ToString();
    }
    private void Reset()
    {
        player = FindObjectOfType<PlayerStats>();
    }
}
