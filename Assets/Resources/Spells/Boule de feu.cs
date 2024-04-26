using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : MonoBehaviour, ISpellInterface
{
    public void Cast(Tile targetTile = null){
        BaseUnit targetUnit = null;
        if (targetTile != null){
            targetUnit = targetTile.GetUnit();
        }

        SpellManager.Instance.InflictDamage(100, targetUnit, Properties.Pyro);
    }
}