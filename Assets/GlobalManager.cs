using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;
    public GridManager gridManagerPrefab;
    public BattleManager battleManagerPrefab;
    public UnitManager unitManagerPrefab;
    public SpellManager spellManagerPrefab;
    public InterfaceManager interfaceManagerPrefab;
    public AnimationManager animationManagerPrefab;
    public AIManager AIManagerPrefab;
    public BattleEventManager battleEventManagerPrefab;

    //

    GridManager gridManager;
    BattleManager battleManager;
    UnitManager unitManager;
    SpellManager spellManager;
    InterfaceManager interfaceManager;
    AnimationManager animationManager;
    AIManager AIManager;
    BattleEventManager battleEventManager;

    void Awake(){
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchBattle(){
        gridManager = Instantiate(gridManagerPrefab);
        gridManager.transform.SetParent(this.transform.parent);
        battleManager = Instantiate(battleManagerPrefab);
        battleManager.transform.SetParent(this.transform.parent);
        unitManager = Instantiate(unitManagerPrefab);
        unitManager.transform.SetParent(this.transform.parent);
        spellManager = Instantiate(spellManagerPrefab);
        spellManager.transform.SetParent(this.transform.parent);
        interfaceManager = Instantiate(interfaceManagerPrefab);
        interfaceManager.transform.SetParent(this.transform.parent);
        animationManager = Instantiate(animationManagerPrefab);
        animationManager.transform.SetParent(this.transform.parent);
        AIManager = Instantiate(AIManagerPrefab);
        AIManager.transform.SetParent(this.transform.parent);
        battleEventManager = Instantiate(battleEventManagerPrefab);
        battleEventManager.transform.SetParent(this.transform.parent);
    }
}
