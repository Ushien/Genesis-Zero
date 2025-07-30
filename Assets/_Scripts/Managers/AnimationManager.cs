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
    public GameObject damageText;
    public Transform DamageSection;
    public float damageZoomAmount = 2.5f;
    public float damageZoomSpeed = 0.5f;
    public float zoomDamping = 2f;
    public float animShakeStrength = 0.3f;
    public float animShakeTime = 0.3f;

    private List<BattleEvent> animationQueue;

    private bool animationLocked = false;

    void Awake()
    {
        animationQueue = new List<BattleEvent>();
        Instance = this;
        DamageSection = InterfaceManager.Instance.GetUI().transform.Find("Damages").transform;
        damageText = DamageSection.Find("DamageEncapsulator").gameObject;
    }

    public void BattleIn()
    {
        DamageSection.gameObject.SetActive(true);
    }

    public void BattleOut()
    {
        DamageSection.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (BattleManager.Instance.GetTurnState() == BattleManager.TurnState.ANIMATION && !animationLocked)
        {
            if (animationQueue.Count != 0)
            {
                //Si la file n'est pas vide, animer le premier évènement, en mode fifo
                BattleEvent toAnimate = animationQueue[0];
                animationQueue.RemoveAt(0);
                Animate(toAnimate);
            }

            else
            {
                BattleManager.Instance.ChangeState(BattleManager.Machine.TURNSTATE, BattleManager.Trigger.FORWARD);
            }
        }
    }

    public void ForceAnimation()
    {
        while (animationQueue.Count != 0)
        {
            //Si la file n'est pas vide, animer le premier évènement, en mode fifo
            BattleEvent toAnimate = animationQueue[0];
            animationQueue.RemoveAt(0);
            Animate(toAnimate);
        }
    }

    private IEnumerator Animate(BeforeCastEvent beforecastEvent)
    {
        beforecastEvent.GetSourceUnit().GetComponent<Animator>().Play("castSpellAnimation");
        yield return null;
    }

    private IEnumerator Animate(DamageEvent damageEvent)
    {
        // Animer les dégats d'armure
        int damage = damageEvent.GetArmorAmount();
        if (damage > 0)
        {
            GameObject damageDisplay = Instantiate(damageText);
            damageDisplay.transform.SetParent(DamageSection);
            damageDisplay.transform.Find("DamagesText").GetComponent<TextMeshProUGUI>().text = "-" + damage.ToString();
            damageDisplay.transform.localScale = new Vector3(1, 1, 1);
            damageDisplay.transform.position = damageEvent.GetTargetUnit().transform.position;
            damageDisplay.gameObject.SetActive(true);
            InterfaceManager.Instance.UpdateLifebar(damageEvent.GetTargetUnit(), 0, 0, -damage);
            yield return null;
        }

        // Animer les dégats aux HP
        damage = damageEvent.GetHealthAmount();
        if (damage > 0)
        {
            GameObject damageDisplay = Instantiate(damageText);
            damageDisplay.transform.SetParent(DamageSection);
            damageDisplay.transform.Find("DamagesText").GetComponent<TextMeshProUGUI>().text = "-" + damage.ToString();
            damageDisplay.transform.localScale = new Vector3(1, 1, 1);
            damageDisplay.transform.position = damageEvent.GetTargetUnit().transform.position;
            damageDisplay.gameObject.SetActive(true);
            InterfaceManager.Instance.UpdateLifebar(damageEvent.GetTargetUnit(), -damage, 0, 0);

            // Camera effects
            // --------------
            CameraEffects.Instance.TriggerZoom(new Vector3(InterfaceManager.Instance.mainCamera.transform.position.x + (damageEvent.GetTargetUnit().transform.position.x - InterfaceManager.Instance.mainCamera.transform.position.x) / zoomDamping,
                                                           InterfaceManager.Instance.mainCamera.transform.position.y + (damageEvent.GetTargetUnit().transform.position.y - InterfaceManager.Instance.mainCamera.transform.position.y) / zoomDamping, -10f),
                                                           damageZoomAmount, damageZoomSpeed, true);
            CameraEffects.Instance.TriggerShake(animShakeStrength, animShakeTime);
            yield return null;
        }

    }

    private IEnumerator Animate(ArmorGainEvent armorGainEvent)
    {
        GameObject armorGainDisplay = Instantiate(damageText);
        armorGainDisplay.transform.SetParent(DamageSection);
        armorGainDisplay.transform.Find("DamagesText").GetComponent<TextMeshProUGUI>().text = "+" + armorGainEvent.GetAmount().ToString();
        armorGainDisplay.transform.localScale = new Vector3(1, 1, 1);
        armorGainDisplay.transform.position = armorGainEvent.GetTargetUnit().transform.position;
        armorGainDisplay.gameObject.SetActive(true);
        InterfaceManager.Instance.UpdateLifebar(armorGainEvent.GetTargetUnit(), 0, 0, armorGainEvent.GetAmount());
        yield return null;
    }

    private IEnumerator Animate(HealEvent healEvent)
    {
        GameObject armorGainDisplay = Instantiate(damageText);
        armorGainDisplay.transform.SetParent(DamageSection);
        armorGainDisplay.transform.Find("DamagesText").GetComponent<TextMeshProUGUI>().text = "+" + healEvent.GetAmount().ToString();
        armorGainDisplay.transform.localScale = new Vector3(1, 1, 1);
        armorGainDisplay.transform.position = healEvent.GetTargetUnit().transform.position;
        armorGainDisplay.gameObject.SetActive(true);
        InterfaceManager.Instance.UpdateLifebar(healEvent.GetTargetUnit(), healEvent.GetAmount(), 0, 0);
        yield return null;
    }

    private IEnumerator Animate(DeathEvent deathEvent)
    {
        deathEvent.GetDeadUnit().gameObject.SetActive(false);
        deathEvent.GetDeadUnit().lifeBar.SetHP(0);
        deathEvent.GetDeadUnit().lifeBar.gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator Animate(HPModificationEvent hpModificationEvent)
    {
        if (hpModificationEvent.AreTotalHP())
        {
            hpModificationEvent.GetTargetUnit().lifeBar.SetTotalHP(hpModificationEvent.GetNewAmount());
        }
        else
        {
            hpModificationEvent.GetTargetUnit().lifeBar.SetHP(hpModificationEvent.GetNewAmount());
        }
        yield return null;
    }

    private IEnumerator Animate(ReviveEvent reviveEvent)
    {
        reviveEvent.GetRevivedUnit().lifeBar.SetHP(reviveEvent.GetHPAmount());
        yield return null;
    }

    private IEnumerator Animate(SummonEvent summonEvent)
    {
        // Afficher l'unité
        summonEvent.GetSummonedUnit().gameObject.SetActive(true);
        yield return null;
    }


    private void Animate(BattleEvent battleEvent)
    {
        //Debug.Log(battleEvent.GetSummary());
        if (battleEvent is BeforeCastEvent)
        {
            StartCoroutine(Animate((BeforeCastEvent)battleEvent));            
        }
        if (battleEvent is DamageEvent)
        {
            StartCoroutine(Animate((DamageEvent)battleEvent));
        }
        if (battleEvent is ArmorGainEvent)
        {
            StartCoroutine(Animate((ArmorGainEvent)battleEvent));
        }
        if (battleEvent is HealEvent)
        {
            StartCoroutine(Animate((HealEvent)battleEvent));
        }
        if (battleEvent is DeathEvent)
        {
            StartCoroutine(Animate((DeathEvent)battleEvent));
        }
        if (battleEvent is HPModificationEvent)
        {
            StartCoroutine(Animate((HPModificationEvent)battleEvent));
        }
        if (battleEvent is ReviveEvent)
        {
            StartCoroutine(Animate((ReviveEvent)battleEvent));
        }
        if (battleEvent is SummonEvent)
        {
            StartCoroutine(Animate((SummonEvent)battleEvent));
        }
    }

    public void AddAnimation(BattleEvent battleEvent)
    {
        //Debug.Log(battleEvent.GetSummary());
        animationQueue.Add(battleEvent);
    }

    [ContextMenu("Display queue count")]
    public void DisplayAnimationQueueCount()
    {
        Debug.Log(animationQueue.Count);
    }

    public void LockAnimation()
    {
        animationLocked = true;
    }

    public void DelockAnimation()
    {
        animationLocked = false;
    }
}
