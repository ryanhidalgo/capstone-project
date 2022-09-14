using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    //grid fields
    public static int gridLength = 11;
    public static float tileSize = 5.0f;
    public static GameObject[,] grid;
    public static GameObject currentTile;

    //highlight fields
    [SerializeField] private Highlight highlight;
    private static Highlight highlightObject;

    //serialized prefab fields
    [SerializeField] private GameObject[] npTilePrefabs;
    [SerializeField] private GameObject[] pTilePrefabs;
    [SerializeField] private GameObject towerTile;

    //manager fields
    private ResourceManager rm;
    private AIManager am;
    [SerializeField] UIManager um;

    //movement field
    [SerializeField] CameraMovement cm;

    //menu flags
    private bool paused;
    private bool training;

    //stores instructions to ai
    private List<ModuleFields> instructions;

    //coroutine variables to discourage pause spamming
    private IEnumerator totalCoRout;
    private IEnumerator prodCoRout;

    //resource update fields
    private const float WAIT_FOR_TOTAL = 5.0f; //wait 30 sec to update totals
    private const float WAIT_FOR_PROD = 10.0f; //wait 5 sec to update production rates

    //resource cost fields
    private const int QUARRY_COST = 500;
    private const int FARM_COST = 500;
    private const int MILL_COST = 500;

    //define enum for type of logic structure to be used in primary module
    public enum LogicType
    {
        If,
        While,
        Repeat
    }
    //define enum for type of resource to be used in primary module
    public enum ResourceType
    {
        Wood,
        Stone,
        Food
    }
    //define enum for type of search to be used in secondary module
    public enum SearchType
    {
        Closest,
        Farthest,
        Best
    }
    //define enum for whether action is to construct or revert building
    public enum Action
    {
        ConstructMill,
        ConstructQuarry,
        ConstructFarm,
        RevertMill,
        RevertQuarry,
        RevertFarm
    }

    //struct that holds parameters for training modules
    public struct ModuleFields
    {
        public ModuleFields(LogicType lT, ResourceType rT, string op, int q, SearchType sT, Action a)
        {
            _lType = lT;
            _rType = rT;
            _compOperator = op;
            _quantity = q;
            _sType = sT;
            _action = a;
        }

        private LogicType _lType;
        public LogicType lType
        {
            get{return _lType;}
        }
        private ResourceType _rType;
        public ResourceType rType
        {
            get{return _rType;}
        }
        private string _compOperator;
        public string compOperator
        {
            get{return _compOperator;}
        }
        private int _quantity;
        public int quantity
        {
            get{return _quantity;}
        }
        private SearchType _sType;
        public SearchType sType
        {
            get{return _sType;}
        }
        private Action _action;
        public Action action
        {
            get{return _action;}
        }
    }

    //called as soon as script is enabled. responsible for grid generation
    void Awake()
    {
        grid = new GameObject[gridLength, gridLength];

        //create grid of non-producing tiles
        for (int lenIndex = 0; lenIndex < SceneController.gridLength; lenIndex++)
        {
            for (int heightIndex = 0; heightIndex < SceneController.gridLength;
                heightIndex++)
            {
                //use random index to select random tile
                int prefabIndex = Random.Range(0, npTilePrefabs.Length);

                //instantiate tile
                GameObject tile = Instantiate(npTilePrefabs[prefabIndex]) as GameObject;

                //initialize values for NPTile
                NPTile npTile = tile.GetComponent<NPTile>();
                npTile.tile = new Tile();
                npTile = GenerateTileValues(npTile);
                
                //place tile and add to grid
                tile.transform.position = new Vector3(tileSize * lenIndex, 0, tileSize * heightIndex);
                grid[lenIndex, heightIndex] = tile;
            }
        }

        //replace center tile with tower tile
        int center = gridLength / 2;
        ReplaceTile(center, center, towerTile.gameObject);
    }

    void Start()
    {
        highlightObject = Instantiate(highlight, new Vector3(0, -1 , 0), Quaternion.identity) as Highlight;

        rm = GetComponent<ResourceManager>();
        am = GetComponent<AIManager>();

        paused = false;
        training = false;

        rm.woodTotal = 1000;
        rm.stoneTotal = 1000;
        rm.foodTotal = 1000;

        instructions = new List<ModuleFields>();

        totalCoRout = updateTotal();
        prodCoRout = updateProduction();

        StartCoroutine(totalCoRout);
        StartCoroutine(prodCoRout);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (paused)
            {
                DisablePause();
            }
            else
            {
                EnablePause();
            }
        }

        if (Input.GetButtonDown("Submit"))
        {
            if (training)
            {
                DisableTraining();
            }
            else
            {
                EnableTraining();
            }
        }

        if (instructions.Count > 0)
        {
            ExecuteTraining();
        }
    }

    //select tile on mouse down
    public static bool SelectTile(RaycastHit hit)
    {
        int xIndex = (int)(hit.transform.position.x / tileSize);
        int zIndex = (int)(hit.transform.position.z / tileSize);

        //update current tile to selection
        currentTile = grid[xIndex, zIndex];

        //create highlight gameobject around current tile
        CreateHighlight(xIndex, zIndex, highlightObject);

        //check if selection worked
        if (currentTile != null)
            return true;
        else
            return false;
    }
    //moves highlight around tile at given coordinates
    private static void CreateHighlight(int xIndex, int zIndex, Highlight highlight)
    {   
        float xPos = xIndex * tileSize;
        float zPos = zIndex * tileSize;
        highlightObject.transform.position = new Vector3 (xPos, 1, zPos);
    }

    //replace tile at grid index with newTile
    public static GameObject ReplaceTile(int xIndex, int zIndex, GameObject newTile)
    {
        //fetch current tile at specified index
        GameObject oldTile = grid[xIndex, zIndex];

        float xPos = 0.0f;
        float zPos = 0.0f;
        Tile oldT = null;

        if (oldTile != null)
        {
            //get global position
            xPos = oldTile.transform.position.x;
            zPos = oldTile.transform.position.z;
            
            //get either NPTile script or PTile script
            if (oldTile.TryGetComponent(out NPTile oldNP))
            {
                oldT = oldNP.tile;
            }
            else if (oldTile.TryGetComponent(out PTile oldP))
            {
                oldT = oldNP.tile;
            }

            //delete old tile
            Destroy(oldTile);
        }

        //check whether newTile is producing or non-producing
        GameObject tile;
        tile = Instantiate(newTile) as GameObject;
        if(tile.TryGetComponent(out NPTile newNP))
        {
            newNP.tile = oldT;
        }
        else if (tile.TryGetComponent(out PTile newP))
        {
            newP.tile = oldT;
        }
        tile.transform.position = new Vector3(xPos, 0, zPos);
        grid[xIndex, zIndex] = tile;

        return grid[xIndex, zIndex];
    }

    //helper to generate initial tile values. assumes non-producing tile
    private static NPTile GenerateTileValues(NPTile npTile)
    {
        const int MIN_VALUE = 0;
        const int MAX_VALUE = 100;

        //generate random fertility and roughness values based on tileType
        if (npTile.tileType == NPTile.TileType.Water || npTile.tileType == NPTile.TileType.Tower)
        {
            npTile.fertility = 0;
            npTile.roughness = 0;
            return npTile;
        }     
        else
        {
            npTile.fertility = Random.Range(MIN_VALUE, MAX_VALUE);
            npTile.roughness = Random.Range(MIN_VALUE, MAX_VALUE);
            return npTile;
        }
    }

    //handles case where primary module is an if statement
    public bool ifPrimary (ModuleFields modFie)
    {
        //load data from modFie
        ResourceType rT = modFie.rType;
        string op = modFie.compOperator;
        int q = modFie.quantity;
        SearchType sT = modFie.sType;
        Action a = modFie.action;

        //load resource total based on given ResourceType
        int resourceTotal = 0;
        switch (rT)
        {
            case ResourceType.Wood:
            {
                resourceTotal = rm.woodTotal;
                break;
            }
            case ResourceType.Stone:
            {
                resourceTotal = rm.stoneTotal;
                break;
            }
            case ResourceType.Food:
            {
                resourceTotal = rm.foodTotal;
                break;
            }
            default:
            {
                return false; //function failed. invalid value for resource type
            }
        }

        bool ifValue = false;
        switch (op)
        {
            case ">":
            {
                ifValue = resourceTotal > q;
                break;
            }
            case "<":
            {
                ifValue = resourceTotal < q;
                break;
            }
            case ">=":
            {
                ifValue = resourceTotal >= q;
                break;
            }
            case "<=":
            {
                ifValue = resourceTotal <= q;
                break;
            }
            case "=":
            {
                ifValue = resourceTotal == q;
                break;
            }
            default:
            {
                return false; //function failed. invalid string for operator
            }
        }

        //when if condition is true, attempt search and action
        if (ifValue)
        {
            GameObject searchResult;

            //determine tile type to search for
            int tileSearchType = -1;
            switch (a)
            {
                case Action.ConstructMill:
                {
                    tileSearchType = AIManager.FOREST;
                    //checks that there is enough food to build mill
                    if (rm.foodTotal < MILL_COST)
                    {
                        return false; //function failed. not enough food to build mill
                    }
                    break;
                }
                case Action.ConstructQuarry:
                {
                    tileSearchType = AIManager.MOUNTAIN;
                    //checks that there is enough wood to build quarry
                    if (rm.woodTotal < QUARRY_COST)
                    {
                        return false; //function failed. not enough wood to build quarry
                    }
                    break;
                }
                case Action.ConstructFarm:
                {
                    tileSearchType = AIManager.EMPTY;
                    //checks that there is enough stone to build farm
                    if (rm.stoneTotal < FARM_COST)
                    {
                        return false; //function failed. not enough stone to build farm
                    }
                    break;
                }
                case Action.RevertMill:
                {
                    tileSearchType = AIManager.MILL;
                    break;
                }
                case Action.RevertQuarry:
                {
                    tileSearchType = AIManager.QUARRY;
                    break;
                }
                case Action.RevertFarm:
                {
                    tileSearchType = AIManager.FARM;
                    break;
                }
                default:
                {
                    return false; //function failed. invalid value for action
                }
            }

            //switch for search type
            switch (sT)
            {
                case SearchType.Closest:
                {
                    searchResult = am.ClosestSearch(grid, tileSearchType);
                    break;
                }
                case SearchType.Farthest:
                {
                    searchResult = am.FarthestSearch(grid, tileSearchType);
                    break;
                }
                case SearchType.Best:
                {
                    searchResult = am.BestSearch(grid, tileSearchType);
                    break;
                }
                default:
                {
                    return false; //function failed. invalid value for search type
                }
            }

            if (searchResult != null)
            {
                //prepare values for tile replacement
                int xIndex = (int)(searchResult.transform.position.x / tileSize);
                int zIndex = (int)(searchResult.transform.position.z / tileSize);

                //determine which prefab to instantiate based on given action
                int prefabIndex = -1;
                switch (a)
                {
                    case Action.ConstructMill:
                    case Action.RevertMill:
                    {
                        prefabIndex = 1; //corresponds to mill/forest tile
                        break;
                    }
                    case Action.ConstructFarm:
                    case Action.RevertFarm:
                    {
                        prefabIndex = 0; //corresponds to farm/empty tile
                        break;
                    }
                    case Action.ConstructQuarry:
                    case Action.RevertQuarry:
                    {
                        prefabIndex = 2; // corresponds to quarry/mountain tile
                        break;
                    }
                }

                //instantiate new tile to be used in replacement
                GameObject newTilePrefab = null;

                switch (a)
                {
                    //if constructing, new tile will be a PTile
                    case Action.ConstructMill:
                    case Action.ConstructQuarry:
                    case Action.ConstructFarm:
                    {
                        newTilePrefab = Instantiate(pTilePrefabs[prefabIndex]);
                        break;
                    }
                    //if reverting, new tile will be a NPTile
                    case Action.RevertMill:
                    case Action.RevertQuarry:
                    case Action.RevertFarm:
                    {
                        newTilePrefab = Instantiate(npTilePrefabs[prefabIndex]);
                        break;
                    }
                }

                //replace tile, update values, then destroy newTilePrefab
                if (newTilePrefab != null)
                {
                    //save instance date for searchResult to transfer to replacement tile
                    Tile resultTile = null;
                    if (searchResult.TryGetComponent(out NPTile resultNP))
                    {
                        resultTile = resultNP.tile;
                    }
                    else if (searchResult.TryGetComponent(out PTile resultP))
                    {
                        resultTile = resultP.tile;
                    }
                    
                    GameObject newTile = ReplaceTile (xIndex, zIndex, newTilePrefab);
                    if (newTile.TryGetComponent(out NPTile newNP))
                    {
                        newNP.tile = resultTile;
                    }
                    else if (newTile.TryGetComponent(out PTile newP))
                    {
                        newP.tile = resultTile;
                        newP.prodRate = newP.CalcProdRate(newP.fertility, newP.roughness,
                            newP.tileType);
                    }

                    Destroy(newTilePrefab);

                    //update resource total
                    switch (a)
                    {
                        case Action.ConstructQuarry:
                        {
                            rm.woodTotal -= QUARRY_COST;
                            break;
                        }
                        case Action.ConstructFarm:
                        {
                            rm.stoneTotal -= FARM_COST;
                            break;
                        }
                        case Action.ConstructMill:
                        {
                            rm.foodTotal -= MILL_COST;
                            break;
                        }
                        //revert refunds for only half of cost
                        case Action.RevertQuarry:
                        {
                            rm.woodTotal += QUARRY_COST / 2;
                            break;
                        }
                        case Action.RevertFarm:
                        {
                            rm.stoneTotal += FARM_COST / 2;
                            break;
                        }
                        case Action.RevertMill:
                        {
                            rm.foodTotal += MILL_COST / 2;
                            break;
                        }
                    }
                }
                else
                {
                    Destroy(newTilePrefab);
                    return false; //function failed. invalid prefab index
                }
            }
            else
            {
                return false; //function failed. no more tiles of appopriate type
            }
        }

        return ifValue; //return true if tile is replaced
    }
    //handles case where primary module is a while statement
    public int whilePrimary(ModuleFields modFie)
    {
        //counts number of iterations performed
        int iterationCounter = 0;
        while (ifPrimary(modFie))
        {
            iterationCounter++;
        }

        return iterationCounter;
    }
    //handles case where primary module is a for statement
    public int forPrimary(ModuleFields modFie)
    {
        int iterationsLeft = 0;
        //quantity is used to define how many iterations are attempted
        for (iterationsLeft = modFie.quantity; iterationsLeft > 0; 
            iterationsLeft--)
        {
            //create ModuleFields instance based on desired action
            ModuleFields ifFields;
            switch (modFie.action)
            {
                case Action.ConstructMill:
                {
                    ifFields = new ModuleFields(LogicType.If, ResourceType.Food, ">=", MILL_COST, modFie.sType, modFie.action);
                    break;
                }
                case Action.ConstructQuarry:
                {
                    ifFields = new ModuleFields(LogicType.If, ResourceType.Wood, ">=", QUARRY_COST, modFie.sType, modFie.action);
                    break;
                }
                case Action.ConstructFarm:
                {
                    ifFields = new ModuleFields(LogicType.If, ResourceType.Stone, ">=", FARM_COST, modFie.sType, modFie.action);
                    break;
                }
                case Action.RevertMill:
                case Action.RevertQuarry:
                case Action.RevertFarm:
                {
                    //for revert actions, only search type and action matter
                    //default values ensure that ifPrimary does not return early
                    ifFields = new ModuleFields(LogicType.If, ResourceType.Food, ">", -1, modFie.sType, modFie.action);
                    break;
                }
                default:
                {
                    return iterationsLeft; //immediately quit loop since invalid action is given
                }
            }

            //on failed attempt to take action, return number of iterations left
            if (!ifPrimary(ifFields))
            {
                return iterationsLeft;
            }
        }

        return iterationsLeft;
    }
    public void ExecuteTraining()
    {
        List<bool> readyToRemove = new List<bool>();
        //instantiate values for readyToRemove
        for (int index = 0; index < instructions.Count; index++)
        {
            readyToRemove.Add(false);
        }

        for (int instructionIndex = 0; instructionIndex < instructions.Count; 
            instructionIndex++)
        {
            switch (instructions[instructionIndex].lType)
            {
                case LogicType.If:
                {
                    //if instructions are removed when successful
                    readyToRemove[instructionIndex] = ifPrimary(instructions[instructionIndex]);
                    break;
                }
                case LogicType.While:
                {
                    //while instructions are never removed automatically
                    whilePrimary(instructions[instructionIndex]);
                    break;
                }
                case LogicType.Repeat:
                {
                    //repeat instructions are removed when the number of iterations left is zero
                    int iterationsLeft = forPrimary(instructions[instructionIndex]);

                    if (iterationsLeft == 0)
                    {
                        readyToRemove[instructionIndex] = true;
                    }
                    else
                    {
                        //replace quantity with iterationsLeft
                        //make copy of current instruction
                        ModuleFields i = instructions[instructionIndex];
                        //copy current instruction except for quantity
                        ModuleFields repeatReplacement = new ModuleFields(i.lType, i.rType, i.compOperator,
                            iterationsLeft, i.sType, i.action);
                        //replace old instruction with new
                        instructions[instructionIndex] = repeatReplacement;
                    }
                    break;
                }
            }
        }

        //once all instructions are executed, remove finished instructions
        instructions = RemoveInstructions(readyToRemove);
    }
    private List<ModuleFields> RemoveInstructions(List<bool> remove)
    {
        List<ModuleFields> trimmedInstructions = new List<ModuleFields>();
        for (int instructionIndex = 0; instructionIndex < instructions.Count;
            instructionIndex++)
        {
            //add instructions that are not flagged to be removed
            if (!remove[instructionIndex])
            {
                trimmedInstructions.Add(instructions[instructionIndex]);
            }
        }

        //remove all modules flagged to be removed
        RemoveModules(remove);

        return trimmedInstructions;
    }
    private void RemoveModules(List<bool> r)
    {
        um.RemoveModules(r);
    }

    //update coroutines
    IEnumerator updateProduction()
    {
        while (true)
        {
            rm.UpdateProduction();
            //update totals every 30 seconds
            yield return new WaitForSeconds(WAIT_FOR_PROD);
        }
    }
    IEnumerator updateTotal()
    {
        while (true)
        {
            rm.UpdateTotal();

            um.DisplayResourceValues(rm.woodTotal, rm.stoneTotal, rm.foodTotal);

            //update production rates every 5 seconds
            yield return new WaitForSeconds(WAIT_FOR_TOTAL);
        }
    }

    //ui methods
    public void EnablePause()
    {
        paused = true;
        cm.enabled = false;
        StopCoroutine(totalCoRout);
        StopCoroutine(prodCoRout);
        um.EnablePause();
    }
    public void DisablePause()
    {
        paused = false;
        cm.enabled = true;
        StartCoroutine(totalCoRout);
        StartCoroutine(prodCoRout);
        um.DisablePause();
    }
    public void EnableTraining()
    {
        training = true;
        cm.enabled = false;
        StopCoroutine(totalCoRout);
        StopCoroutine(prodCoRout);
        um.EnableTraining();
    }
    public void DisableTraining()
    {
        instructions = um.GenerateTraining();
        training = false;
        cm.enabled = true;
        StartCoroutine(totalCoRout);
        StartCoroutine(prodCoRout);
        um.DisableTraining();
    }
    public void Exit()
    {
        Application.Quit();
    }
}
