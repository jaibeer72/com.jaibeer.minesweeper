using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MinesweeperWrapper
{
    const string libraryName = "libMinesweeperLibrary"; 

    [DllImport(libraryName)]
    private static extern IntPtr Minesweeper_Create(int rows, int cols, int mines);

    [DllImport(libraryName)]
    private static extern IntPtr Minesweeper_CreateCustom(int rows, int cols, int[] mineLocations, int size);

    [DllImport(libraryName)]
    private static extern void Minesweeper_Destroy(IntPtr minesweeper);

    [DllImport(libraryName)]
    private static extern void Minesweeper_ResetBoard(IntPtr minesweeper, bool samePos);

    [DllImport(libraryName)]
    private static extern void Minesweeper_RevealCell(IntPtr minesweeper, int row, int col, int[] revealedNumberedCells, ref int size, int[] revealedEmptyCells, ref int size2);

    [DllImport(libraryName)]
    private static extern bool Minesweeper_IsMine(IntPtr minesweeper, int row, int col);

    [DllImport(libraryName)]
    private static extern bool Minesweeper_IsOpen(IntPtr minesweeper, int row, int col);

    [DllImport(libraryName)]
    private static extern void Minesweeper_SetCustomeMines(IntPtr minesweeper, int rows, int cols, int[] mineLocations, int size);

    [DllImport(libraryName)]
    private static extern bool Minesweeper_GetIfTileIsNumbered(IntPtr minesweeper, int row, int cols, ref int num); 

    private IntPtr minesweeperInstance;

    public MinesweeperWrapper(int rows, int cols, int mines)
    {
        minesweeperInstance = Minesweeper_Create(rows, cols, mines);
    }

    public MinesweeperWrapper(int rows, int cols, List<Vector2Int> mineLocations)
    {
        int[] mineLocationsArray = new int[mineLocations.Count * 2];
        for (int i = 0; i < mineLocations.Count; i++)
        {
            mineLocationsArray[i * 2] = mineLocations[i].x;
            mineLocationsArray[i * 2 + 1] = mineLocations[i].y;
        }
        minesweeperInstance = Minesweeper_CreateCustom(rows, cols, mineLocationsArray, mineLocations.Count);
    }

    ~MinesweeperWrapper()
    {
        Minesweeper_Destroy(minesweeperInstance);
    }

    public void ResetBoard(bool samePos)
    {
        Minesweeper_ResetBoard(minesweeperInstance, samePos);
    }

    public void RevealCell(int row, int col , out List<(int, int)> NumberedCellCoordantes , out List<(int,int)> emptyCellCoprds)
    {
        int size = 0;
        int[] revealedCellsNumbered = new int[100]; // Allocate an array of sufficient size

        int size2 = 0;
        int[] revealCellsNonNumbered = new int[100];

        Minesweeper_RevealCell(minesweeperInstance, row, col, revealedCellsNumbered, ref size, revealCellsNonNumbered, ref size2);

        NumberedCellCoordantes = new List<(int, int)>();
        for (int i = 0; i < size; i++)
        {
            NumberedCellCoordantes.Add((revealedCellsNumbered[i * 2], revealedCellsNumbered[i * 2 + 1]));
        }

        emptyCellCoprds = new List<(int, int)>();
        for(int i = 0; i < size2; i++)
        {
            emptyCellCoprds.Add((revealCellsNonNumbered[i * 2], revealCellsNonNumbered[i * 2 + 1]));
        }
    }

    public bool IsMine(int row, int col)
    {
        return Minesweeper_IsMine(minesweeperInstance, row, col);
    }

    public bool IsOpen(int row, int col)
    {
        return Minesweeper_IsOpen(minesweeperInstance, row, col);
    }

    public void SetCustomMines(int rows, int cols, List<(int, int)> mineLocations)
    {
        int[] mineLocationsArray = new int[mineLocations.Count * 2];
        for (int i = 0; i < mineLocations.Count; i++)
        {
            mineLocationsArray[i * 2] = mineLocations[i].Item1;
            mineLocationsArray[i * 2 + 1] = mineLocations[i].Item2;
        }
        Minesweeper_SetCustomeMines(minesweeperInstance, rows, cols, mineLocationsArray, mineLocations.Count);
    }

    public bool GetNumIfTileIsNumberd (int row , int col , out int num)
    {
        num = 0; 
        return Minesweeper_GetIfTileIsNumbered(minesweeperInstance, row, col, ref num);
    }
}


