using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject resourceCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject trainingCanvas;
    [SerializeField] private Text resourceValues;
    [SerializeField] private TrainingManager tm;
    
    void Start()
    {
        resourceCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        trainingCanvas.SetActive(false);
    }

    public void EnablePause()
    {
        resourceCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
        trainingCanvas.SetActive(false);
    }
    public void DisablePause()
    {
        resourceCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        trainingCanvas.SetActive(false);
    }
    public void EnableTraining()
    {
        resourceCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        trainingCanvas.SetActive(true);
    }
    public void DisableTraining()
    {
        resourceCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        trainingCanvas.SetActive(false);
    }
    public void DisplayResourceValues(int wood, int stone, int food)
    {
        resourceValues.text = wood + "\n" + stone + "\n" + food;
    }

    public List<SceneController.ModuleFields> GenerateTraining()
    {
        return tm.GenerateTraining();
    }

    public void RemoveModules(List<bool> r)
    {
        tm.RemoveModules(r);
    }
}
