using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private RectTransform rectComponent;
    [SerializeField]
    protected Image imageComp;
    // Start is called before the first frame update
    void Start()
    {
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
        imageComp.fillAmount= PhotonManager.Instance.myHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonManager.Instance.myHp <= 0)
            imageComp.fillAmount = 0;
    }
}
