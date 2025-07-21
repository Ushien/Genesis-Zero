using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

/// <summary>
/// Contient les informations relatives à une technique, afin de l'initialiser
/// </summary>
[CreateAssetMenu(fileName = "New Scriptable Spell", menuName = "ScriptableSpell")]
public class ScriptableSpell : ScriptableObject
{
    public int id;
    public float[] ratios = {1f, 1f, 1f};
    public float[] hyper_ratios = {1f, 1f, 1f};
    public int cooldown;
    public bool isAnAttack = false;
    public List<Properties> properties;
    public GridManager.Selection_mode range;
    public GridManager.Team_restriction team_restriction;
    public Sprite artwork;

    public BaseSpell spellScriptPrefab;
    public bool lootable;
    public Rarity rarity = Rarity.Common;
    public List<Tag> tags;
    public string GetName(){
        return LocalizationSettings.StringDatabase.GetLocalizedString("Spells", id.ToString() + "_name");
    }
    
    public string GetFightDescription(BaseUnit unit)
    {
        string _fight_description = LocalizationSettings.StringDatabase.GetLocalizedString("Spells", id.ToString() + "_effect");
        _fight_description = _fight_description.Replace("%%1", GetFinalDamages(ratios[0], unit).ToString());
        _fight_description = _fight_description.Replace("%%2", GetFinalDamages(ratios[1], unit).ToString());
        _fight_description = _fight_description.Replace("%%3", GetFinalDamages(ratios[2], unit).ToString());

        _fight_description = _fight_description.Replace("__1", DisplayPercents(ratios[0]));
        _fight_description = _fight_description.Replace("__2", DisplayPercents(ratios[1]));
        _fight_description = _fight_description.Replace("__3", DisplayPercents(ratios[2]));

        _fight_description = _fight_description.Replace("**1", ratios[0].ToString());
        _fight_description = _fight_description.Replace("**2", ratios[1].ToString());
        _fight_description = _fight_description.Replace("**3", ratios[2].ToString());

        return _fight_description;
    }

    public int GetFinalDamages(float _ratio, BaseUnit unit){
        return Tools.Ceiling(_ratio * unit.GetFinalPower());
    }

    public string DisplayPercents(float percentRatio){
        return (percentRatio * 100).ToString();
    }
}