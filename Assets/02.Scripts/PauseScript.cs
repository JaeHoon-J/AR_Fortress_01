using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    
    [SerializeField]
    protected GameObject setting;

    protected SoundManager soundManager;
    // Start is called before the first frame update
    void Start()
    {
        soundManager = SoundManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ClosePause()
    {
        soundManager.SetEffectClip("click");
        this.gameObject.SetActive(false);
    }
    public void OpenSetting()
    {
        soundManager.SetEffectClip("click");
        setting.SetActive(false);
    }
    public void ExitGames()
    {
        soundManager.SetEffectClip("movestart");
        SceneManager.LoadScene("03.Lobby");
    }
}
