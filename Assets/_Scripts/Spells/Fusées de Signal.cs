using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuséesdeSignal : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _FuséesSignal);
    }

    private void _FuséesSignal(Tile targetTile, List<Properties> properties = null){
        // Do stuff        
    }
}