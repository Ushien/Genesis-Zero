using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// Représente une unité au sens large
/// </summary>
public class BaseUnit : MonoBehaviour
{
        #region Fields
        #region Fields de setup

    public ScriptableUnit scriptableUnit;
    public Modifier emptyModifier;
        #endregion

        #region Références à d'autres objets

    public Tile OccupiedTile;
    public GameObject lifeBar;
    private List<Passive> passives;
    public BaseSpell attack;
    public BaseSpell[] availableSpells = new BaseSpell[4];

        #endregion

        #region Caractéristiques

    private string unit_name = "Name";
    // Puissance de l'unité
    private int basePower = 1;
    private int finalPower = 1;
    // Points de vie maximaux
    private int totalHealth = 1;
    // Points de vie actuels
    private int finalHealth = 1;
    // Niveau (influence la vie et la puissance)
    private int level = 1;
    [TextArea(5,10)]
    private string lore_description = "Lore Description";
    [TextArea(5,10)]
    private string fight_description = "Fight Description";
    private Team Team = Team.Enemy;
    // Combien de tours d'étourdissement restants
    private int stunTime = 0;
    // Est-ce que l'unité est étourdie ?
    private bool stun;
    // Est-ce que l'unité est hors-combat ?
    private bool dead = false;
    private int armor = 0;
    private int curatifCount = 0;

    private Vector2 position;
        #endregion

        #region Fields relatifs au moteur de jeu

    // Liste des modificateurs associés à l'unité
    private Dictionary<Func<int, BattleEvent>, List<Modifier>> modifiers = new Dictionary<Func<int, BattleEvent>, List<Modifier>>();
    private List<Modifier> globalModifiers = new List<Modifier>();
    // Liste des actions enregistrées. Le tuple est composé de 3 éléments: La méthode qui doit être appelée, le paramètre avec lequel elle doit être appelée, le nombre de tours dans lequel l'action doit être effectuée.
    private List<Tuple<Func<int, BattleEvent>, int, int>> actionQueue = new List<Tuple<Func<int, BattleEvent>, int, int>>();
    // L'unité a-t-elle déjà reçu une instruction ?
    private bool instructionGiven = false;
        #endregion
        #endregion

        #region Fields relatifs à l'interface
    private Vector3 targetLifeBarScale = Vector3.one;
    private Vector3 targetArmorBarScale = new Vector3(0, 1, 1);

        #endregion

    ////////////////////////////////////////////////////////

        #region Méthodes d'initialisation

    /// <summary>
    /// Initialise l'unité
    /// </summary>
    /// <param name="originUnit">Modèle d'unité</param>
    /// <param name="setup_level">Niveau de l'unité</param>
    /// <param name="team">Equipe de l'unité</param>
    public void Setup(ScriptableUnit originUnit, int setup_level, Team team, Vector2 _position){
        scriptableUnit = originUnit;
        name = scriptableUnit.unit_name;
        
        GetComponent<SpriteRenderer>().sprite = scriptableUnit.sprite;
        unit_name = scriptableUnit.unit_name;

        Team = team;
        position = _position;

        level = setup_level;

        finalPower = GetStatFromLevel(scriptableUnit.original_power, level);
        basePower = finalPower;
        totalHealth = GetStatFromLevel(scriptableUnit.original_health, level);
        finalHealth = totalHealth;

        lore_description = scriptableUnit.lore_description;
        fight_description = scriptableUnit.fight_description;

        // Il faut ajouter un modifier pour chaque méthode qui pourrait y être sujette
        modifiers[Heal] = new List<Modifier>();
        modifiers[Damage] = new List<Modifier>();

        passives = new List<Passive>();

        foreach(ScriptablePassive _passive in originUnit.passives)
        {
            _passive.SetupPassive(this);
        }

        SpellManager.Instance.SetupAttack(originUnit.attack, this);

        availableSpells = new BaseSpell[4];
        int i = 0;
        foreach (ScriptableSpell spell in scriptableUnit.spells)
        { 
            if(spell != null){
                SpellManager.Instance.SetupSpell(spell, this, i);
            }
            i++;
        }

        targetArmorBarScale = new Vector3((float)GetArmor()/GetTotalHealth(), 1, 1);
        lifeBar = InterfaceManager.Instance.SetupLifebar(this);
    }

    #endregion

        #region Méthodes relatives au moteur de jeu

    /// <summary>
    /// Mets à jour visuellement l'unité à chaque frame
    /// </summary>
    public void Update(){
        // Rend l'unité grisée si elle a déjà reçu une instruction
        if(HasGivenInstruction()){
            gameObject.GetComponent<SpriteRenderer>().color = new Color32( 173, 173, 173, 200);
        }
        else{
            gameObject.GetComponent<SpriteRenderer>().color = new Color32( 255, 255, 255, 255);
        }
    }

    /// <summary>
    /// Rend l'unité prête pour le combat suivant.
    /// Les HP et certains états sont maintenus, le reste est remis à jour en conséquence.
    /// </summary>
    public void EndBattle(){
        GiveInstruction(false);
        Cleanse();
        modifiers[Heal] = new List<Modifier>();
        modifiers[Damage] = new List<Modifier>();
        globalModifiers = new List<Modifier>();
        CheckModifiers();
        actionQueue = new List<Tuple<Func<int, BattleEvent>, int, int>>();
        
        List<Passive> newPassives = new List<Passive>();
        foreach (Passive passive in GetPassives())
        {
            passive.Activate(false);
            if(!passive.IsMinor()){
                newPassives.Add(passive);
            }
        }
        passives = newPassives;

        foreach (BaseSpell spell in availableSpells)
        {
            if(spell != null){
                spell.EndBattle();
            }
        }
    }

    public void StartBattle(){
        foreach (Passive passive in GetPassives())
        {
            if(!passive.IsActivated()){
                passive.Activate();
            }
        }
    }

    /// <summary>
    /// Applique tous les effets de fin de tour liés à l'unité
    /// </summary>
    public void ApplyEndturnEffects(){
        // Diminue le temps d'étourdissement de 1
        ModifyStunTime(-1);
        // Applique les effets de fin de tours de chacun des sorts de l'unité
        foreach (BaseSpell spell in GetSpells())
        {
            if(spell != null){
                spell.ApplyEndTurnEffects(); 
            }
        }
        GetAttack().ApplyEndTurnEffects();
        // Modifiers
        ModifiersEndTurn();
        // ActionQueue
        ReduceQueueTurns();
        ApplyQueueActions();
    }

    /// <summary>
    /// Utilise l'attaque de l'unité
    /// </summary>
    /// <param name="targetTile"></param>
    public void Attack(Tile targetTile){
        attack.Cast(targetTile);
    }

    /// <summary>
    /// L'unité peut-elle recevoir une instruction
    /// </summary>
    /// <returns></returns>
    public bool IsAvailable(){
        bool available = true;
        if(dead){
            available = false;
        }
        if(stun){
            available = false;
        }
        return available;
    }

    /// <summary>
    /// Renvoie la valeur d'une statistique du niveau 100 adaptée à un autre niveau
    /// </summary>
    /// <param name="level_100_stat"></param>
    /// <param name="real_level"></param>
    /// <returns></returns>
    public int GetStatFromLevel(int level_100_stat, int real_level){

        var level_1_stat = (float)level_100_stat/10;
        var growth_by_level = (level_100_stat-level_1_stat)/99;
        var real_level_stat = Tools.Ceiling(level_1_stat+growth_by_level*(real_level-1));

        return real_level_stat;
    }

    /// <summary>
    /// Active une technique de l'unité
    /// </summary>
    /// <param name="index"></param>
    public void CastSpell(int index){
        availableSpells[index].Cast();
    }

    /// <summary>
    /// Indique si l'unité a reçu une instruction ou non
    /// </summary>
    /// <param name="newInstructionGiven"></param>
    public void GiveInstruction(bool newInstructionGiven){
        instructionGiven = newInstructionGiven;
    }

    /// <summary>
    /// L'unité a-t-elle reçu une instruction
    /// </summary>
    /// <returns></returns>
    public bool HasGivenInstruction(){
        return instructionGiven;
    }

        #region Gestion de la queue d'actions

    /// <summary>
    /// Ajoute une action à la queue
    /// </summary>
    /// <param name="action"></param>
    /// <param name="parameter"></param>
    /// <param name="howManyTurns"></param>
    public void QueueAction(Func<int, BattleEvent> action, int parameter, int howManyTurns){
        actionQueue.Add(new Tuple<Func<int, BattleEvent>, int, int>(action, parameter, howManyTurns));
    }

    /// <summary>
    /// Réduit de 1 le nombre de tours de la queue d'actions
    /// </summary>
    public void ReduceQueueTurns(){
        List<Tuple<Func<int, BattleEvent>, int, int>> newQueue = new List<Tuple<Func<int, BattleEvent>, int, int>>();

        foreach (Tuple<Func<int, BattleEvent>, int, int> queuedAction in actionQueue)
        {
            newQueue.Add(new Tuple<Func<int, BattleEvent>, int, int>(queuedAction.Item1, queuedAction.Item2, queuedAction.Item3 - 1));
        }

        actionQueue = newQueue;
    }

    /// <summary>
    /// Applique les actions de la queue dont le nombre de tour restants est de 0. Les supprime ensuite de la queue.
    /// </summary>
    public void ApplyQueueActions(){
        foreach (Tuple<Func<int, BattleEvent>, int, int> queuedAction in actionQueue)
        {
            if(queuedAction.Item3 == 0){
                queuedAction.Item1(queuedAction.Item2);
            }
        }

        List<Tuple<Func<int, BattleEvent>, int, int>> newQueue = new List<Tuple<Func<int, BattleEvent>, int, int>>();

        foreach (Tuple<Func<int, BattleEvent>, int, int> queuedAction in actionQueue)
        {
            if(queuedAction.Item3 != 0){
                newQueue.Add(new Tuple<Func<int, BattleEvent>, int, int>(queuedAction.Item1, queuedAction.Item2, queuedAction.Item3));
            }
        }

        actionQueue = newQueue;
    }

    public int GetAvailableSpellIndex(){
        for (int index = 1; index <= 3; index++)
        {
            if(availableSpells[index] == null){
                return index;
            }
        }
        return -1;
    }

    public bool HasSpell(ScriptableSpell _spell){
        foreach (BaseSpell spell in availableSpells)
        {
            if(spell != null){
                if(_spell == spell.GetScriptableSpell()){
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Renvoie un passif précis, si l'unité en possède une copie.
    /// </summary>
    /// <param name="_passive"></param>
    /// <returns></returns>
    public Passive GetPassive(ScriptablePassive _passive){
        foreach (Passive passive in GetPassives())
        {
            if(passive != null){
                if(_passive == passive.GetScriptablePassive()){
                    return passive;
                }
            }
        }
        return null;  
    }

    public bool HasPassive(ScriptablePassive _passive){
        foreach (Passive passive in GetPassives())
        {
            if(passive != null){
                if(_passive == passive.GetScriptablePassive()){
                    return true;
                }
            }
        }
        return false; 
    }

    public  bool HasPassive(Passive _passive){
        foreach (Passive passive in GetPassives())
        {
            if(passive != null){
                if(_passive.GetScriptablePassive() == passive.GetScriptablePassive()){
                    return true;
                }
            }
        }
        return false; 
    }

    #endregion

        #region Gestion des modificateurs

    public void AddGlobalModifier(Modifier modifier){
        globalModifiers.Add(modifier);
        CheckModifiers();
    }

    public void DeleteGlobalModifier(Modifier modifier){
        globalModifiers.Remove(modifier);
        CheckModifiers();
    }

    /// <summary>
    /// Ajoute un modificateur sur une méthode
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="function"></param>
    public void AddModifier(Modifier modifier, Func<int, BattleEvent> function){
        modifiers[function].Add(modifier);
        CheckModifiers();
    }

    /// <summary>
    /// Supprime un modificateur sur une méthode
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="function"></param>
    public void DeleteModifier(Modifier modifier, Func<int, BattleEvent> function){
        modifiers[function].Remove(modifier);
        CheckModifiers();
    }

    /// <summary>
    /// Diminue la durée restante des modificateurs de 1 et les supprime si celle-ci vaut 0
    /// </summary>
    private void ModifiersEndTurn(){
        foreach (Modifier modifier in globalModifiers){
            modifier.ModifyTurns(-1);
        }
        foreach (var action in modifiers)
        {
            foreach (Modifier _modifier in action.Value)
            {
                _modifier.ModifyTurns(-1);
            }
        }
        CheckModifiers();
    }

    private void CheckModifiers(){
        int newPower = basePower;
        foreach (Modifier modifier in globalModifiers){
            if(modifier.IsEnded()){
                //FIXME this les listes aiment pas beaucoup ça
                globalModifiers.Remove(modifier);
            }
            else{
                if(modifier.powerBonus != 0){
                    newPower = Tools.Ceiling(modifier.powerBonus * newPower + newPower);
                }
            }
        }

        finalPower = newPower;
        CheckFinalPower();

        foreach (var action in modifiers)
        {
            foreach (Modifier _modifier in action.Value)
            {
                if(_modifier.IsEnded()){
                    //FIXME this les listes aiment pas beaucoup ça
                    action.Value.Remove(_modifier);
                }
            }
        }
    }

    #endregion
        #endregion

        #region Manipulation des caractéristiques de l'unité

    /// <summary>
    /// Renvoie le nom de l'unité
    /// </summary>
    /// <returns></returns>
    public string GetName(){
        return unit_name;
    }

    /// <summary>
    /// Renvoie la description en combat de l'unité
    /// </summary>
    /// <returns></returns>
    public string GetFightDescription(){
        return fight_description;
    }

    /// <summary>
    /// Renvoie le niveau de l'unité
    /// </summary>
    /// <returns></returns>
    public int GetLevel(){
        return level;
    }

    /// <summary>
    /// Renvoie l'équipe de l'unité
    /// </summary>
    /// <returns></returns>
    public Team GetTeam(){
        return Team;
    }

    public void SetTeam(Team _team){
        Team = _team;
    }

    public Vector2 GetPosition(){
        return position;
    }

    public List<Passive> GetPassives(){
        return passives;
    }

    public void AddPassive(Passive newPassive){
        passives.Add(newPassive);
    }

    /// <summary>
    /// Renvoie la liste de techniques de l'unité
    /// </summary>
    /// <param name="includingAttack">Si fixé à True, intègre l'attaque de l'unité à la liste, à l'index 4</param>
    /// <returns></returns>
    public BaseSpell[] GetSpells(bool includingAttack = false){

        if(includingAttack){
            
            BaseSpell[] completeAvailableSpells = new BaseSpell[5];
            int i = 0;
            foreach(BaseSpell spell in availableSpells){
                completeAvailableSpells[i] = spell;
                i++;
            }
            completeAvailableSpells[4] = GetAttack();

            return completeAvailableSpells;
        }

        else{
            return availableSpells;
        }

    }

    /// <summary>
    /// Supprime un sort donné de la liste de sorts
    /// </summary>
    /// <param name="spell_to_remove"></param>
    public void RemoveSpell(BaseSpell spell_to_remove){
        int i = 0;
        foreach (BaseSpell _spell in GetSpells())
        {
            if (_spell == spell_to_remove)
            {
                spell_to_remove.SetOwner(null);
                RemoveSpell(i);
            }
            i++;
        }
    }

    /// <summary>
    /// Supprime un sort de la liste de sorts, par index
    /// </summary>
    /// <param name="index"></param>
    public void RemoveSpell(int index){
        availableSpells[index] = null;
    }

    /// <summary>
    /// Renvoie une technique aléatoire de l'unité
    /// </summary>
    /// <param name="includingAttack">Si fixé à True, intègre l'attaque de l'unité à la liste</param>
    /// <returns></returns>
    public BaseSpell GetRandomSpell(bool includingAttack){
        return GetSpells(includingAttack).Where(t => t != null).OrderBy(t => UnityEngine.Random.value).First();
    }

    /// <summary>
    /// Renvoie l'attaque de l'unité
    /// </summary>
    /// <returns></returns>
    public BaseSpell GetAttack(){
        return attack;
    }

    /// <summary>
    /// Renvoie la case occupée par l'unité
    /// </summary>
    /// <returns></returns>
    public Tile GetTile(){
        return OccupiedTile;
    }

        #region Gestion de la puissance

    /// <summary>
    /// Renvoie la puissance de l'unité
    /// </summary>
    /// <returns></returns>
    public int GetFinalPower(){
        return finalPower;
    }

    /// <summary>
    /// Fixe la puissance de l'unité
    /// </summary>
    /// <param name="amount"></param>
    public void SetFinalPower(int amount){
        finalPower = amount;
    }

    /// <summary>
    /// Modifie la puissance de l'unité d'un certain pourcentage
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyPower(float amount){
        SetFinalPower(Tools.Ceiling(amount * finalPower + finalPower));
        CheckFinalPower();
    }

    /// <summary>
    /// Vérifie si la puissance de l'unité est légale et la réajuste au besoin
    /// </summary>
    private void CheckFinalPower(){
        if(GetFinalPower() < 0){
            SetFinalPower(0);
        }
    }
        #endregion
        
        #region Gestion des HP

    /// <summary>
    /// Renvoie les HP totaux
    /// </summary>
    /// <returns>
    /// HP totaux de l'unité
    /// </returns>
    public int GetTotalHealth(){
        return totalHealth;
    }

    /// <summary>
    /// Renvoie les HP actuels
    /// </summary>
    /// <returns>
    /// HP actuels de l'unité
    /// </returns>
    public int GetFinalHealth(){
        return finalHealth;
    }

    /// <summary>
    /// Ajoute une quantité de HP aux HP actuels et totaux
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyBothHP(int amount){
        ModifyTotalHP(amount);
        ModifyHP(amount);
    }

    /// <summary>
    /// Ajoute une quantité de HP au nombre de HP actuels
    /// </summary>
    /// <param name="amount"></param>
    private int ModifyHP(int amount){
        finalHealth += amount;
        return CheckHP();
    }

    /// <summary>
    /// Ajoute une quantité de HP au nombre de HP totaux
    /// </summary>
    /// <param name="amount">
    /// Nombre de HP à ajouter
    /// </param>
    public void ModifyTotalHP(int amount){
        totalHealth += amount;
        CheckTotalHP();
    }

    /// <summary>
    /// Multiplie les HP actuels et les HP totaux de l'unité par un facteur donné
    /// </summary>
    /// <param name="amount"></param>
    public void MultiplyBothHP(int amount){
        MultiplyTotalHP(amount);
        MultiplyHP(amount);
    }

    /// <summary>
    /// Multiplie les HP actuels de l'unité par un facteur donné
    /// </summary>
    /// <param name="amount"></param>
    public void MultiplyHP(int amount){
        finalHealth *= amount;
        CheckHP();
    }

    /// <summary>
    /// Multiplie les HP totaux de l'unité par un facteur donné
    /// </summary>
    /// <param name="amount"></param>
    public void MultiplyTotalHP(int amount){
        totalHealth *= amount;
        CheckTotalHP();
        ModifyTotalHP(amount);
    }

    /// <summary>
    /// Fixe les HP actuels de l'unité à une valeur donnée
    /// </summary>
    /// <param name="HP"></param>
    /// <param name="check"></param>
    public void SetHP(int HP, bool check = true){
        finalHealth = HP;
        if(check){
            CheckHP();
        }
    }

    /// <summary>
    /// Fixe les HP totaux de l'unité à une valeur donnée
    /// </summary>
    /// <param name="HP"></param>
    /// <param name="check"></param>
    public void SetTotalHP(int HP, bool check = true){
        totalHealth = HP;
        if(check){
            CheckTotalHP();
        }
    }

    /// <summary>
    /// Vérifie que les HP actuels de l'unité sont à légaux et les fixe au besoin. Applique également le statut KO de l'unité si ses HP sont inférieurs ou égaux à 0.
    /// </summary>
    public int CheckHP(){
        int difference = 0;
        if(AreHPBeyondMax()){
            difference = -(GetTotalHealth()-GetFinalHealth());
            SetHP(GetTotalHealth(), false);
        }
        if(AreHPBelowZero()){
            difference = -GetFinalHealth();
            SetHP(0, false);
            //InterfaceManager.Instance.KillLifeBar(this.lifeBar);
            Kill();
        }
        return difference;
    }

    /// <summary>
    /// Vérifie que les HP totaux de l'unité sont légaux et les fixe au besoin
    /// </summary>
    public void CheckTotalHP(){
        if(AreTotalHPBelowZero()){
            SetTotalHP(0, false);
        }
        CheckHP();
    }

    /// <summary>
    /// Renvoie True si les HP actuels de l'unité sont inférieurs à zéro
    /// </summary>
    /// <returns></returns>
    public bool AreHPBelowZero(){
        return GetFinalHealth() <= 0;
    }

    /// <summary>
    /// Renvoie True si les HP totaux de l'unité sont inférieurs à zéro
    /// </summary>
    /// <returns></returns>
    public bool AreTotalHPBelowZero(){
        return GetTotalHealth() <= 0;
    }

    /// <summary>
    /// Renvoie True si les HP actuels de l'unité dépassent ses HP totaux
    /// </summary>
    /// <returns></returns>
    public bool AreHPBeyondMax(){
        return GetFinalHealth() >= GetTotalHealth();
    }
    #endregion

        #region Gestion de l'armure

    /// <summary>
    /// Renvoie la quantité d'armure possédée par l'unité
    /// </summary>
    /// <returns></returns>
    public int GetArmor(){
        return armor;
    }

    /// <summary>
    /// Fixe l'armure de l'unité à une valeur donnée
    /// </summary>
    /// <param name="amount"></param>
    public void SetArmor(int amount){
        if(amount >= 0){
            if(amount != GetArmor()){
                ModifyArmor(GetArmor()-amount);
            }
        }
    }

    /// <summary>
    /// Ajoute une quantité d'armure donnée à l'unité
    /// </summary>
    /// <param name="amount"></param>
    public int ModifyArmor(int amount, bool animation = true){
        armor += amount;
        if(amount > 0){
            BattleEventManager.Instance.CreateArmorGainEvent(this, amount, animation);
        }
        return CheckArmor();
    }

    /// <summary>
    /// Vérifie si l'armure de l'unité est légale et la fixe au besoin.
    /// </summary>
    public int CheckArmor(){
        int armorDifference = 0;
        if(armor < 0){
            armorDifference = -armor;
            SetArmor(0);
        }
        return armorDifference;
    }

    /// <summary>
    /// Renvoie True si l'unité possède de l'armure
    /// </summary>
    /// <returns></returns>
    public bool isArmored(){
        return armor > 0;
    }

    /// <summary>
    /// Convertit une quantité donnée d'armure de l'unité en points de vie
    /// Si l'unité ne possède pas assez d'armure, la méthode ne fait rien
    /// </summary>
    /// <param name="amount"></param>
    public BattleEvent ConvertArmorIntoHP(int amount){
        if(GetArmor() >= amount){
            ModifyArmor(-amount);
            ModifyBothHP(amount);
        }
        return null;
    }
    #endregion

    /// <summary>
    /// Inflige un montant de dégats à l'unité
    /// </summary>
    /// <param name="amount"></param>
    public DamageEvent Damage(int amount){

        int finalDamage = amount - GetArmor();

        // Si l'armure ne suffit pas à tanker l'attaque
        if(finalDamage > 0){
            int armorDamages = 0;
            // Si l'unité possède de l'armure, retire l'armure
            if(GetArmor() > 0){
                armorDamages = GetArmor();
                ModifyArmor(-GetArmor());
            }
            int difference = ModifyHP(-finalDamage);
            return BattleEventManager.Instance.CreateDamageEvent(this, finalDamage - difference, armorDamages);
        }
        // Si l'armure suffit à tanker l'attaque
        else{
            ModifyArmor(-amount);
            return BattleEventManager.Instance.CreateDamageEvent(this, 0, amount);
        }
    }

    /// <summary>
    /// Soigne l'unité d'un montant donné
    /// </summary>
    /// <param name="amount"></param>
    public HealEvent Heal(int amount){
        int finalAmount = amount;
        foreach (Modifier _modifier in modifiers[Heal])
        {
            finalAmount = Tools.Ceiling(_modifier.GetNewAmount(finalAmount));
        }
        int difference = ModifyHP(+finalAmount);
        return BattleEventManager.Instance.CreateHealEvent(null, this, amount - difference, true);
    }

        #region Gestion des états

    /// <summary>
    /// Fixe le nombre de tours d'étourdissement de l'unité à une valeur donnée
    /// </summary>
    /// <param name="amount"></param>
    public void SetStunTime(int amount){
        stunTime = amount;
        CheckStun();
    }

    /// <summary>
    /// Ajoute un certain nombre de tours d'étourdissement à l'unité
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyStunTime(int amount){
        stunTime += amount;
        CheckStun();
    }

    /// <summary>
    /// Vérifie si l'unité est étourdie et ajuste la variable stun en fonction
    /// </summary>
    public void CheckStun(){
        if (stunTime > 0){
            stun = true;
        }
        else{
            stun = false;
        }
    }

    /// <summary>
    /// Supprime l'état négatif passé en paramètre
    /// </summary>
    /// <param name="status"></param>
    public void Cleanse(Status status){
        switch (status)
        {
            case Status.Stun:
                SetStunTime(0);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Supprime la liste d'états négatifs passés en paramètre
    /// </summary>
    /// <param name="statusList"></param>
    public void Cleanse(List<Status> statusList){

        foreach (Status status in statusList)
        {
            Cleanse(status);
        }
    }

    /// <summary>
    /// Supprime tous les états négatifs possibles
    /// </summary>
    public void Cleanse(){
        Cleanse(SpellManager.Instance.GetCleansableStatus());
    }

    public void ModifyCuratifCount(int amount){
        curatifCount += amount;
    }

    public int GetCuratifCount(){
        return curatifCount;
    }

    /// <summary>
    /// A utiliser pour ajouter l'état dead.
    /// </summary>
    public void Kill(){
        dead = true;
        BattleEventManager.Instance.ApplyDeathEvent(BattleEventManager.Instance.CreateDeathEvent(this, GetTile()));
        UnitManager.Instance.Kill(this);
    }
        #endregion

    public void CheckAssertions(){
        Assert.AreEqual(GetFinalHealth().ToString() + " HP", lifeBar.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text);
        Assert.AreEqual(GetArmor().ToString() + " AR", lifeBar.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text);
    }

    #endregion

}