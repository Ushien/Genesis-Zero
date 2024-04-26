using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : MonoBehaviour, ISpellInterface
{
    private BaseSpell spell = null;
    
    public void Cast(Tile targetTile = null){
        BaseUnit targetUnit = null;
        if (targetTile != null){
            targetUnit = targetTile.GetUnit();
        }

        Debug.Log(spell.owner.GetName() + " lance " + spell.GetName() + " sur " + targetTile.GetUnit().GetName());

        SpellManager.Instance.InflictDamage(100, targetUnit, Properties.Pyro);
    }

    public void SetSpell(BaseSpell baseSpell){
        spell = baseSpell;
    }
}