using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ResourceManager Instance;
    private List<ScriptableSpell> spellList;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void LoadResources(){
        spellList = Resources.LoadAll("Spells", typeof(ScriptableSpell)).Cast<ScriptableSpell>().ToList();
    }

    public List<ScriptableSpell> GetSpells(bool lootable = false){
        if(lootable){
            return spellList.Where(spell => spell.lootable == true).ToList();
        }
        return spellList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
