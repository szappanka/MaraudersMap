using UnityEngine;

public class MarauderUser : MonoBehaviour
{
    //private bool moving = false;
    readonly float speed = 5.0f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(Vector3.left * speed);
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(Vector3.right * speed);
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 Movement = new Vector3(0,0,1);
            rb.AddForce(Movement * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 Movement = new Vector3(0,0,-1);
            rb.AddForce(Movement * speed);
        }
    }
}
