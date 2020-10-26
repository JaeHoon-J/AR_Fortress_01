using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFire : MonoBehaviour
{
    public GameObject bullet;//날릴 오브젝트
    public GameObject gun;//포신
    public Transform firePos;//발사지점

    void Start()
    {
        StartCoroutine(Shot());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Shot()
    {
        yield return null;
        while(true)
        {
            yield return null;
            Fire();
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void Fire()
    {
        GameObject obj = Instantiate(bullet, firePos.transform.position, Quaternion.identity);
        obj.GetComponent<TestBall>().dir = gun.transform.up;
        obj.GetComponent<TestBall>().speed = 5;
    }
}
