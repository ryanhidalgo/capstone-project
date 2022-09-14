using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //resource totals
    private int _stoneTotal;
    public int stoneTotal
    {
        get {return _stoneTotal;}
        set {_stoneTotal = value;}
    }
    private int _woodTotal;
    public int woodTotal
    {
        get {return _woodTotal;}
        set {_woodTotal = value;}
    }
    private int _foodTotal;
    public int foodTotal
    {
        get {return _foodTotal;}
        set {_foodTotal = value;}
    }

    //production rates
    private int _stoneProd;
    public int stoneProd
    {
        get {return _stoneProd;}
        set {_stoneProd = value;}
    }
    private int _woodProd;
    public int woodProd
    {
        get {return _woodProd;}
        set {_woodProd = value;}
    }
    private int _foodProd;
    public int foodProd
    {
        get {return _foodProd;}
        set {_foodProd = value;}
    }

    //iterates over grid, adding individual tiles' production rates into overall production rate
    public void UpdateProduction()
    {
        stoneProd = 0;
        woodProd = 0;
        foodProd = 0;

        for (int lenIndex = 0; lenIndex < SceneController.gridLength; lenIndex++)
        {
            for (int heightIndex = 0; heightIndex < SceneController.gridLength;
                heightIndex++)
            {
                GameObject currentElement = SceneController.grid[lenIndex, heightIndex];
                //attempts to get PTile into variable prodElement. if it fails, if-statement returns false
                if (currentElement.TryGetComponent(out PTile prodElement))
                {
                    //checks tileType and updates corresponding resource production
                    if (prodElement.tileType == PTile.TileType.Quarry)
                        stoneProd += prodElement.prodRate;
                    else if (prodElement.tileType == PTile.TileType.Mill)
                        woodProd += prodElement.prodRate;
                    else if (prodElement.tileType == PTile.TileType.Farm)
                        foodProd += prodElement.prodRate;
                }
            }
        }
    }

    //updates resource totals by adding production rate
    public void UpdateTotal()
    {
        stoneTotal += stoneProd;
        woodTotal += woodProd;
        foodTotal += foodProd;
    }
}
