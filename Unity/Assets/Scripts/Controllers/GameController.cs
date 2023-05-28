using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void NotifyControllerDelegate();

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private User currentUser;
    private Information information;

    [SerializeField]
    private TcpMarauderClient client = null;

    private bool connected = false;
    private GameObject player_prefab;
    private GameObject path_prefab;

    private List<Path> paths;

    public void UpdateDatas()
    {
        string s = client.GetS();
        Message m = new Message(s);
        switch (m.type)
        {
            case 0:
                Debug.Log("Connected");
                break;
            case (Assets.Scripts.Models.Type)1:
                break;
            case (Assets.Scripts.Models.Type)2:
                GetDrawings(m.message);
                break;
            case (Assets.Scripts.Models.Type)3:
                Debug.Log("Disconnect");
                break;
            default:
                break;
        }
    }

    public void GetDrawings(string s)
    {
        try
        {
            Drawing drawing = new Drawing(s);
            Debug.Log("Rajz: " + drawing.ToString());
            GameObject p = Instantiate(path_prefab, new Vector3(0, 1, 0), Quaternion.identity);
            Path pathh = p.GetComponent<Path>();
            pathh.SetDrawing(drawing);
            Debug.Log(pathh);
            paths.Add(pathh);
            DrawPaths();

        } 
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        //try
        //{
        //    Drawing drawing = new Drawing(s);
            
        //    drawings.Add(drawing);

        //    Debug.Log(drawings.Count());

        //    foreach(Drawing d in drawings)
        //    {
                
        //    }

        //} catch (Exception e)
        //{
        //    Debug.Log(e.ToString());
        //}
    }

    public void DrawPaths()
    {
        foreach (Path p in paths)
        {
            p.Draw();
        }
    }

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

        player_prefab = Resources.Load("Prefabs/Player") as GameObject;
        path_prefab = Resources.Load("Prefabs/Path") as GameObject;


        Init();
        InvokeRepeating(nameof(SendCoordinates), 0, 0.05f);
        paths = new List<Path>();
}

    private void InitPlayer()
    {
        if (connected)
        {
            currentUser ??= new User();
            currentUser.SetName(client.GetId());
            currentUser.SetCoordinates("0;1;0");
            Instantiate(player_prefab, new Vector3(0, 1, 0), Quaternion.identity);

            Message m = new Message(currentUser.name, 0, "");
            client.SendAsynch(m.ToByteArray());
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
            ConnectToServer();
            client.NotifyParentEvent += new NotifyControllerDelegate(UpdateDatas);
            InitPlayer();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void ConnectToServer()
    {
        client.ApplyInputAndConnect(information.address, information.port);
        connected = true;
    }

    public void SendCoordinates()
    {
        if (connected)
        {
            MarauderUser g = FindObjectOfType<MarauderUser>();
            var position = g.transform.position;
            currentUser.SetCoordinates(position.x + ";" + position.y + ";" + position.z);
            Message m = new Message(currentUser.name, (Assets.Scripts.Models.Type)1, currentUser.GetCoordinates());
            client.SendAsynch(m.ToByteArray());
        }
    }

    void OnApplicationQuit()
    {
        if (connected)
        {
            Message m = new Message(currentUser.name, (Assets.Scripts.Models.Type)3, "");
            client.SendAsynch(m.ToByteArray());
        }
    }

    public void ClearMap()
    {
        paths.Clear();
        // TODO

    }
}
