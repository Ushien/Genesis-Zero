using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpell : MonoBehaviour
{
    public ScriptableSpell scriptableSpell;
    public BaseUnit owner;

    public string spell_name = "Name";
    [TextArea(5,10)]
    public string lore_description = "Lore Description";
    [TextArea(5,10)]
    public string fight_description = "Fight Description";
    public int cooldown = 0;
    public Sprite artwork = null;
    public ISpellInterface spell = null;

    public void Setup(ScriptableSpell originSpell, BaseUnit ownerUnit){
        scriptableSpell = originSpell;
        spell = originSpell.spellScriptPrefab.GetComponent<ISpellInterface>();

        this.name = scriptableSpell.spell_name;

        owner = ownerUnit;
        
        spell_name = scriptableSpell.spell_name;
        lore_description = scriptableSpell.lore_description;
        fight_description = scriptableSpell.fight_description;
        cooldown = scriptableSpell.cooldown;
        artwork = scriptableSpell.artwork;
    }

    public void Cast(){
        spell.Cast();
    }
    public string GetName(){
        return spell_name;
    }

    public string GetFightDescription(){
        return fight_description;
    }
}
