using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventManager : MonoBehaviour
{
    public static BattleEventManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public BeforeCastEvent CreateBeforeCastEvent(BaseUnit sourceUnit, BaseSpell castedSpell, Tile targetTile, bool animation = true)
    {
        return new BeforeCastEvent(sourceUnit, castedSpell, targetTile);
    }

    public void ApplyBeforeCastEvent(BeforeCastEvent castEvent, bool animation = true)
    {
        BattleManager.Instance.AddEvent(castEvent);
        if (animation)
        {
            AnimationManager.Instance.AddAnimation(castEvent);
        }
        EventManager.Instance.BeforeTechCast(castEvent);
    }

    public AfterCastEvent CreateAfterCastEvent(BaseUnit sourceUnit, BaseSpell castedSpell, Tile targetTile, bool animation = true)
    {
        return new AfterCastEvent(sourceUnit, castedSpell, targetTile);
    }

    public void ApplyAfterCastEvent(AfterCastEvent castEvent, bool animation = true)
    {
        BattleManager.Instance.AddEvent(castEvent);
        if (animation)
        {
            AnimationManager.Instance.AddAnimation(castEvent);
        }
        EventManager.Instance.AfterTechCast(castEvent);
    }

    public DamageEvent CreateDamageEvent(BaseUnit targetUnit, int health_amount, int armor_amount)
    {
        return new DamageEvent(null, targetUnit, health_amount, armor_amount);
    }

    public DamageEvent CreateDamageEvent(BaseUnit originUnit, BaseUnit targetUnit, int health_amount, int armor_amount)
    {
        return new DamageEvent(originUnit, targetUnit, health_amount, armor_amount);
    }

    public void ApplyDamageEvent(DamageEvent damageEvent, bool animation = true)
    {
        BattleManager.Instance.AddEvent(damageEvent);
        if (animation)
        {
            AnimationManager.Instance.AddAnimation(damageEvent);
        }
        EventManager.Instance.UnitDamaged(damageEvent);
    }

    public ArmorGainEvent CreateArmorGainEvent(BaseUnit targetUnit, int amount, bool animation = true)
    {
        return new ArmorGainEvent(targetUnit, amount);
    }

    public void ApplyArmorGainEvent(ArmorGainEvent armorGainEvent, bool animation = true)
    {
        BattleManager.Instance.AddEvent(armorGainEvent);
        if (animation)
        {
            AnimationManager.Instance.AddAnimation(armorGainEvent);
        }
        EventManager.Instance.UnitGainedArmor(armorGainEvent);
    }


    public HealEvent CreateHealEvent(BaseUnit originUnit, BaseUnit targetUnit, int amount, bool animation = true)
    {
        return new HealEvent(originUnit, targetUnit, amount);
    }

    public void ApplyHealEvent(HealEvent healEvent, bool animation = true)
    {
        BattleManager.Instance.AddEvent(healEvent);
        if (animation)
        {
            AnimationManager.Instance.AddAnimation(healEvent);
        }
        EventManager.Instance.UnitHealed(healEvent);
    }

    public DeathEvent CreateDeathEvent(BaseUnit deadUnit, Tile deathTile, bool animation = true)
    {
        return new DeathEvent(deadUnit, deathTile, null);
    }

    public void ApplyDeathEvent(DeathEvent deathEvent, bool animation = true)
    {
        BattleManager.Instance.AddEvent(deathEvent);
        if (animation)
        {
            AnimationManager.Instance.AddAnimation(deathEvent);
        }
        EventManager.Instance.UnitDied(deathEvent);
    }

    public HPModificationEvent CreateHPModificationEvent(BaseUnit targetUnit, int oldAmount, int newAmount, bool total)
    {
        return new HPModificationEvent(targetUnit, oldAmount, newAmount, total);
    }

    public void ApplyHPModificationEvent(HPModificationEvent hpModificationEvent)
    {
        BattleManager.Instance.AddEvent(hpModificationEvent);
        AnimationManager.Instance.AddAnimation(hpModificationEvent);
    }

    public AttackEvent CreateAttackEvent(BaseUnit originUnit)
    {
        return new AttackEvent(originUnit);
    }

    public void ApplyAttackEvent(AttackEvent attackEvent)
    {
        BattleManager.Instance.AddEvent(attackEvent);
        AnimationManager.Instance.AddAnimation(attackEvent);
    }

    public ReviveEvent CreateReviveEvent(BaseUnit revivedUnit, int hpAmount)
    {
        return new ReviveEvent(revivedUnit, hpAmount);
    }

    public void ApplyReviveEvent(ReviveEvent reviveEvent)
    {
        BattleManager.Instance.AddEvent(reviveEvent);
        AnimationManager.Instance.AddAnimation(reviveEvent);
    }

    public GrowthEvent CreateGrowthEvent(BaseSpell spell)
    {
        return new GrowthEvent(spell);
    }

    public void ApplyReviveEvent(GrowthEvent growthEvent)
    {
        BattleManager.Instance.AddEvent(growthEvent);
        AnimationManager.Instance.AddAnimation(growthEvent);
    }

    public SummonEvent CreateSummonEvent(BaseUnit summonedUnit, Tile summonTile, BaseUnit summoner)
    {
        return new SummonEvent(summonedUnit, summonTile, summoner, null);
    }

    public void ApplySummonEvent(SummonEvent summonEvent)
    {
        BattleManager.Instance.AddEvent(summonEvent);
        AnimationManager.Instance.AddAnimation(summonEvent);
    }

    public void Notify(Upgrade upgrade)
    {
        ApplyNotificationEvent(CreateNotificationEvent(upgrade));
    }

    public NotifyEvent CreateNotificationEvent(Upgrade upgrade)
    {
        return new NotifyEvent(upgrade);
    }

    public void ApplyNotificationEvent(NotifyEvent notifyEvent)
    {
        BattleManager.Instance.AddEvent(notifyEvent);
        AnimationManager.Instance.AddAnimation(notifyEvent);
    }

    public void EndTurn(Team team)
    {
        ApplyEndTurnEvent(CreateEndTurnEvent(team));
    }

    public EndTurnEvent CreateEndTurnEvent(Team team)
    {
        return new EndTurnEvent(team);
    }

    public void ApplyEndTurnEvent(EndTurnEvent endTurnEvent)
    {
        BattleManager.Instance.AddEvent(endTurnEvent);
        AnimationManager.Instance.AddAnimation(endTurnEvent);
        EventManager.Instance.EndTurn(endTurnEvent);
    }
}
