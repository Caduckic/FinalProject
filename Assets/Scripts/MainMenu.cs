using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject ship;
    void Start () {
        // sets color of ship based on selectedColor, have to compare to color list as if it was null
        // it wouldn't have a main texture
        for (int i = 0; i < OptionsMenu.colors.Length; i++) {
            if (OptionsMenu.selectedColor == OptionsMenu.colors[i] && OptionsMenu.selectedColor != null) {
                ship.GetComponent<Renderer>().material.SetTexture("_MainTex", OptionsMenu.colors[i]);
                break;
            }
        }
    }
    // used from Brackeys start menu video https://www.youtube.com/watch?v=zc8ac_qUXQY
    public void Play () {
        ChangeScene.NextScene("PlayScene");
    }
    public void Quit () {
        // won't quit in editor but would work in a build
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
