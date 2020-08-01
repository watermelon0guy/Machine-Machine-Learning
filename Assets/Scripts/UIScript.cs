using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScript : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject inGameMenu;
    public TextMeshProUGUI timeScaleText;
    public TextMeshProUGUI currentGeneration;
    public TextMeshProUGUI timeSinceStart;
    public TextMeshProUGUI inputOfCount;
    public GeneticAlgoritmCar genAlg;
    public Information inf;
    [Header("ToNextGen")]
    public GameObject ok;
    public GameObject not_ok;
    

    void Update()
    {
        UpdateTimeSinceStart();
        UpdateCurrentGeneration();
    }

    public void SpeedUp()
    {
        if (Time.timeScale < 5)
        {
            Time.timeScale += 1;
            timeScaleText.text = Time.timeScale.ToString();
        }
    }

    public void SpeedDown()
    {
        if (Time.timeScale > 1)
        {
            Time.timeScale -= 1;
            timeScaleText.text = Time.timeScale.ToString();
        }
    }

    public void ToNextGen()
    {
        if (genAlg.nextWave == false)
        {
            genAlg.nextWave = true;
            ok.SetActive(true);
            not_ok.SetActive(false);
        }
        else
        {
            genAlg.nextWave = false;
            ok.SetActive(false);
            not_ok.SetActive(true);
        }
    }

    public void UpdateTimeSinceStart()
    {
        timeSinceStart.text = "Time: " + inf.timeSinceStartOfGen;
    }

    public void UpdateCurrentGeneration()
    {
        currentGeneration.text = "Gen number: " + inf.currentGeneration;
    }

    public void ChangeCountOfAgents()
    {
        Debug.Log(inputOfCount.text);
        genAlg.countOfAgentsInOnePopulation = int.Parse(inputOfCount.text);
    }

    void SwitchMenu(GameObject first, GameObject second)
    {
        first.SetActive(false);
        second.SetActive(true);
    }

    public void StartGame()
    {
        genAlg.start = true;
        genAlg.SpawnPopulation();
        SwitchMenu(startMenu, inGameMenu);
        genAlg.start = true;
    }
}
