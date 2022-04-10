// so I can store it, it needs [System.Serializable]
[System.Serializable]
public class ScoreData {
    public int enemies;
    public int asteroids;
    public float levelSize;
    public string time;
    
    // This class is used for displaying and storing play session data
    public ScoreData (int enemies, int asteroids, float levelSize, string time) {
        this.enemies = enemies;
        this.asteroids = asteroids;
        this.levelSize = levelSize;
        this.time = time;
    }
}
