using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    //tile attributes
    private int _fertility;
    public int fertility
    {
        get {return _fertility;}
        set {_fertility = value;}
    }
    private int _roughness;
    public int roughness 
    {
        get {return _roughness;}
        set {_roughness = value;}
    }

    //constructors
    public Tile()
    {
        this._fertility = 0;
        this._roughness = 0;
    }
    public Tile(int fert, int rough)
    {
        this._fertility = fert;
        this._roughness = rough;
    }
}
