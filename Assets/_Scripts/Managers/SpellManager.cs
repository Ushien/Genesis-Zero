using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using Unity.VisualScripting;
using System;


/// <summary>
/// Gestion des sorts (Dépréciée)
/// </summary>

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    public BaseSpell EmptySpell;
    public BaseSpell baseAttack;
    [SerializeField]
    private Modifier baseModifier;
    private List<ScriptableSpell> _spells;
    private List<Properties> emptyProperties;

    private List<Status> cleansableStatus = new List<Status>(){Status.Poison, Status.Stun};
    void Awake(){
        Instance = this;
    }
    void Start()
    {
        _spells = LoadSpells();
        emptyProperties = new List<Properties>();
    }
    
    public ScriptableSpell GetRandomSpell(){
        return _spells.OrderBy(o=> UnityEngine.Random.value).First();
    }

    private List<ScriptableSpell> LoadSpells(){
        var spellList = Resources.LoadAll<ScriptableSpell>("Spells").ToList();
        return spellList;
    }

    public BaseSpell SetupAttack(BaseUnit unit){
        BaseSpell new_spell = Instantiate(baseAttack);
        new_spell.transform.parent = unit.transform;
        new_spell.Setup(unit);
        return new_spell;
    }

    public BaseSpell SetupSpell(BaseSpell spell, BaseUnit unit, int availableSpellsIndex){
        BaseSpell new_spell = Instantiate(spell);
        new_spell.transform.parent = unit.transform;
        new_spell.Setup(unit, availableSpellsIndex);
        return new_spell;
    }

    public void AutoAttack(int amount, BaseUnit target, Properties property){
        Debug.Log("J'inflige des dégats avec mon attaque");
    }

    public void UseSpell(BaseUnit originUnit, float amount, BaseUnit target, List<Properties> property = null, SpellType spellType = SpellType.Damage){
        switch (spellType)
        {
            case SpellType.Damage:
                Debug.Log("Piou");
                if((property.Contains(Properties.Curatif) || originUnit.GetCuratifCount() > 0) && originUnit.GetTeam() == target.GetTeam()){
                    Debug.Log("Piou2");
                    if(originUnit.GetCuratifCount() > 0){
                        Debug.Log("Piou3");
                        originUnit.ModifyCuratifCount(-1);
                    }
                    HealDamage(originUnit, amount, target, property);
                }
                else{
                    InflictDamage(originUnit, amount, target, property);
                }
                break;
            case SpellType.Heal:
                HealDamage(originUnit, amount, target, property);
                break;
            default:
                // TODO Throw exception
                break;
        }
    }

    public DamageEvent InflictDamage(BaseUnit originUnit, float amount, BaseUnit target, List<Properties> properties = null){

        int finalDamages = Tools.Ceiling(amount);
        DamageEvent damageEvent = target.Damage(finalDamages);
        damageEvent.SetOriginUnit(originUnit);
        BattleEventManager.Instance.ApplyDamageEvent(damageEvent);
        
        if(properties != null){
            if(properties.Contains(Properties.Vampirisme)){
                // On supprime le vampirisme des propriétés du heal pour éviter les boucles
                List<Properties> propertiesWithoutVampirisme = new List<Properties>();
                foreach (Properties property in properties)
                {
                    if(property != Properties.Vampirisme){
                        propertiesWithoutVampirisme.Add(property);
                    }
                }
                HealDamage(originUnit, damageEvent.GetHealthAmount(), originUnit, propertiesWithoutVampirisme);
            }
        }

        return damageEvent;   
    }

    public void InflictDamage(BaseUnit originUnit, float amount, List<BaseUnit> targets, List<Properties> properties = null){
        
        int totalHealthDamages = 0;
    
        foreach (BaseUnit target in targets)
        {
            int finalDamages = Tools.Ceiling(amount);
            
            DamageEvent damageEvent = target.Damage(finalDamages);
            damageEvent.SetOriginUnit(originUnit);
            BattleEventManager.Instance.ApplyDamageEvent(damageEvent);

            totalHealthDamages += damageEvent.GetHealthAmount();
        }

        if(properties != null){
            // On supprime le vampirisme des propriétés du heal pour éviter les boucles
            List<Properties> propertiesWithoutVampirisme = new List<Properties>();
            foreach (Properties property in properties)
            {
                if(property != Properties.Vampirisme){
                    propertiesWithoutVampirisme.Add(property);
                }
            }
            HealDamage(originUnit, totalHealthDamages, originUnit, propertiesWithoutVampirisme);
        }
    }

    public HealEvent HealDamage(BaseUnit originUnit, float amount, BaseUnit target, List<Properties> property = null){
        int finalAmount = Tools.Ceiling(amount);
        HealEvent healEvent = target.Heal(finalAmount);
        healEvent.SetOriginUnit(originUnit);
        BattleEventManager.Instance.ApplyHealEvent(healEvent);

        return healEvent;
    }

    public void ModifyRange(BaseSpell spell, bool definitive, Ranges new_range){
        Debug.Log("Je modifie ma portée");
    }

    public void ModifyPower(float amount, BaseUnit target){
        target.ModifyPower(amount);
    }

    public void MultiplyHP(float amount, BaseUnit target){
        target.MultiplyBothHP((int)amount);
    }

    public void Stun(int amount, BaseUnit target){
        target.SetStunTime(amount);
    }

    public List<Status> GetCleansableStatus(){
        return cleansableStatus;
    }

    public Modifier GetModifier(){
        return baseModifier;
    }
}

public enum Properties {   
    Empty = 0, 
    Pyro = 1,
    Létalité = 2,
    Curatif = 3,
    Vampirisme = 4
}

public enum Rarity {
    Common = 0,
    Rare = 1,
    Mythic = 2
}

public enum Ranges {
    Single_Target = 0,
    All_Targets = 1
}

public enum Status {
    All = 0,
    Kill = 1,
    Stun = 2,
    Poison = 3
}

public enum SpellType {
    Damage = 0,
    Heal = 1
}

/// <summary>
/// Tags communiquant au jeu les synergies possibles autour d'un passif ou d'une technique.
/// Influence le taux de drop de cette récompense.
/// </summary>
public enum Tag {
    Empty = 0,
    Multi_hit,
    Vampirisme,
    Heal,
    Pyro,
    Attack,
    AOE,
    Buff,
    Armor,
    Shield,
    Debuff,
    Status,
    Poison,
    Stun,
    Silence

}
