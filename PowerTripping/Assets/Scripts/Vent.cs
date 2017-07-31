using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{

    [Range(0f, 1f)]
    public float coolingEffect = 0.1f; //temp cool per delay
    public float coolingDelay = 1f;

    public float targetTemperature = -10f;

    public void Start()
    {
        StartCoroutine(Cooling());
    }

    IEnumerator Cooling()
    {
        while (true)
        {
            if
                (targetTemperature <= Thermometer.instance.temperature)
            {
                Thermometer.instance.temperature -= (coolingEffect);
                Thermometer.instance.temperature = Mathf.Clamp(Thermometer.instance.temperature, targetTemperature, Thermometer.instance.temperature);
            }

            else
            {
                Thermometer.instance.temperature += (coolingEffect);
                Thermometer.instance.temperature = Mathf.Clamp(Thermometer.instance.temperature, Thermometer.instance.temperature, targetTemperature);
            }
            yield return new WaitForSeconds(coolingDelay);
        }
    }
}
