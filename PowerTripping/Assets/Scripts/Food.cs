using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    public float temperature = 0f;
    float delta = 1.01f;
    public ImageSlider imageSlider;
    public int foodBonus = 1;

    public SpriteRenderer sr;
    public float tempChangeDelay = 0.5f;

    float tempInc = 0.25f;
    float tempRiseDelay = .1f;
    float nextRiseTime = 0f;

    public FoodType foodType;
    public FoodColor foodColor;

    bool spoiled = false;

    public CanvasGroup cg;

    private void Awake()
    {
        imageSlider = GetComponentInChildren<ImageSlider>();
        temperature = 1f;
        delta = Random.Range(1.001f, 1.01f);
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(ManageTemperature());
    }

    public void Setup(FoodType foodType)
    {

        this.foodType = foodType;
        sr.sprite = PowerController.instance.foodSprites[(int)foodType];
        foodColor = PowerController.instance.foodColors[(int)foodType];
        sr.color = foodColor.colors[Random.Range(0, PowerController.instance.foodColors[(int)foodType].colors.Length)];
    }

    private void Update()
    {
        //raycast to check
        sr.sortingOrder = (int)((-100) * transform.position.y);
        if (Input.GetButtonDown("Cool"))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    CoolItem();
                }
            }
        }

        imageSlider.fillAmount = Thermometer.instance.GetPercentage(temperature);

        if (temperature <= Thermometer.instance.minTemp && !cooled)
        {
            CooledDown();
        }

        if (temperature >= Thermometer.instance.maxTemp && !spoiled)
        {
            Spoiled();
        }
    }


    IEnumerator ManageTemperature()
    {
        //set the temperature based on the current fridge temperature and your current temperature
        while (true)
        {
            yield return new WaitForSeconds(tempChangeDelay);
            temperature += tempInc;
        }
        yield return null;
    }

    bool cooled = false;
    public void CooledDown()
    {
        //if item gets fully cooled down
        StartCoroutine(HandleCoolDown());
    }

    float cooldownTime = 0.5f;
    IEnumerator HandleCoolDown()
    {
        PowerTripController.instance.PlayFoodClip(true);
        cooled = true;
        float t = 0;
        float n = cooldownTime;
        float start = 0.07f;
        float end = 1f;

        cg.gameObject.transform.position = (Vector2)transform.position+ new Vector2(0f, start);
        while (t <= n)
        {
            cg.gameObject.transform.position = (Vector2)transform.position + new Vector2(0f, Mathf.Lerp(start, end, t / n));
            t += Time.deltaTime;
            yield return null;
        }
        cg.gameObject.transform.position = (Vector2)transform.position + new Vector2(0f, end);
        gameObject.SetActive(false);
        if (!PowerTripController.instance.powerTripping) PowerController.instance.currentBonus += foodBonus;
        GameController.instance.CoolTargetFood(this);
        GameController.instance.itemsChilled++;

    }
    public void Spoiled()
    {
        spoiled = true;
        //if item gets fully cooled down
        gameObject.AddComponent<Rigidbody2D>();
        gameObject.layer = LayerMask.NameToLayer("Spoiled");
        StartCoroutine(HandleSpoil());
        transform.GetChild(0).gameObject.SetActive(false);
    }


    IEnumerator HandleSpoil()
    {
        //fade out after 25 seconds
        PowerTripController.instance.PlayFoodClip(false);
        PowerController.instance.SpoilFood();
        yield return new WaitForSeconds(15f);
        gameObject.SetActive(false);
    }
    public void CoolItem()
    {
        if (PowerTripController.instance.powerTripping)
        {
            temperature = Thermometer.instance.minTemp - 1f;
        }
        else
        {

            temperature -= 1f;
            PowerController.instance.currentPower--;
        }
    }
    bool mouseOver = false;
    public void OnMouseOver()
    {
        //Debug.Log("Draggin 1 "+ GameController.instance.isClicked);
        //if (GameController.instance. isClicked)
        //{
        //    Debug.Log("Draggin 2");
        //    CoolItem();
        //    mouseOver = true;
        //}
        //CoolItem();
    }

    public void OnMouseExit()
    {
        //mouseOver = false;
    }
}
