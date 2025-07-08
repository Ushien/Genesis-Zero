using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// Contient toutes les méthodes relatives à l'animation
/// </summary>

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    public TextMeshProUGUI damageText;
    public Transform DamageSection;
    public float damageZoomAmount = 2.5f;
    public float damageZoomSpeed = 0.5f;
    public float zoomDamping = 2f;
    public float animShakeStrength = 0.3f;
    public float animShakeTime = 0.3f;

    private List<BattleEvent> animationQueue;
    
    [SerializeField]
    int delayTime = 300;
    [SerializeField]
    float accelerator = 1f;

    void Awake(){
        animationQueue = new List<BattleEvent>();
        Instance = this;
        DamageSection = InterfaceManager.Instance.GetUI().transform.Find("Damages").transform;
        damageText = DamageSection.Find("DamagesText").GetComponent<TextMeshProUGUI>();
    }

    void Start(){
        
    }

    public void BattleIn(){
        DamageSection.gameObject.SetActive(true);
    }

    public void BattleOut(){
        DamageSection.gameObject.SetActive(false);
    }

    void Update(){
        if (BattleManager.Instance.GetTurnState() == BattleManager.TurnState.ANIMATION){
            if (animationQueue.Count != 0){
                if(!BattleManager.Instance.IsInAnimation()){
                    //Si la file n'est pas vide, animer le premier évènement, en mode fifo
                    BattleManager.Instance.SetInAnimation(true);
                    List<BattleEvent> listWrapper = new()
                    {
                        animationQueue[0]
                    };
                    animationQueue.RemoveAt(0);
                    var task = Animate(listWrapper);
                }
            }

            else{
                BattleManager.Instance.ChangeState(BattleManager.Machine.TURNSTATE, BattleManager.Trigger.FORWARD);
            }
        }
    }

    public void ForceAnimation(){
        //Debug.Log("Queue count: " + animationQueue.Count);
        while(animationQueue.Count != 0){
            //Si la file n'est pas vide, animer le premier évènement, en mode fifo
            BattleManager.Instance.SetInAnimation(true);
            List<BattleEvent> listWrapper = new()
            {
                animationQueue[0]
            };
            animationQueue.RemoveAt(0);
            var task = Animate(listWrapper);
        }
    }

    public async Task Animate(List<BattleEvent> battleEvents){
        for (var i = 0; i < battleEvents.Count; i++)
        {
            await Animate(battleEvents[i]);
            await Task.Delay((int)(delayTime / accelerator));
        }
        BattleManager.Instance.SetInAnimation(false);
    }

    public async Task Animate(BeforeCastEvent beforecastEvent){
        if(beforecastEvent.GetCastedSpell().IsAnAttack()){
            float distanceValue = beforecastEvent.GetSourceUnit().GetTeam() == Team.Ally ? 0.02f : -0.02f;

            for (float distance = 0.0f; distance <= 0.4f; distance += 0.02f * accelerator)
            {
                beforecastEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(distanceValue, 0, 0));
                await Task.Yield();
            }
            for (float distance = 0.4f; distance >= 0.0f; distance -= 0.02f * accelerator)
            {
                beforecastEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(-distanceValue, 0, 0));
                await Task.Yield();
            }
        }
        else{
            for (float distance = 0.0f; distance <= 0.4f; distance += 0.02f * accelerator)
            {
                beforecastEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(0, 0.02f, 0));
                await Task.Yield();
            }
            for (float distance = 0.4f; distance >= 0.0f; distance -= 0.02f * accelerator)
            {
                beforecastEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(0, -0.02f, 0));
                await Task.Yield();
            }
        }
    }

    private async Task Animate(DamageEvent damageEvent){

        // Animer les dégats d'armure
        int damage = damageEvent.GetArmorAmount();
        if(damage > 0){
            TextMeshProUGUI damageDisplay = Instantiate(damageText);
            damageDisplay.transform.SetParent(DamageSection);
            damageDisplay.text = "-" + damage.ToString();
            damageDisplay.transform.position = damageEvent.GetTargetUnit().transform.position;
            damageDisplay.transform.localScale = new Vector3(1, 1, 1);
            damageDisplay.gameObject.SetActive(true);
            InterfaceManager.Instance.UpdateLifebar(damageEvent.GetTargetUnit(), 0, 0, -damage);
            for (float distance = 0.0f; distance <= 0.5f; distance += 0.005f * accelerator)
            {
                damageDisplay.gameObject.transform.Translate(new Vector3(0, 0.005f, 0));
                await Task.Yield();
            }
            damageDisplay.gameObject.SetActive(false);
            Destroy(damageDisplay.gameObject);
        }

        // Animer les dégats aux HP
        damage = damageEvent.GetHealthAmount();
        if(damage > 0){
            TextMeshProUGUI damageDisplay = Instantiate(damageText);
            damageDisplay.transform.SetParent(DamageSection);
            damageDisplay.text = "-" + damage.ToString();
            damageDisplay.transform.position = damageEvent.GetTargetUnit().transform.position;
            damageDisplay.transform.localScale = new Vector3(1, 1, 1);
            damageDisplay.gameObject.SetActive(true);
            InterfaceManager.Instance.UpdateLifebar(damageEvent.GetTargetUnit(), -damage, 0, 0);

            // Camera effects
            // --------------
            CameraEffects.Instance.TriggerZoom(new Vector3(InterfaceManager.Instance.mainCamera.transform.position.x + (damageEvent.GetTargetUnit().transform.position.x - InterfaceManager.Instance.mainCamera.transform.position.x)/zoomDamping,
                                                           InterfaceManager.Instance.mainCamera.transform.position.y + (damageEvent.GetTargetUnit().transform.position.y - InterfaceManager.Instance.mainCamera.transform.position.y)/zoomDamping, -10f),
                                                           damageZoomAmount, damageZoomSpeed, true);
            CameraEffects.Instance.TriggerShake(animShakeStrength, animShakeTime);

            for (float distance = 0.0f; distance <= 0.5f; distance += 0.005f * accelerator)
            {
                damageDisplay.gameObject.transform.Translate(new Vector3(0, 0.005f, 0));
                await Task.Yield();
            }
            damageDisplay.gameObject.SetActive(false);
            Destroy(damageDisplay.gameObject);
        }

    }

    private async Task Animate(ArmorGainEvent armorGainEvent){
        TextMeshProUGUI armorGainDisplay = Instantiate(damageText);
        armorGainDisplay.transform.SetParent(DamageSection);
        armorGainDisplay.text = "+" + armorGainEvent.GetAmount().ToString();
        armorGainDisplay.transform.position = armorGainEvent.GetTargetUnit().transform.position;
        armorGainDisplay.transform.localScale = new Vector3(1, 1, 1);
        armorGainDisplay.gameObject.SetActive(true);
        InterfaceManager.Instance.UpdateLifebar(armorGainEvent.GetTargetUnit(), 0, 0, armorGainEvent.GetAmount());
        for (float distance = 0.0f; distance <= 0.5f; distance += 0.005f * accelerator)
        {
            armorGainDisplay.gameObject.transform.Translate(new Vector3(0, 0.005f, 0));
            await Task.Yield();
        }
        armorGainDisplay.gameObject.SetActive(false);
        Destroy(armorGainDisplay.gameObject);
    }

    private async Task Animate(HealEvent healEvent){
        TextMeshProUGUI armorGainDisplay = Instantiate(damageText);
        armorGainDisplay.transform.SetParent(DamageSection);
        armorGainDisplay.text = "+" + healEvent.GetAmount().ToString();
        armorGainDisplay.transform.position = healEvent.GetTargetUnit().transform.position;
        armorGainDisplay.transform.localScale = new Vector3(1, 1, 1);
        armorGainDisplay.gameObject.SetActive(true);
        InterfaceManager.Instance.UpdateLifebar(healEvent.GetTargetUnit(), healEvent.GetAmount(), 0, 0);
        for (float distance = 0.0f; distance <= 0.5f; distance += 0.005f * accelerator)
        {
            armorGainDisplay.gameObject.transform.Translate(new Vector3(0, 0.005f, 0));
            await Task.Yield();
        }
        armorGainDisplay.gameObject.SetActive(false);
        Destroy(armorGainDisplay.gameObject);
    }

    private async Task Animate(DeathEvent deathEvent){
        deathEvent.GetDeadUnit().gameObject.SetActive(false);
        deathEvent.GetDeadUnit().lifeBar.SetHP(0);
        deathEvent.GetDeadUnit().lifeBar.gameObject.SetActive(false);
        await Task.Yield();
    }

    private async Task Animate(HPModificationEvent hpModificationEvent){
        if(hpModificationEvent.AreTotalHP()){
            hpModificationEvent.GetTargetUnit().lifeBar.SetTotalHP(hpModificationEvent.GetNewAmount());
        }
        else{
            hpModificationEvent.GetTargetUnit().lifeBar.SetHP(hpModificationEvent.GetNewAmount());
        }
        await Task.Yield();
    }

    private async Task Animate(ReviveEvent reviveEvent){
        reviveEvent.GetRevivedUnit().lifeBar.SetHP(reviveEvent.GetHPAmount());
        await Task.Yield();
    }

    private async Task Animate(SummonEvent summonEvent){
        // Afficher l'unité
        summonEvent.GetSummonedUnit().gameObject.SetActive(true);
        await Task.Yield();
    }


    private async Task Animate(BattleEvent battleEvent){
        //Debug.Log(battleEvent.GetSummary());
        if (battleEvent is BeforeCastEvent){
            await Animate((BeforeCastEvent)battleEvent);
        }
        if (battleEvent is DamageEvent){
            await Animate((DamageEvent)battleEvent);
        }
        if (battleEvent is ArmorGainEvent){
            await Animate((ArmorGainEvent)battleEvent);
        }
        if (battleEvent is HealEvent){
            await Animate((HealEvent)battleEvent);
        }
        if (battleEvent is DeathEvent){
            await Animate((DeathEvent)battleEvent);
        }
        if (battleEvent is HPModificationEvent){
            await Animate((HPModificationEvent)battleEvent);
        }
        if (battleEvent is ReviveEvent){
            await Animate((ReviveEvent)battleEvent);
        }
        if (battleEvent is SummonEvent){
            await Animate((SummonEvent)battleEvent);
        }
    }

    public void addAnimation(BattleEvent battleEvent){
        //Debug.Log(battleEvent.GetSummary());
        animationQueue.Add(battleEvent);
    }

    [ContextMenu("Display queue count")]
    public void DisplayAnimationQueueCount(){
        Debug.Log(animationQueue.Count);
    }
}
