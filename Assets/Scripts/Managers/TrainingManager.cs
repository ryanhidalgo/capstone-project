using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] GameObject[] moduleCanvases;
    [SerializeField] Dropdown moduleDropdown;
    [SerializeField] GameObject buildingCanvas;
    [SerializeField] List<Dropdown> propDropdowns;
    [SerializeField] InputField propQuantityField;

    List<PrimaryModule> allPrimaries;
    List<PrimaryModule> completePrimaries;
    
    PrimaryModule selectedModule;

    bool loadingModule;

    void Start()
    {
        LoadOption();

        allPrimaries = LoadAllPrimaries();

        loadingModule = false;
    }

    //enables/disables module canvases based on dropdown
    public void LoadOption()
    {
        for (int canvasIndex = 0; canvasIndex < moduleCanvases.Length; canvasIndex++)
        {
            //value stores index of currently select option
            //if loop index equals value, set that (and only that canvas) to be active
            if (canvasIndex == moduleDropdown.value)
            {
                moduleCanvases[canvasIndex].SetActive(true);
            }
            else
            {
                moduleCanvases[canvasIndex].SetActive(false);
            }
        }
    }

    //loads all primary modules from the building canvas
    private List<PrimaryModule> LoadAllPrimaries()
    {
        List<PrimaryModule> primaryList = new List<PrimaryModule>();

        for (int primaryIndex = 0; primaryIndex < buildingCanvas.transform.childCount;
            primaryIndex++)
        {
            Transform childTransform = buildingCanvas.transform.GetChild(primaryIndex);
            primaryList.Add(childTransform.gameObject.GetComponent<PrimaryModule>());
        }

        return primaryList;
    }
    private List<PrimaryModule> LoadCompletedPrimaries()
    {
        List<PrimaryModule> primaryList = new List<PrimaryModule>();

        for (int primaryIndex = 0; primaryIndex < buildingCanvas.transform.childCount;
            primaryIndex++)
        {
            Transform childTransform = buildingCanvas.transform.GetChild(primaryIndex);
            PrimaryModule childPM = childTransform.gameObject.GetComponent<PrimaryModule>();

            if (IsCompletePrimary(childPM))
            {
                primaryList.Add(childPM);
            }
        }

        return primaryList;
    }

    //sets given primary module as selected module and then loads its values
    public void SelectModule(PrimaryModule pm)
    {
        selectedModule = pm;

        LoadValues();

        const int RESOURCE_DROP = 1;
        const int OP_DROP = 2;
        if (pm.lType == SceneController.LogicType.Repeat)
        { 
            //hide resource type and operator dropdowns since they do not affect repeat logic
            propDropdowns[RESOURCE_DROP].gameObject.SetActive(false);
            propDropdowns[OP_DROP].gameObject.SetActive(false);
        }
        else
        {
            //show resource type and operator dropdowns since they affect if and while logic
            propDropdowns[RESOURCE_DROP].gameObject.SetActive(true);
            propDropdowns[OP_DROP].gameObject.SetActive(true);
        }
    }
    //removes currently selected module
    public void RemoveSelectedModule()
    {
        Destroy(selectedModule.gameObject);
        selectedModule = null;
    }
    //removes all modules with completed instructions
    public void RemoveModules(List<bool> readyToRemove)
    {
        //since this is before instructions is trimmed, it is the same size as completedPrimaries
        //thus, readyToRemove also correlates to completePrimaries
        for (int index = readyToRemove.Count - 1; index >= 0; index--)
        {
            if (readyToRemove[index])
            {
                //first destroy game object, then remove from list
                Destroy(completePrimaries[index].gameObject);
                completePrimaries.RemoveAt(index);
            }
        }
    }

    //loads values from selected module into property interactives
    public void LoadValues()
    {
        const int LOGIC_DROP = 0;
        const int RESOURCE_DROP = 1;
        const int OP_DROP = 2;
        const int SEARCH_DROP = 3;

        loadingModule = true;

        //get values from selected module
        SceneController.LogicType selLType = selectedModule.lType;
        SceneController.ResourceType selRType = selectedModule.rType;
        if (selectedModule.compOperator == null)
        {
            selectedModule.compOperator = "<";
        }
        string selOp = selectedModule.compOperator;
        int selQuant = selectedModule.quantity;
        SceneController.SearchType selSType = SceneController.SearchType.Closest;
        if (selectedModule.searchModule != null)
        {
            selSType = selectedModule.searchModule.sType;
        }

        //store values in properties interactives
        propDropdowns[LOGIC_DROP].value = UpdateLogicDropdown(selLType);
        propDropdowns[RESOURCE_DROP].value = UpdateResourceDropdown(selRType);
        propDropdowns[OP_DROP].value = UpdateOperatorDropdown(selOp);
        propQuantityField.text = UpdateQuantityField(selQuant);
        if (selectedModule.searchModule != null)
        {
            propDropdowns[SEARCH_DROP].value = UpdateSearchDropdown(selSType);
        }
        
        loadingModule = false;
    }
    private int UpdateLogicDropdown(SceneController.LogicType lType)
    {
        const int IF = 0;
        const int WHILE = 1;
        const int REPEAT = 2;
        const int FAIL = -1;

        switch(lType)
        {
            case SceneController.LogicType.If:
            {
                return IF;
            }
            case SceneController.LogicType.While:
            {
                return WHILE;
            }
            case SceneController.LogicType.Repeat:
            {
                return REPEAT;
            }
            default:
            {
                return FAIL;
            }
        }
    }
    private int UpdateResourceDropdown(SceneController.ResourceType rType)
    {
        const int WOOD = 0;
        const int STONE = 1;
        const int FOOD = 2;
        const int FAIL = -1;

        switch(rType)
        {
            case SceneController.ResourceType.Wood:
            {
                return WOOD;
            }
            case SceneController.ResourceType.Stone:
            {
                return STONE;
            }
            case SceneController.ResourceType.Food:
            {
                return FOOD;
            }
            default:
            {
                return FAIL;
            }
        }
    }
    private int UpdateOperatorDropdown(string op)
    {
        const int LESS_THAN = 0;
        const int GREATER_THAN = 1;
        const int EQUAL_TO = 2;
        const int LESS_THAN_OR_EQUAL_TO = 3;
        const int GREATER_THAN_OR_EQUAL_TO = 4;
        const int FAIL = -1;

        switch(op)
        {
            case "<":
            {
                return LESS_THAN;
            }
            case ">":
            {
                return GREATER_THAN;
            }
            case "=":
            {
                return EQUAL_TO;
            }
            case "<=":
            {
                return LESS_THAN_OR_EQUAL_TO;
            }
            case ">=":
            {
                return GREATER_THAN_OR_EQUAL_TO;
            }
            default:
            {
                return FAIL;
            }
        }
    }
    private string UpdateQuantityField(int q)
    {
        return q.ToString();
    }
    private int UpdateSearchDropdown(SceneController.SearchType sType)
    {
        const int CLOSEST = 0;
        const int FARTHEST = 1;
        const int BEST = 2;
        const int FAIL = -1;

        switch (sType)
        {
            case SceneController.SearchType.Closest:
            {
                return CLOSEST;
            }
            case SceneController.SearchType.Farthest:
            {
                return FARTHEST;
            }
            case SceneController.SearchType.Best:
            {
                return BEST;
            }
            default:
            {
                return FAIL;
            }
        }
    }

    //loads values from property interactives into selected module
    public void UpdateValues()
    {
        const int LOGIC_DROP = 0;
        const int RESOURCE_DROP = 1;
        const int OP_DROP = 2;
        const int SEARCH_DROP = 3;

        //function is called anytime interactives are modified
        //this prevents them from changing selected module during LoadValues()
        if (loadingModule)
        {
            return;
        }

        //get values from properties interactives
        SceneController.LogicType propLType = LoadLogicDropdown(propDropdowns[LOGIC_DROP].value);
        SceneController.ResourceType propRType = LoadResourceDropdown(propDropdowns[RESOURCE_DROP].value);
        string propOp = LoadOperatorDropdown(propDropdowns[OP_DROP].value);
        int propQuant = LoadQuantityField(propQuantityField.text);
        SceneController.SearchType propSType = LoadSearchDropdown(propDropdowns[SEARCH_DROP].value);

        //store values in selected module
        selectedModule.lType = propLType;
        selectedModule.rType = propRType;
        selectedModule.compOperator = propOp;
        selectedModule.quantity = propQuant;
        if (selectedModule.searchModule != null)
        {
            selectedModule.searchModule.sType = propSType;
        }
    }
    private SceneController.LogicType LoadLogicDropdown(int value)
    {
        const int IF = 0;
        const int WHILE = 1;
        const int REPEAT = 2;

        switch (value)
        {
            case IF:
            {
                return SceneController.LogicType.If;
            }
            case WHILE:
            {
                return SceneController.LogicType.While;
            }
            case REPEAT:
            {
                return SceneController.LogicType.Repeat;
            }
            default:
            {
                return SceneController.LogicType.If;
            }
        }
    }
    private SceneController.ResourceType LoadResourceDropdown(int value)
    {
        const int WOOD = 0;
        const int STONE = 1;
        const int FOOD = 2;

        switch (value)
        {
            case WOOD:
            {
                return SceneController.ResourceType.Wood;
            }
            case STONE:
            {
                return SceneController.ResourceType.Stone;
            }
            case FOOD:
            {
                return SceneController.ResourceType.Food;
            }
            default:
            {
                return SceneController.ResourceType.Wood;
            }
        }
    }
    private string LoadOperatorDropdown(int value)
    {
        const int LESS_THAN = 0;
        const int GREATER_THAN = 1;
        const int EQUAL_TO = 2;
        const int LESS_THAN_OR_EQUAL_TO = 3;
        const int GREATER_THAN_OR_EQUAL_TO = 4;
        
        switch (value)
        {
            case LESS_THAN:
            {
                return "<";
            }
            case GREATER_THAN:
            {
                return ">";
            }
            case EQUAL_TO:
            {
                return "=";
            }
            case LESS_THAN_OR_EQUAL_TO:
            {
                return "<=";
            }
            case GREATER_THAN_OR_EQUAL_TO:
            {
                return ">=";
            }
            default:
            {
                return "<";
            }
        }
    }
    private int LoadQuantityField(string text)
    {
        return int.Parse(text);
    }
    private SceneController.SearchType LoadSearchDropdown(int value)
    {
        const int CLOSEST = 0;
        const int FARTHEST = 1;
        const int BEST = 2;

        switch (value)
        {
            case CLOSEST:
            {
                return SceneController.SearchType.Closest;
            }
            case FARTHEST:
            {
                return SceneController.SearchType.Farthest;
            }
            case BEST:
            {
                return SceneController.SearchType.Best;
            }
            default:
            {
                return SceneController.SearchType.Closest;
            }
        }
    }

    public List<SceneController.ModuleFields> GenerateTraining()
    {
        completePrimaries = LoadCompletedPrimaries();
        List<SceneController.ModuleFields> instructions = new List<SceneController.ModuleFields>();

        //check if primary module is completed (has secondary with SearchType and tertiary)
        for (int primaryIndex = 0; primaryIndex < completePrimaries.Count;
            primaryIndex++)
        {
            instructions.Add(completePrimaries[primaryIndex].GenerateTraining());
        }

        return instructions;
    }
    private bool IsCompletePrimary(PrimaryModule module)
    {
        bool result = false;

        //check that primary module has been initialized
        if (module != null)
        {
            //check that nullable types and secondary module have been initialized
            if (module.compOperator != null && 
                module.searchModule != null)
            {
                //check that tertiary module has been initialized
                if (module.searchModule.actionModule != null)
                {
                    result = true;
                }
            }
        }

        return result;
    }
}
