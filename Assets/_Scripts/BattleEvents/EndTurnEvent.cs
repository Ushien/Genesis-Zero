using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnEvent : BattleEvent
{
    private Team teamTurn;

    public EndTurnEvent(Team _teamTurn){
        teamTurn = _teamTurn;
    }
    public Team GetTeam(){
        return teamTurn;
    }

    public override string GetSummary()
    {
        return "End turn Event: " + GetTeam().ToString() + " team turn ended";
    }
}
