using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Road : MonoBehaviour
{
    public int generation;

    public GameObject startPoint;
    public GameObject endPoint;

    public GameObject junction;
    public GameObject house;

    public float noiseSeedX;
    public float noiseSeedY;

    public bool housesPopulated;
    public bool roadSubdivided;
    public bool roadAmended;

    public int houseCount;
    public float roadAngle;
    public List<Vector2> intersections;
    void Start()
    {
        generation = Spawner.instance.generation;
        setRoad();
        StartCoroutine(checkForIntersections());
        
    }

    private void Update()
    {
        
    }

    public void setRoad()
    {
        roadAngle = returnAngle(startPoint.transform.position, endPoint.transform.position);
        //transform.Rotate(0, 0, roadAngle);
        checkForNearnessToOtherRoads();
        GetComponent<LineRenderer>().SetPosition(0, new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, 0));
        GetComponent<LineRenderer>().SetPosition(1, new Vector3(endPoint.transform.position.x, endPoint.transform.position.y, 0));
        float dist = Vector2.Distance(startPoint.transform.position, endPoint.transform.position);
        float clamped = Mathf.Clamp(dist / 100, .025f, .075f);
        GetComponent<LineRenderer>().startWidth = clamped;
        GetComponent<LineRenderer>().endWidth = clamped;
    }
    public void createJunction()
    {
        if (!roadSubdivided)
        {
            float ranGen = 0;
            int rand = Random.Range(2, 8);
            
            ranGen = rand * .1f;
            Debug.Log(ranGen);
            for (float i = 0.1f; i < ranGen; i+=.1f)
            {
                float r = map(i, .1f, ranGen, 0.1f, 1);
                GameObject newJunction = Instantiate(junction, Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, r), Quaternion.identity);
                newJunction.GetComponent<Junction>().origin = startPoint.transform.position;
                newJunction.GetComponent<Junction>().generation = generation;
                newJunction.transform.SetParent(transform);
                newJunction.transform.localScale = new Vector3(.2f, .2f, .2f);
            }
            roadSubdivided = true;
        }
    }

    public void setUpHouses()
    {
        //DESTROY ALL EXISTING HOUSES
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "House")
            {
                Destroy(transform.GetChild(i));
            }
        }
        if (!housesPopulated)
        {
            createHouses(1);
            createHouses(-1);
            housesPopulated = true;
            //checkForNearnessToOtherRoads();
        }
       StartCoroutine(destroyHousesOnRoads());
       StartCoroutine(countHouses());
    }

    public void createHouses(int mult)
    {
        float dist = Vector2.Distance(startPoint.transform.position, endPoint.transform.position);
        for (float i = 0.25f; i < dist-.25f; i+=.3f)
        {
            GameObject newHouse = Instantiate(house, Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, i/dist),Quaternion.identity);
            newHouse.transform.SetParent(gameObject.transform);
            newHouse.transform.localScale = new Vector3(1, 1, 1);
            float angle = returnAngle(newHouse.transform.position, startPoint.transform.position);
            newHouse.transform.Rotate(new Vector3(0, 0, angle));
            newHouse.transform.Translate(Vector3.up * .1f * mult);
            //SET NOISE COMPONENT
            newHouse.GetComponent<House>().col = Mathf.PerlinNoise(noiseSeedX,noiseSeedY);
            noiseSeedX += 0.1f;
            noiseSeedY += 0.1f;
            //CHOOSE ROAD NAME
            newHouse.GetComponent<House>().roadName = "Tamworth Road";


        }
    }

    IEnumerator destroyHousesOnRoads()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        RaycastHit2D[] hits = Physics2D.LinecastAll(startPoint.transform.position, endPoint.transform.position);
        foreach(RaycastHit2D hit in hits)
        {
            Destroy(hit.collider.gameObject);
        }
    }

    IEnumerator countHouses()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).tag == "House")
            {
                houseCount++;
            }
        }
        if(houseCount <= 5)
        {
            Destroy(gameObject);
        }
    }

    float returnAngle(Vector2 p1, Vector2 p2)
    {
        float angle = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * 180 / Mathf.PI;
        return angle;
    }

    IEnumerator checkForIntersections()
    {
        yield return new WaitForSeconds(1);
        intersections.Clear();
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        bool found;
        for (int i = 0; i < roads.Length; i++)
        {
            Road other = roads[i].GetComponent<Road>();
            Vector2 intersection = GetIntersectionPointCoordinates(startPoint.transform.position, endPoint.transform.position, other.startPoint.transform.position, other.endPoint.transform.position, out found);
            if (roads[i] != gameObject && intersection != (Vector2) startPoint.transform.position )
            {
                if (found)
                {
                    if (!intersections.Contains(intersection))
                    {

                        Road r = roads[i].GetComponent<Road>();

                        Vector2 midPoint = Vector2.Lerp(startPoint.transform.position, endPoint.transform.position, .5f);
                        Vector2 otherMidPoint = Vector2.Lerp(r.startPoint.transform.position, r.endPoint.transform.position, .5f);
                        float distToMid = Vector2.Distance(startPoint.transform.position, midPoint);
                        float otherDistToMid = Vector2.Distance(r.startPoint.transform.position,otherMidPoint);
                        float houseToMid = Vector2.Distance(intersection, midPoint);
                        float houseToOtherMid = Vector2.Distance(intersection, otherMidPoint);

                        if (houseToMid < distToMid && houseToOtherMid < otherDistToMid)
                        {
                            intersections.Add(intersection);
                        }
                        else
                        {

                        }
                    }
                }
            }
        }
       if (intersections.Count >= 2)
        {

          if (Vector2.Distance(startPoint.transform.position, findClosestIntersection(startPoint.transform.position)) < .5f)
            {
               // startPoint.transform.position = findClosestIntersection(startPoint.transform.position);
            }
            else if (Vector2.Distance(endPoint.transform.position, findClosestIntersection(endPoint.transform.position)) < 5f && Spawner.instance.generation == generation)
            {
                endPoint.transform.position = findClosestIntersection(endPoint.transform.position);
            }
        }

            
            setRoad();
        
        StartCoroutine(checkForIntersections());
    }

    Vector3 findClosestIntersection(Vector2 comparisonPoint)
    {
        float closestDist = 100;
        Vector2 closestPoint = new Vector2();
        foreach(Vector2 inter in intersections)
        {
            float dist = Vector2.Distance(comparisonPoint, inter);
            if ( dist < closestDist)
            {
                closestDist = dist;
                closestPoint = inter;
            }
        }
        return closestPoint;
    }

    void checkForNearnessToOtherRoads()
    {
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        
        for (int i = 0; i < roads.Length; i++)
        {
            Road other = roads[i].GetComponent<Road>();
            if (roads[i] != gameObject)
            {
                Vector2 midPoint = Vector2.Lerp(startPoint.transform.position, endPoint.transform.position, .5f);
                for (float j = 0; j < 1; j += 0.1f)
                {
                    Vector2 otherPoint = Vector2.Lerp(other.startPoint.transform.position, other.endPoint.transform.position, i);
                    if (Vector2.Distance(midPoint, otherPoint) < .5f)
                    {
                        if (Mathf.Round(roadAngle) == Mathf.Round(other.roadAngle) || Mathf.Round(roadAngle) == 180 - Mathf.Round(other.roadAngle))
                        {
                            if (generation >= other.generation)
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
            }
        }
    }



    Vector2 findClosestEndPoint()
    {
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        Vector2 closestPoint = new Vector2();
        float dist = 100;
        foreach (GameObject road in roads)
        {


            if (road != gameObject)
            {
                float newDist = Vector2.Distance(startPoint.transform.position, road.GetComponent<Road>().endPoint.transform.position);
                if (newDist < dist)
                {
                    dist = newDist;
                    closestPoint = road.GetComponent<Road>().endPoint.transform.position;
                }
            }
        }
        if (dist > .1f)
        {
            return closestPoint;
        }
        else
        {
            return Vector2.zero;
        }
    }

    public Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
    {
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

        if (tmp == 0)
        {
            found = false;
            return Vector2.zero;
        }

        found = true;
        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;
        return new Vector2(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu
        );
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
