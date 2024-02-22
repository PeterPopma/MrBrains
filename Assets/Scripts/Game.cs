using NeuralNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    public static Game Instance;
    private TimeSpan trainingTime;

    private NeuralNetwork neuralNetwork;
    int numLayers = 3;
    TimeSpan runningTime;
    private List<double> directionChanges0 = new List<double>() { -30, -10, -20, 40, 0, 20, 70, -10, 10, 0 };
    private List<double> directionChanges1 = new List<double>() { -20, 0, -20, 80, 30, -80, -20, -40, 0, 0 };
    [SerializeField] TMP_InputField inputLearningRate;
    [SerializeField] TMP_InputField inputInputNeurons;
    [SerializeField] TMP_InputField inputHiddenNeurons;
    [SerializeField] TMP_InputField inputOutputNeurons;
    [SerializeField] TextMeshProUGUI textIterations;
    [SerializeField] TextMeshProUGUI textTrainingTime;
    [SerializeField] TextMeshProUGUI textTrainingAmount;

    public TimeSpan TrainingTime { get => trainingTime; set => trainingTime = value; }

    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        neuralNetwork = new NeuralNetwork(Convert.ToDouble(inputLearningRate.text),
            new int[] { Convert.ToInt32(inputInputNeurons.text),
                Convert.ToInt32(inputHiddenNeurons.text),
                Convert.ToInt32(inputOutputNeurons.text) });
        List<double> output;
        List<double> inputs = new List<double>() { 0 };

        // train the network just a little bit to make the characters move the right initial direction
        for(int i=0; i<15; i++)
        {
            inputs[0] = 0;
            neuralNetwork.Train(inputs, AnglesToNeurons(directionChanges0));

            inputs[0] = 1;
            neuralNetwork.Train(inputs, AnglesToNeurons(directionChanges1));
        }
    }

    void Update()
    {
    }

    public void OnButtonTrainClick()
    {
        neuralNetwork = new NeuralNetwork(Convert.ToDouble(inputLearningRate.text),
            new int[] { Convert.ToInt32(inputInputNeurons.text),
                Convert.ToInt32(inputHiddenNeurons.text),
                Convert.ToInt32(inputOutputNeurons.text) });

        StartCoroutine(TrainNeuralNetwork());
    }

    private IEnumerator TrainNeuralNetwork()
    {
        long count = 0;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        List<double> output;
        List<double> inputs = new List<double>() { 0 };

        while (count < 200000)
        {
            inputs[0] = 0;
            neuralNetwork.Train(inputs, AnglesToNeurons(directionChanges0));

            inputs[0] = 1;
            neuralNetwork.Train(inputs, AnglesToNeurons(directionChanges1));

            count++;
            runningTime = stopWatch.Elapsed;
            DisplayNeuralNetworkStats();

            yield return new WaitForSeconds(0.001f);
        }

        stopWatch.Stop();
        DisplayNeuralNetworkStats();
    }

    private void DisplayNeuralNetworkStats()
    {
        textTrainingAmount.text = neuralNetwork.MeasureNetworkQuality().ToString("N3");
        textIterations.text = neuralNetwork.LearningIterations.ToString("0");
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        neuralNetwork.TrainingTime.Hours, neuralNetwork.TrainingTime.Minutes, neuralNetwork.TrainingTime.Seconds,
        neuralNetwork.TrainingTime.Milliseconds / 10);
        textTrainingTime.text = elapsedTime;
    }

    // Convert angle to neuron value between 0 en 1
    private List<double> AnglesToNeurons(List<double> angles)
    {
        List<double> neuronValues = new List<double>();
        foreach (double angle in angles)
        {
            neuronValues.Add((angle + 90) / 180f);
        }

        return neuronValues;
    }

    private List<double> NeuronsToAngles(List<double> neurons)
    {
        List<double> angleValues = new List<double>();
        foreach (double neuron in neurons)
        {
            angleValues.Add((neuron * 180) - 90);
        }

        return angleValues;
    }

    public List<double> GetPlanFromNeuralNetwork(int SpawnPosition)
    {
        List<double> inputs = new List<double>();
        List<double> result = new List<double>();
        if (SpawnPosition == 0)
        {
            inputs.Add(0);
        }
        else
        {
            inputs.Add(1);
        }
        neuralNetwork.Run(inputs);

        foreach (Neuron neuron in neuralNetwork.OutputLayer().Neurons)
        {
            result.Add(neuron.Value);
        }

        result = NeuronsToAngles(result);

        return result;
    }
}
