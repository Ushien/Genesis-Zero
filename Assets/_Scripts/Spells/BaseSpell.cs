using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Assertions;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

/// <summary>
/// Représente une attaque ou une technique
/// </summary>
public class BaseSpell : MonoBehaviour
{

        #region Fields

        #region Fields de setup
    [SerializeField]
    private ScriptableSpell scriptableSpell;

        #endregion
        #region Références à d'autres objets
    private BaseUnit owner;
    #endregion

    #region Caractérstiques

    private int unique_id;
    private bool isAAttack = false;
    private bool isAnAttack = false;
    private bool isATechnique = true;
    public int base_cooldown = 0;
    // Cooldown actuel du sort. Lorsque celui-ci est égal au cooldown total, le sort peut être lancé. Quand le sort est lancé, celui-ci passe à zéro. Il augmente ensuite de 1 par tour.
    public int cooldown = 0;
    private bool activated = false;
    private Sprite artwork = null;
    // Indique la portée du sort (horizontale, verticale, toutes les unités, etc...)
    private GridManager.Selection_mode range;
    // Indique si un sort ne peut être lancé que sur une équipe en particulier
    private GridManager.Team_restriction team_restriction;

    /// Ratio associés au sort, doivent être définis au sein de la fonction Awake propre au sort.
    private float ratio1 = 1f;
    private float ratio2 = 1f;
    private float ratio3 = 1f;

    private float h_ratio1 = 1f;
    private float h_ratio2 = 1f;
    private float h_ratio3 = 1f;

    private int index = 0;

        #endregion

        #region Fields relatifs au moteur de jeu

    [SerializeField]
    public Modifier baseModifier;
    private List<Modifier> modifiers = new List<Modifier>();
        #endregion
        #endregion

    ////////////////////////////////////////////////////////

        #region Méthodes d'initialisation

    /// <summary>
    /// Initialise le sort
    /// </summary>
    /// <param name="ownerUnit">Unité possédant le sort</param>
    public void Setup(ScriptableSpell scriptableSpell, BaseUnit ownerUnit, int spellListIndex){
        Assert.IsTrue(spellListIndex >= 0 || spellListIndex <= 4);

        owner = ownerUnit;

        unique_id = scriptableSpell.id;
        name = scriptableSpell.name;
        base_cooldown = scriptableSpell.cooldown;
        cooldown = base_cooldown;
        artwork = scriptableSpell.artwork;
        range = scriptableSpell.range;
        team_restriction = scriptableSpell.team_restriction;
        isAnAttack = scriptableSpell.isAnAttack;
        // Si c'est une attaque de base
        if(spellListIndex == 0){
            ownerUnit.aAttack = this;
            base_cooldown = 0;
            isAAttack = true;
            isATechnique = false;
            index = 0;
        }
        // Si c'est une technique
        else{
            isAAttack = false;
            isATechnique = true;
            ownerUnit.GetFourSpells()[spellListIndex-1] = this;
            index = spellListIndex;
        }

        ratio1 = scriptableSpell.ratios[0];
        ratio2 = scriptableSpell.ratios[1];
        ratio3 = scriptableSpell.ratios[2];

        h_ratio1 = scriptableSpell.hyper_ratios[0];
        h_ratio2 = scriptableSpell.hyper_ratios[1];
        h_ratio3 = scriptableSpell.hyper_ratios[2];

        Activate();
        activated = true;
    }

    virtual public void EndBattle(){
        //SetCooldown(GetBaseCooldown());
    }

    virtual public void Activate(){
        //
    }

    public bool IsActivated(){
        return activated;
    }

    public void Activate(bool activation){
        activated = activation;
    }

    public int GetIndex(){
        return index;
    }
        #endregion
        #region Actions du sort
    /// <summary>
    /// Lance le sort. Méthode à overrider pour intégrer l'effet du sort que l'on souhaite implémenter.
    /// </summary>
    /// 
    virtual public void Cast(List<Properties> properties){
        Debug.Log("Méthode overridée");
    }

    /// <summary>
    /// Lance un sort en version surchargée. Méthode à overrider pour intégrer l'effet du sort que l'on souhaite implémenter.
    /// </summary>
    virtual public void HyperCast(List<Properties> properties){
        Debug.Log("Méthode overridée");
    }

    /// <summary>
    /// Lance le sort. Méthode à overrider pour intégrer l'effet du sort que l'on souhaite implémenter.
    /// </summary>
    virtual public void Cast(Tile targetTile, List<Properties> properties){
        Debug.Log("Méthode overridée");
    }

    /// <summary>
    /// /// Lance un sort en version surchargée. Méthode à overrider pour intégrer l'effet du sort que l'on souhaite implémenter.
    /// </summary>
    /// <param name="targetTile"></param>
    virtual public void HyperCast(Tile targetTile, List<Properties> properties){
        // Méthode overridée

        // Si le sort ne propose pas d'implémentation de la surcharge, lance juste la version de base.
        Debug.Log("Méthode overridée");
    }

    /// <summary>
    /// Méthode à appeler dans l'implémentation du sort, qui s'occupe de toutes les vérifications communes à chaque sort.
    /// </summary>
    /// <param name="targetTile"></param>
    /// <param name="spellFunction"></param>
    virtual public void CastSpell(Tile targetTile, List<Properties> properties, Action<Tile, List<Properties>> spellFunction, bool hyper = false){
        BaseUnit targetUnit = null;
        if (targetTile != null){
            targetUnit = targetTile.GetUnit();
        }

        if(targetUnit != null && IsAvailable() && GetOwner().IsAvailable()){

            BeforeCastEvent beforeCastEvent = BattleEventManager.Instance.CreateBeforeCastEvent(owner, this, targetTile);
            BattleEventManager.Instance.ApplyBeforeCastEvent(beforeCastEvent);

            SetCooldown(0);

            spellFunction(targetTile, properties);

            AfterCastEvent afterCastEvent = BattleEventManager.Instance.CreateAfterCastEvent(owner, this, targetTile);
            BattleEventManager.Instance.ApplyAfterCastEvent(afterCastEvent, true);
            if(hyper){
                // Détacher la technique de son propriétaire (comme si elle était supprimée)
                owner.RemoveSpell(this);
            }

        }
    }
        #endregion

        #region Manipulation des caractéristiques du sort

    /// <summary>
    /// Renvoie l'unité en possession du sort
    /// </summary>
    /// <returns></returns>
    public BaseUnit GetOwner(){
        return owner;
    }

    /// <summary>
    /// Définit un nouveau propriétaire du sort
    /// </summary>
    /// <param name="new_owner"></param>
    public void SetOwner(BaseUnit new_owner){
        owner = new_owner;
    }

    public ScriptableSpell GetScriptableSpell(){
        return scriptableSpell;
    }

    /// <summary>
    /// Renvoie la portée du sort
    /// </summary>
    /// <returns></returns>
    public GridManager.Selection_mode GetRange(){
        return range;
    }

    public Sprite GetArtwork(){
        return artwork;
    }
    
    /// <summary>
    /// Renvoie les différents ratios associés au sort
    /// </summary>
    /// <returns></returns>
    virtual public List<float> GetRatio(bool hyper = false){
        if(hyper){
            return new List<float>{
                h_ratio1,
                h_ratio2,
                h_ratio3
            }; 
        }
        else{
            return new List<float>{
                ratio1,
                ratio2,
                ratio3
            };
        }
    }

    /// <summary>
    /// Fixe un ratio au choix à une valeur
    /// </summary>
    /// <param name="index">Index du ratio (1, 2 ou 3)</param>
    /// <param name="newNumber">Nouvelle valeur du ratio</param>
    public void SetRatio(int index, float newNumber, bool hyper = false){
        if(index == 1){
            if(hyper){
                h_ratio1 = newNumber;
            }
            else{
                ratio1 = newNumber;
            }
        }
        if(index == 2){
            if(hyper){
                h_ratio2 = newNumber;
            }
            else{
                ratio2 = newNumber;
            }
        }
        if(index == 3){
            if(hyper){
                h_ratio2 = newNumber;
            }
            else{
                ratio2 = newNumber;
            }
        }
    }

        #endregion

        #region Gestion des cooldowns

    /// <summary>
    /// Renvoie le nom du sort
    /// </summary>
    /// <returns></returns>
    public string GetName(){
        return LocalizationSettings.StringDatabase.GetLocalizedString("Spells", unique_id.ToString() + "_name");
    }
    
    /// <summary>
    /// Renvoie la description en combat du sort, avec les nombres convertis en fonction de la puissance du personnage.
    /// </summary>
    /// <returns></returns>
    public string GetDescription(bool hyper = false){
        string _fight_description;
        if (hyper)
        {
            _fight_description = LocalizationSettings.StringDatabase.GetLocalizedString("Spells", unique_id.ToString() + "_effect_h");
            // Si l'effet de la technique surchargée est similaire à la technique classique, elle est notée "//"
            if (_fight_description == "//")
            {
                _fight_description = LocalizationSettings.StringDatabase.GetLocalizedString("Spells", unique_id.ToString() + "_effect");
            }
        }
        else
        {
            _fight_description = LocalizationSettings.StringDatabase.GetLocalizedString("Spells", unique_id.ToString() + "_effect");
        }

        _fight_description = _fight_description.Replace("%%1", GetFinalDamages(GetRatio(hyper:hyper)[0]).ToString());
        _fight_description = _fight_description.Replace("%%2", GetFinalDamages(GetRatio(hyper:hyper)[1]).ToString());
        _fight_description = _fight_description.Replace("%%3", GetFinalDamages(GetRatio(hyper:hyper)[2]).ToString());

        _fight_description = _fight_description.Replace("__1", DisplayPercents(GetRatio(hyper:hyper)[0]));
        _fight_description = _fight_description.Replace("__2", DisplayPercents(GetRatio(hyper:hyper)[1]));
        _fight_description = _fight_description.Replace("__3", DisplayPercents(GetRatio(hyper:hyper)[2]));

        _fight_description = _fight_description.Replace("**1", GetRatio(hyper:hyper)[0].ToString());
        _fight_description = _fight_description.Replace("**2", GetRatio(hyper:hyper)[1].ToString());
        _fight_description = _fight_description.Replace("**3", GetRatio(hyper:hyper)[2].ToString());

        return _fight_description;
    }

    /// <summary>
    /// Augmente le cooldown actuel d'un nombre donné
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyCooldown(int amount)
    {
        cooldown += amount;
        CheckCooldown();
    }
    
    /// <summary>
    /// Vérifie si les cooldowns sont légaux et les ajuste au besoin
    /// </summary>
    private void CheckCooldown(){
        if(cooldown > base_cooldown){
            cooldown = base_cooldown;
        }
        if(cooldown < 0){
            cooldown = 0;
        }
    }

    /// <summary>
    /// Fixe le cooldown actuel du sort à un nombre donné
    /// </summary>
    /// <param name="amount"></param>
    public void SetCooldown(int amount){
        cooldown = amount;
    }

    /// <summary>
    /// Renvoie le cooldown actuel du sort
    /// </summary>
    /// <returns></returns>
    public int GetCooldown(){
        return cooldown;
    }

    /// <summary>
    /// Renvoie le cooldown total du sort
    /// </summary>
    /// <returns></returns>
    public int GetBaseCooldown(){
        return base_cooldown;
    }
        #endregion

        #region Methodes relatives au moteur de jeu
    /// <summary>
    /// Indique si le sort est prêt à être utilisé ou pas, en respectant les différentes contraintes définies
    /// </summary>
    /// <returns></returns>
    public bool IsAvailable(){
        bool availability = true;
        if(GetCooldown() < GetBaseCooldown()){
            availability = false;
        }
        return availability;
    }

    /// <summary>
    /// Indique si le sort est une technique ou pas
    /// </summary>
    /// <returns></returns>
    public bool IsATechnique(){
        return isATechnique;
    }

    /// <summary>
    /// Définit si le sort est une technique ou non
    /// </summary>
    /// <param name="value"></param>
    public void SetIsATechnique(bool value){
        isATechnique = value;
    }

    public bool IsAnAttack(){
        return isAnAttack;
    }

    public bool IsAAttack(){
        return isAAttack;
    }

    /// <summary>
    /// Applique tous les effets de fin de tour liés au sort
    /// </summary>
    public void ApplyEndTurnEffects(){
        ModifierEndTurn();
    }

    public void ApplyStartTurnEffects(){
        ModifyCooldown(+1);
    }
        #endregion

        #region Gestion de types et conversions
    /// <summary>
    /// Convertit un ratio donné avec la puissance de l'unité et renvoie la quantité finale de dégats
    /// </summary>
    /// <param name="_ratio"></param>
    /// <returns></returns>
    virtual public int GetFinalDamages(float _ratio){
        int finalAmount = Tools.Ceiling(_ratio * GetOwner().GetFinalPower());
        
        foreach (Modifier _modifier in modifiers)
        {
            finalAmount = Tools.Ceiling(_modifier.GetNewAmount(finalAmount));
        }

        return finalAmount;
    }

    /// <summary>
    /// Dépréciée
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public float ApplyPower(float ratio){
        return ratio * GetOwner().GetFinalPower();
    }

    /// <summary>
    /// Convertit un ratio du décimal vers les pourcentages. (0.2 -> 20%)
    /// </summary>
    /// <param name="percentRatio"></param>
    /// <returns></returns>
    public string DisplayPercents(float percentRatio){
        return (percentRatio * 100).ToString();
    }
        #endregion

        #region Gestion des modificateurs

    /// <summary>
    /// Ajoute un modificateur sur le sort
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(Modifier modifier){
        modifiers.Add(modifier);
    }

    /// <summary>
    /// Supprime un modificateur sur le sort
    /// </summary>
    /// <param name="modifier"></param>
    private void DeleteModifier(Modifier modifier){
        modifiers.Remove(modifier);
    }


    /// <summary>
    ///  Diminue la durée restante des modificateurs de 1 et les supprime si celle-ci vaut 0
    /// </summary>
    private void ModifierEndTurn(){
        if(modifiers.Count > 0){
        }
        foreach (Modifier modifier in modifiers)
        {
            modifier.ReduceTurns();
            if(modifier.IsEnded()){
                //FIXME les listes aiment pas beaucoup ça
                DeleteModifier(modifier);
            }
        }
    }
        #endregion
}
