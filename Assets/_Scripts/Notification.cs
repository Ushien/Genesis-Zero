using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Notification : MonoBehaviour
{
    [SerializeField] float displayTime = 2f;
    [SerializeField] float fadeDuration = 0.5f;
    void Start()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        // Apparition depuis alpha 0
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.one * 0.8f; // effet pop-in

        // Sequence DOTween
        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1, fadeDuration))       // Fade in
           .Join(transform.DOScale(1f, fadeDuration))         // Scale up
           .AppendInterval(displayTime)                       // Pause
           .Append(canvasGroup.DOFade(0, fadeDuration))       // Fade out
           .Join(transform.DOScale(0.8f, fadeDuration))       // Scale down
           .OnComplete(() => Destroy(gameObject));            // Détruire à la fin
    }
}
