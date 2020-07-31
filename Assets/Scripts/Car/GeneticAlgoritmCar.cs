using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class GeneticAlgoritmCar : MonoBehaviour
{
    [Header("Agent Setup")]
    public GameObject agent;
    public Car mostEff;
    public Transform spawnPoint;
    [Header("Population Settings")]
    public int countOfAgentsInOnePopulation = 40;
    public int countOfCrossovers = 30;
    public int mutationTimes = 1;
    [Range(0f,1f)]
    public float mutantChance = 0.05f;
    public int countOfAlive = 10;
    public float timeForOnePopulation = 30;
    public float currentTimeForOnePopulation = 0;
    public List<GameObject> currentPopulation = new List<GameObject>();
    public List<GameObject> nextPopulation = new List<GameObject>();
    public Information inf;

    public bool start = false;

    void Awake()
    {
        inf.aliveCreature = 0;
        SpawnPopulation();
    }

    public void SpawnPopulation()
    {
        for (int i = 0; i < countOfAgentsInOnePopulation; i++)
        {
            currentPopulation.Add(Instantiate(agent, spawnPoint.position, spawnPoint.rotation));
            currentPopulation[i].GetComponent<Car>().network = new NeuralNetworkWithArrays();
            currentPopulation[i].GetComponent<Car>().network.RandomiseValue();
        }
    }

    void FixedUpdate()
    {
        if (!start) return;
        currentTimeForOnePopulation += Time.fixedDeltaTime;
        if (currentTimeForOnePopulation >= timeForOnePopulation || inf.aliveCreature == 0)
        {
            currentTimeForOnePopulation = 0;
            
            Repopulate();
        }
    }

    void Repopulate()
    {
        currentPopulation = SortByFitness(currentPopulation);

        for (int i = 0; i < 5; i++)
        {
            nextPopulation.Add(Instantiate(agent, spawnPoint.position, spawnPoint.rotation));
            nextPopulation.Last().GetComponent<Car>().network = NetCopy(currentPopulation[i].GetComponent<Car>().network);
        }


        List<NeuralNetworkWithArrays> parents = TournamentChoice(currentPopulation);
        
        for (int i = 0; i < parents.Count - 1; i+=2)
        {
            List<NeuralNetworkWithArrays> childs = Crossover(parents[i], parents[i + 1]);
            nextPopulation.Add(Instantiate(agent, spawnPoint.position, spawnPoint.rotation));
            nextPopulation.Last().GetComponent<Car>().network = NetCopy(childs[0]);

            if(Random.value < mutantChance)
            {
                Mutate(nextPopulation.Last().GetComponent<Car>().network,mutationTimes);
                Debug.Log("========================================== MUTATION ==========================================");
            }

            nextPopulation.Add(Instantiate(agent, spawnPoint.position, spawnPoint.rotation));
            nextPopulation.Last().GetComponent<Car>().network = NetCopy(childs[1]);

            if (Random.value < mutantChance)
            {
                Mutate(nextPopulation.Last().GetComponent<Car>().network, mutationTimes);
                Debug.Log("========================================== MUTATION ==========================================");
            }
        }

        foreach (GameObject obj in currentPopulation)
        {
            Destroy(obj);
        }
        currentPopulation.Clear();
        foreach (GameObject ag in nextPopulation)
        {
            currentPopulation.Add(ag);
        }
        nextPopulation.Clear();
    }

    //сортировка по эффективности
    List<GameObject> SortByFitness(List<GameObject> list)
    {
        List<GameObject> sortList = new List<GameObject>();
        sortList = list.OrderByDescending(ag => ag.GetComponent<Car>().overallFitness).ToList<GameObject>();
        Debug.Log(sortList[0].GetComponent<Car>().overallFitness);
        return sortList;
    }

    //просто скрещенивание. берем чистое существо. подбрасывая монетку даём ему ген, то от одного, то от друго родителя
    List<NeuralNetworkWithArrays> Crossover(NeuralNetworkWithArrays parent1, NeuralNetworkWithArrays parent2)//возвращаем агент с сетью
    {
        NeuralNetworkWithArrays childNet1;
        NeuralNetworkWithArrays childNet2;
        NeuralNetworkWithArrays p1Net = parent1;
        NeuralNetworkWithArrays p2Net = parent2;
        childNet1 = NetCopy(p1Net);
        childNet2 = NetCopy(p2Net);

        #region legacy
        /*
        //веса c входного слоя на скрытый
        for (int i = 0; i < Random.Range(0, p1Net.weightsInputLayer.GetLength(0)); i++)
        {
            for (int j = 0; j < Random.Range(0, p1Net.weightsInputLayer.GetLength(1)); j++)
            {
                childNet1.weightsInputLayer[i, j] = p2Net.weightsInputLayer[i, j];
            }
        }

        for (int i = Random.Range(0, p1Net.weightsInputLayer.GetLength(0)); i < p1Net.weightsInputLayer.GetLength(0); i++)
        {
            for (int j = Random.Range(0, p1Net.weightsInputLayer.GetLength(1)); j < p1Net.weightsInputLayer.GetLength(1); j++)
            {
                childNet2.weightsInputLayer[i, j] = p1Net.weightsInputLayer[i, j];
            }
        }


        //веса со скрытого на выходной
        for (int i = 0; i < Random.Range(0, p1Net.weightsHiddneLayers.GetLength(0)); i++)
        {
            for (int j = 0; j < Random.Range(0, p1Net.weightsHiddneLayers.GetLength(1)); j++)
            {
                childNet1.weightsHiddneLayers[i, j] = p2Net.weightsHiddneLayers[i, j];
            }
        }

        for (int i = Random.Range(0, p1Net.weightsHiddneLayers.GetLength(0)); i < p1Net.weightsHiddneLayers.GetLength(0); i++)
        {
            for (int j = Random.Range(0, p1Net.weightsHiddneLayers.GetLength(1)); j < p1Net.weightsHiddneLayers.GetLength(1); j++)
            {
                childNet2.weightsHiddneLayers[i, j] = p1Net.weightsHiddneLayers[i, j];
            }
        }


        //веса входного слоя
        for (int i = 0; i < Random.Range(0, p1Net.biasesInputLayer.GetLength(0)); i++)
        {
            childNet1.biasesInputLayer[i, 0] = p2Net.biasesInputLayer[i, 0];
        }

        for (int i = Random.Range(0, p1Net.biasesInputLayer.GetLength(0)); i < p1Net.biasesInputLayer.GetLength(0); i++)
        {
            childNet2.biasesInputLayer[i, 0] = p1Net.biasesInputLayer[i, 0];
        }


        //веса скрытого слоя
        for (int i = 0; i < Random.Range(0, p1Net.biasesHiddneLayers.GetLength(0)); i++)
        {
            childNet1.biasesHiddneLayers[i, 0] = p2Net.biasesHiddneLayers[i, 0];
        }

        for (int i = Random.Range(0, p1Net.biasesHiddneLayers.GetLength(0)); i < p1Net.biasesHiddneLayers.GetLength(0); i++)
        {
            childNet1.biasesHiddneLayers[i, 0] = p1Net.biasesHiddneLayers[i, 0];
        }
        */
        #endregion

        //веса c входного слоя на скрытый
        for (int i = 0; i < p1Net.weightsInputLayer.GetLength(0); i++)
        {
            for (int j = 0; j < p1Net.weightsInputLayer.GetLength(1); j++)
            {
                if (Random.value > 0.5f)
                {
                    childNet1.weightsInputLayer[i, j] = p2Net.weightsInputLayer[i, j];
                    childNet2.weightsInputLayer[i, j] = p1Net.weightsInputLayer[i, j];
                }
            }
        }

        //веса со скрытого на выходной
        for (int i = 0; i < Random.Range(0, p1Net.weightsHiddneLayers.GetLength(0)); i++)
        {
            for (int j = 0; j < Random.Range(0, p1Net.weightsHiddneLayers.GetLength(1)); j++)
            {
                if (Random.value > 0.5f)
                {
                    childNet1.weightsHiddneLayers[i, j] = p2Net.weightsHiddneLayers[i, j];
                    childNet2.weightsHiddneLayers[i, j] = p1Net.weightsHiddneLayers[i, j];
                }
            }
        }

        //веса входного слоя
        for (int i = 0; i < Random.Range(0, p1Net.biasesInputLayer.GetLength(0)); i++)
        {
            if (Random.value > 0.5f)
            {
                childNet1.biasesInputLayer[i, 0] = p2Net.biasesInputLayer[i, 0];
                childNet2.biasesInputLayer[i, 0] = p1Net.biasesInputLayer[i, 0];
            }
        }

        //веса скрытого слоя
        for (int i = 0; i < Random.Range(0, p1Net.biasesHiddneLayers.GetLength(0)); i++)
        {
            if (Random.value > 0.5f)
            {
                childNet1.biasesHiddneLayers[i, 0] = p2Net.biasesHiddneLayers[i, 0];
                childNet1.biasesHiddneLayers[i, 0] = p1Net.biasesHiddneLayers[i, 0];
            } 
        }


        List<NeuralNetworkWithArrays> childs = new List<NeuralNetworkWithArrays>();
        childs.Add(childNet1);
        childs.Add(childNet2);

        return childs;
    }

    List<NeuralNetworkWithArrays> TournamentChoice(List<GameObject> population)
    {
        List<NeuralNetworkWithArrays> newNets = new List<NeuralNetworkWithArrays>();
        while (newNets.Count < countOfCrossovers)
        {
            Car first = population[Random.Range(0,population.Count)].GetComponent<Car>();
            Car second = population[Random.Range(0, population.Count)].GetComponent<Car>();
            while (first == second)
            {
                second = population[Random.Range(0, population.Count)].GetComponent<Car>();
            }
            if (first.overallFitness > second.overallFitness)
            {
                if (newNets.Contains(first.network))
                {
                    continue;
                }
                newNets.Add(NetCopy(first.network));
            }
            else
            {
                if (newNets.Contains(second.network))
                {
                    continue;
                }
                newNets.Add(NetCopy(second.network));
            }
        }

        return newNets;
    }

    //выбираем кого то из популяции. те кто имеют более высокую эффективность имеют шансов больше(фигня, а не метод)
    NeuralNetworkWithArrays FitnessProportinalChoice(List<GameObject> population)
    {
        var proportionsSum = population.Sum(f => f.GetComponent<Car>().overallFitness);
        var normalizedPropotions = population.Select(f => f.GetComponent<Car>().overallFitness / proportionsSum);

        List<float> cumulativeProportions = new List<float>();
        float cumulativeTotal = 0f;
        foreach (var proportion in normalizedPropotions)
        {
            cumulativeTotal += proportion;
            cumulativeProportions.Add(cumulativeTotal);
        }
        
        foreach (float f in cumulativeProportions)
        {
            Debug.Log("prop " + f);
        }


        float selectedValue = Random.Range(0.0f, 1f);
        Debug.Log("select " + selectedValue);
        for (int i = 0; i < cumulativeProportions.Count();i++)
        {
            float value = cumulativeProportions[i];
            if (value >= selectedValue)
            {
                return population[i].GetComponent<Car>().network;
            }
        }
        throw new System.Exception("Что то пошло не так в отборе. А ну иди проверяй");
    }

    NeuralNetworkWithArrays Mutate(NeuralNetworkWithArrays experimental, int mutationTimes)
    {
        for (int i = 0; i < mutationTimes; i++)
        {
            int chance = Random.Range(0, 4);
            switch (chance)
            {
                case 0:
                    {
                        experimental.weightsInputLayer[Random.Range(0, experimental.weightsInputLayer.GetLength(0)), Random.Range(0, experimental.weightsInputLayer.GetLength(1))] = Random.Range(0.99f, -0.99f);
                        return experimental;
                    }

                case 1:
                    {
                        experimental.weightsHiddneLayers[Random.Range(0, experimental.weightsHiddneLayers.GetLength(0)), Random.Range(0, experimental.weightsHiddneLayers.GetLength(1))] = Random.Range(0.99f, -0.99f);
                        return experimental;
                    }

                case 2:
                    {
                        experimental.biasesInputLayer[Random.Range(0, experimental.biasesInputLayer.Length), 0] = Random.Range(0.99f, -0.99f);
                        return experimental;
                    }

                case 3:
                    {
                        experimental.biasesHiddneLayers[Random.Range(0, experimental.biasesHiddneLayers.Length), 0] = Random.Range(0.99f, -0.99f);
                        return experimental;
                    }
            }
        }
        return experimental;//без этой строчки компилятор ругается прост
    }


    //поскульку классы имеют прекрасное свойство создавать ссылки друг на друга, а не копироваться, существует эта функция
    NeuralNetworkWithArrays NetCopy(NeuralNetworkWithArrays net)
    {
        NeuralNetworkWithArrays copy = new NeuralNetworkWithArrays();
        copy.number = net.number;
        for (int i = 0; i < net.weightsInputLayer.GetLength(0); i++)
        {
            for (int j = 0; j < net.weightsInputLayer.GetLength(1); j++)
            {
                copy.weightsInputLayer[i, j] = net.weightsInputLayer[i, j];
            }
        }

        for (int i = 0; i < net.weightsHiddneLayers.GetLength(0); i++)
        {
            for (int j = 0; j < net.weightsHiddneLayers.GetLength(1); j++)
            {
                copy.weightsHiddneLayers[i, j] = net.weightsHiddneLayers[i, j];
            }
        }

        for (int i = 0; i < net.biasesInputLayer.GetLength(0); i++)
        {
            for (int j = 0; j < net.biasesInputLayer.GetLength(1); j++)
            {
                copy.biasesInputLayer[i, j] = net.biasesInputLayer[i, j];
            }
        }

        for (int i = 0; i < net.biasesHiddneLayers.GetLength(0); i++)
        {
            for (int j = 0; j < net.biasesHiddneLayers.GetLength(1); j++)
            {
                copy.biasesHiddneLayers[i, j] = net.biasesHiddneLayers[i, j];
            }
        }
        return copy;
    }
}
