using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public static Clock instance;

    public int minutes;

    public GameObject hourHand, minuteHand;

    public float tickDelay = 0.1f;
    public float angleInc;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(TickClock());
    }
    public void Update()
    {
        //set hour and second hand angles based on time
        angleInc = 360f / (721f - (minutes / 720));
        hourHand.transform.localEulerAngles = new Vector3(0f, 0f, -(minutes) * angleInc);
        angleInc = 360f / (61f - (minutes / 60));
        minuteHand.transform.localEulerAngles = new Vector3(0f, 0f, -minutes * angleInc);
    }

    public IEnumerator TickClock()
    {
        while (true)
        {
            minutes++;
            yield return new WaitForSeconds(tickDelay);
        }
    }
}
