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

    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject alliesLifeBar;
    [SerializeField] private GameObject ennemiesLifeBar;

    [SerializeField] private GameObject UIobject;

    [SerializeField] private PickPhaseManager pickPhaseManagerPrefab;

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
    [SerializeField] private TestScript testScript;

    private PickPhaseManager pickPhaseManager;

    public bool debug;

    void Awake(){
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        gridManager = Instantiate(gridManagerPrefab);
        GridManager.Instance.SetCam(_cam.transform);
        gridManager.transform.SetParent(transform.parent);
        battleManager = Instantiate(battleManagerPrefab);
        battleManager.transform.SetParent(transform.parent);
        unitManager = Instantiate(unitManagerPrefab);
        unitManager.transform.SetParent(transform.parent);
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

        BattleManager.Instance.LaunchBattle(testScript.ally_composition.GetTuples(), testScript.enemy_composition.GetTuples());
        BattleManager.Instance.DebugSetState();
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.FORWARD);

        if(debug){
            testScript.LaunchDebug();
        }
    }

    public void BattlePhaseOut(){
        //Deleting all the managers
        Destroy(gridManager.gameObject);
        Destroy(battleManager.gameObject);
        Destroy(unitManager.gameObject);
        Destroy(spellManager.gameObject);
        Destroy(interfaceManager.gameObject);
        Destroy(animationManager.gameObject);
        Destroy(AIManager.gameObject);
        Destroy(battleEventManager.gameObject);
        Destroy(eventManager.gameObject);
        
        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject _gameobject in rootGameObjects)
        {
            if(_gameobject.name == "Grid" || _gameobject.name == "Units" || _gameobject.name.Contains("UI Screen Space") || _gameobject.name.Contains("UI - World Space")){
                Destroy(_gameobject);
            }
        }
    }

    public Camera GetCam(){
        return _cam;
    }

    public void PickPhaseIn(){
        pickPhaseManager = Instantiate(pickPhaseManagerPrefab);
        pickPhaseManager.transform.SetParent(transform.parent);
    }

    public void PickPhaseOut(){
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

}
