using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Passive", menuName = "ScriptablePassive")]
public class ScriptablePassive : ScriptableObject
{
    public string passive_name;
    [TextArea(5,10)]
    public string fight_description;
    public float[] ratios = {1f, 1f, 1f};
    public Sprite artwork;
    public Passive passivePrefab;
    public bool minor = false;
    public bool lootable;

    public void SetupPassive(BaseUnit unit){
        Passive passive = Instantiate(passivePrefab);
        passive.Setup(unit, this);
    }
    
    public string GetFightDescription(BaseUnit unit){
        string _fight_description = fight_description.Clone().ToString();
        _fight_description = _fight_description.Replace("%%1", GetFinalDamages(ratios[0], unit).ToString());
        _fight_description = _fight_description.Replace("%%2", GetFinalDamages(ratios[1], unit).ToString());
        _fight_description = _fight_description.Replace("%%3", GetFinalDamages(ratios[2], unit).ToString());

        _fight_description = _fight_description.Replace("__1", DisplayPercents(ratios[0]));
        _fight_description = _fight_description.Replace("__2", DisplayPercents(ratios[1]));
        _fight_description = _fight_description.Replace("__3", DisplayPercents(ratios[2]));
        return _fight_description;
    }

    public int GetFinalDamages(float _ratio, BaseUnit unit){
        return Tools.Ceiling(_ratio * unit.GetFinalPower());
    }

    public string DisplayPercents(float percentRatio){
        return (percentRatio * 100).ToString();
    }

}
