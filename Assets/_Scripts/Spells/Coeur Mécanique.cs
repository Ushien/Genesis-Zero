using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoeurMécanique : BaseSpell
{
    void Awake(){
        SetRatio(1, 1f);
        SetRatio(2, 1f);
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _CoeurMecanique);
    }

    private void _CoeurMecanique(Tile targetTile){   
        //
    }
}
