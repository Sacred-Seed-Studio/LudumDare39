using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class Thermometer : MonoBehaviour
{
    public static Thermometer instance;

    public int minutes;

    public GameObject hand;

    public float temperature = 5f;
    public float minTemp = -20f;
    public float maxTemp = 20f;
    public float minAngle = -80f;
    public float maxAngle = 80f;
    public float tickDelay = 0.1f;
    public float angle;
    public float tempChangeDelay = 0.5f;
    public float tempInc = 0.01f; //always getting warmer -click to cool down by 1 degree - uses lots of power
    public Color coldColor, warmColor;

    Animator anim;

    private void Awake()
    {
        instance = this;
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        StartCoroutine(ManageTemperature());
    }
    IEnumerator ManageTemperature()
    {
        //set the temperature based on the current fridge temperature and your current temperature
        while (true)
        {
            yield return new WaitForSeconds(tempChangeDelay);
            if (extremeTemperature)
            {
                if (!GameController.instance.inTitle) PowerController.instance.currentPower--;
            }
            temperature += tempInc;
        }
        yield return null;
    }
    bool extremeTemperature = false;
    public PostProcessingBehaviour ppb;

    public void Update()
    {
        VignetteModel.Settings settings = ppb.profile.vignette.settings;
        settings.color = Color.Lerp(coldColor, warmColor, (temperature - minTemp) / (maxTemp - minTemp));
        ppb.profile.vignette.settings = settings;
        if (Input.GetButtonDown("Cool"))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    LowerTemperature();
                }
            }
        }

        anim.SetFloat("Temperature", temperature);
        //anim.SetBool("Extreme", setNextTurn);
        //if ((temperature > 4f || temperature < 0f))
        //{
        //    setNextTurn = true;
        //}
        extremeTemperature = (temperature > 4f || temperature < 0f);

        //set hour and second hand angles based on time
        float k = GetPercentage(temperature);
        angle = Mathf.Lerp(minAngle, maxAngle, k);
        hand.transform.localEulerAngles = new Vector3(0f, 0f, angle);
        temperature = Mathf.Clamp(temperature, minTemp, maxTemp);
    }

    public float GetPercentage(float temperature)
    {

        return 1f - (temperature - minTemp) / (maxTemp - minTemp);
    }

    public void LowerTemperature(int power = 2)
    {
        if (temperature <= 0)
        {
            temperature += 1;
        }
        else
        {

            temperature -= 1;
        }
        PowerController.instance.UsePower(power);
    }
}
