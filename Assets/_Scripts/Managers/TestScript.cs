using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestScript : MonoBehaviour
{
    private List<ScriptableSpell> _spells;

    public BaseSpell BaseSpell;

    void Start()
    {
        _spells = LoadSpells();

        var selected_spell = GetRandomSpell();

        var new_spell = Instantiate(BaseSpell);
        new_spell.Setup(selected_spell);

        Debug.Log(selected_spell);
    }
    
    private ScriptableSpell GetRandomSpell(){
        return _spells.OrderBy(o=> Random.value).First();
    }

    private List<ScriptableSpell> LoadSpells(){
        var spellList = Resources.LoadAll<ScriptableSpell>("Spells").ToList();
        return spellList;
    }
}
