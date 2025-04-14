using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEvent : BattleEvent
{
    private BaseUnit summonedUnit;
    private Tile summonTile;
    private BaseUnit summoner;
    private Object origin;

    public SummonEvent(BaseUnit _summonedUnit, Tile _summonTile, BaseUnit _summoner, Object _origin){
        summonedUnit = _summonedUnit;
        summonTile = _summonTile;
        summoner = _summoner;
        origin = _origin;
    }

    public BaseUnit GetSummonedUnit(){
        return summonedUnit;
    }

    public Tile GetSummonTile(){
        return summonTile;
    }

    public BaseUnit GetSummoner(){
        return summoner;
    }

    public Object GetOrigin(){
        return origin;
    }

    public override string GetSummary()
    {
        return "Unit summoned: " + GetSummonedUnit().GetName() + " by " + GetSummoner().GetName();
    }
}
