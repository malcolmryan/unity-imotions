using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Demo : MonoBehaviour
{
    [SerializeField] private IMotions iMotions;
    [SerializeField] private float period = 1;
    private float timer;

    void Start() 
    {
        iMotions.SendStartMarker("Game");
    }

    void OnDestroy()
    {
        iMotions.SendEndMarker("Game");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > period)
        {
            timer -= period;

            iMotions.SetSensor("Milliseconds", DateTime.Now.Millisecond);
            iMotions.SetSensor("Seconds", DateTime.Now.Second);
            iMotions.SendSensors();
            iMotions.SendDiscreteMarker("Tick");
        }
    }
}
