using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

/// <summary>
/// Contient toutes les méthodes relatives au moteur de combat
/// </summary>

public class BattleManager : MonoBehaviour
{
    
    public static BattleManager Instance;

    //Game states
    public enum BattleState {OUT, START, TURN, END, WON, LOST}
    public enum TurnState {OUT, START, ACTION_CHOICE, APPLY_ACTIONS, ANIMATION, END}
    public enum PlayerActionChoiceState {OUT, START, CHARACTER_SELECTION, SWITCH_CHARACTER, SPELL_SELECTION, TARGET_SELECTION, VALIDATED_ACTION, OTHER_STATE, EXIT}

    public enum Machine{BATTLESTATE, TURNSTATE, PLAYERACTIONCHOICESTATE}
    public enum Trigger {VALIDATE, CANCEL, FORWARD, START, OUT}
    public enum TeamTurn{OUT, ALLY, ENEMY}

    public BattleState battleState;
    public TurnState turnState;
    public PlayerActionChoiceState playerActionChoiceState;
    public TeamTurn teamTurn;

    public Instruction emptyInstruction;
    public BattleTurn emptyBattleTurn;
    public int nTurn = 1;
    public BattleTurn currentTurn;

    public List<BattleTurn> archivedTurns = new List<BattleTurn>();

    private bool inAnimation = false;
    private GameObject battleArchive;
    private 

    void Awake(){
        Instance = this;
    }

    public void LaunchBattle(List<BaseUnit> ally_composition, List<BaseUnit> enemy_composition)
    {

        GridManager.Instance.GenerateGrids();
        UnitManager.Instance.SpawnAllies(ally_composition);
        UnitManager.Instance.SpawnEnemies(enemy_composition);
        UnitManager.Instance.MakeUnitsVisible(Team.Both, true);
        UnitManager.Instance.StartBattle();

        nTurn = 1;
        battleArchive = new GameObject("Current Battle Archive");
        
        Debug.Log(GetCurrentStatesSummary());
        ChangeState(Machine.BATTLESTATE, Trigger.START);
        //StartBattle();
    }

    public void ChangeState(Machine machine, Trigger trigger){
        Debug.Log("3");
        switch (machine)
        {
            case Machine.PLAYERACTIONCHOICESTATE:
                Debug.Log("4");
                ChangePlayerActionChoiceState(trigger);
                break;
            case Machine.TURNSTATE:
                ChangeTurnState(trigger);
                break;
            case Machine.BATTLESTATE:
                ChangeBattleState(trigger);
                break;
            default:
                break;
        }
    }

    #region Machine à états de Action Choice

    private void CharacterSelectionPlayerActionChoicePhaseIn()
    {
        //
    }

    private void SpellSelectionPlayerActionChoicePhaseIn()
    {
        //
    }

    private void TargetSelectionPlayerActionChoicePhaseIn()
    {
        //
    }

    private void ChangePlayerActionChoiceState(Trigger trigger)
    {
        switch (playerActionChoiceState)
        {

            case PlayerActionChoiceState.OUT:
                Debug.Log("Out");
                switch (trigger)
                {
                    case Trigger.START:
                        playerActionChoiceState = PlayerActionChoiceState.CHARACTER_SELECTION;
                        CharacterSelectionPlayerActionChoicePhaseIn();
                        break;
                    default:
                        break;
                }
                break;

            case PlayerActionChoiceState.CHARACTER_SELECTION:
                Debug.Log("Character Selection");
                switch (trigger)
                {
                    case Trigger.VALIDATE:
                        playerActionChoiceState = PlayerActionChoiceState.SPELL_SELECTION;
                        SpellSelectionPlayerActionChoicePhaseIn();
                        break;
                    case Trigger.CANCEL:
                        // On reste dans le même état mais on annule l'instruction précédente
                        CancelLastInstruction();
                        break;
                    default:
                        break;
                }
                break;

            case PlayerActionChoiceState.SPELL_SELECTION:
                Debug.Log("Spell Selection");
                switch (trigger)
                {
                    case Trigger.VALIDATE:
                        playerActionChoiceState = PlayerActionChoiceState.TARGET_SELECTION;
                        TargetSelectionPlayerActionChoicePhaseIn();
                        break;
                    case Trigger.CANCEL:
                        playerActionChoiceState = PlayerActionChoiceState.CHARACTER_SELECTION;
                        CharacterSelectionPlayerActionChoicePhaseIn();
                        break;
                    default:
                        break;
                }
                break;

            case PlayerActionChoiceState.TARGET_SELECTION:
                Debug.Log("Target Selection");
                switch (trigger)
                {
                    case Trigger.VALIDATE:
                        Debug.Log("8");
                        if (UnitManager.Instance.DidEveryCharacterGaveInstruction())
                        {
                            playerActionChoiceState = PlayerActionChoiceState.OUT;
                            ChangeState(Machine.TURNSTATE, Trigger.FORWARD);
                        }
                        else
                        {
                            playerActionChoiceState = PlayerActionChoiceState.CHARACTER_SELECTION;
                            CharacterSelectionPlayerActionChoicePhaseIn();
                        }
                        break;
                    case Trigger.CANCEL:
                        playerActionChoiceState = PlayerActionChoiceState.SPELL_SELECTION;
                        SpellSelectionPlayerActionChoicePhaseIn();
                        break;

                    default:
                        break;
                }
                break;
        }
    }
    
    #endregion

    #region Machine à états de TurnState

    private void StartTurnPhaseIn()
    {
        // Start turn effects
        if (teamTurn == TeamTurn.ALLY)
        {
            StartTurnEffects();
        }

        if (GlobalManager.Instance.debug)
        {
            CheckAssertions();
        }

        NextTurn();
        SwitchCurrentTeam();
        UnitManager.Instance.MakeUnitsActive();

        // Passage automatique à la phase de action choice
        ChangeState(Machine.BATTLESTATE, Trigger.FORWARD);
    }

    private void ActionChoicePhaseIn(){
        // TODO moyen de refactor un peu tout ça

        // Si c'est au tour d'un joueur humain
        if(teamTurn == TeamTurn.ALLY && !(GlobalManager.Instance.debug && TestScript.Instance.AreThereScriptedInstructions())){
            ChangeState(Machine.PLAYERACTIONCHOICESTATE, Trigger.FORWARD);
        }

        // Si c'est au tour d'un ennemi (IA ou scripté)
        else if(teamTurn == TeamTurn.ENEMY)
        {
            List<Instruction> EnemyOrders = AIManager.Instance.GetAIOrders(ConvertTeamTurn(teamTurn));
            // S'il y a des instructions scriptées elles écrasent les ordres IA
            // C'est important que les ordres IA soient calculés quand même pour préserver le même état d'aléatoire
            if(GlobalManager.Instance.debug && TestScript.Instance.AreThereScriptedInstructions()){
                foreach (Instruction AI_Instruc in EnemyOrders)
                {
                    Destroy(AI_Instruc.gameObject);
                }
                EnemyOrders = TestScript.Instance.GetScriptedInstructions();
            }

            currentTurn.SetInstructions(EnemyOrders);
            playerActionChoiceState = PlayerActionChoiceState.OUT;
            // Passage automatique à la phase APPLY_ACTIONS
            ChangeState(Machine.BATTLESTATE, Trigger.FORWARD);
        }

        // Si c'est un tour scripté
        else if(GlobalManager.Instance.debug && TestScript.Instance.AreThereScriptedInstructions()){
            currentTurn.SetInstructions(TestScript.Instance.GetScriptedInstructions());
            playerActionChoiceState = PlayerActionChoiceState.OUT;
            // Passage automatique à la phase APPLY_ACTIONS
            ChangeState(Machine.BATTLESTATE, Trigger.FORWARD);
        }
    }

    private void ApplyActionsTurnPhaseIn(){
        ApplyInstructions();

        // Passage automatique à la phase ANIMATION
        ChangeState(Machine.BATTLESTATE, Trigger.FORWARD);
    }

    private void AnimationTurnPhaseIn(){
        // Passage automatique à la phase END
        ChangeState(Machine.BATTLESTATE, Trigger.FORWARD);
    }

    private void EndTurnPhaseIn(){
        EndTurnEffects();
        ArchiveTurn();
        // Passage automatique à la phase OUT
        ChangeState(Machine.BATTLESTATE, Trigger.FORWARD);
    }

    private void OutTurnPhaseIn(){
        // Passage automatique à la phase suivante du BattleState
        ChangeState(Machine.BATTLESTATE, Trigger.FORWARD);
    }

    private void ChangeTurnState(Trigger trigger){

        // Il est nécessaire d'envoyer des triggers FORWARD pour passer d'une phase à la suivante
        switch (turnState)
        {
            case TurnState.OUT:
                switch (trigger)
                {
                    case Trigger.START:
                        // Do stuff if needed
                        turnState = TurnState.START;
                        StartTurnPhaseIn();
                        break;
                    default:
                        break;
                }
                break;

            case TurnState.START:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        // Do stuff if needed
                        turnState = TurnState.ACTION_CHOICE;
                        ActionChoicePhaseIn();
                        break;
                    default:
                        break;
                }
                break;

            case TurnState.ACTION_CHOICE:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        turnState = TurnState.APPLY_ACTIONS;
                        ApplyActionsTurnPhaseIn();
                        break;
                    default:
                        break;
                }
                break;

            case TurnState.APPLY_ACTIONS:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        turnState = TurnState.ANIMATION;
                        AnimationTurnPhaseIn();
                        break;
                    default:
                        break;
                }
                break;

            case TurnState.ANIMATION:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        turnState = TurnState.END;
                        EndTurnPhaseIn();
                        break;
                    default:
                        break;
                }
                break;

            case TurnState.END:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        turnState = TurnState.OUT;
                        OutTurnPhaseIn();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    #endregion

    private void StartBattlePhaseIn()
    {
        // Actions de début de bataille
        ChangeTurnState(Trigger.FORWARD);

        // Passage automatique à la phase START
        ChangeBattleState(Trigger.FORWARD);
    }

    private void TurnBattlePhaseIn()
    {
        // Actions de début de la phase de tours
    }

    private void EndBattlePhaseIn()
    {
        // Passage automatique à la phase WON ou la phase LOST
        ChangeBattleState(Trigger.FORWARD);
    }

    private void WonBattlePhaseIn()
    {
        // Ce qui se passe lorsqu'on atteint l'écran de victoire
        // Passage automatique à la phase OUT
        ChangeBattleState(Trigger.OUT);

        // Communication avec le GlobalManager
        GlobalManager.Instance.ChangeState(GlobalManager.RunPhase.PICKPHASE);
    }

    private void LostBattlePhaseIn()
    {
        // Ce qui se passe lorsqu'on atteint l'écran de défaite
        // Passage automatique à la phase OUT
        ChangeBattleState(Trigger.OUT);

        // Communication avec le GlobalManager
        GlobalManager.Instance.ChangeState(GlobalManager.RunPhase.LOSESCREEN);
    }

    #region Machine à états de BattleState
    private void ChangeBattleState(Trigger trigger)
    {
        switch (battleState)
        {
            case BattleState.OUT:
                switch (trigger)
                {
                    case Trigger.START:
                        battleState = BattleState.START;
                        StartBattlePhaseIn();
                        break;
                }
                break;
            case BattleState.START:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        battleState = BattleState.TURN;
                        TurnBattlePhaseIn();
                        break;
                }
                break;
            case BattleState.TURN:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        // Check if game is over
                        if (isGameOver() != BattleState.OUT)
                        {
                            battleState = BattleState.END;
                            EndBattlePhaseIn();
                        }
                        else
                        {
                            // On reste dans le même état, et on commence un nouveau tour dans la machine TurnState
                            ChangeTurnState(Trigger.START);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case BattleState.END:
                switch (trigger)
                {
                    case Trigger.FORWARD:
                        battleState = isGameOver();
                        if (battleState == BattleState.WON)
                        {
                            WonBattlePhaseIn();
                        }
                        if (battleState == BattleState.LOST)
                        {
                            LostBattlePhaseIn();
                        }
                        break;
                }
                break;
            case BattleState.WON:
                switch (trigger)
                {
                    case Trigger.OUT:
                        battleState = BattleState.OUT;
                        break;
                }
                break;
            case BattleState.LOST:
                switch (trigger)
                {
                    case Trigger.OUT:
                        battleState = BattleState.OUT;
                        break;
                }
                break;
        }
    }
    
    #endregion

    private BattleState isGameOver()
    {
        if (UnitManager.Instance.GetUnits(Team.Ally).Count == 0)
        {
            return BattleState.LOST;
        }
        if (UnitManager.Instance.GetUnits(Team.Enemy).Count == 0)
        {
            return BattleState.WON;
        }
        return BattleState.OUT;
    }

    private void SwitchCurrentTeam()
    {
        if(teamTurn == TeamTurn.ALLY){
            teamTurn = TeamTurn.ENEMY;
        }
        else{
            teamTurn = TeamTurn.ALLY;
        }
    }

    public Team InvertTeam(Team team){
        if(team == Team.Ally){
            return Team.Enemy;
        }
        else if(team == Team.Enemy){
            return Team.Ally;
        }
        else{
            return Team.Both;
        }
    }

    public string GetCurrentStatesSummary(){
        string currentStates = "";
        currentStates = currentStates + "BattleState: " + battleState + "\n";
        currentStates = currentStates + "TurnState: " + turnState + "\n";
        currentStates = currentStates + "PlayerActionChoiceState: " + playerActionChoiceState + "\n";
        return currentStates;

    }
    public BattleState GetBattleState(){
        return battleState;
    }

    public TurnState GetTurnState(){
        return turnState;
    }

    public PlayerActionChoiceState GetPlayerActionChoiceState(){
        return playerActionChoiceState;
    }
    private void StartBattle(){
        
        teamTurn = TeamTurn.ALLY;
        battleState = BattleState.START;
        turnState = TurnState.OUT;
        playerActionChoiceState = PlayerActionChoiceState.OUT;
    }

    public void EndTurnEffects(){
        UnitManager.Instance.ApplyEndTurnEffects();
    }

    public void StartTurnEffects(){
        UnitManager.Instance.ApplyStartTurnEffects();
    }

    private void ArchiveTurn(){
        currentTurn.ArchiveTurn();
        archivedTurns.Add(currentTurn);
    }

    public Team ConvertTeamTurn(TeamTurn teamTurn){
        switch (teamTurn)
        {
            case TeamTurn.ALLY:
                return Team.Ally;
                
            case TeamTurn.ENEMY:
                return Team.Enemy;

            default:
                return Team.Both;
        }
    }

    private void NextTurn(){
        nTurn ++;
        currentTurn = Instantiate(emptyBattleTurn);
        currentTurn.transform.SetParent(battleArchive.transform);
        currentTurn.name = "Turn " + nTurn;
        currentTurn.Setup(nTurn);
        
    }


    public Instruction CreateInstruction(BaseUnit source_unit, BaseSpell spell_to_cast, Tile target_tile, bool hyper = false){
        Instruction new_instruction = Instantiate(emptyInstruction);
        new_instruction.transform.SetParent(currentTurn.transform);
        new_instruction.Setup(source_unit, spell_to_cast, target_tile, hyper : hyper);
        new_instruction.name = new_instruction.GetSummary();
        return new_instruction;
    }

    public void AssignInstruction(Instruction instruction){
        AddInstruction(instruction);
        instruction.GetSourceUnit().GiveInstruction(true);
    }

    public void CancelLastInstruction(){
        currentTurn.RemoveInstruction(currentTurn.GetInstructions().Count - 1);
    }

    private void AddInstruction(Instruction instruction){
        currentTurn.AddInstruction(instruction);
    }

    private void ApplyInstructions(){

        List<string> logLine = new List<string>();
        foreach (Instruction instruction in currentTurn.GetInstructions())
        {
            logLine.Add(instruction.GetLog());
        }
        TestScript.Instance.Log("I:" + string.Join("/", logLine));

        foreach (Instruction instruction in currentTurn.GetInstructions())
        {
            ApplyInstruction(instruction);    
        }
    }

    private void ApplyInstruction(Instruction instruction){
        if(instruction.IsOverloaded()){
            instruction.GetSourceUnit().CastSpell(instruction.GetSpell(), instruction.GetTargetTile(), true);
        }
        else{
            instruction.GetSourceUnit().CastSpell(instruction.GetSpell(), instruction.GetTargetTile(), false);
        }

    }

    public int CountInstructions(){
        return currentTurn.GetInstructions().Count;
    }

    public bool AreInstructionsFull(){
        int registeredInstructionCount = 0;
        foreach (BaseUnit unit in UnitManager.Instance.GetUnits(Team.Ally))
        {
            foreach (Instruction instruction in currentTurn.GetInstructions()){
                if(instruction.GetSourceUnit() == unit){
                    registeredInstructionCount ++;
                }
            }
        }
        return registeredInstructionCount == UnitManager.Instance.CountUnits(Team.Ally);
    }

    public void AddEvent(BattleEvent _event){
        if(currentTurn != null){
            currentTurn.AddEvent(_event);
        }
    }

    public void SetInAnimation(bool value){
        inAnimation = value;
    }

    public bool IsInAnimation(){
        return inAnimation;
    }

    public GameObject GetArchive(){
        return battleArchive;
    }

    private void CheckAssertions()
    {
        foreach (BaseUnit unit in UnitManager.Instance.GetUnits())
        {
            unit.CheckAssertions();
        }
    }

    public void Out(){
        UnitManager.Instance.EndBattle();
        if(GetBattleState() == BattleState.WON){
            UnitManager.Instance.RemoveUnits(Team.Enemy);
            //UnitManager.Instance.MakeUnitsVisible(Team.Ally, false);
        }
        if(GetBattleState() == BattleState.LOST){
            UnitManager.Instance.RemoveUnits(Team.Both);
        }
    }
}
