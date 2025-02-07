using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetourDeFlammes : BaseSpell
{
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _RetourDeFlammes);
    }

    private void _RetourDeFlammes(Tile targetTile){
        // Do something
    }
}