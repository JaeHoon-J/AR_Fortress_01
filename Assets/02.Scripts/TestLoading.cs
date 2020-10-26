using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoading : MonoBehaviour
{
    int speed = 20;
    int movespeed = 20;
    public GameObject robot;
    public GameObject white; 
    public GameObject final;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        robot.transform.Rotate(Vector3.up, speed*Time.deltaTime, Space.World);
        if(white.transform.position.y>22.4)
        {
            white.transform.position=new Vector3(0,-17,68);
        }
        white.transform.Translate(Vector3.up*Time.deltaTime*movespeed, Space.World);
    }
}
