using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetourDeFlammes : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _RetourDeFlammes);
    }

    private void _RetourDeFlammes(Tile targetTile, List<Properties> properties = null){
        // Do something
    }
}