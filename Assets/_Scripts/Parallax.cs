using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Parallax : MonoBehaviour
{

    public Transform[] layers;
    private Transform camTransform; 
    public static Parallax Instance;
    public float easing;
    public int meanIndex;

    void Awake(){
        Instance = this;
    }

    public void UpdateParallax(Vector3 currentCamPos, Vector3 targetPos){
        float parallax = (currentCamPos.x - targetPos.x);
        for (int i = 0; i < layers.Length; i++){
            float deltaX = layers[i].position.x + parallax * (i-meanIndex) * easing;
            Vector3 newPos = new Vector3(deltaX, layers[i].position.y, layers[i].position.z);
            if(i == 2){
                //Debug.Log(newPos.x);
            }
            layers[i].position = Vector3.Lerp(layers[i].position, newPos, 0.1f);
        }
    }
}
