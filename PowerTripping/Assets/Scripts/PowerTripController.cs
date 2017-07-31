using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerTripController : MonoBehaviour
{

    public static PowerTripController instance;

    public bool powerTripping = false;

    public float powerTripTime = 5f;
    public AudioSource audioSource;
    public AudioClip[] songs;
    public AudioClip[] foodSounds;

    CanvasGroup powerTrippingCanvas;
    public Text powerTripText;

    public int startingSize = 30;

    private void Awake()
    {
        powerTrippingCanvas = GetComponent<CanvasGroup>();
        instance = this;
    }

    public void PlayFoodClip(bool cooled)
    {
        audioSource.PlayOneShot(foodSounds[cooled ? 0 : 1]);
    }
    public IEnumerator StartPowerTrip()
    {
        SetMax();
        powerTripping = true;
        //words fly in
        //power stays max
        //insta cool
        // more food spawn rate
        powerTripText.fontSize = startingSize;
        float start = 1f;
        float end = 0f;
        powerTrippingCanvas.alpha = end;

        float t = 0;
        float n = 0.25f;
        while (t <= n)
        {
            powerTrippingCanvas.alpha = Mathf.Lerp(end, start, t / n);
            t += Time.deltaTime;
            yield return null;
        }
        audioSource.clip = songs[1];
        audioSource.Play();
        powerTrippingCanvas.alpha = start;
        SetMax();

        t = 0;
        n = powerTripTime;
        while (t <= n)
        {
            SetMax();
            powerTripText.fontSize++;
            powerTrippingCanvas.alpha = Mathf.Lerp(start, end, t / n);
            t += Time.deltaTime;
            yield return null;
        }
        powerTrippingCanvas.alpha = end;
        powerTripping = false;

        audioSource.clip = songs[0];
        audioSource.Play();
    }

    public void SetMax()
    {
        Thermometer.instance.temperature = Thermometer.instance.minTemp;
        PowerController.instance.currentPower = PowerController.instance.maxPower;
    }
}
