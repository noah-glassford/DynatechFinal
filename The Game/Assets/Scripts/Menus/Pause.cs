using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField]
    GameObject PauseMenuObject;

    [SerializeField]
    GameObject OptionsScreenObject;
    [SerializeField]
    GameObject MainScreenObject;
    [SerializeField]


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuObject.activeSelf == false)
            {
                OpenPause();
              
            }
            else
            {
                UnPause();
            }
        }
    }

    public void OpenPause()
    {
        PauseMenuObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        OptionsScreenObject.SetActive(false);
        MainScreenObject.SetActive(true);
        Cursor.visible = true;
        if(PhotonNetwork.PlayerList.Length > 1)
        {
            return;
        }
        Time.timeScale = 0f;
        AudioListener.volume = 0f;
    }

    public void UnPause()
    {
        PauseMenuObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        OptionsScreenObject.SetActive(false);
        MainScreenObject.SetActive(true);
        Cursor.visible = false;
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
    }

    public void LeaveGame()
    {
        JSAM.AudioManager.StopAllSounds();
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
