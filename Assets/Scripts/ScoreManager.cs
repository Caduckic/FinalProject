using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

// Followed Brackeys video on save and load systems https://www.youtube.com/watch?v=XOjd_qU2Ido&t=829s
// to get me started
public static class ScoreManager {
    public static List<ScoreData> scoresData = new List<ScoreData>();
    public static void SaveData(List<ScoreData> scores = null) {
        // in case player entered the win menu without playing
        if (Timer.minutes + ":" + Timer.seconds.ToString("F2") != "0:0.00") {
            BinaryFormatter formatter = new BinaryFormatter();
            // set this way as Application.persistentDataPath is constant across all devices
            string path = Application.persistentDataPath + "/scores.data";
            FileStream stream = new FileStream(path, FileMode.Create);

            // creates a new score with session params
            ScoreData scoreData = new ScoreData(OptionsMenu.enemies, OptionsMenu.asteroids, OptionsMenu.levelsize, Timer.minutes + ":" + Timer.seconds.ToString("F2"));

            // if function has been given a score just iterate and sort
            if (scores != null) {
                // whenever a score is added the list is then sorted based on enemies and time
                scores.Add(scoreData);
                for (int i = 0; i < scores.Count - 1; i++) {
                    scores.Sort((scoreData, i) => {
                        // CompareTo function returns 1 if the element is greater than what it is comparing to
                        // -1 if its less and 0 if they're equal, useful for sorting the scores
                        int enemieCount = i.enemies.CompareTo(scoreData.enemies);
                        // if first CompareTo returns 0 then Compare their times instead, this is now a string so
                        // its probably not perfect but it works well enough for this project
                        return enemieCount != 0 ? enemieCount : scoreData.time.CompareTo(i.time);
                        });
                }
                // finally writes stream using binary formatter, inputting scoresData
                formatter.Serialize(stream, scoresData);
            }
            // else simply add scoreData to start the list
            else {
                scoresData.Add(scoreData);
                formatter.Serialize(stream, scoresData);
            }

            // make sure to close stream so prevent errors and "free" it
            stream.Close();
        }
    }
    public static List<ScoreData> LoadScores() {
        // gets the path of the scores
        string path = Application.persistentDataPath + "/scores.data";
        // if the file exists, open the file and using the binary formatter convert back to
        // a list of scoreData and return that data
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            List<ScoreData> data = formatter.Deserialize(stream) as List<ScoreData>;
            stream.Close();

            scoresData = data;
            return data;
        }
        // else log no save file found
        else {
            Debug.Log("No save file found at" + path);
            return null;
        }
    }
}
