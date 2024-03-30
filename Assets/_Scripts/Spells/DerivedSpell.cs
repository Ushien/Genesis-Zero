using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DerivedSpell : ScriptableSpell
{
    public override void OnUse()
    {
        Debug.Log("derived");
    }
}
