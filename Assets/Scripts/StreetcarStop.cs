using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]

public class StreetcarStop : MonoBehaviour
{
    public Animator minimapIconAnimator;
    public SpriteRenderer streetcarStopSpriteRenderer;
    public Sprite whiteStreetcarStop;
    public Sprite greenStreetcarStop;
    public Sprite yellowStreetcarStop;
    public Sprite redStreetcarStop;
    public GameObject streetcarTimerCanvas;
    public Image timerFill;

    Streetcar streetcar;
    bool streetcarInRange = false;
    bool streetcarStopped = false;
    bool streetcarFull = false;

    void Awake()
    {
        streetcar = GameObject.FindGameObjectWithTag("Streetcar").GetComponent<Streetcar>();
    }

    void Update()
    {
        UpdateMinimap();
        CheckIfFull();

        if (streetcarInRange)
            CheckStreetcar();
    }

    void CheckStreetcar()
    {
        //Check if streetcar has stopped.
        if (Mathf.Abs(streetcar.GetMoveSpeed()) < 0.01f)
            streetcarStopped = true;
        else
            streetcarStopped = false;

        /*
        if (streetcarStopped)
        {
            //If the streetcar isn't full send pedestrians.
            if (!streetcar.IsFull())
            {
                for (int i = 1; i < this.transform.childCount; i++)
                {
                    if (!this.transform.GetChild(i).GetComponent<Pedestrian>().returning)
                    {
                        Vector3 newDestination = streetcar.transform.position;
                        Pedestrian ped = this.transform.GetChild(i).GetComponent<Pedestrian>();
                        ped.SetReturnDestination(this.transform.GetChild(i).transform.position);
                        ped.SetDestination(newDestination);
                        ped.SetMoveSpeed(1.5f);
                        ped.boarding = true;

                    }
                }
            }
        }
        */
    }

    //Check if streetcar stop is at capacity.
    void CheckIfFull()
    {
        if (this.transform.childCount > 5)
        {
            streetcarTimerCanvas.SetActive(true);
            if (streetcarTimerCanvas.activeSelf)
            {
                timerFill.fillAmount -= 0.1f * Time.deltaTime;
                if (timerFill.fillAmount <= 0)
                {
                    for (int i = 1; i < this.transform.childCount; i++)
                    {
                        Destroy(this.transform.GetChild(i).gameObject, 0.5f);
                    }
                }
            }
            if (streetcarStopped == true)
            {
                timerFill.fillAmount = 1f;
                streetcarTimerCanvas.SetActive(false);
            }
        }
        else if (this.transform.childCount < 5)
        {
            timerFill.fillAmount = 1f;
            streetcarTimerCanvas.SetActive(false);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "StreetcarRadius")
            streetcarInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "StreetcarRadius")
            streetcarInRange = false;
    }

    public bool StreetcarStopped()
    {
        return streetcarStopped;
    }

    public void UpdateMinimap()
    {
        int pedestriansWaiting = this.transform.childCount - 1;
        if (pedestriansWaiting >= 5)
        {
            minimapIconAnimator.SetTrigger("Red");
            UpdatePedestrianAnimationSpeed(2);
            streetcarStopSpriteRenderer.sprite = redStreetcarStop;
        }
        else if (pedestriansWaiting >= 3)
        {
            minimapIconAnimator.SetTrigger("Yellow");
            UpdatePedestrianAnimationSpeed(1.5f);
            streetcarStopSpriteRenderer.sprite = yellowStreetcarStop;
        }
        else if (pedestriansWaiting >= 1)
        {
            minimapIconAnimator.SetTrigger("Green");
            UpdatePedestrianAnimationSpeed(1);
            streetcarStopSpriteRenderer.sprite = greenStreetcarStop;
        }
        else if (pedestriansWaiting == 0)
        {
            minimapIconAnimator.SetTrigger("White");
            UpdatePedestrianAnimationSpeed(1);
            streetcarStopSpriteRenderer.sprite = whiteStreetcarStop;
        }
    }

    void UpdatePedestrianAnimationSpeed(float newSpeed)
    {
        Pedestrian[] pedestrians = this.GetComponentsInChildren<Pedestrian>();
        foreach (Pedestrian pedestrian in pedestrians) { pedestrian.GetComponent<Animator>().speed = newSpeed; }
    }

    public bool HasRole(Role role)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).GetComponent<Pedestrian>())
            {
                if (this.transform.GetChild(i).GetComponent<Pedestrian>().GetRole() == role)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
