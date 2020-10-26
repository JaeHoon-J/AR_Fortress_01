using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tonado : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tonadoEdge;
    public Transform tonadoCenter;
    public Transform tonadoHigh;
    public float tonadoSpeed;
    void Start()
    {
        tonadoSpeed = 20;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, 200 * Time.deltaTime);
    }
}
