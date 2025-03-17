using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// petite fonction utile pour désactiver un gameobject à la fin d'une anim
public class AnimEndKill : MonoBehaviour
{
        public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}

