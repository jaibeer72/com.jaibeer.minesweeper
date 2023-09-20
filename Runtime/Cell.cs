using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential)]
public struct Cell
{
    public int Row, Col;
    public int NeighborMineCount;
    public bool IsOpen;
    public bool IsMine;
}
