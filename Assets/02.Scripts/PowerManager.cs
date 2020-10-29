﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    private RectTransform rectComponent;
    [SerializeField]
    protected Image imageComp;


    // Use this for initialization
    void Start()
    {
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
        imageComp.fillAmount = (PhotonManager.Instance.lange/15);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
