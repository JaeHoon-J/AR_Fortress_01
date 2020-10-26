using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public int a; //inspector 노출  
    private int b; //내꺼 
    protected int c; //아낌없이 주는 나무

    [SerializeField]
    protected CustomizingButton button;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickLeftButton() {
        if (button != null) button.SetCustom();
    }
}
