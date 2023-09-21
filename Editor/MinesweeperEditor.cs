using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Buffers.Text;
using System.Collections.Generic;

public class MinesweeperEditor : EditorWindow
{
    private BoardConfig boardConfig;
    private VisualTreeAsset visualTree;
    private ScrollView m_Sv;
    public ScrollView scrollView
    {
        get
        {
            if (m_Sv == null)
            {
                m_Sv = rootVisualElement.Q<ScrollView>("GameBoardScrollView");
            }
            return m_Sv;
        }
    }

    public string PathToEditorUXML = "Packages/com.jaibeer.minesweeper/Assets/EditorAssets/MinesweeperEditorWindow.uxml";
    //private int mineCount = 0;
    private int m_Rows;
    public int Rows
    {
        get
        {
            return m_Rows;
        }
        set
        {
            if (m_Rows != value)
            {
                m_Rows = value;
                GenerateGrid(value, Cols);
            }
        }
    }

    private int m_Cols;
    public int Cols
    {
        get
        {
            return m_Cols;
        }

        set
        {
            if (m_Cols != value)
            {
                m_Cols = value;
                GenerateGrid(Rows, value);
            }
        }
    }

    private int m_MaxMines;
    public int MaxMines
    {
        get
        {
            return m_MaxMines; 
        }
        set
        {
            if(m_MaxMines != value)
            {
                m_MaxMines = value;
                GenerateGrid(Rows, Cols, true);
            }
        }
    }

    private int placedMines = 0;
    public List<Vector2Int> minePositions = new List<Vector2Int>();

    public void GenerateGrid(int rows, int columns, bool SetMinesFromConfig = false)
    {
        placedMines = 0; 
        scrollView.Clear(); // Clear any existing elements in the scroll view

        for (int i = 0; i < rows; i++)
        {
            VisualElement rowContainer = new VisualElement();
            rowContainer.style.flexDirection = FlexDirection.Row;

            for (int j = 0; j < columns; j++)
            {
                TileButton tileButton = new TileButton(i, j);
                tileButton.style.minWidth = new Length(50, LengthUnit.Pixel);
                tileButton.style.minHeight = new Length(50, LengthUnit.Pixel);

                tileButton.TileButtonClicked += TileButton_TileButtonClicked;
                tileButton.TileButtonRightClicked += TileButton_TileButtonRightClicked; 

                if(SetMinesFromConfig)
                {
                    // Set the initial status of the tile based on the boardConfig
                    if (boardConfig.minePositions.Contains(new Vector2Int(i, j)))
                    {
                        placedMines++;
                        tileButton.status = Tile_Status.Bomb;
                    }
                }

                rowContainer.Add(tileButton);
            }
            scrollView.Add(rowContainer);
        }
    }

    private void TileButton_TileButtonRightClicked(TileButton button)
    {
        button.status = Tile_Status.Flaged;
    }

    private void TileButton_TileButtonClicked(TileButton obj)
    {
        if (obj.status == Tile_Status.Bomb)
        {
            obj.status = Tile_Status.Closed;
            placedMines--;
            if(minePositions.Contains(new Vector2Int(obj.Loc_X, obj.Loc_Y)))
            {
                minePositions.Remove(new Vector2Int(obj.Loc_X, obj.Loc_Y));
            }
        }
        else
        {
            if (placedMines >= MaxMines)
                return;

            obj.status = Tile_Status.Bomb;
            placedMines++;
            minePositions.Add(new Vector2Int(obj.Loc_X, obj.Loc_Y)); 
        }
    }

    public static void ShowWindow(BoardConfig config)
    {
        MinesweeperEditor window = GetWindow<MinesweeperEditor>("Minesweeper Editor");
        window.boardConfig = config;
        window.Show();

        // Now load the board config values
        window.LoadBoardConfig();
    }

    private void OnEnable()
    {

        visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PathToEditorUXML);

        var root = rootVisualElement;
        visualTree.CloneTree(root);

        var rowInputFiled = root.Q<IntegerField>("Rows");
        var colInputField = root.Q<IntegerField>("Columns");
        var mineInputField = root.Q<IntegerField>("Mines");

        var applyAllButton = root.Q<Button>("ApplyAllButton");
        applyAllButton.clicked += OnApplyAllClicked;

        rowInputFiled.RegisterValueChangedCallback((evt) =>
        {
            Rows = evt.newValue;
        });

        colInputField.RegisterValueChangedCallback((evt) =>
        {
            Cols = evt.newValue;
        });

        mineInputField.RegisterValueChangedCallback((evt) =>
        {
            MaxMines = evt.newValue;
        });

        GenerateGrid(Rows, Cols, true);
    }

    private void OnApplyAllClicked()
    {
        boardConfig.minePositions = new List<Vector2Int>(minePositions);
        boardConfig.rows = m_Rows;
        boardConfig.columns = m_Cols;
        EditorUtility.SetDirty(boardConfig);
        AssetDatabase.SaveAssets();
    }

    private void LoadBoardConfig()
    {
        var root = rootVisualElement;
        var rowInputFiled = root.Q<IntegerField>("Rows");
        var colInputField = root.Q<IntegerField>("Columns");
        var mineInputField = root.Q<IntegerField>("Mines");

        if (boardConfig != null)
        {
            rowInputFiled.value = boardConfig.rows;
            colInputField.value = boardConfig.columns;
            mineInputField.value = boardConfig.minePositions.Count;
            GenerateGrid(Rows, Cols, true);
        }
        else
        {
            Debug.Log("BoardConfig is null");
        }
    }
}
