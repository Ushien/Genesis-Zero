using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;
    [SerializeField] private GridManager gridManagerPrefab;
    [SerializeField] private BattleManager battleManagerPrefab;
    [SerializeField] private UnitManager unitManagerPrefab;
    [SerializeField] private SpellManager spellManagerPrefab;
    [SerializeField] private InterfaceManager interfaceManagerPrefab;
    [SerializeField] private AnimationManager animationManagerPrefab;
    [SerializeField] private AIManager AIManagerPrefab;
    [SerializeField] private BattleEventManager battleEventManagerPrefab;
    [SerializeField] private EventManager eventManagerPrefab;

    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject alliesLifeBar;
    [SerializeField] private GameObject ennemiesLifeBar;

    [SerializeField] private GameObject UIobject;

    //

    private GridManager gridManager;
    private BattleManager battleManager;
    private UnitManager unitManager;
    private SpellManager spellManager;
    private InterfaceManager interfaceManager;
    private AnimationManager animationManager;
    private AIManager AIManager;
    private BattleEventManager battleEventManager;
    private EventManager eventManager;

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
        eventManager = Instantiate(eventManagerPrefab);
        eventManager.transform.SetParent(this.transform.parent);
    }

    public Camera GetCam(){
        return _cam;
    }
}
