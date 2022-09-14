using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    //stores tiles that have already been visited
    private bool[,] visited;

    //list of currently visited tiles
    private List<GameObject> current;

    private int gridLength = SceneController.gridLength;
    private float tileSize = SceneController.tileSize;

    //searches for tile of given type that is closest to tower
    public GameObject ClosestSearch(GameObject[,] grid, int type)
    {
        visited = new bool[gridLength, gridLength];
        current = new List<GameObject>();

        //start at AI tower (which is in center of grid)
        int center = gridLength / 2;
        int centerDisplacement = 0;
        GameObject result = null;
        while (result == null && centerDisplacement < (gridLength / 2))
        {
            //clear set of current tiles
            current.Clear();

            //adjust range of search
            int min = center - centerDisplacement;
            int max = center + centerDisplacement;
            //search over range
            for (int xIndex = min; xIndex <= max; xIndex++)
            {
                for (int zIndex = min; zIndex <= max; zIndex++)
                {
                    if (!visited[xIndex, zIndex])
                    {
                        VisitTile(xIndex, zIndex, ref visited, ref current);
                    }
                }
            }

            result = CheckCurrent(current, type);
            centerDisplacement++;
        }

        return result;
    }
    //searches for tile of given tile that is farthest away from tower
    public GameObject FarthestSearch(GameObject[,] grid, int type)
    {
        visited = new bool[gridLength, gridLength];
        current = new List<GameObject>();

        //use AI tower as reference
        int center = gridLength / 2;
        GameObject result = null;
        int edgeDisplacement = 0;
        while (result == null && edgeDisplacement < (gridLength / 2))
        {
            //clear set of current tiles
            current.Clear();

            //adjust range of search
            int edge = gridLength - edgeDisplacement - 1;
            //search moves counterclockwise around grid (bottom, right, top, left)
            int xIndex = edgeDisplacement;
            int zIndex = edgeDisplacement;
            //bottom edge first
            for (xIndex = edgeDisplacement; xIndex < edge; xIndex++)
            {
                if (!visited[xIndex, zIndex])
                {
                    VisitTile(xIndex, zIndex, ref visited, ref current);
                }
            }
            //right edge
            for (zIndex = edgeDisplacement; zIndex < edge; zIndex++)
            {
                if (!visited[xIndex, zIndex])
                {
                    VisitTile(xIndex, zIndex, ref visited, ref current);
                }
            }
            //top edge
            for (xIndex = edge; xIndex > edgeDisplacement; xIndex--)
            {
                if (!visited[xIndex, zIndex])
                {
                    VisitTile(xIndex, zIndex, ref visited, ref current);
                }
            }
            //left edge
            for (zIndex = edge; zIndex > edgeDisplacement; zIndex--)
            {
                if (!visited[xIndex, zIndex])
                {
                    VisitTile(xIndex, zIndex, ref visited, ref current);
                }
            }
            edgeDisplacement++;
            result = CheckCurrent(current, type);
        }

        return result;
    }
    //searches entire grid for tile that yields highest production
    public GameObject BestSearch(GameObject[,] grid, int type)
    {
        GameObject result = null;
        //linear search over grid
        for (int xIndex = 0; xIndex < gridLength; xIndex++)
        {
            for (int zIndex = 0; zIndex < gridLength; zIndex++)
            {
                //first try to get NPTile script
                if (grid[xIndex, zIndex].TryGetComponent(out NPTile npTile))
                {
                    switch (type)
                    {
                        case AIManager.EMPTY:
                        {
                            if (npTile.tileType == NPTile.TileType.Empty)
                            {
                                //replace null result with current tile
                                if (result == null)
                                {
                                    result = grid[xIndex, zIndex];
                                }
                                //check if current tile is better than result
                                else if (result.TryGetComponent(out NPTile resultNP) &&
                                    isBetter(grid[xIndex, zIndex], result, type))
                                {
                                    result = grid[xIndex, zIndex];
                                }
                            }
                            break;
                        }
                        case AIManager.FOREST:
                        {
                            if (npTile.tileType == NPTile.TileType.Forest)
                            {
                                //replace null result with current tile
                                if (result == null)
                                {
                                    result = grid[xIndex, zIndex];
                                }
                                //check if current tile is better than result
                                else if (result.TryGetComponent(out NPTile resultNP) &&
                                    isBetter(grid[xIndex, zIndex], result, type))
                                {
                                    result = grid[xIndex, zIndex];
                                }
                            }
                            break;
                        }
                        case AIManager.MOUNTAIN:
                        {
                            if (npTile.tileType == NPTile.TileType.Mountain)
                            {
                                //replace null result with current tile
                                if (result == null)
                                {
                                    result = grid[xIndex, zIndex];
                                }
                                //check if current tile is better than result
                                else if (result.TryGetComponent(out NPTile resultNP) &&
                                    isBetter(grid[xIndex, zIndex], result, type))
                                {
                                    result = grid[xIndex, zIndex];
                                }
                            }
                            break;
                        }
                        case AIManager.WATER:
                        case AIManager.TOWER:
                        {
                            if (npTile.tileType == NPTile.TileType.Water ||
                                npTile.tileType == NPTile.TileType.Tower)
                            {
                                //since water and tower tiles never produce, simply return current
                                return grid[xIndex, zIndex];
                            }
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }
                //then try to get PTile script
                else if (grid[xIndex, zIndex].TryGetComponent(out PTile pTile))
                {
                    switch (type)
                    {
                        case AIManager.MILL:
                        {
                            if (npTile.tileType == NPTile.TileType.Mountain)
                            {
                                //replace null result with current tile
                                if (result == null)
                                {
                                    result = grid[xIndex, zIndex];
                                }
                                //check if current tile is better than result
                                else if (result.TryGetComponent(out NPTile resultNP) &&
                                    isBetter(grid[xIndex, zIndex], result, type))
                                {
                                    result = grid[xIndex, zIndex];
                                }
                            }
                            break;
                        }
                        case AIManager.QUARRY:
                        {
                            if (npTile.tileType == NPTile.TileType.Mountain)
                            {
                                //replace null result with current tile
                                if (result == null)
                                {
                                    result = grid[xIndex, zIndex];
                                }
                                //check if current tile is better than result
                                else if (result.TryGetComponent(out NPTile resultNP) &&
                                    isBetter(grid[xIndex, zIndex], result, type))
                                {
                                    result = grid[xIndex, zIndex];
                                }
                            }
                            break;
                        }
                        case AIManager.FARM:
                        {
                            if (npTile.tileType == NPTile.TileType.Mountain)
                            {
                                //replace null result with current tile
                                if (result == null)
                                {
                                    result = grid[xIndex, zIndex];
                                }
                                //check if current tile is better than result
                                else if (result.TryGetComponent(out NPTile resultNP) &&
                                    isBetter(grid[xIndex, zIndex], result, type))
                                {
                                    result = grid[xIndex, zIndex];
                                }
                            }
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }

    //helper method that visits tile
    private void VisitTile(int x, int z, ref bool[,] visit, ref List<GameObject> cur)
    {
        visit[x, z] = true;
        cur.Add(SceneController.grid[x,z]);
    }
    //helper method that checks current tiles for desired type
    private GameObject CheckCurrent(List<GameObject> cur, int type)
    {
        for (int index = 0; index < cur.Count; index++)
        {
            GameObject curTile = cur[index];
            switch(type)
            {
                case AIManager.EMPTY:
                {
                    if (curTile.TryGetComponent(out NPTile curNP))
                    {
                        if (curNP.tileType == NPTile.TileType.Empty)
                        {
                            return curTile;
                        }
                    }
                    break;
                }
                case AIManager.FOREST:
                {
                    if (curTile.TryGetComponent(out NPTile curNP))
                    {
                        if (curNP.tileType == NPTile.TileType.Forest)
                        {
                            return curTile;
                        }
                    }
                    break;
                }
                case AIManager.MOUNTAIN:
                {
                    if (curTile.TryGetComponent(out NPTile curNP))
                    {
                        if (curNP.tileType == NPTile.TileType.Mountain)
                        {
                            return curTile;
                        }
                    }
                    break;
                }
                case AIManager.TOWER:
                {
                    if (curTile.TryGetComponent(out NPTile curNP))
                    {
                        if (curNP.tileType == NPTile.TileType.Tower)
                        {
                            return curTile;
                        }
                    }
                    break;
                }
                case AIManager.WATER:
                {
                    if (curTile.TryGetComponent(out NPTile curNP))
                    {
                        if (curNP.tileType == NPTile.TileType.Water)
                        {
                            return curTile;
                        }
                    }
                    break;
                }
                case AIManager.MILL:
                {
                    if (curTile.TryGetComponent(out PTile curP))
                    {
                        if (curP.tileType == PTile.TileType.Mill)
                        {
                            return curTile;
                        }
                    }
                    break;
                }
                case AIManager.QUARRY:
                {
                    if (curTile.TryGetComponent(out PTile curP))
                    {
                        if (curP.tileType == PTile.TileType.Quarry)
                        {
                            return curTile;
                        }
                    }
                    break;
                }
                case AIManager.FARM:
                {
                    if (curTile.TryGetComponent(out PTile curP))
                    {
                        if (curP.tileType == PTile.TileType.Farm)
                        {
                            return curTile;
                        }
                    }
                    break;
                }
                default:
                {
                    //invalid value for type given
                    return null;
                }
            }
        }
        //search failed, return null
        return null;
    }
    //helper to determine if first argument is better tile than second
    private bool isBetter(GameObject first, GameObject second, int type)
    {
        //check if tiles are of same type (NPTile vs PTile)
        if (first.TryGetComponent(out NPTile firstNP) && 
            second.TryGetComponent(out NPTile secondNP))
        {
            switch(type)
            {
                case AIManager.EMPTY:
                {
                    if (firstNP.tileType == NPTile.TileType.Empty &&
                        secondNP.tileType == NPTile.TileType.Empty)
                    {
                        //compare potential production rates
                        return (CalcProdRate(firstNP) >= CalcProdRate(secondNP));
                    }
                    else
                    {
                        //type mismatch
                        return false;
                    }
                }
                case AIManager.FOREST:
                {
                    if (firstNP.tileType == NPTile.TileType.Forest &&
                        secondNP.tileType == NPTile.TileType.Forest)
                    {
                        //compare potential production rates
                        return (CalcProdRate(firstNP) >= CalcProdRate(secondNP));
                    }
                    else
                    {
                        //type mismatch
                        return false;
                    }
                }
                case AIManager.MOUNTAIN:
                {
                    if (firstNP.tileType == NPTile.TileType.Mountain &&
                        secondNP.tileType == NPTile.TileType.Mountain)
                    {
                        //compare potential production rates
                        return (CalcProdRate(firstNP) >= CalcProdRate(secondNP));
                    }
                    else
                    {
                        //type mismatch
                        return false;
                    }
                }
                case AIManager.WATER:
                case AIManager.TOWER:
                {
                    if ((firstNP.tileType == NPTile.TileType.Water &&
                        secondNP.tileType == NPTile.TileType.Water) ||
                        (firstNP.tileType == NPTile.TileType.Tower &&
                        secondNP.tileType == NPTile.TileType.Tower))
                    {
                        return true;
                    }
                    else
                    {
                        //type mismatch
                        return false;
                    }
                }
                default:
                {
                    return false;
                }
            }
        }
        else if (first.TryGetComponent(out PTile firstP) &&
            second.TryGetComponent(out PTile secondP))
        {
            switch (type)
            {
                case AIManager.MILL:
                {
                    if (firstP.tileType == PTile.TileType.Mill &&
                        secondP.tileType == PTile.TileType.Mill)
                    {
                        if (firstP.prodRate >= secondP.prodRate)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //type mismatch
                        return false;
                    }
                }
                case AIManager.QUARRY:
                {
                    if (firstP.tileType == PTile.TileType.Quarry &&
                        secondP.tileType == PTile.TileType.Quarry)
                    {
                        if (firstP.prodRate >= secondP.prodRate)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //type mismatch
                        return false;
                    }
                }
                case AIManager.FARM:
                {
                    if (firstP.tileType == PTile.TileType.Farm &&
                        secondP.tileType == PTile.TileType.Farm)
                    {
                        if (firstP.prodRate >= secondP.prodRate)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //type mismatch
                        return false;
                    }
                }
                default:
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }
    //helper to calculate potential production rates for NPTiles
    private int CalcProdRate(NPTile np)
    {
        const int BASE_RATE = 100;
        float result = 0.0f;
        if (np.tileType == NPTile.TileType.Forest)
        {
            result = Mathf.Floor(BASE_RATE + (np.fertility / 2) - (np.roughness / 2));
        }
        else if (np.tileType == NPTile.TileType.Empty)
        {
            result = Mathf.Floor(BASE_RATE + np.fertility - (np.roughness / 2));
        }
        else if (np.tileType == NPTile.TileType.Mountain)
        {
            result = Mathf.Floor(BASE_RATE + np.roughness - (np.fertility / 2));
        }

        //use of floor function ensures that no data is lost during int conversion
        return (int)result;
    }
}
