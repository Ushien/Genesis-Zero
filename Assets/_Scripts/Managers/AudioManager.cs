using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource BGM;
    public AudioSource overloadSound;
    public AudioSource errorSound;
    public AudioSource UIBip1;
    public AudioSource UIBip2;
    public AudioSource UIBip3;
    public AudioSource transition;
    void Awake(){
        Instance = this;
    }
}
