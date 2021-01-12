using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    public int count;
    public Vector3 startPoint;
    public Vector3 endPoint;

    public GameObject road;
    public GameObject parent;

    public GameObject infoBar;
    public TextMeshProUGUI infoBarText;

    public int generation;

    public bool overlay;

    void Start()
    {
        instance = this;
        infoBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        setPoints(); 
    }

    void setPoints()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        if (Input.GetMouseButtonDown(1))
        {
            if(count == 0) { startPoint = mousePos; count++; }
            else if(count == 1) { endPoint = mousePos; count = 0; createRoad(); }

        }
    }

    void createRoad()
    {
        GameObject newRoad = Instantiate(road, transform.position, transform.rotation);
        newRoad.transform.SetParent(parent.transform);
        newRoad.transform.localScale = new Vector3(1, 1, 1);
        newRoad.GetComponent<Road>().startPoint.transform.position = startPoint;
        newRoad.GetComponent<Road>().endPoint.transform.position = endPoint;
    }

    public void populateRoads()
    {
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        foreach(GameObject road in roads)
        {
            road.GetComponent<Road>().setUpHouses();
        }
    }

    public void createJunctions()
    {
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        foreach (GameObject road in roads)
        {
            road.GetComponent<Road>().createJunction();
        }
        generation++;
    }

    public void switchOverlay()
    {
        overlay = !overlay;
    }

}
