using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSomething : MonoBehaviour

{
    public float speed = 10f;


    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
    }
}