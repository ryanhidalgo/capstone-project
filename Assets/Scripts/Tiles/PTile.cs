using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTile : MonoBehaviour
{
    //define TileType
    public enum TileType
    {
        Mill,
        Farm,
        Quarry
    }

    //tile attributes
    private int _prodRate;
    public int prodRate
    {
        get {return _prodRate;}
        set {_prodRate = value;}
    }
    [SerializeField]private TileType _tileType;
    public TileType tileType
    {
        get {return _tileType;}
    }
    private Tile _tile;
    public Tile tile
    {
        get{return _tile;}
        set{_tile = value;}
    }
    public int fertility
    {
        get{return _tile.fertility;}
        set{_tile.fertility = value;}
    }
    public int roughness
    {
        get{return _tile.roughness;}
        set{_tile.roughness = value;}
    }

    /*
        calculate production rate
    */
    public int CalcProdRate(int fertility, int roughness, TileType type)
    {
        const int BASE_RATE = 100;
        float result = 0.0f;
        if (type == TileType.Mill)
        {
            result = Mathf.Floor(BASE_RATE + (fertility / 2) - (roughness / 2));
        }
        else if (type == TileType.Farm)
        {
            result = Mathf.Floor(BASE_RATE + fertility - (roughness / 2));
        }
        else if (type == TileType.Quarry)
        {
            result = Mathf.Floor(BASE_RATE + roughness - (fertility / 2));
        }

        //use of floor function ensures that no data is lost during int conversion
        return (int)result;
    }

    void OnMouseDown()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast (ray, out hit))
        {
            SceneController.SelectTile(hit);
        }
    }
}
