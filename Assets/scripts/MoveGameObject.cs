using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
    public List<Vector3> points;
    private List<GameObject> pointsGo = new List<GameObject>();
    public float speed = 10f;
    private int pointsIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(0, 0, 0);
        if (points.Count > 0)
        {
            for (int i = 0; i < points.Count; i++)
            {
                GameObject go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go1.transform.position = points[i];
                pointsGo.Add(go1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveGameObject();
        rotationGameObject();
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = pointsGo[i].transform.position;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void moveGameObject()
    {
        // khoảng cách giữa vị trí hiện tại và vị trí đích
        float distance = Vector3.Distance(transform.position, points[pointsIndex]);
        
        if (distance < 0.1f)
        {
            pointsIndex++;
            if (pointsIndex > 3)
            {
                pointsIndex = 0;
            }
        }
        else
        {
            Vector3 nextPos = points[pointsIndex];
            float maxDistanceDelta = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, nextPos , maxDistanceDelta);
        }
    }

    private void rotationGameObject()
    {
        Vector3 directionToTarget = points[pointsIndex] - transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, 3f * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (points != null && points.Count > 0)
        {
            for (int i = 1; i < points.Count; i++)
            {
                Gizmos.DrawLine(points[i - 1],points[i]);
            }
            Gizmos.DrawLine(points[0] ,points[points.Count - 1]);
        }
    }
}
