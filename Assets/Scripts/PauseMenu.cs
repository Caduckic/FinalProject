using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    static public bool paused = false;
    private bool pressed = false;
    public GameObject pauseMenuUI;
    public GameObject playStateUI;
    // Brackeys again can be thanked for his great video on Making a pause menu
    void Update() {
        // takes cancel inputs and switches on and off pause
        if (Input.GetAxisRaw("Cancel") > 0 && paused && !pressed) {
            pressed = true;
            Resume();
        }
        else if (Input.GetAxisRaw("Cancel") > 0 && !paused && !pressed) {
            pressed = true;
            Pause();
        }
        else if (Input.GetAxisRaw("Cancel") == 0) pressed = false;
    }
    // turns off pause UI and play UI on, sets paused to false and timescale to 1
    public void Resume() {
        pauseMenuUI.SetActive(false);
        playStateUI.SetActive(true);
        Time.timeScale = 1f;
        paused = false;
    }
    // turns off play UI and on pause UI, sets paused true and timescale 0
    void Pause() {
        pauseMenuUI.SetActive(true);
        playStateUI.SetActive(false);
        Time.timeScale = 0f;
        paused = true;
    }
    // goes back to the start menu and sets paused off and timescale back to 1
    public void LoadMenu() {
        paused = false;
        Time.timeScale = 1f;
        ChangeScene.NextScene("StartMenu");
    }
    public void QuitGame() {
        Application.Quit();
        Debug.Log("QUIT");
    }
}
