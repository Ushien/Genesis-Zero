using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Linq;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;
    [SerializeField] private GridManager gridManagerPrefab;
    [SerializeField] private BattleManager battleManagerPrefab;
    [SerializeField] private UnitManager unitManagerPrefab;
    [SerializeField] private SpellManager spellManagerPrefab;
    [SerializeField] private InterfaceManager interfaceManagerPrefab;
    [SerializeField] private AnimationManager animationManagerPrefab;
    [SerializeField] private AudioManager audioManagerPrefab;
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
    private AudioManager audioManager;
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
    private int battleID;
    [SerializeField]
    private int startLevel = 1;
    public int runSeed = 0;

    void Awake(){
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    { 
        cam = Instantiate(camPrefab);
        audioManager = Instantiate(audioManagerPrefab); // Pour l'instant ici, je gère pas ou il doit être supprimé/ajouté
        audioManager.transform.SetParent(transform.parent);
        ResetRun();
        ChangeState(RunPhase.STARTPHASE);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ResetRun()
    {
        battleID = 0;
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

        lifeBarUI.gameObject.SetActive(true);

        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject _gameobject in rootGameObjects)
        {
            if(_gameobject.name == "Units" || _gameobject.name.Contains("UI - World Space")){
                _gameobject.SetActive(true);
            }
        }


        // Générer un nouveau groupe d'ennemis
        List<ScriptableComposition> enemiesCompositions = resourceManager.GetEnemyCompositions(lootable:true);
        ScriptableComposition enemyComposition = enemiesCompositions.Where(_ => true).OrderBy(_ => UnityEngine.Random.value).First();
        if(debug && testScript.enemy_composition != null){
            enemyComposition = testScript.enemy_composition;
        }
        enemies = UnitManager.Instance.CreateUnits(enemyComposition.GetTuples(battleID), Team.Enemy);
        
        BattleManager.Instance.LaunchBattle(allies, enemies);
        AnimationManager.Instance.BattleIn();

        //BattleManager.Instance.DebugSetState();
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.FORWARD);

        if(debug && battleID == 1){
            testScript.LaunchDebug();
        }
    }

    public void BattlePhaseOut(){
        battleManager.GetArchive().name = "Battle " + battleID;
        battleManager.GetArchive().transform.SetParent(battleArchive.transform);

        allies = UnitManager.Instance.GetUnits(Team.Ally, includingDead : true);
        UnitManager.Instance.ReviveAllyUnits(1);
        AnimationManager.Instance.ForceAnimation();

        BattleManager.Instance.Out();
        AnimationManager.Instance.BattleOut();

        Destroy(AIManager.gameObject);
        Destroy(eventManager.gameObject);
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
        Destroy(gridManager.gameObject);
        pickPhaseManager.End();
        Destroy(pickPhaseManager.gameObject);        
        
        lifeBarUI.gameObject.SetActive(false);
        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (Transform t in GameObject.FindObjectsOfType<Transform>())
        {
            GameObject go = t.gameObject;
            if(go.name == "Grid"){
                Destroy(go);
            }
            if(go.name.Contains("UI - World Space")){
                go.SetActive(false);
            }
            if(go.name.Contains("reward_")){
                Destroy(go);
            }
        }
    }

    public void LoseScreenIn(){
        endScreenManager = Instantiate(endScreenManagerPrefab);
        endScreenManager.transform.SetParent(transform.parent);
        endScreenManager.name = "EndScreenManager";
        endScreenManager.Setup(false);
        endScreenManager.In();
    }

    public void LoseScreenOut(){
        endScreenManager.Out();
        Destroy(endScreenManager.gameObject);
    }

    public void StartPhaseIn(){

        if(runSeed == 0){
            runSeed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        UnityEngine.Random.InitState(runSeed);

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

        TestScript.Instance.Log("S:" + runSeed.ToString());

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
        Destroy(resourceManager.gameObject);
        Destroy(unitManager.gameObject);
        Destroy(lifeBarUI.gameObject);
        Destroy(interfaceManager.gameObject);
        Destroy(spellManager.gameObject);
        Destroy(animationManager.gameObject);
        Destroy(battleEventManager.gameObject);
        Destroy(battleManager.gameObject);

        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject _gameobject in rootGameObjects)
        {
            if(_gameobject.name.Contains("UI Screen Space") || _gameobject.name.Contains("Units") || _gameobject.name.Contains("Battle Archive")){
                Destroy(_gameobject);
            }
        }

        ResetRun();
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
