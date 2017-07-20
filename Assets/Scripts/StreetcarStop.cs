using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]

public class StreetcarStop : MonoBehaviour
{
    [SerializeField] Animator minimapIconAnimator;
    [SerializeField] SpriteRenderer streetcarStopSpriteRenderer;
    [SerializeField] Sprite whiteStreetcarStop;
    [SerializeField] Sprite greenStreetcarStop;
    [SerializeField] Sprite yellowStreetcarStop;
    [SerializeField] Sprite redStreetcarStop;

    GameObject streetcar;
    GameObject streetcarTimerCanvas;
    Image timerFill;
    bool streetcarStopped = false;
    bool streetcarFull = false;

    void Awake()
    {
        streetcar = GameObject.FindGameObjectWithTag("Streetcar");
        streetcarTimerCanvas = this.transform.GetChild(0).gameObject;
        streetcarTimerCanvas.SetActive(false);
        timerFill = streetcarTimerCanvas.GetComponentInChildren<Image>();
    }
    void Update()
    {
        UpdateMinimap();
        CheckForStreetcar();
        CheckIfFull();
    }
    void CheckForStreetcar()
    {
        if (streetcarStopped == true)
        {
            if (streetcarFull == false)
            {
                for (int i = 1; i < this.transform.childCount; i++)
                {
                    Vector3 newDestination = this.transform.GetChild(i).position + Mathf.Sign(this.transform.position.y) * 2 * Vector3.down;
                    this.transform.GetChild(i).GetComponent<Pedestrian>().SetDestination(newDestination);
                    this.transform.GetChild(i).GetComponent<Pedestrian>().SetMoveSpeed(1.5f);
                }
            }
            if (streetcarFull == true)
            {
                for (int i = 1; i < this.transform.childCount; i++)
                {
                    Vector3 newDestination = this.transform.position;
                    this.transform.GetChild(i).GetComponent<Pedestrian>().SetDestination(newDestination);
                    this.transform.GetChild(i).GetComponent<Pedestrian>().SetMoveSpeed(1.5f);
                }
            }
        }
    }
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
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.name == "Streetcar" && other.transform.GetComponent<Streetcar>() && other.transform.GetComponent<Streetcar>().IsFull() == false)
        {
            float moveSpeed = other.GetComponent<Streetcar>().GetMoveSpeed();
            if (Mathf.Abs(moveSpeed) < 0.01f && !streetcarStopped)
            {
                streetcarStopped = true;
            }
        }
        if (other.transform.name == "Streetcar" && other.transform.GetComponent<Streetcar>() && other.transform.GetComponent<Streetcar>().IsFull() == true)
        {
            streetcarFull = true;
        }
        else if(other.transform.name == "Streetcar" && other.transform.GetComponent<Streetcar>() && other.transform.GetComponent<Streetcar>().IsFull() == false)
        {
            streetcarFull = false;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.CompareTag("Streetcar") && other.transform.GetComponent<Streetcar>() && other.transform.GetComponent<Streetcar>().IsFull() == false)
        {
            streetcarStopped = false;
        }
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

