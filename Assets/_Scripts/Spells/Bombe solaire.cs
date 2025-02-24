using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombesolaire : BaseSpell
{
     override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _BombeSolaire);
    }

    private void _BombeSolaire(Tile targetTile, List<Properties> properties = null){
        //Do things        
    }
}
