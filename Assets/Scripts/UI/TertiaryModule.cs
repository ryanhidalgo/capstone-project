using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TertiaryModule : MonoBehaviour
{
    [SerializeField] SceneController.Action _action;
    public SceneController.Action action
    {
        get { return _action; }
    }
}
