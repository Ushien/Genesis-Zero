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

    void Awake(){
        animationQueue = new List<BattleEvent>();
        Instance = this;
    }

    void Start(){
        DamageSection = InterfaceManager.Instance.GetUI().transform.Find("Damages").transform;
        damageText = DamageSection.Find("DamagesText").GetComponent<TextMeshProUGUI>();
    }

    void Update(){
        if (!BattleManager.Instance.IsInAnimation() && animationQueue.Count != 0){
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
            await Task.Delay(500);
        }
        BattleManager.Instance.SetInAnimation(false);
    }

    public async Task Animate(CastEvent castEvent){
        for (float distance = 0.0f; distance <= 0.4f; distance += 0.02f)
        {
            castEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(0, 0.02f, 0));
            await Task.Yield();
        }
        for (float distance = 0.4f; distance >= 0.0f; distance -= 0.02f)
        {
            castEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(0, -0.02f, 0));
            await Task.Yield();
        }
    }

    private async Task Animate(DamageEvent damageEvent){
        TextMeshProUGUI damageDisplay = Instantiate(damageText);
        damageDisplay.transform.SetParent(DamageSection);
        damageDisplay.text = "-" + damageEvent.GetAmount().ToString();
        damageDisplay.transform.position = damageEvent.GetTargetUnit().transform.position;
        damageDisplay.transform.localScale = new Vector3(1, 1, 1);
        damageDisplay.gameObject.SetActive(true);
        InterfaceManager.Instance.UpdateLifebar(damageEvent.GetTargetUnit());
        for (float distance = 0.0f; distance <= 0.5f; distance += 0.005f)
        {
            damageDisplay.gameObject.transform.Translate(new Vector3(0, 0.005f, 0));
            await Task.Yield();
        }
        damageDisplay.gameObject.SetActive(false);
        Destroy(damageDisplay.gameObject);
    }

    private async Task Animate(ArmorGainEvent armorGainEvent){
        TextMeshProUGUI armorGainDisplay = Instantiate(damageText);
        armorGainDisplay.transform.SetParent(DamageSection);
        armorGainDisplay.text = "+" + armorGainEvent.GetAmount().ToString();
        armorGainDisplay.transform.position = armorGainEvent.GetTargetUnit().transform.position;
        armorGainDisplay.transform.localScale = new Vector3(1, 1, 1);
        armorGainDisplay.gameObject.SetActive(true);
        InterfaceManager.Instance.UpdateLifebar(armorGainEvent.GetTargetUnit());
        for (float distance = 0.0f; distance <= 0.5f; distance += 0.005f)
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
        InterfaceManager.Instance.UpdateLifebar(healEvent.GetTargetUnit());
        for (float distance = 0.0f; distance <= 0.5f; distance += 0.005f)
        {
            armorGainDisplay.gameObject.transform.Translate(new Vector3(0, 0.005f, 0));
            await Task.Yield();
        }
        armorGainDisplay.gameObject.SetActive(false);
        Destroy(armorGainDisplay.gameObject);
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
    }

    public void addAnimation(BattleEvent battleEvent){
        animationQueue.Add(battleEvent);
    }
}
