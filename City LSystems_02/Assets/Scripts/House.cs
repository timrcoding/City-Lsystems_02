using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : MonoBehaviour
{
    public float col;
    public string roadName;
    public int houseHoldCount;
    public int neighbourCount;
    public string status;
    void Start()
    {
        StartCoroutine(findNeigbours());
        houseHoldCount = Random.Range(1, 6);
        
    }

    IEnumerator findNeigbours()
    {
        Collider2D[] neighbourColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        neighbourCount = neighbourColliders.Length;
        yield return new WaitForSeconds(.2f);
        StartCoroutine(findNeigbours());
        
        float rand = map(neighbourCount, 0, 40, .6f, 1.2f);
        transform.localScale = new Vector3(rand, rand, rand);
        //setStatus();
    }

    private void Update()
    {
        if (Spawner.instance.overlay)
        {
            setStatus();
        }
        else
        {
            GetComponent<Image>().color = Color.HSVToRGB(0, 0, col);
        }
    }

    void setStatus()
    {
        if(neighbourCount <= 20)
        {
            GetComponent<Image>().color = Color.HSVToRGB(.3f, col, 1);
            status = "Residential";
        }
        else if(neighbourCount > 20 && neighbourCount <=25)
        {
            GetComponent<Image>().color = Color.HSVToRGB(.6f, col, 1);
            status = "Estate";
        }
        else if(neighbourCount > 25)
        {
            GetComponent<Image>().color = Color.HSVToRGB(.9f, col, 1);
            status = "Commercial";
        }
    }

    public void toggleInfo(bool b)
    {
        Spawner.instance.infoBar.SetActive(b);
        setInfoText();
    }

    public void setInfoText()
    {
        Spawner.instance.infoBarText.text = $"Sector: {status} '\n'House Size: {houseHoldCount}";
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "House")
        {
            if (other.GetComponentInParent<Road>().generation > GetComponentInParent<Road>().generation)
            {
                Destroy(other.gameObject);
            }
            else if(other.GetComponentInParent<Road>().generation == GetComponentInParent<Road>().generation)
            {
                if(GetInstanceID() > other.GetInstanceID())
                {
                    Destroy(other.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            
        }
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
