using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Drawing drawing;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Draw()
    {
        lineRenderer.positionCount = drawing.coordinates.Count();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, drawing.coordinates[i]);
        }
    }
    // setter for drawing
    public void SetDrawing(Drawing d)
    {
        drawing = d;
    }

    public override string ToString()
    {
        return $"Az út: {drawing}";
    }
}
