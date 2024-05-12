using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benediction : BaseSpell
{
    void Awake(){
        ratio1 = 0.15f;
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Benediction);
    }

    private void _Benediction(Tile targetTile){

        Modifier _modifier = Instantiate(modifier);
        _modifier.Setup(_powerBonus : ratio1, _turns : 3, _permanent : false);

        targetTile.GetUnit().GetAttack().AddModifier(_modifier);
    }
}