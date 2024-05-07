using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : MonoBehaviour
{
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
        Debug.Log(name);
    }

    virtual public string GetName(){
        return passiveName;
    }

    virtual public string GetFightDescription(){
        return fight_description;
    }
}