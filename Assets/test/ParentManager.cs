using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParentManager : MonoBehaviour
{
    [SerializeField]
    public Button publicBtn;

    [SerializeField]
    private Button privateBtn;

    [SerializeField]
    protected Button protectedBtn; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
