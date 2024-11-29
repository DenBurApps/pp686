using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private const string ArcheryFileName = "A_game_scores.json";
    private const string FlightFileName = "F_game_scores.json";
    private const string VolleyballFileName = "V_game_scores.json";

    public static List<GameScores> FlightScores = new List<GameScores>();
    public static List<GameScores> ArcheryScores = new List<GameScores>();
    public static List<GameScores> VolleyballScores = new List<GameScores>();

    public static void SaveScores(string fileName, GameScores score)
    {
        List<GameScores> scores;

        if (string.Equals(fileName, ArcheryFileName))
        {
            scores = ArcheryScores;
        }
        else if (string.Equals(fileName, FlightFileName))
        {
            scores = FlightScores;
        }
        else if (string.Equals(fileName, VolleyballFileName))
        {
            scores = VolleyballScores;
        }
        else
        {
            Debug.LogError("Invalid file name specified!");
            return;
        }

        scores.Add(score);

        GameScoresList scoresList = new GameScoresList { Scores = scores };
        string json = JsonUtility.ToJson(scoresList, true);
        File.WriteAllText(GetSaveFilePath(fileName), json);
        Debug.Log("Scores saved: " + json);
    }

    public static List<GameScores> LoadScores(string fileName)
    {
        string path = GetSaveFilePath(fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("Scores loaded: " + json);

            GameScoresList scoresList = JsonUtility.FromJson<GameScoresList>(json);

            if (string.Equals(fileName, ArcheryFileName))
            {
                ArcheryScores = scoresList.Scores;
            }
            else if (string.Equals(fileName, FlightFileName))
            {
                FlightScores = scoresList.Scores;
            }
            else if (string.Equals(fileName, VolleyballFileName))
            {
                VolleyballScores = scoresList.Scores;
            }

            return scoresList.Scores;
        }

        Debug.Log("No saved scores found. Returning an empty list.");
        return new List<GameScores>();
    }

    private static string GetSaveFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}

[System.Serializable]
public class GameScores
{
    public int PlayerScore;
    public int EnemyScore;
}

[System.Serializable]
public class GameScoresList
{
    public List<GameScores> Scores = new List<GameScores>();
}
