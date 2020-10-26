using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBot : MonoBehaviour
{
    
    void Start()
    {
        transform.forward = NetWork.Get.hostingResultList[0].Result.Anchor.transform.position - transform.position;
    }

    
    void Update()
    {
        
    }
}
