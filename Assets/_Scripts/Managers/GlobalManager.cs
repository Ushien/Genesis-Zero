using System;
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
    [SerializeField] private Canvas UIWorldSpacePrefab;

    [SerializeField] private Camera camPrefab;

    [SerializeField] private PickPhaseManager pickPhaseManagerPrefab;
    [SerializeField] private ResourceManager resourceManagerPrefab;

    public enum RunPhase {OUT, PICKPHASE, BATTLEPHASE}
    [SerializeField]
    private RunPhase runPhase;

    private GridManager gridManager;
    private BattleManager battleManager;
    private UnitManager unitManager;
    private SpellManager spellManager;
    private InterfaceManager interfaceManager;
    private AnimationManager animationManager;
    private AIManager AIManager;
    private BattleEventManager battleEventManager;
    private EventManager eventManager;
    private Canvas lifeBarUI;
    [SerializeField] private TestScript testScript;

    private PickPhaseManager pickPhaseManager;
    private ResourceManager resourceManager;
    private Camera cam;
    private List<BaseUnit> allies;
    private GameObject battleArchive;

    [SerializeField]

    public bool debug;
    private int battleID = 0;

    void Awake(){
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Instantiate(camPrefab);
        resourceManager = Instantiate(resourceManagerPrefab);
        resourceManager.transform.SetParent(transform.parent);
        resourceManager.LoadResources();
        unitManager = Instantiate(unitManagerPrefab);
        unitManager.transform.SetParent(transform.parent);
        battleArchive = new GameObject("Battle Archive");
        lifeBarUI = Instantiate(UIWorldSpacePrefab);

        ChangeState(RunPhase.PICKPHASE);
    }

    // Update is called once per frame
    void Update()
    {
        if(battleManager != null){
            if(BattleManager.Instance.GetBattleState() == BattleManager.BattleState.WON || BattleManager.Instance.GetBattleState() == BattleManager.BattleState.LOST){
                ChangeState(RunPhase.PICKPHASE);
            }
        }
    }

    public void BattlePhaseIn(){
        battleID += 1;
        gridManager = Instantiate(gridManagerPrefab);
        GridManager.Instance.SetCam(cam.transform);
        gridManager.transform.SetParent(transform.parent);
        battleManager = Instantiate(battleManagerPrefab);
        battleManager.transform.SetParent(transform.parent);
        spellManager = Instantiate(spellManagerPrefab);
        spellManager.transform.SetParent(transform.parent);
        interfaceManager = Instantiate(interfaceManagerPrefab);
        interfaceManager.transform.SetParent(transform.parent);
        
        animationManager = Instantiate(animationManagerPrefab);
        animationManager.transform.SetParent(transform.parent);
        AIManager = Instantiate(AIManagerPrefab);
        AIManager.transform.SetParent(transform.parent);
        battleEventManager = Instantiate(battleEventManagerPrefab);
        battleEventManager.transform.SetParent(transform.parent);
        eventManager = Instantiate(eventManagerPrefab);
        eventManager.transform.SetParent(transform.parent);

        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject _gameobject in rootGameObjects)
        {
            if(_gameobject.name == "Units" || _gameobject.name.Contains("UI - World Space")){
                _gameobject.SetActive(true);
            }
        }

        if(allies == null){
            BattleManager.Instance.LaunchBattle(testScript.ally_composition.GetTuples(), testScript.enemy_composition.GetTuples());
        }
        else
        {
            BattleManager.Instance.LaunchBattle(allies, testScript.enemy_composition.GetTuples());
        }
        BattleManager.Instance.DebugSetState();
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.FORWARD);

        if(debug){
            testScript.LaunchDebug();
        }
    }

    public void BattlePhaseOut(){
        //Deleting all the managers
        Destroy(gridManager.gameObject);
        battleManager.GetArchive().name = "Battle " + battleID;
        battleManager.GetArchive().transform.SetParent(battleArchive.transform);
        Destroy(battleManager.gameObject);
        allies = UnitManager.Instance.GetUnits(Team.Ally);
        unitManager.EndBattle();
        Destroy(spellManager.gameObject);
        Destroy(interfaceManager.gameObject);
        Destroy(animationManager.gameObject);
        Destroy(AIManager.gameObject);
        Destroy(battleEventManager.gameObject);
        Destroy(eventManager.gameObject);
        
        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject _gameobject in rootGameObjects)
        {
            if(_gameobject.name == "Grid" || _gameobject.name.Contains("UI Screen Space")){
                Destroy(_gameobject);
            }
            if(_gameobject.name == "Units" || _gameobject.name.Contains("UI - World Space")){
                _gameobject.SetActive(false);
            }
        }
    }

    public Camera GetCam(){
        return cam;
    }

    public void PickPhaseIn(){
        pickPhaseManager = Instantiate(pickPhaseManagerPrefab);
        pickPhaseManager.transform.SetParent(transform.parent);
        pickPhaseManager.SetResourceManager(resourceManager);
    }

    public void PickPhaseOut(){
        pickPhaseManager.End();
        Destroy(pickPhaseManager.gameObject);
    }

    public void ChangeState(RunPhase trigger){
        if(runPhase == RunPhase.OUT){
            switch(trigger){
                case RunPhase.PICKPHASE:
                    PickPhaseIn();
                    break;
                case RunPhase.BATTLEPHASE:
                    BattlePhaseIn();
                    break;
                default:
                    break;
            }
            runPhase = trigger;
        }
        switch (runPhase)
        {
            case RunPhase.PICKPHASE:
                switch (trigger)
                {
                    case RunPhase.BATTLEPHASE:
                        PickPhaseOut();
                        runPhase = RunPhase.BATTLEPHASE;
                        BattlePhaseIn();
                        break;
                    default:
                        break;
                }
                break;
            case RunPhase.BATTLEPHASE:
                switch (trigger)
                {
                    case RunPhase.PICKPHASE:
                        BattlePhaseOut();
                        runPhase = RunPhase.PICKPHASE;
                        PickPhaseIn();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public RunPhase GetRunPhase(){
        return runPhase;
    }

    public Canvas GetUIWorldSpace(){
        return lifeBarUI;
    }

}
