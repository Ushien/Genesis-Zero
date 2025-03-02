/// <summary>
/// Un AttackEvent est créé lorsqu'une unité attaque, peu importe le contexte.
/// </summary>
public class AttackEvent : BattleEvent
{
    private BaseUnit originUnit;

    public AttackEvent(BaseUnit _originUnit){
        originUnit = _originUnit;
    }

    public void SetOriginUnit(BaseUnit _originUnit){
        originUnit = _originUnit;
    }

    public BaseUnit GetOriginUnit(){
        return originUnit;
    }

    public override string GetSummary()
    {
        return "Attack Event: " + GetOriginUnit().GetName();
    }
}