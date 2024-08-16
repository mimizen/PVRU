using Com.MyCompany.MyGame;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    private LauncherLobby launcher;
    // Start is called before the first frame update
    void Start()
    {
        launcher = FindObjectOfType<LauncherLobby>(); ;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "SceneMichelle") 
        {
            Destroy(this.gameObject);
        }
    }


    // Update is called once per frame
    public void OnPress()
    {
        launcher.StartGame();
    }
}
