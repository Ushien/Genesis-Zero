using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    public BaseSpell EmptySpell;
    public BaseSpell baseAttack;

    private List<ScriptableSpell> _spells;

    private List<Status> cleansableStatus = new List<Status>(){Status.Poison, Status.Stun};
    void Awake(){
        Instance = this;
    }
    void Start()
    {
        _spells = LoadSpells();
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

    public BaseSpell SetupSpell(BaseSpell spell, BaseUnit unit){
        BaseSpell new_spell = Instantiate(spell);
        new_spell.transform.parent = unit.transform;
        new_spell.Setup(unit);
        return new_spell;
    }

    public void AutoAttack(int amount, BaseUnit target, Properties property){
        Debug.Log("J'inflige des dégats avec mon attaque");
    }

    public void InflictDamage(float amount, BaseUnit target, Properties property= Properties.Empty){
        int finalDamages = Tools.Ceiling(amount);
        target.Damage(finalDamages);
    }

    public void InflictDamage(float amount, List<BaseUnit> targets, Properties property= Properties.Empty){
        foreach (BaseUnit target in targets)
        {
            int finalDamages = Tools.Ceiling(amount);
            target.Damage(finalDamages);
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
}

public enum Properties {   
    Empty = 0, 
    Pyro = 1,
    Létalité = 2,
    Curatif = 3
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
