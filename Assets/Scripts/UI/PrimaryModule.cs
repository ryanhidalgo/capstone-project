using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryModule : MonoBehaviour
{
    SecondaryModule _searchModule;
    public SecondaryModule searchModule
    {
        get { return _searchModule; }
        set { _searchModule = value; }
    }

    SceneController.LogicType _lType;
    public SceneController.LogicType lType
    {
        get { return _lType; }
        set { _lType = value; }
    }
    SceneController.ResourceType _rType;
    public SceneController.ResourceType rType
    {
        get { return _rType; }
        set { _rType = value; }
    }
    string _compOperator;
    public string compOperator
    {
        get { return _compOperator; }
        set { _compOperator = value; }
    }
    int _quantity;
    public int quantity
    {
        get { return _quantity; }
        set { _quantity = value; }
    }

    public SceneController.ModuleFields GenerateTraining()
    {
        return new SceneController.ModuleFields(lType, rType, compOperator, quantity, searchModule.sType, 
            searchModule.actionModule.action);
    }
}
