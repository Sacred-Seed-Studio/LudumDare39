using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum FoodType
{
    Apple,
    Bowl,
    LargeCarton,
    Lemon,
    Lime,
    Orange,
    SmallCarton,
    SmallDish,
    Tomato,
    Wine
}

[System.Serializable]
public class FoodColor
{
    public FoodColor(Color[] c)
    {
        colors = c;
    }
    public Color[] colors;
}

public class PowerController : MonoBehaviour
{

    public static PowerController instance;
    public float maxPower = 100f;
    public float currentPower = 100f;
    public float currentBonus = 0f;
    public float maxBonus = 100f;

    public Text powerText, bonusText;

    public ImageSlider imageSliderPower, imageSliderBonus;

    public Transform[] foodSpawns;
    public HashSet<int> spawnsUsed;

    public GameObject foodPrefab;
    public GameObject[] foodPrefabs;
    public float spawnDelay = 1f;

    public List<GameObject> foods;

    public Image[] spoiledImages;
    public int spoilCount;

    public Sprite[] foodSprites;
    public FoodColor[] foodColors;

    float fillSpoilTime = 0.5f;
    public List<GameObject> powerModulePool;
    public GameObject powerModulePrefab;

    public bool spawingFood = false;
    private void Awake()
    {
        instance = this;
        spawnsUsed = new HashSet<int>();
        foods = new List<GameObject>();
        powerModulePool = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            powerModulePool.Add(Instantiate<GameObject>(powerModulePrefab));
            powerModulePool[i].gameObject.SetActive(false);
        }
    }


    private void Start()
    {
    }

    public void ClearFood()
    {
        int k = foods.Count;
        for (int i = 0; i < k; i++)
        {
            Destroy(foods[i].gameObject);//.SetActive(false);
        }

        foods.Clear();
    }
    private void Update()
    {
        imageSliderPower.fillAmount = currentPower / maxPower;
        imageSliderBonus.fillAmount = currentBonus / maxBonus;
        powerText.text = "RemainingPower - " + (int)(imageSliderPower.fillAmount * 100f) + "%";
        bonusText.text = "NextBonus - " + (int)(imageSliderBonus.fillAmount * 100f) + "%";

        if (spoilCount >= 10 || currentPower <= 0)
        {
            if (!gameOver) StartCoroutine(GameOver());
        }

        if (currentBonus >= 100 || currentPower > 110)
        {
            //currentPower = 100;
            if (currentBonus >= 100) currentBonus -= 100;
            StartCoroutine(PowerTripController.instance.StartPowerTrip());
        }
    }

    public bool gameOver = false;
    public IEnumerator GameOver()
    {
        GameController.instance.inTitle = true;
        Debug.Log("Game over");
        GameController.instance.gameOverReasonText.text = "";
        if (spoilCount >= 10) GameController.instance.gameOverReasonText.text += "You spoiled too much food.";
        if (spoilCount >= 10 && currentPower <= 0) GameController.instance.gameOverReasonText.text += "And you ran out of power! Wow that takes skill.";
        else if (currentPower <= 0) GameController.instance.gameOverReasonText.text += "You ran out of power.";
        gameOver = true;
        stopSpawning = true;
        for (int i = 0; i < foods.Count; i++)
        {
            foods[i].GetComponent<Food>().enabled = false;
        }

        GameController.instance.SetWaitingForGameOver(false); //waiting for game over 
        spoilCount = 0;
        currentPower = 1;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        gameOver = false;
    }
    public void UsePower(float p = 1f)
    {
        currentPower -= p;
    }

    public void SpoilFood()
    {
        StartCoroutine(FillInSpoiledFood());
    }


    IEnumerator FillInSpoiledFood()
    {
        //fill in current spoil count
        float t = 0;
        float n = fillSpoilTime;
        float start = 0f;
        float end = 1f;
        if (spoilCount < spoiledImages.Length) spoiledImages[spoilCount].fillAmount = start;
        while (t <= n)
        {
            if (spoilCount < spoiledImages.Length) spoiledImages[spoilCount].fillAmount = Mathf.Lerp(start, end, t / n);
            t += Time.deltaTime;
            yield return null;
        }
        if (spoilCount < spoiledImages.Length) spoiledImages[spoilCount].fillAmount = end;
        if (!PowerTripController.instance.powerTripping) spoilCount++;
    }
    IEnumerator FillOutSpoiledFood(int i)
    {
        //fill in current spoil count
        float t = 0;
        float n = fillSpoilTime;
        float start = 1f;
        float end = 0f;
        spoiledImages[i].fillAmount = start;
        while (t <= n)
        {
            spoiledImages[i].fillAmount = Mathf.Lerp(start, end, t / n);
            t += Time.deltaTime;
            yield return null;
        }
        spoiledImages[i].fillAmount = end;
    }
    public void ResetSpoiledImages()
    {
        for (int i = 0; i < spoiledImages.Length; i++)
        {
            StartCoroutine(FillOutSpoiledFood(i));
        }
        spoilCount = 0;
    }
    bool stopSpawning;
    public IEnumerator StartSpawnFood()
    {
        stopSpawning = false;
        spawingFood = true;
        while (spoilCount < 10 && !stopSpawning)
        {
            SpawnFood();
            if (!PowerTripController.instance.powerTripping)
            {
                if (Random.Range(0, 10) <= 2)
                {
                    SpawnPowerModule();
                }
            }
            else
            {
                yield return null;
                yield return null;
                SpawnFood();
                SpawnFood();
            }
            yield return new WaitForSeconds(spawnDelay);
        }
        spawingFood = false;
    }

    public void SpawnFood()
    {
        if (spawnsUsed.Count == foodSpawns.Length) spawnsUsed.Clear();
        Food food = Instantiate<GameObject>(foodPrefab).GetComponent<Food>();
        food.Setup((FoodType)Random.Range(0, foodPrefabs.Length));
        GameController.instance.AddPossibleTarget(food);
        foods.Add(food.gameObject);
        int i = 0;
        int count = 0;
        while (spawnsUsed.Contains(i))
        {
            i = Random.Range(0, foodSpawns.Length);
            count++;
            if (count >= 100)
            {
                break;
            }
        }
        spawnsUsed.Add(i);
        food.transform.position = foodSpawns[i].position;
    }


    public void SpawnPowerModule()
    {
        int k = 0;
        int count = 0;
        while (spawnsUsed.Contains(k))
        {
            k = Random.Range(0, foodSpawns.Length);
            count++;
            if (count >= 100)
            {
                break;
            }
        }
        spawnsUsed.Add(k);
        for (int i = 0; i < powerModulePool.Count; i++)
        {
            if (!powerModulePool[i].gameObject.activeInHierarchy)
            {
                powerModulePool[i].transform.position = foodSpawns[k].position;
                powerModulePool[i].SetActive(true);
                powerModulePool[i].GetComponent<PowerModule>().addingPower = false;
            }
        }
    }
}
