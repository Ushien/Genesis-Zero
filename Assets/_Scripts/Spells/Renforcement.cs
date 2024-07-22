using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renforcement : BaseSpell
{
    void Awake(){

    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Renforcement);
    }

    private void _Renforcement(Tile targetTile){
        // Do something
    }
}