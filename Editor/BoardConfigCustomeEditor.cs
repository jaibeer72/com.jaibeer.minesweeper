using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(BoardConfig))]
public class BoardConfigCustomeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoardConfig boardConfig = (BoardConfig)target;

        if (GUILayout.Button("Open Minesweeper Editor"))
        {
            MinesweeperEditor.ShowWindow(boardConfig);
        }
    }
}
