using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum State
{
    Drawing,
    Erasing,
    Default
}

public class Brush : MonoBehaviour
{
    [SerializeField]
    private GameObject linePrefab;
    private Line activeLine;
    private State state = State.Drawing;
    [SerializeField]
    private Button sendBtn;
    [SerializeField]
    private Dropdown options;
    public event NotifyControllerDelegate NotifyParentEvent;
    private bool erasing = false;

    // Start is called before the first frame update
    private void Start()
    {
        sendBtn.onClick.AddListener(Send);
        options.onValueChanged.AddListener(delegate {
            DropdownValueChanged(options);
        });
    }

    void DropdownValueChanged(Dropdown change)
    {
        if (change != null)
        {
            switch (change.value)
            {
                case 0:
                    state = State.Drawing; 
                    break;
                case 1:
                    state = State.Erasing; 
                    break;
                default: 
                    break;
            }
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Drawing:
                Drawing();
                break;
            case State.Erasing:
                Erasing();
                break;
            default:
                break;
        }
    }

    public void Erasing()
    {
        // ha lenyomódik, akkor egy bool mutatja, hogy épp törlés van. Ahogy 


        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            erasing = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            erasing = false;
        }

        if (erasing)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = 1;

            Line[] lines = FindObjectsOfType<Line>();

            foreach (Line line in lines)
            {
                line.Distance(mousePos);
            }
        }
    }

    public void Drawing()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            GameObject newLine = Instantiate(linePrefab);
            activeLine = newLine.GetComponent<Line>();
        }

        if (Input.GetMouseButtonUp(0))
        {
            activeLine = null;
        }

        if (activeLine != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = 1;

            activeLine.UpdateLine(mousePos);
        }
    }

    public void Send()
    {
        NotifyParentEvent?.Invoke();
    }

    public List<List<Vector3>> AllData()
    {
        List<List<Vector3>> allData = new List<List<Vector3>>();
        Line[] objectsInScene = FindObjectsOfType<Line>();

        foreach (Line obj in objectsInScene)
        {
            allData.Add(obj.Points);
        }
        return allData;
    }
}
