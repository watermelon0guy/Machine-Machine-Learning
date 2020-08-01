using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Information : ScriptableObject
{
    public int aliveCreature = 0;
    public int currentGeneration = 1;
    public float timeSinceStartOfGen;
    public int countOfAgentsInOnePopulation = 50;
}
