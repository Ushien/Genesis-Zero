using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : BaseSpell
{
    void cast(BaseUnit target){
        SpellManager.Instance.InflictDamage(100, target, Properties.Pyro);
    }
}
