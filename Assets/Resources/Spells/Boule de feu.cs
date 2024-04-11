using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : MonoBehaviour, ISpellInterface
{
    public void Cast(BaseUnit target = null){
        if (target == null){
            target = UnitManager.Instance.GetRandomUnit(); 
        }

        SpellManager.Instance.InflictDamage(100, target, Properties.Pyro);
    }
}