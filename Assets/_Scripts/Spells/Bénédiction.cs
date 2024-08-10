using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benediction : BaseSpell
{
    void Awake(){
        SetRatio(1, 0.15f);
        SetRatio(1, 0.5f, true);
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Benediction);
    }

    override public void HyperCast(Tile targetTile = null){
        base.CastSpell(targetTile, _Benediction_H);
    }

    private void _Benediction(Tile targetTile){

        Modifier _modifier = Instantiate(SpellManager.Instance.GetModifier());
        _modifier.Setup(_powerBonus : GetRatio()[0], _turns : 3, _permanent : false);

        targetTile.GetUnit().GetAttack().AddModifier(_modifier);
    }

    private void _Benediction_H(Tile targetTile){

        Modifier _modifier = Instantiate(SpellManager.Instance.GetModifier());
        _modifier.Setup(_powerBonus : GetRatio()[0], _turns : 3, _permanent : false);

        targetTile.GetUnit().GetAttack().AddModifier(_modifier);
    }
}