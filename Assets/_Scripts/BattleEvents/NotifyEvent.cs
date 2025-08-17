using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyEvent : BattleEvent
{
    private Upgrade upgrade;

    public NotifyEvent(Upgrade _upgrade){
        upgrade = _upgrade;
    }
    public Upgrade GetUpgrade(){
        return upgrade;
    }

    public override string GetSummary()
    {
        return "Notify Event: " + GetUpgrade() + " notified something";
    }
}
