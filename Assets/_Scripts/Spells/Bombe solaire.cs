using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombesolaire : BaseSpell
{
    void Awake(){

    }
    
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _BombeSolaire);
    }

    private void _BombeSolaire(Tile targetTile){
        //Do things        
    }
}
