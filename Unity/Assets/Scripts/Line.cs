using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    private List<Vector3> points;
    public List<Vector3> Points { get { return points; } }

    public void UpdateLine(Vector3 position)
    {
        if(points == null)
        {
            points = new List<Vector3>();
            SetPoint(position);
            return;
        }
        if(Vector3.Distance(points.Last(), position) > .1 && points.Count < 75)
        {
            SetPoint(position);
        }
    }

    void SetPoint(Vector3 point)
    {
        points.Add(point);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(lineRenderer.positionCount-1, point);
    }

    public void Distance(Vector3 vector)
    {
        foreach(Vector3 v in points)
        {
            if(Vector3.Distance(vector, v) < .2)
            {
                Destroy(gameObject);
            }
        }
    }
}
