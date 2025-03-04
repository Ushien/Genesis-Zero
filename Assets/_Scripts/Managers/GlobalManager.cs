using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

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
    [SerializeField] private EndScreenManager endScreenManagerPrefab;

    public enum RunPhase {OUT, STARTPHASE, PICKPHASE, BATTLEPHASE, LOSESCREEN, ENDPHASE}
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
    private EndScreenManager endScreenManager;
    private Camera cam;
    [SerializeField]

    private List<BaseUnit> allies;
    private List<BaseUnit> enemies;
    private GameObject battleArchive;

    [SerializeField]

    public bool debug;
    private int battleID = 0;
    [SerializeField]
    private int startLevel = 1;
    public int runSeed = 0;

    void Awake(){
        Instance = this;
        if(runSeed != 0){
            UnityEngine.Random.InitState(runSeed);
        }
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
        interfaceManager = Instantiate(interfaceManagerPrefab);
        interfaceManager.transform.SetParent(transform.parent);
        spellManager = Instantiate(spellManagerPrefab);
        spellManager.transform.SetParent(transform.parent);
        animationManager = Instantiate(animationManagerPrefab);
        animationManager.transform.SetParent(transform.parent);
        battleEventManager = Instantiate(battleEventManagerPrefab);
        battleEventManager.transform.SetParent(transform.parent);
        battleManager = Instantiate(battleManagerPrefab);
        battleManager.transform.SetParent(transform.parent);

        ChangeState(RunPhase.STARTPHASE);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BattlePhaseIn(){
        battleID += 1;
        gridManager = Instantiate(gridManagerPrefab);
        GridManager.Instance.SetCam(cam.transform);
        gridManager.transform.SetParent(transform.parent);
        
        AIManager = Instantiate(AIManagerPrefab);
        AIManager.transform.SetParent(transform.parent);
        eventManager = Instantiate(eventManagerPrefab);
        eventManager.transform.SetParent(transform.parent);

        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject _gameobject in rootGameObjects)
        {
            if(_gameobject.name == "Units" || _gameobject.name.Contains("UI - World Space")){
                _gameobject.SetActive(true);
            }
        }


        // Générer un nouveau groupe d'ennemis
        enemies = UnitManager.Instance.CreateUnits(testScript.enemy_composition.GetTuples(battleID), Team.Enemy);
        
        BattleManager.Instance.LaunchBattle(allies, enemies);

        //BattleManager.Instance.DebugSetState();
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.FORWARD);

        if(debug && battleID == 1){
            testScript.LaunchDebug();
        }
    }

    public void BattlePhaseOut(){
        //Deleting all the managers
        Destroy(gridManager.gameObject);
        battleManager.GetArchive().name = "Battle " + battleID;
        battleManager.GetArchive().transform.SetParent(battleArchive.transform);

        allies = UnitManager.Instance.GetUnits(Team.Ally, includingDead : true);
        UnitManager.Instance.ReviveAllyUnits(1);
        AnimationManager.Instance.ForceAnimation();

        BattleManager.Instance.Out();

        Destroy(AIManager.gameObject);
        Destroy(eventManager.gameObject);
        
        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject _gameobject in rootGameObjects)
        {
            if(_gameobject.name == "Grid"){
                Destroy(_gameobject);
            }
            if(_gameobject.name.Contains("UI - World Space")){
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

    public void LoseScreenIn(){
        endScreenManager = Instantiate(endScreenManagerPrefab);
        endScreenManager.Setup(false);
    }

    public void LoseScreenOut(){
        endScreenManager.Out();
        Destroy(endScreenManager.gameObject);
    }

    public void StartPhaseIn(){
        // Spawn allies units
        allies = UnitManager.Instance.CreateUnits(testScript.ally_composition.GetTuples(startLevel), Team.Ally);

        ChangeState(RunPhase.BATTLEPHASE);
    }

    public void StartPhaseOut(){
        AnimationManager.Instance.ForceAnimation();
    }

    public void EndPhaseIn(){
        // Reset the game
        UnitManager.Instance.Reset();

        ChangeState(RunPhase.STARTPHASE);
    }

    public void EndPhaseOut(){
        //
    }

    public void ChangeState(RunPhase trigger){
        if(runPhase == RunPhase.OUT){
            switch(trigger){
                case RunPhase.PICKPHASE:
                    runPhase = RunPhase.PICKPHASE;
                    PickPhaseIn();
                    break;
                case RunPhase.BATTLEPHASE:
                    runPhase = RunPhase.BATTLEPHASE;
                    BattlePhaseIn();
                    break;
                case RunPhase.LOSESCREEN:
                    runPhase = RunPhase.LOSESCREEN;
                    LoseScreenIn();
                    break;
                case RunPhase.STARTPHASE:
                    runPhase = RunPhase.STARTPHASE;
                    StartPhaseIn();
                    break;
                default:
                    break;
            }
        }
        else{
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
                        case RunPhase.LOSESCREEN:
                            PickPhaseOut();
                            runPhase = RunPhase.LOSESCREEN;
                            LoseScreenIn();
                            break;
                        case RunPhase.STARTPHASE:
                            PickPhaseOut();
                            runPhase = RunPhase.STARTPHASE;
                            StartPhaseIn();
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
                        case RunPhase.LOSESCREEN:
                            BattlePhaseOut();
                            runPhase = RunPhase.LOSESCREEN;
                            LoseScreenIn();
                            break;
                        case RunPhase.STARTPHASE:
                            BattlePhaseOut();
                            runPhase = RunPhase.STARTPHASE;
                            StartPhaseIn();
                            break;
                        default:
                            break;
                    }
                    break;
                case RunPhase.LOSESCREEN:
                    switch (trigger){
                        /*
                        case RunPhase.BATTLEPHASE:
                            LoseScreenOut();
                            runPhase = RunPhase.BATTLEPHASE;
                            BattlePhaseIn();
                            break;
                        */
                        /*
                        case RunPhase.PICKPHASE:
                            LoseScreenOut();
                            runPhase = RunPhase.PICKPHASE;
                            PickPhaseIn();
                            break;
                        */
                        case RunPhase.STARTPHASE:
                            LoseScreenOut();
                            runPhase = RunPhase.ENDPHASE;
                            StartPhaseIn();
                            break;
                        case RunPhase.ENDPHASE:
                            LoseScreenOut();
                            runPhase = RunPhase.ENDPHASE;
                            EndPhaseIn();
                            break;
                        default:
                            break;
                    }
                    break;
                case RunPhase.STARTPHASE:
                    switch (trigger)
                    {
                        case RunPhase.BATTLEPHASE:
                            StartPhaseOut();
                            runPhase = RunPhase.BATTLEPHASE;
                            BattlePhaseIn();
                            break;
                        
                        case RunPhase.LOSESCREEN:
                            StartPhaseOut();
                            runPhase = RunPhase.LOSESCREEN;
                            LoseScreenIn();
                            break;
                        case RunPhase.PICKPHASE:
                            StartPhaseOut();
                            runPhase = RunPhase.PICKPHASE;
                            PickPhaseIn();
                            break;
                        
                        default:
                            break;
                    }
                    break;
                case RunPhase.ENDPHASE:
                    switch (trigger)
                    {
                        case RunPhase.STARTPHASE:
                            EndPhaseOut();
                            runPhase = RunPhase.STARTPHASE;
                            StartPhaseIn();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public RunPhase GetRunPhase(){
        return runPhase;
    }

    public Canvas GetUIWorldSpace(){
        return lifeBarUI;
    }

    public List<BaseUnit> GetAllies(){
        return allies;
    }

}
