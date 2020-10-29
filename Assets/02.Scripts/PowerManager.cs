using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    [SerializeField]
    protected Slider powerGage;
    // Start is called before the first frame update
    void Start()
    {
        powerGage.value = PhotonManager.Instance.lange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
