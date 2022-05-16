using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
    public List<Vector3> points;
    public float speed = 10f;
    private int pointsIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        moveGameObject();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void moveGameObject()
    {
        // khoảng cách giữa vị trí hiện tại và vị trí đích
        float distance = Vector3.Distance(transform.position, points[pointsIndex]);
        Debug.Log("--------------------------  " + pointsIndex);
        
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (points != null && points.Count > 0)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawLine(gameObject.transform.position, points[pointsIndex]);
            }
            Gizmos.DrawLine(gameObject.transform.position, points[pointsIndex]);
        }
    }
}
