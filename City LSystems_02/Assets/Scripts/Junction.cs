using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junction : MonoBehaviour
{
    public Vector2 origin;
    public float angle;
    public int generation;

    

    [SerializeField]
    private GameObject road;
    void Start()
    {
        angle = returnAngle(origin,transform.position);
        StartCoroutine(move());
    }

    float returnAngle(Vector2 p1, Vector2 p2)
    {
        float angle = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * 180 / Mathf.PI;
        return angle;
    }

    IEnumerator move()
    {
        yield return new WaitForSeconds(Time.deltaTime);

        Vector3 orig = new Vector3();
        Vector3 stored = new Vector3();
        Vector3 newEnd = new Vector3();

        GameObject newRoad = Instantiate(road, transform.position, Quaternion.identity);
        orig = transform.position;
        transform.Rotate(new Vector3(0, 0, angle));
        float mult = Random.Range(1,4);
        int rand = Random.Range(0, 2);
        if(rand == 0) { rand = -1; }
        
            transform.Translate(Vector3.up * rand * mult);
            stored = transform.position;
            transform.position = orig;
            transform.Translate(Vector3.up * -rand * mult);
            newEnd = transform.position;

        GameObject parent = GameObject.FindGameObjectWithTag("Parent");
        newRoad.GetComponent<Road>().transform.SetParent(parent.transform);
        newRoad.transform.localScale = new Vector3(1, 1, 1);
        newRoad.GetComponent<Road>().startPoint.transform.position = stored;
        newRoad.GetComponent<Road>().endPoint.transform.position = newEnd;
        newRoad.GetComponent<Road>().housesPopulated = false;
    }
}
