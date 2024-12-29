using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject alliesLifeBar;
    [SerializeField] private GameObject ennemiesLifeBar;

    [SerializeField] private GameObject UIobject;

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
        GridManager.Instance.SetCam(_cam.transform);
        gridManager.transform.SetParent(this.transform.parent);
        battleManager = Instantiate(battleManagerPrefab);
        battleManager.transform.SetParent(this.transform.parent);
        unitManager = Instantiate(unitManagerPrefab);
        unitManager.transform.SetParent(this.transform.parent);
        spellManager = Instantiate(spellManagerPrefab);
        spellManager.transform.SetParent(this.transform.parent);
        interfaceManager = Instantiate(interfaceManagerPrefab);
        InterfaceManager.Instance.Setup(alliesLifeBar, ennemiesLifeBar, UIobject);
        interfaceManager.transform.SetParent(this.transform.parent);
        animationManager = Instantiate(animationManagerPrefab);
        animationManager.transform.SetParent(this.transform.parent);
        AIManager = Instantiate(AIManagerPrefab);
        AIManager.transform.SetParent(this.transform.parent);
        battleEventManager = Instantiate(battleEventManagerPrefab);
        battleEventManager.transform.SetParent(this.transform.parent);
    }

    public Camera GetCam(){
        return _cam;
    }
}
