using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEvent : BattleEvent
{
    private BaseUnit deadUnit;
    private Tile deathTile;
    private Object origin;

    public DeathEvent(BaseUnit _deadUnit, Tile _deathTile, Object _origin){
        deadUnit = _deadUnit;
        deathTile = _deathTile;
        origin = _origin;
    }

    public BaseUnit GetDeadUnit(){
        return deadUnit;
    }

    public Tile GetDeathTile(){
        return deathTile;
    }

    public Object GetOrigin(){
        return origin;
    }

    public override string GetSummary()
    {
        //return "Death Event: " + GetDeadUnit().GetName() + " - " + GetDeathTile().name + " - " + origin.name;
        return "Unit died";
    }
}
