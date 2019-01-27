using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyControl : MonoBehaviour
{
    public int energy = 50;

    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider.value = energy;
        Invoke("Timer", 1.0f);
    }

    void Timer()
    {
        slider.value -= 1;
        Invoke("Timer", 1.0f);
    }
}
