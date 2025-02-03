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

    private List<BattleEvent> animationQueue;
    
    [SerializeField]
    int delayTime = 300;
    [SerializeField]
    float accelerator = 1f;

    void Awake(){
        animationQueue = new List<BattleEvent>();
        Instance = this;
    }

    void Start(){
        DamageSection = InterfaceManager.Instance.GetUI().transform.Find("Damages").transform;
        damageText = DamageSection.Find("DamagesText").GetComponent<TextMeshProUGUI>();
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
                BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERTURNSTATE, BattleManager.Trigger.FORWARD);
            }
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

    public async Task Animate(CastEvent castEvent){
        for (float distance = 0.0f; distance <= 0.4f; distance += 0.02f * accelerator)
        {
            castEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(0, 0.02f, 0));
            await Task.Yield();
        }
        for (float distance = 0.4f; distance >= 0.0f; distance -= 0.02f * accelerator)
        {
            castEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(0, -0.02f, 0));
            await Task.Yield();
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
            InterfaceManager.Instance.UpdateLifebar(damageEvent.GetTargetUnit(), 0, -damage);
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
            InterfaceManager.Instance.UpdateLifebar(damageEvent.GetTargetUnit(), -damage, 0);
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
        InterfaceManager.Instance.UpdateLifebar(armorGainEvent.GetTargetUnit(), 0, armorGainEvent.GetAmount());
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
        InterfaceManager.Instance.UpdateLifebar(healEvent.GetTargetUnit(), healEvent.GetAmount(), 0);
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
        deathEvent.GetDeadUnit().lifeBar.gameObject.SetActive(false);
        await Task.Yield();
    }


    private async Task Animate(BattleEvent battleEvent){
        if (battleEvent is CastEvent){
            await Animate((CastEvent)battleEvent);
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
    }

    public void addAnimation(BattleEvent battleEvent){
        animationQueue.Add(battleEvent);
    }
}
