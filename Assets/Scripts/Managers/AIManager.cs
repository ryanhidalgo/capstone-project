using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    //constants for tile type
    public const int EMPTY = 0;
    public const int FOREST = 1;
    public const int MOUNTAIN = 2;
    public const int TOWER = 3;
    public const int WATER = 4;
    public const int MILL = 5;
    public const int QUARRY = 6;
    public const int FARM = 7;

    //ai behavior field
    private AI _ai;
    public AI ai
    {
        get {return _ai;}
    }

    public AIManager()
    {
        _ai = new AI();
    }

    public GameObject ClosestSearch(GameObject[,] grid, int type)
    {
        return ai.ClosestSearch(grid, type);
    }

    public GameObject FarthestSearch(GameObject[,] grid, int type)
    {
        return ai.FarthestSearch(grid, type);
    }

    public GameObject BestSearch(GameObject[,] grid, int type)
    {
        return ai.BestSearch(grid, type);
    }
}
