using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class TargetFood
{
    public Image image;
    public Food food;
    public Text text;
    public int bonus;
}
public class GameController : MonoBehaviour
{

    public static GameController instance;
    public CanvasGroup titleCanvas, gameOverCanvas;
    public Text gameOverText, gameOverReasonText;

    bool waiting;

    public List<TargetFood> targetFoods;
    public Sprite emptyTargetSprite;
    public int itemsChilled = 0;
    public Text rankText, foodsCooledText, minutesLastedText;

    public bool isClicked = false;
    public bool inTitle = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(HandleGame());
    }
    private void Update()
    {
        isClicked = Input.GetButtonDown("Cool");
    }
    public void AddPossibleTarget(Food food)
    {
        //when a food is created, if there is target space - add it
        for (int i = 0; i < targetFoods.Count; i++)
        {
            if (targetFoods[i].food == null)
            {
                targetFoods[i].food = food;
                targetFoods[i].image.sprite = food.sr.sprite;
                targetFoods[i].image.color = food.sr.color;
                targetFoods[i].bonus = (new int[] { 5, 5, 10, 10, 15 })[Random.Range(0, 5)];
                targetFoods[i].text.text = "+" + targetFoods[i].bonus;
                break;
            }
        }
    }

    public void CoolTargetFood(Food f)
    {
        for (int i = 0; i < targetFoods.Count; i++)
        {
            if (targetFoods[i].food != null)
            {
                if (f.foodType == targetFoods[i].food.foodType && f.foodColor == targetFoods[i].food.foodColor)
                {
                    targetFoods[i].food = null;
                    if (!PowerTripController.instance.powerTripping) PowerController.instance.currentBonus += targetFoods[i].bonus;
                    targetFoods[i].image.sprite = emptyTargetSprite;
                    targetFoods[i].image.color = Color.white;
                    targetFoods[i].text.text = "";
                    //todo big bouns
                }
            }
        }
    }

    public void SetWaiting(bool b)
    {
        waiting = b;
    }

    IEnumerator StartWaiting()
    {
        waiting = true;
        while (waiting)
        {
            yield return null;
        }
    }
    public void SetWaitingForGameOver(bool b)
    {
        waitingForGameOver = b;
    }
    IEnumerator StartWaitingForGameOver()
    {
        waitingForGameOver = true;
        while (waitingForGameOver)
        {
            yield return null;
        }
    }

    public void ResetGame()
    {
        PowerController.instance.ClearFood();
        PowerController.instance.currentBonus = 0f;
        //PowerController.instance.gameOver = false; //why the fuck dont these work?????? to do
        //PowerController.instance.spoilCount = 0;
        PowerController.instance.ResetSpoiledImages();
        targetFoods[0].image.sprite = emptyTargetSprite;
        targetFoods[1].image.sprite = emptyTargetSprite;

        Clock.instance.minutes = 0;
        itemsChilled = 0;
        PowerController.instance.currentPower = PowerController.instance.maxPower;
        StartCoroutine(PlayGame());
    }
    IEnumerator HandleGame()
    {

        inTitle = true;
        yield return StoryController.instance.HandleStoryLine();
        titleCanvas.gameObject.SetActive(true);
        gameOverCanvas.gameObject.SetActive(false);
        yield return StartWaiting();
        yield return PlayGame();
    }

    public bool waitingForGameOver;
    IEnumerator PlayGame()
    {
        inTitle = false;
        PowerController.instance.gameOver = false;
        gameOverCanvas.gameObject.SetActive(false);
        StartCoroutine(Fridge.instance.OpenDoor());
        yield return FadeCanvas(false, true);//show title

        if (!PowerController.instance.spawingFood) StartCoroutine(PowerController.instance.StartSpawnFood());

        yield return StartWaitingForGameOver();

        gameOverCanvas.gameObject.SetActive(true);
        gameOverText.text = GetRandomGameoverMessage();

        StartCoroutine(Fridge.instance.CloseDoor());
        minutesLastedText.text = (int)(Clock.instance.minutes / 60f) + " hours & " + (int)(Clock.instance.minutes % 60) + " minutes";
        foodsCooledText.text = itemsChilled + " cooled foods";
        rankText.text = "Rank: " + GetRank(Clock.instance.minutes, itemsChilled);
        yield return FadeCanvas(true, false); //turn on game over
    }
    IEnumerator FadeCanvas(bool on, bool title)
    {
        float t = 0;
        float n = 1f;
        float start = on ? 0f : 1f;
        float end = on ? 1f : 0f;
        CanvasGroup cg = title ? titleCanvas : gameOverCanvas;

        cg.alpha = start;
        while (t <= n)
        {
            cg.alpha = Mathf.Lerp(start, end, t / n);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }

    public string GetRandomGameoverMessage()
    {
        string[] s = new string[]
        {
            "Your chill level is low...",
            "You had the power but you lost the power...",
            "It was an ice try!"
        };

        return s[Random.Range(0, s.Length)];
    }

    public string GetRank(int minutes, int foodsCooled)
    {
        string[] s = new string[] { "" };
        if (minutes < 10 || foodsCooled < 5)
        {
            s = new string[] { "F is for fridge", "Terrible", "Basically a Duddle of Water" };
        }
        else if (minutes < 30 || foodsCooled < 10)
        {
            s = new string[] { "D Fridge", "Melted", "Warmish" };
        }
        else if (minutes < 60 || foodsCooled < 15)
        {
            s = new string[] { "C Fridge", "Sorta cool", "A bit chilly" };
        }
        else if (minutes < 90 || foodsCooled < 20)
        {
            s = new string[] { "B Fridge", "Lukewarm", "Almost Chilled" };
        }
        else if (minutes < 120 || foodsCooled < 25)
        {
            s = new string[] { "A-", "Cool Enough", "Frozen" };
        }
        else
        {
            s = new string[] { "A+", "Amazing", "So Cool", "Chill Fridge", "Fridgidair","Great Chiller", "Max Chill", "Epic Chill Master", "Chill Blaster" };
        }
        return s[Random.Range(0, s.Length)];
    }
}

