using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryModule : MonoBehaviour
{
    TertiaryModule _actionModule;
    public TertiaryModule actionModule
    {
        get { return _actionModule; }
        set { _actionModule = value; }
    }

    SceneController.SearchType _sType;
    public SceneController.SearchType sType
    {
        get { return _sType; }
        set { _sType = value; }
    }
}
