using System;
using UnityEngine;

public class MapUser : MonoBehaviour
{
    private readonly string id;
    public string Id { get { return id; } }

    public MapUser(string id)
    {
        this.id = id;
    }

    void Start()
    {
        
    }

     public void Move(string c)
    {
        string[] coordinates = c.Split(';');
        try
        {
            float x = float.Parse(coordinates[0]);
            float y = float.Parse(coordinates[1]);
            float z = float.Parse(coordinates[2]);
            transform.position = new Vector3(x, y, z);
        } catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    
}
