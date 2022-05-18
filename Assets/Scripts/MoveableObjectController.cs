using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObjectController : MonoBehaviour
{
    [SerializeField]
    private PointController pointController;

    public Color dirColor;

    public float MoveSpeed;
    public float Diff;

    public int CurrentPointIndex;

    void Update()
    {
        if (IsValidateIndex())
        {
            float distance = Vector3.Distance(pointController.Positions[CurrentPointIndex], transform.position);
            if (distance <= Diff)
            {
                if (CurrentPointIndex + 1 >= pointController.Positions.Length)
                {
                    if (pointController.Loop)
                    {
                        CurrentPointIndex = 0;
                    }
                }
                else
                {
                    CurrentPointIndex++;

                }
            }
            MoveToPoint();
            RotateToPoint();

        }
        else
        {
            CurrentPointIndex = pointController.Positions.Length - 1;
        }
    }
    private void OnDrawGizmos()
    {
        DrawDirection();
    }
    bool IsValidateIndex()
    {
        return (CurrentPointIndex >= 0 && CurrentPointIndex < pointController.Positions.Length);
    }
    void MoveToPoint()
    {
        Vector3 direction = (pointController.Positions[CurrentPointIndex] - transform.position).normalized;
        direction *= MoveSpeed * Time.deltaTime;
        float dirDistance = Vector3.Distance(Vector3.zero, direction);
        float objDistance = Vector3.Distance(transform.position, pointController.Positions[CurrentPointIndex]);
        if (dirDistance > objDistance)
        {
            transform.position = pointController.Positions[CurrentPointIndex];
        }
        else
        {
            transform.Translate(direction, Space.World);
        }
    }
    void RotateToPoint()
    {
        transform.LookAt(pointController.Positions[CurrentPointIndex]);
    }
    void DrawDirection()
    {
        Gizmos.color = dirColor;
        if (IsValidateIndex())
        {
            Gizmos.DrawLine(transform.position, pointController.Positions[CurrentPointIndex]);
        }
    }
}
