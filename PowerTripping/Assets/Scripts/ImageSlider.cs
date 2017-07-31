using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlider : MonoBehaviour
{

    public float fillAmount = 0f;
    public Image fillImage;
    float time = 1f;
    public bool instantChange = true;
    float lastFill = -1f;
    private void Update()
    {
        if (lastFill != fillAmount)
        {
            SetFillAmount(fillAmount);
        }
        //fillImage.fillAmount = fillAmount;
    }

    public void SetFillAmount(float fillAmount)
    {
        if (!instantChange)
        {
            start = lastFill;
            end = fillAmount;
            StopCoroutine("Shift");
            StartCoroutine("Shift");
        }
        else fillImage.fillAmount = fillAmount;
        lastFill = fillAmount;
    }
    float start, end;
    public IEnumerator Shift()
    {
        float t = 0;
        float n = time;
        this.fillImage.fillAmount = start;
        while (t <= n)
        {
            this.fillImage.fillAmount = Mathf.Lerp(start, end, t / n);
            t += Time.deltaTime;
            yield return null;
        }
        this.fillImage.fillAmount = end;
    }
}
