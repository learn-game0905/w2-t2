using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class PointController : MonoBehaviour
{
    public static readonly int OFFSET_HOTCONTROL = 10;

    public float ZOffsetCamera;

    public bool Loop;
    [HideInInspector]
    public Vector3[] Positions;

    public Color LineColor;
    public Color ObjectColor;

    public float ObjectScale;

    public float Diff;

    public bool AddMiddlePoint;


    [HideInInspector]
    public int SelectedPointIndex { get; set; }

    [HideInInspector]
    public int AddPointIndex { get; set; }


    [HideInInspector]
    public Vector3 AddPoint;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            DrawConntectedLine(i);
            DrawPoint(i);
        }
    }

    public void DeletePosition()
    {
        if (SelectedPointIndex == -1) return;
        Vector3[] tmp = new Vector3[Positions.Length - 1];
        int count = 0;
        for (int i = 0; i < Positions.Length; i++)
        {
            if (SelectedPointIndex == i) continue;
            tmp[count++] = Positions[i];
        }
        Positions = tmp;
    }

    public void AddPosition()
    {
        Vector3[] tmp = new Vector3[Positions.Length + 1];
        int count = 0;
        if (!AddMiddlePoint)
        {
            for (int i = 0; i < Positions.Length; i++)
            {
                tmp[i] = Positions[i];
            }
            tmp[tmp.Length - 1] = AddPoint;
        }
        else
        {
            if (AddPointIndex == -1) return;
            for (int i = 0; i < tmp.Length; i++)
            {
                if (AddPointIndex == i)
                {
                    tmp[i] = AddPoint;
                    continue;
                }
                tmp[i] = Positions[count++];
            }
        }
        Positions = tmp;
    }
    private void DrawConntectedLine(int i)
    {
        Gizmos.color = LineColor;
        int nextLineIndex = 0;
        if (i + 1 >= Positions.Length)
        {
            if (Loop)
            {
                nextLineIndex = 0;
            } else
            {
                nextLineIndex = i;
            }
        }
        else nextLineIndex = i + 1;
        Gizmos.DrawLine(Positions[i], Positions[nextLineIndex]);
    }
    private void DrawPoint(int i)
    {
        Gizmos.color = ObjectColor;
        Gizmos.DrawIcon(Positions[i], "sv_icon_dot11_pix16_gizmo");
    }

}

[CustomEditor(typeof(PointController))]
public class PointInteraction : Editor
{
    private Camera sceneCamera;

    public int currentIndex;

    private int countRun = 0;

    private PointController p;

    private ReorderableList listPoint;

    public void OnEnable()
    {
        p = target as PointController;
        listPoint = new ReorderableList(serializedObject, serializedObject.FindProperty("Positions"), true, true, true, true);
        listPoint.drawElementCallback = DrawListItems;
        listPoint.drawHeaderCallback = DrawHeader;
        listPoint.onSelectCallback = LookAtItem;

    }
    public override void OnInspectorGUI()
    {

        listPoint.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();

    }

    private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight), "Position " + (index).ToString());

        EditorGUI.PropertyField(new Rect(new Rect(rect.x + 81, rect.y, rect.width - 81, EditorGUIUtility.singleLineHeight)),
                                listPoint.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
    }
    private void DrawHeader(Rect rect)
    {
        string name = "Positions";
        EditorGUI.LabelField(rect, name);
    }

    private void LookAtItem(ReorderableList list)
    {
        SceneView.lastActiveSceneView.LookAt(((PointController)target).Positions[list.index]);
    }

    private void OnSceneGUI()
    {
        switch (countRun)
        {
            case 0:
                {
                    countRun++;
                    AddPoint();
                    DeletePoint();
                    break;
                }
            case 1:
                {
                    countRun = 0;
                    break;
                }
        }

        UpdatePoint();

    }
    private void UpdatePoint()
    {
        for (int i = 0; i < p.Positions.Length; i++)
        {
            p.Positions[i] = Handles.PositionHandle(p.Positions[i], Quaternion.identity);

            float size = 0.5f * p.ObjectScale;
            Vector3 handleDirection = Vector3.up;

            EditorGUI.BeginChangeCheck();
            Handles.color = p.ObjectColor;
            Vector3 newTargetPosition = Handles.FreeMoveHandle(i + PointController.OFFSET_HOTCONTROL + 1, p.Positions[i], Quaternion.identity, size, Vector3.one, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "Change Look At Target Position");
                p.Positions[i] = newTargetPosition;
            }
        }
    }
    private void AddPoint()
    {

        if (Event.current.isKey && Event.current.keyCode == KeyCode.A && Event.current.type == EventType.KeyDown)
        {
            Debug.Log("aaaaaaaaaaaaaaaaaaa");
            int index = -1;
            Vector3 point = CalculatePoint(ref index);
            p.AddPointIndex = index;
            p.AddPoint = point;
            p.AddPosition();
        }

    }
    private void DeletePoint()
    {
        p.SelectedPointIndex = GetIndexHotControl();
        if (p.SelectedPointIndex != -1)
        {
            listPoint.index = p.SelectedPointIndex;
        }

        if (Event.current.isKey && Event.current.keyCode == KeyCode.D)
        {
            Debug.Log("ddddddddddddddddddddd");
            if (p.SelectedPointIndex != -1)
            {
                p.DeletePosition();
                GUIUtility.hotControl = -1;
            }
        }
    }

    private Vector3 CalculatePoint(ref int index)
    {
        sceneCamera = SceneView.currentDrawingSceneView.camera;
        Vector3 mousePosition = Event.current.mousePosition;
        float ppp = EditorGUIUtility.pixelsPerPoint;

        Vector3 point = new Vector3();

        mousePosition.x *= ppp;
        mousePosition.y = sceneCamera.pixelHeight - mousePosition.y * ppp;

        point = sceneCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, p.ZOffsetCamera));

        Vector3 dir1;
        Vector3 dir2;

        Vector3 mPoint1;
        Vector3 mPoint2;
        Vector3 dirMPoint;

        Vector3 mPoint;

        float distance = 0f;


        if (p.AddMiddlePoint)
        {
            for (int i = 0; i < p.Positions.Length; i++)
            {
                int nextLineIndex = 0;
                if (i + 1 >= p.Positions.Length)
                {
                    if (p.Loop)
                    {
                        nextLineIndex = 0;
                    }
                    else
                    {
                        nextLineIndex = i;
                    }
                }
                else nextLineIndex = i + 1;
                dir1 = p.Positions[nextLineIndex] - p.Positions[i];
                dir2 = sceneCamera.transform.position - point;

                mPoint1 = p.Positions[i];
                mPoint2 = sceneCamera.transform.position;
                dirMPoint = mPoint2 - mPoint1;

                distance = Mathf.Abs(Vector3.Dot(Vector3.Cross(dir1, dir2), dirMPoint)) / AbsVector(Vector3.Cross(dir1, dir2));
                if (distance < p.Diff)
                {
                    mPoint = mPoint1 + (AbsVector(Vector3.Cross(dir2, dirMPoint)) / AbsVector(Vector3.Cross(dir1, dir2))) * dir1;
                    if (CheckPointBetweenLine(mPoint, p.Positions[i], p.Positions[nextLineIndex]))
                    {
                        index = nextLineIndex;
                        return mPoint;
                    }

                }
            }
        }
        index = -1;
        return point;
    }

    private int GetIndexHotControl()
    {
        if (GUIUtility.hotControl - PointController.OFFSET_HOTCONTROL - 1 >= 0 && GUIUtility.hotControl - PointController.OFFSET_HOTCONTROL - 1 < ((PointController)target).Positions.Length)
        {
            return GUIUtility.hotControl - PointController.OFFSET_HOTCONTROL - 1;
        }
        return -1;
    }
    private float AbsVector(Vector3 vector)
    {
        return (Mathf.Sqrt(Mathf.Pow(vector.x, 2) + Mathf.Pow(vector.y, 2) + Mathf.Pow(vector.z, 2)));
    }

    private bool CheckPointBetweenLine(Vector3 point, Vector3 pointLine1, Vector3 pointLine2)
    {
        return Vector3.Dot((pointLine2 - pointLine1).normalized, (point - pointLine2).normalized) < 0f && Vector3.Dot((pointLine1 - pointLine2).normalized, (point - pointLine1).normalized) < 0f;
    }
}

