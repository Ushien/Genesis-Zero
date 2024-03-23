using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    public BaseSpell EmptySpell;

    private List<ScriptableSpell> _spells;
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

    public BaseSpell SetupSpell(ScriptableSpell scriptableSpell, BaseUnit unit){
        var new_spell = Instantiate(EmptySpell);
        new_spell.Setup(scriptableSpell, unit);
        return new_spell;
    }

    public void InflictDamage(int amount, BaseUnit target, Properties property){
        Debug.Log("J'inflige des dégats avec ma technique !");
    }
}

public enum Properties {    
    Pyro = 0,
    Létalité = 1
}
