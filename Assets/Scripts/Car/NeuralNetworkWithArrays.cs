using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNetworkWithArrays
{
    public int number;

    public static int inputLayer = 5, hiddenLayer = 10, hiddenLayerCount = 1, outputLayer = 2;
    
    public double[,] inputNodes = new double[inputLayer, 1];
    
    double[,] hiddenNodes = new double[hiddenLayer, hiddenLayerCount];
    
    public double[,] outputNodes = new double[outputLayer, 1];

    public double[,] weightsInputLayer = new double[inputLayer, hiddenLayer];

    public double[,] weightsHiddneLayers = new double[hiddenLayer, outputLayer];

    public double[,] biasesInputLayer = new double[hiddenLayer, 1];
    
    public double[,] biasesHiddneLayers = new double[outputLayer, 1];

    public kindsOfActivationFunction kindOfActivationFunction = kindsOfActivationFunction.TanH;
    public enum kindsOfActivationFunction
    {
        Sigmoid,
        TanH
    }

    public void RandomiseValue()
    {
        number = Random.Range(0, 10000);

        for (int i = 0; i < inputLayer; i++)//случайно выбираем веса входного слоя
        {
            for (int j = 0; j < hiddenLayer; j++)
            {
                weightsInputLayer[i, j] = Random.Range(-0.5f, 0.5f);
            }
        }

        for (int i = 0; i < hiddenLayer; i++)//случайно выбираем веса скрытых слоёв
        {
            for (int j = 0; j < outputLayer; j++)
            {
                weightsHiddneLayers[i, j] = Random.Range(-0.5f, 0.5f);
            }
        }

        for (int i = 0; i < hiddenLayer; i++)//случайно выбираем сдвиги входного слоя
        {
            biasesInputLayer[i, 0] = Random.Range(-0.5f, 0.5f);
        }

        for (int i = 0; i < outputLayer; i++)//случайно выбираем сдвиги входного слоя
        {
            biasesHiddneLayers[i, 0] = Random.Range(-0.5f, 0.5f);
        }
    }

    public NeuralNetworkWithArrays CopyNet()
    {
        NeuralNetworkWithArrays netCopy = new NeuralNetworkWithArrays();

        for (int i = 0; i > weightsInputLayer.GetLength(0); i++)
        {
            for (int j = 0; j > weightsInputLayer.GetLength(1); j++)
            {
                netCopy.weightsInputLayer[i, j] = weightsInputLayer[i, j];
            }
        }

        for (int i = 0; i > weightsHiddneLayers.GetLength(0); i++)
        {
            for (int j = 0; j > weightsHiddneLayers.GetLength(1); j++)
            {
                netCopy.weightsHiddneLayers[i, j] = weightsHiddneLayers[i, j];
            }
        }

        return netCopy;
    }

    public void Calculate()
    {
        switch (kindOfActivationFunction)
        {
            case (kindsOfActivationFunction.Sigmoid):
                {
                    hiddenNodes = Sigmoid(Add(DotProduct(Transpose(weightsInputLayer), inputNodes), biasesInputLayer));
                    outputNodes = Sigmoid(Add(DotProduct(Transpose(weightsHiddneLayers), hiddenNodes), biasesHiddneLayers));
                    return;
                }
            case (kindsOfActivationFunction.TanH):
                {
                    hiddenNodes = Sigmoid(Add(DotProduct(Transpose(weightsInputLayer), inputNodes), biasesInputLayer));
                    outputNodes = TanH(Add(DotProduct(Transpose(weightsHiddneLayers), hiddenNodes), biasesHiddneLayers));
                    return;
                }
        }
        

    }

    private double[,] DotProduct(double [,] m1, double [,] m2)//так как наши массивы представляют из себя анналог матрицы, то мы используем метод Dot Product для перемножения двух матриц
    {
        int rowsA = m1.GetLength(0);
        int colsA = m1.GetLength(1);

        int rowsB = m2.GetLength(0);
        int colsB = m2.GetLength(1);

        double[,] result = new double[rowsA, colsB];
        int rowsRes = result.GetLength(0);
        int colsRes = result.GetLength(1);

        for (int i = 0; i < rowsRes; i++)//случайно выбираем веса скрытых слоёв
        {
            for (int j = 0; j < colsRes; j++)
            {
                double sum = 0;
                for (int k = 0; k < colsA;k++)
                {
                    sum += m1[i, k] * m2[k, j];
                }

                result[i, j] = sum;
            }
        }
        return result;
    }

    private double[,] Transpose(double[,] m)//функция для отражения матрицы-массива по диагонали
    {
        double[,] temp = new double[m.GetLength(1), m.GetLength(0)];
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                temp[j, i] = m[i, j];
            }
        }
        return temp;
    }

    private double[,] Add(double[,] m1, double[,] m2)
    {
        double[,] temp = new double[m1.GetLength(0), m1.GetLength(1)];
        for (int i = 0; i < m1.GetLength(1); i++)
        {
            for (int j = 0; j < m1.GetLength(0); j++)
            {
                temp[j, i] = m1[j, i] + m2[j, i];
            }
        }
        return temp;
    }
    ///////////////////////////////////////////////////////////////////
    private double Sigmoid(double x)
    {
        return (1.0f / (1.0f + Mathf.Exp((float)-x)));
    }

    private double[,] Sigmoid(double[,] m)
    {
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                m[i, j] = Sigmoid(m[i, j]);
            }
        }
        return m;
    }

    private double TanH(double x)
    {
        return 2 * Sigmoid(2*x) - 1;
    }

    private double[,] TanH(double[,] m)
    {
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                m[i, j] = TanH(m[i, j]);
            }
        }
        return m;
    }
    ///////////////////////////////////////////////////////////////////
}
