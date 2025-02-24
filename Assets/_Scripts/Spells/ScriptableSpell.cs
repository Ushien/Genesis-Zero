using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contient les informations relatives à une technique, afin de l'initialiser
/// </summary>
[CreateAssetMenu(fileName = "New Scriptable Spell", menuName = "ScriptableSpell")]
public class ScriptableSpell : ScriptableObject
{
    public string spell_name;
    [TextArea(5,10)]
    public string fight_description;
    public float[] ratios = {1f, 1f, 1f};
    public float[] hyper_ratios = {1f, 1f, 1f};
    public string overloaded_fight_description;
    public int cooldown;
    public List<Properties> properties;
    public GridManager.Selection_mode range;
    public GridManager.Team_restriction team_restriction;

    public Sprite artwork;

    public BaseSpell spellScriptPrefab;
    public bool lootable;
    public Rarity rarity = Rarity.Common;
    public List<Tag> tags;
}