using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    // UI elements
    public GameObject informationPanel;
    public TextMeshProUGUI unitNamePanel;
    public TextMeshProUGUI unitPowerPanel;
    public TextMeshProUGUI unitHealthPanel;
    public TextMeshProUGUI unitLevelPanel;
    public TextMeshProUGUI unitPassiveNamePanel;
    public TextMeshProUGUI unitPassiveDescriptionPanel;
    public GameObject spellSelector;
    public GameObject shade;

    // La Tile sélectionnée à tout moment
    private Tile selectedTile;

    // La Tile contenant la source du spell, lorsqu'un spell est lancé (lorsqu'on revient en arrière pendant la sélection de cible)
    private Tile sourceTile;

    private enum SpellChoice{CHARACTER, LEFT, RIGHT, UP, DOWN}
    private SpellChoice spellChoice;

    // Le spell pour lequel on va sélectionner une cible
    private BaseSpell selectedSpell;

    // La Tile contenant la cible du spell, lorsqu'un spell est lancé
    private Tile targetTile;
    // TODO Implémenter un tuple qui contient toutes les ordres définis.

    private bool sourceSelectionActivated;
    private bool spellSelectionActivated;
    private Dictionary<BattleManager.PlayerActionChoiceState, bool> activated_states;

    void Awake(){
        activated_states = new Dictionary<BattleManager.PlayerActionChoiceState, bool>();
        foreach (BattleManager.PlayerActionChoiceState state in System.Enum.GetValues(typeof(BattleManager.PlayerActionChoiceState)))
        {
            activated_states[state] = false;
        }
    }
    void Update()
    {
        switch (BattleManager.Instance.GetPlayerActionChoiceState())
        {
            case BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION:
                SourceSelectionDisplay();
                break;
            case BattleManager.PlayerActionChoiceState.SPELL_SELECTION:
                SpellSelectionDisplay();
                break;
            case BattleManager.PlayerActionChoiceState.TARGET_SELECTION:
                TargetSelectionDisplay();
                break;
            default:
                Debug.Log(BattleManager.Instance.GetCurrentStatesSummary());
                break;
        }
        
    }   

    void SourceSelectionDisplay(){
        if(!activated_states[BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION]){
            // Just changed from another state

            // Reset view
            ResetDisplay();

            // Activate the needed interface
            informationPanel.SetActive(true);

            ActivateState(BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION);

        }
        selectedTile = GridManager.Instance.GetMainSelection();

        if (Input.GetKeyDown(KeyCode.B)){
            if(selectedTile.GetUnit()!= null){
                if(selectedTile.GetUnit().GetTeam() == Team.Ally){
                    SpellSelectionTrigger(BattleManager.Trigger.VALIDATE);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.N)){
            spellSelector.SetActive(false);
            shade.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            if(selectedTile.GetNextTile(Directions.UP) != null){
                selectedTile.GetNextTile(Directions.UP).Select();
                selectedTile.Unselect();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            if(selectedTile.GetNextTile(Directions.DOWN) != null){
                selectedTile.GetNextTile(Directions.DOWN).Select();
                selectedTile.Unselect();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            if(selectedTile.GetNextTile(Directions.LEFT) != null){
                selectedTile.GetNextTile(Directions.LEFT).Select();
                selectedTile.Unselect();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            if(selectedTile.GetNextTile(Directions.RIGHT) != null){
                selectedTile.GetNextTile(Directions.RIGHT).Select();
                selectedTile.Unselect();
            }
        }

        BaseUnit currentUnit = selectedTile.GetUnit();
        if(currentUnit != null){
            informationPanel.SetActive(true);
            unitNamePanel.text = currentUnit.GetName();
            unitPowerPanel.text = "Puissance : " + currentUnit.GetFinalPower().ToString();
            unitHealthPanel.text = "PV : " + currentUnit.GetFinalHealth().ToString();
            unitLevelPanel.text = "Niveau : " + currentUnit.GetLevel().ToString();
            unitPassiveNamePanel.text = currentUnit.GetPassive().GetName();
            unitPassiveDescriptionPanel.text = currentUnit.GetPassive().GetFightDescription();
        }
        else{
            informationPanel.SetActive(false);
        }
    }

    /*
    void SpellSelectionActivation(bool activation, Tile source_tile = null){

        spellSelector.SetActive(activation);
        shade.SetActive(activation);

        sourceTile = source_tile;
        BaseUnit sourceUnit = sourceTile.GetUnit();

        List<BaseSpell> currentSpells = sourceUnit.availableSpells;

        if (activation){
            spellSelector.transform.position = selectedTile.transform.position;
            sourceUnit = selectedTile.GetUnit();

            int currentSpellIndex = 0;
            foreach (var spell in sourceUnit.availableSpells)
            {
                spellSelector.transform.GetChild(currentSpellIndex).GetComponent<UnityEngine.UI.Image>().sprite = spell.artwork;
                currentSpellIndex += 1;
            }
            BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.VALIDATE);
        }
        else{
            //    
        }

    }
    */

    void SpellSelectionTrigger(BattleManager.Trigger trigger){

        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.VALIDATE);
    }

    void SpellSelectionDisplay(){
        BaseUnit sourceUnit = sourceTile.GetUnit();
        List<BaseSpell> currentSpells = sourceUnit.availableSpells;

        if(!activated_states[BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION]){
            // Just changed from another state

            // Reset view
            ResetDisplay();

            // Activate the needed interface
            spellSelector.SetActive(true);
            shade.SetActive(true);
            informationPanel.SetActive(true);

            spellSelector.transform.position = selectedTile.transform.position;

            int currentSpellIndex = 0;

            foreach (var spell in currentSpells)
            {
                spellSelector.transform.GetChild(currentSpellIndex).GetComponent<UnityEngine.UI.Image>().sprite = spell.artwork;
                currentSpellIndex += 1;
            }
            
            ActivateState(BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION);
        }

        //Display highlight
        spellSelector.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(false);

        switch(spellChoice){
            case SpellChoice.CHARACTER:
                spellSelector.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
                break;
            case SpellChoice.LEFT:
                spellSelector.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                break;
            case SpellChoice.RIGHT:
                spellSelector.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                break;
            case SpellChoice.UP:
                spellSelector.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
                break;
            case SpellChoice.DOWN:
                spellSelector.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
                break;
            default:
                break;
        }

        // Navigation au sein de la sélection
        switch(spellChoice){
            case SpellChoice.CHARACTER:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner attaque
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour à la sélection de personnages
                    SpellSelectionTrigger(BattleManager.Trigger.CANCEL);
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                break;
            case SpellChoice.LEFT:
            // selectedSpell = availableSpells[0];
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner spell gauche
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                break;
            case SpellChoice.RIGHT:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner spell à droite
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                break;
            case SpellChoice.UP:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner spell en haut
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                break;
            case SpellChoice.DOWN:
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner spell en bas
                    // TODO
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                break;

            default:
                break;
        }

        // Affichage du spell dans l'information panel
        switch(spellChoice){
            case SpellChoice.CHARACTER:
                unitPassiveNamePanel.text = sourceUnit.GetPassive().GetName();
                // FIXME Lorsqu'on quitte l'interface de spells ça renvoie une nullreference
                // Je pense que c'est lié à la variable sourceUnit qui est mal gérée
                unitPassiveDescriptionPanel.text = sourceUnit.GetPassive().GetFightDescription();
                break;
            case SpellChoice.LEFT:
                unitPassiveNamePanel.text = currentSpells[0].GetName();
                unitPassiveDescriptionPanel.text = currentSpells[0].GetFightDescription();
                break;
            case SpellChoice.RIGHT:
                unitPassiveNamePanel.text = currentSpells[1].GetName();
                unitPassiveDescriptionPanel.text = currentSpells[1].GetFightDescription();
                break;
            case SpellChoice.UP:
                unitPassiveNamePanel.text = currentSpells[2].GetName();
                unitPassiveDescriptionPanel.text = currentSpells[2].GetFightDescription();
                break;
            case SpellChoice.DOWN:
                unitPassiveNamePanel.text = currentSpells[3].GetName();
                unitPassiveDescriptionPanel.text = currentSpells[3].GetFightDescription();
                break;
            default:
                break;
        }
    }
    
    /*
    void TargetSelectionActivation(bool activation, BaseSpell spell = null){
        if (activation){
            // Place the cursor on the first valid target in front of the unit
            //selectedTarget = GridManager.Instance.FindFirstTarget();

            // Hihglight all the tiles in the selection zone

            BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.VALIDATE);
        }
        else{
            //
        }
        
    }
    */

    void TargetSelectionDisplay(){
        //
    }

    void ResetDisplay(){
            spellSelector.SetActive(false);
            shade.SetActive(false);
            informationPanel.SetActive(false);
    }

    void ActivateState(BattleManager.PlayerActionChoiceState stateToActivate){
        Dictionary<BattleManager.PlayerActionChoiceState, bool> new_states = new Dictionary<BattleManager.PlayerActionChoiceState, bool>(activated_states);
        foreach (var state in activated_states)
        {
            if(state.Key == stateToActivate){
                new_states[state.Key] = true;
            }
            else{
                new_states[state.Key] = false;
            }
        }
        activated_states = new_states;
    }
}


public enum Directions {RIGHT, LEFT, UP, DOWN}
