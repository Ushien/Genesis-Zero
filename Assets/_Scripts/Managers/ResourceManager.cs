using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ResourceManager Instance;
    private List<ScriptableSpell> spellList;
    private List<ScriptablePassive> passiveList;
    private List<ScriptableUnit> enemyUnitList;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void LoadResources(){
        spellList = Resources.LoadAll("Spells", typeof(ScriptableSpell)).Cast<ScriptableSpell>().ToList();
        passiveList = Resources.LoadAll("Passives", typeof(ScriptablePassive)).Cast<ScriptablePassive>().ToList();
        enemyUnitList = Resources.LoadAll<ScriptableUnit>("Units/Enemies").ToList();
    }

    public List<ScriptableSpell> GetSpells(bool lootable = true){
        if(lootable){
            return spellList.Where(spell => spell.lootable == lootable).ToList();
        }
        return spellList;
    }

    public List<ScriptablePassive> GetPassives(bool lootable = true){
        if(lootable){
            return passiveList.Where(passive => passive.lootable == lootable).ToList();
        }
        return passiveList;
    }

    public List<ScriptableUnit> GetEnemyUnit(){
        return enemyUnitList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
