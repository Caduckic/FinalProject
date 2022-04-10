using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    static public float levelsize = 200f;
    static public int asteroids = 200, enemies = 10;
    public GameObject levelsizeobj, asteroidsobj, enemiesobj, colorobj, ship;
    static public Texture[] colors = new Texture[10];
    public Texture yellow, black, blue, cyan, green, grey, orange, purple, red, white;
    static public Texture selectedColor;
    
    // Thanks again to Brackeys, his Settings menu video helped a lot with getting started
    void Start () {
        // sets the colors list
        colors[0] = yellow;
        colors[1] = black;
        colors[2] = blue;
        colors[3] = cyan;
        colors[4] = green;
        colors[5] = grey;
        colors[6] = orange;
        colors[7] = purple;
        colors[8] = red;
        colors[9] = white;
        // sets the text the reps the playScene params
        levelsizeobj.GetComponent<InputField>().text = levelsize.ToString();
        asteroidsobj.GetComponent<InputField>().text = asteroids.ToString();
        enemiesobj.GetComponent<InputField>().text = enemies.ToString();
        // default color values is 0 so that if its not able to be set, it has a vaule
        var currentColorValue = 0;
        // checks if selectedColor is any of the existing color textures, if so it sets ship texture to it
        // and sets current color vaule to it
        for (int i = 0; i < colors.Length; i++) {
            if (selectedColor == colors[i]) {
                currentColorValue = i;
                ship.GetComponent<Renderer>().material.SetTexture("_MainTex", colors[i]);
                break;
            }
        }
        // sets the dropdowns current value to current color value
        // I had to make sure I set the array in the same order as the dropdown for this to work
        colorobj.GetComponent<Dropdown>().value = currentColorValue;
    }

    // used when input field is modified
    public void SetLevelSize(string leveltext) {
        // if level text isn't empty or a minus symbal check if it once converted to a float is larger than our levelminsize
        if (leveltext != "" && leveltext != "-") {
            if (float.Parse(leveltext) > 10f) {
                // sets levelsize to leveltext as a float
                levelsize = float.Parse(leveltext);
            }
        }
        Debug.Log(levelsize);
    }
    // used if the player clicks off the input
    public void LevelsizeMIN(string leveltext) {
        // if the inputted string is less than minsize after being casted into float,
        // set levelsize to min and sets the text to min as a string
        if (leveltext == "" || float.Parse(leveltext) < 10) {
            levelsizeobj.GetComponent<InputField>().text = "10";
            leveltext = "10";
            levelsize = float.Parse(leveltext);
        }
        Debug.Log(levelsize);
    }
    // the rest of these basically do the same thing but with ints
    public void SetEnemyCount(string enemytext) {
        if (enemytext != "" && enemytext != "-") {
            if (int.Parse(enemytext) >= 1f) {
                enemies = int.Parse(enemytext);
            }
        }
        Debug.Log(enemies);
    }
    public void SetEnemyMIN(string enemytext) {
        if (enemytext == "" || int.Parse(enemytext) < 1) {
            enemiesobj.GetComponent<InputField>().text = "1";
            enemytext = "1";
            enemies = int.Parse(enemytext);
        }
    }
    public void SetAsteroidCount(string asteroidtext) {
        if (asteroidtext != "" && asteroidtext != "-") {
            if (int.Parse(asteroidtext) >= 40) {
                asteroids = int.Parse(asteroidtext);
            }
        }
        Debug.Log(asteroids);
    }
    public void SetAsteroidMIN(string asteroidtext) {
        if (asteroidtext == "" || int.Parse(asteroidtext) < 40) {
            asteroidsobj.GetComponent<InputField>().text = "40";
            asteroidtext = "40";
            asteroids = int.Parse(asteroidtext);
        }
    }
    // sets color to color from list using dropdown value and sets selected color to it
    public void SetColor(int colorvalue) {
        Debug.Log(colorvalue);
        ship.GetComponent<Renderer>().material.SetTexture("_MainTex", colors[colorvalue]);
        selectedColor = colors[colorvalue];
    }
}
