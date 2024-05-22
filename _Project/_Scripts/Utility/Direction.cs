using UnityEngine;
using System.Collections.Generic;

public struct Direction
{
    public int X;
    public int Y;
    public int Z;

    public Direction(int x, int y, int z)
    {
        _ = Mathf.Sqrt(x * x + y * y + z * z);
        X = x;
        Y = y;
        Z = z;
    }
    
    public static implicit operator Vector3(Direction direction) => new(direction.X, direction.Y, direction.Z);
    public static implicit operator Direction(Vector3 vector) => new((int)vector.x, (int)vector.y, (int)vector.z);

    public static readonly Direction None = new(0, 0, 0);
    public static readonly Direction North = new(0, 0, 1);
    public static readonly Direction NorthEast = new(1, 0, 1);
    public static readonly Direction East = new(1, 0, 0);
    public static readonly Direction SouthEast = new(1, 0, -1);
    public static readonly Direction South = new(0, 0, -1);
    public static readonly Direction SouthWest = new(-1, 0, -1);
    public static readonly Direction West = new(-1, 0, 0);
    public static readonly Direction NorthWest = new(-1, 0, 1);

    public static Direction Forward => North;
    public static Direction Right => East;
    public static Direction Backward => South;
    public static Direction Left => West;


    private static readonly List<Direction> Directions = new();

}