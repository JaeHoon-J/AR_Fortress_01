using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    [SerializeField]
    protected GameObject pause;
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
        pause.SetActive(false);
    }
    public void OpenSetting()
    {
        setting.SetActive(false);
    }
    public void ExitGames()
    {
        SceneManager.LoadScene("03.Lobby");
    }
}
