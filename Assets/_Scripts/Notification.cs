using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Notification : MonoBehaviour
{
    void Start()
    {
        ShowToast();
    }

    void ShowToast()
    {
        Sequence s = DOTween.Sequence();
        /*
        s.Append(toastCanvas.DOFade(1, 0.5f))      // Apparition
         .AppendInterval(2f)                       // Attente
         .Append(toastCanvas.DOFade(0, 0.5f))      // Disparition
         .OnComplete(() => toastCanvas.gameObject.SetActive(false));
        */
    }
}
