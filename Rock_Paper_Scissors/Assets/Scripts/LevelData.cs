using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Rock_Paper_Scissors/Level", order = 0)]
public class LevelData : ScriptableObject 
{
    public int width;
    public int height;
    public int spawnPoints;
    public Wave wave;
    public int threeStarRequirement;
    public int twoStarRequirement;
}