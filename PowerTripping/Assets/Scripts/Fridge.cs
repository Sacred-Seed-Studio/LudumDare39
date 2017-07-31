using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour
{
    public static Fridge instance;
    public GameObject fridgeDoor;

    public float openDoorTime = 1f;

    public bool openDoor, closeDoor;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (openDoor)
        {
            openDoor = false;
            StartCoroutine(OpenDoor());
        }

        if (closeDoor)
        {
            closeDoor = false;
            StartCoroutine(CloseDoor());
        }
    }
   public IEnumerator OpenDoor()
    {
        float t = 0;
        float n = openDoorTime;
        float start = 0f, end = 90f;
        fridgeDoor.transform.localEulerAngles = new Vector3(0f, start, 0f);
        while (t <= n)
        {
            fridgeDoor.transform.localEulerAngles = new Vector3(0f, Mathf.Lerp(start, end, t / n), 0f);
            t += Time.deltaTime;
            yield return null;
        }
        fridgeDoor.transform.localEulerAngles = new Vector3(0f, end, 0f);
    }

    public IEnumerator CloseDoor()
    {
        float t = 0;
        float n = openDoorTime;
        float start = 90f, end = 0f;
        fridgeDoor.transform.localEulerAngles = new Vector3(0f, start, 0f);
        while (t <= n)
        {
            fridgeDoor.transform.localEulerAngles = new Vector3(0f, Mathf.Lerp(start, end, t / n), 0f);
            t += Time.deltaTime;
            yield return null;
        }
        fridgeDoor.transform.localEulerAngles = new Vector3(0f, end, 0f);
    }


    public void OnMouseDown()
    {
        //openDoor = true;
    }
}
