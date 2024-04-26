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
    public GridManager.Selection_mode range;
    public GridManager.Team_restriction team_restriction;

    public ISpellInterface spell = null;

    public void Setup(ScriptableSpell originSpell, BaseUnit ownerUnit){
        scriptableSpell = originSpell;
        spell = originSpell.spellScriptPrefab.GetComponent<ISpellInterface>();
        spell.SetSpell(this);

        this.name = scriptableSpell.spell_name;

        owner = ownerUnit;
        
        spell_name = scriptableSpell.spell_name;
        lore_description = scriptableSpell.lore_description;
        fight_description = scriptableSpell.fight_description;
        cooldown = scriptableSpell.cooldown;
        artwork = scriptableSpell.artwork;
        range = scriptableSpell.range;
        team_restriction = scriptableSpell.team_restriction;
    }

    public void Cast(){
        spell.Cast();
    }

    public void Cast(Tile tile){
        spell.Cast(tile);
    }
    public string GetName(){
        return spell_name;
    }

    public GridManager.Selection_mode GetRange(){
        return range;
    }

    public string GetFightDescription(){
        return fight_description;
    }
}
