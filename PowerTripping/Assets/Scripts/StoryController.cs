using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryController : MonoBehaviour
{

    public static StoryController instance;
    public float textWaitTime = 1f;
    public CanvasGroup cg;
    public Text text;

    string[] storyline = new string[]
    {
        "I need your help...",
        "Humans store food inside me..."+System.Environment.NewLine+"They're picky about what food should be cooled first to avoid spoiling."+System.Environment.NewLine+"If too much food spoils the humans will find a better fridge!",
        "Click on food to cool it down."+System.Environment.NewLine+"Click on the thermometer to cool the environment."+System.Environment.NewLine+"But watch out, both use power!",
        "Cool food instantly when you're"+System.Environment.NewLine+"POWER TRIPPING!",
        "Good luck!",
        "Don't let me run out of power..."
    };

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator HandleStoryLine()
    {
        //fade in from black
        //show line of text
        //fade out 
        float start = 1f;
        float end = 0f;

        float t = 0;
        float n = 0.75f;
        while (t <= n)
        {
            //cg.alpha = Mathf.Lerp(end, start, t / n);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = start;

        for (int i = 0; i < storyline.Length; i++)
        {
            for (int k = 0; k <= storyline[i].Length; k++)
            {
                text.text = storyline[i].Substring(0, k) + "<color=#0000>" + storyline[i].Substring(k) + "</color>";
                yield return null;
                yield return null;
            }
            yield return new WaitForSeconds(textWaitTime);
        }
        yield return null;

        t = 0;
        while (t <= n)
        {
            cg.alpha = Mathf.Lerp(start, end, t / n);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
        gameObject.SetActive(false);
    }
}
