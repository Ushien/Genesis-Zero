using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]

public class ScriptableSpell : ScriptableObject
{
    public string spell_name;
    [TextArea(5,10)]
    public string description;
    public BaseUnit SpellPrefab;
}