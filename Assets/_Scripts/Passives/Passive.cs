using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

/// <summary>
/// Un passif s'attache à une unité et s'active continuellement sous certaines conditions. 
/// Tous les passifs du jeu héritent de cette superclasse.
/// </summary>

public class Passive : MonoBehaviour
{
    private int unique_id;
    public float ratio1;
    public float ratio2;
    public float ratio3;
    public BaseUnit holder;
    public ScriptablePassive scriptablePassive;

    public Modifier modifier;
    private bool minor = false;
    private bool activated = false;

    virtual public void AttachToUnit(BaseUnit unit){
        holder = unit;
    }

    virtual public void Setup(BaseUnit unit, ScriptablePassive _scriptablePassive){
        unique_id = scriptablePassive.id;
        name = _scriptablePassive.name;
        ratio1 = _scriptablePassive.ratios[0];
        ratio2 = _scriptablePassive.ratios[1];
        ratio3 = _scriptablePassive.ratios[2];
        scriptablePassive = _scriptablePassive;
        minor = _scriptablePassive.minor;
        modifier = unit.emptyModifier;

        AttachToUnit(unit);
        unit.AddPassive(this);
        transform.parent = unit.transform;

        Activate();
        activated = true;
    }

    virtual public void Activate(){
        //
    }

    virtual public void Trigger1(){
        Debug.Log("Must be overriden");
    }

    virtual public string GetName(){
        return LocalizationSettings.StringDatabase.GetLocalizedString("Passives", unique_id.ToString() + "_name");
    }

    virtual public string GetDescription(){
        string _fight_description = LocalizationSettings.StringDatabase.GetLocalizedString("Passives", unique_id.ToString() + "_effect");
        _fight_description = _fight_description.Replace("%%1", GetFinalDamages(GetRatio()[0]).ToString());
        _fight_description = _fight_description.Replace("%%2", GetFinalDamages(GetRatio()[1]).ToString());
        _fight_description = _fight_description.Replace("%%3", GetFinalDamages(GetRatio()[2]).ToString());

        _fight_description = _fight_description.Replace("__1", DisplayPercents(GetRatio()[0]));
        _fight_description = _fight_description.Replace("__2", DisplayPercents(GetRatio()[1]));
        _fight_description = _fight_description.Replace("__3", DisplayPercents(GetRatio()[2]));
        return _fight_description;
    }

    virtual public List<float> GetRatio()
    {
        return new List<float>{
            ratio1,
            ratio2,
            ratio3
        };

    }

    public float ApplyPower(float ratio){
        return ratio * GetOwner().GetFinalPower();
    }

    public BaseUnit GetOwner(){
        return holder;
    }

    public ScriptablePassive GetScriptablePassive(){
        return scriptablePassive;
    }

    public bool IsMinor(){
        return minor;
    }

    public string DisplayPercents(float percentRatio){
        return (percentRatio * 100).ToString();
    }

    public int GetFinalDamages(float _ratio){
        return Tools.Ceiling(_ratio * GetOwner().GetFinalPower());
    }

    public bool IsActivated(){
        return activated;
    }

    public void Activate(bool activation){
        activated = activation;
    }
    
    virtual public void Desactivate(){
        //
    }
}