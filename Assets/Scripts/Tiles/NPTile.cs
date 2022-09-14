using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPTile : MonoBehaviour
{
    //define enum for tileType
    public enum TileType
    {
        Empty,
        Forest,
        Mountain,
        Tower,
        Water
    }

    //NPTile attributes
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
