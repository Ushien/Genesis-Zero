using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    protected BaseUnit owner;
    protected Sprite artwork;

    /// <summary>
    /// Renvoie l'unité en possession de l'upgrade
    /// </summary>
    /// <returns></returns>
    public BaseUnit GetOwner()
    {
        return owner;
    }

    /// <summary>
    /// Renvoie l'artwork de l'upgrade
    /// </summary>
    /// <returns></returns>
    public Sprite GetArtwork()
    {
        return artwork;
    }
}
