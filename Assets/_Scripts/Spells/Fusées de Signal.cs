using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuséesdeSignal : BaseSpell
{
    void Awake(){

    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _FuséesSignal);
    }

    private void _FuséesSignal(Tile targetTile){
        // Do stuff        
    }
}