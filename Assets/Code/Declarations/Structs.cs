using UnityEngine;
public struct Table
{
    public Vector3 StartPoint;
    public int Rows;
    public int Columns;

    public Table(Vector3 startPoint, int rows, int columns)
    {
        StartPoint = startPoint;
        Rows = rows;
        Columns = columns;
    }
}