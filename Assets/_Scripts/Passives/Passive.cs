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
}