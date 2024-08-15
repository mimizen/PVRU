using Com.MyCompany.MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonClick : MonoBehaviour
{
    Launcher launcher;

    private void Start()
    {
        launcher = FindObjectOfType<Launcher>();
    }

    public void StartOnPress()
    {
        if (launcher != null)
        {
            Debug.Log("Button pressed.. Starting Race..");
            launcher.StartGame();
        }
    }
}
