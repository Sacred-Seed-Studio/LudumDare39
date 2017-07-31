using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerModule : MonoBehaviour
{

    public int powerInc = 5;

    public bool addingPower = false;
    public void Update()
    {
        if (Input.GetButtonDown("Cool") && !addingPower )
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    PowerController.instance.currentPower += powerInc;
                    if (PowerController.instance.currentPower > 100)
                    {
                        PowerController.instance.currentPower = 100;
                    }
                    gameObject.SetActive(false);
                    addingPower = true;
                }
            }
        }
    }
}
