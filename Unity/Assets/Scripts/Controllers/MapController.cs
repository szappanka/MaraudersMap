using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController Instance;
    private readonly List<User> users = new List<User>();
    private Information information;
    [SerializeField]
    private Brush brush;

    [SerializeField]
    private TcpMarauderClient client;

    private bool connected = false;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Init();
        client.NotifyParentEvent += new NotifyControllerDelegate(UpdateDatas);
        brush.NotifyParentEvent += new NotifyControllerDelegate(SendDrawing);
    }

    // Update is called once per frame
    void Update()
    {
        if (connected)
        {
            WallInit();
        }
    }

    public void WallInit()
    {
        for(int i = 0; i < information.walls.Count; i++)
        {
            string[] s = information.walls[i].Split('=');
            string[] startCoords = s[0].Split(';');
            string[] endCoords = s[1].Split(';');
            Vector3 start1 = new Vector3(float.Parse(startCoords[0]), float.Parse(startCoords[1]), float.Parse(startCoords[2]));
            Vector3 end1 = new Vector3(float.Parse(endCoords[0]), float.Parse(endCoords[1]), float.Parse(endCoords[2]));
            Debug.DrawLine(start1, end1);
        }
    }

    public void UpdateDatas()
    {
        string s = client.GetS();
        Message m = new Message(s);
        switch (m.type)
        {
            case 0:
                User u = new User();
                u.SetName(m.id);
                users.Add(u);
                break;
            case (Assets.Scripts.Models.Type)1:
                User user1 = users.Find(x => x.name == m.id);
                user1?.SetCoordinates(m.message);
                break;
            case (Assets.Scripts.Models.Type)2:
                // itt ezt nem kell lekezelni, mert a szervernek nem kell tudnia a rajzolásról
                break;
            case (Assets.Scripts.Models.Type)3:
                User user2 = users.Find(x => x.name == m.id);
                users.Remove(user2);
                break;
            default:
                break;
        }
        Draw(m);
    }

    public void Draw(Message m)
    {
        if (m.type == 0)
        {
            GameObject go = Resources.Load("Prefabs/MapUser") as GameObject;
            GameObject player = Instantiate(go, new Vector3(0, 0, 0), Quaternion.identity);
            player.name = m.id;
        }
        else if (m.type == (Assets.Scripts.Models.Type)1)
        {
            GameObject mapUser = GameObject.Find(m.id);
            string[] coords = m.message.Split(';');
            Vector3 v = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
            if (mapUser)
            {
                mapUser.transform.position = v;
            } else
            {
                GameObject go = Resources.Load("Prefabs/MapUser") as GameObject;
                GameObject player = Instantiate(go, new Vector3(0, 0, 0), Quaternion.identity);
                player.name = m.id;
            }
        }
        else if (m.type == (Assets.Scripts.Models.Type)3)
        {
            GameObject mapUser = GameObject.Find(m.id);
            Destroy(mapUser);
        }
    }


    public async void Init()
    {
        try
        {
            var i = await ServerConnection.Instance.GetInformation();
            information = new Information
            {
                port = i.port,
                address = i.address,
                walls = i.walls
            };

            ConnectedToServer();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void ConnectedToServer()
    {
        client.ApplyInputAndConnect(information.address, information.port);
        connected = true;
    }

    public void SendDrawing()
    {
        List<List<Vector3>> drawings = brush.AllData();

        foreach (List<Vector3> drawing in drawings)
        {
            Drawing d = new Drawing(drawing);
            string b = d.ToString();
            Message m = new Message("map", (Assets.Scripts.Models.Type)2, b);
            client.SendAsynch(m.ToByteArray());
        }
    }
}
