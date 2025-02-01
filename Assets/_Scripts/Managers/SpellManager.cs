using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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
        return _spells.OrderBy(o=> Random.value).First();
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

    public void InflictDamage(BaseUnit originUnit, float amount, BaseUnit target, List<Properties> property = null){
        int finalDamages = Tools.Ceiling(amount);
        target.Damage(finalDamages);
        
        if(property != null){
            if(property.Contains(Properties.Vampirisme)){
                originUnit.Heal(finalDamages);
            }
        }
    }

    public void InflictDamage(BaseUnit originUnit, float amount, List<BaseUnit> targets, List<Properties> property = null){
        
        int totalDamages = 0;
        foreach (BaseUnit target in targets)
        {
            int finalDamages = Tools.Ceiling(amount);
            totalDamages += finalDamages;
            target.Damage(finalDamages);
        }

        if(property != null){
            if(property.Contains(Properties.Vampirisme)){
                originUnit.Heal(totalDamages);
            }
        }   
    }

    public void HealDamage(float amount, BaseUnit target, Properties property = Properties.Empty){
        int finalAmount = Tools.Ceiling(amount);
        target.Heal(finalAmount);
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
