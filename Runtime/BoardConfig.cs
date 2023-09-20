using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BoardConfig", menuName = "Minesweeper/BoardConfig", order = 1)]
public class BoardConfig : ScriptableObject
{
    public int rows;
    public int columns;
    public List<Vector2Int> minePositions = new List<Vector2Int>();
}