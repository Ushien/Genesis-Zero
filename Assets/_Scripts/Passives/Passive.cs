using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : MonoBehaviour
{
    public string name;
    public string fight_description;
    public BaseUnit holder;

    void AttachToUnit(BaseUnit unit){
        holder = unit;
    }
}