using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : MonoBehaviour
{
    public float ratio1 = 1f;
    public float ratio2 = 1f;
    public float ratio3 = 1f;
    public string passiveName;
    public string fight_description;
    public BaseUnit holder;

    virtual public void AttachToUnit(BaseUnit unit){
        holder = unit;
    }

    virtual public void Setup(BaseUnit unit){
        name = passiveName;
        AttachToUnit(unit);
        transform.parent = unit.transform;

        Activate();
    }

    virtual public void Activate(){
        Debug.Log("Must be overriden");
    }

    virtual public string GetName(){
        return passiveName;
    }

    virtual public List<float> GetRatio(){
        return new List<float>{
            ratio1,
            ratio2,
            ratio3
        };

    }

    virtual public string GetFightDescription(){
        string _fight_description = fight_description.Clone().ToString();
        _fight_description = _fight_description.Replace("%%1", GetFinalDamages(GetRatio()[0]).ToString());
        _fight_description = _fight_description.Replace("%%2", GetFinalDamages(GetRatio()[1]).ToString());
        _fight_description = _fight_description.Replace("%%3", GetFinalDamages(GetRatio()[2]).ToString());

        _fight_description = _fight_description.Replace("__1", DisplayPercents(ApplyPower(GetRatio()[0])));
        _fight_description = _fight_description.Replace("__2", DisplayPercents(ApplyPower(GetRatio()[1])));
        _fight_description = _fight_description.Replace("__3", DisplayPercents(ApplyPower(GetRatio()[2])));
        return _fight_description;
    }

    public float ApplyPower(float ratio){
        return ratio * GetOwner().GetFinalPower();
    }

    public BaseUnit GetOwner(){
        return holder;
    }

    public string DisplayPercents(float percentRatio){
        return Tools.Ceiling(percentRatio * 100).ToString();
    }

    public int GetFinalDamages(float _ratio){
        return Tools.Ceiling(_ratio * GetOwner().GetFinalPower());
    }
}