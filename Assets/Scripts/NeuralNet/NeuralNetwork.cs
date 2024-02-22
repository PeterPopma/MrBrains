using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet
{
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; set; }
        public double LearningRate { get; set; }
        private long learningIterations;
        private long inputNeurons;
        private long outputNeurons;
        private long hiddenNeurons;
        private TimeSpan trainingTime;
        public long LearningIterations { get => learningIterations; set => learningIterations = value; }
        public long InputNeurons { get => inputNeurons; set => inputNeurons = value; }
        public long OutputNeurons { get => outputNeurons; set => outputNeurons = value; }
        public long HiddenNeurons { get => hiddenNeurons; set => hiddenNeurons = value; }
        public TimeSpan TrainingTime { get => trainingTime; set => trainingTime = value; }

        public int LayerCount
        {
            get
            {
                return Layers.Count;
            }
        }

        // Measure the amount of training by calculating the "randomness" of the dendrites weights.
        // the dendrites were initalized with random values. After traing, these values will change
        // If the values are changing more slowly, the network is fully trained
        public double MeasureNetworkQuality()
        {
            double randomness = 0;
            double previousValue = 0;
            for (int l = 0; l < Layers.Count; l++)
            {
                Layer layer = Layers[l];

                for (int n = 0; n < layer.Neurons.Count; n++)
                {
                    Neuron neuron = layer.Neurons[n];

                    foreach (Dendrite dendrite in neuron.Dendrites)
                    {
                        randomness += Math.Abs(previousValue - dendrite.Weight);
                        previousValue = dendrite.Weight;
                    }
                }
            }

            return randomness / TotalDendriteCount();
        }

        public int MaxNeuronsInOneLayer()
        {
            int max = 0;
            foreach(Layer layer in Layers)
            {
                if(layer.NeuronCount> max)
                {
                    max = layer.NeuronCount;
                }
            }
            return max;
        }

        public long TotalNeurons()
        {
            int count = 0;
            foreach (Layer layer in Layers)
            {
                count += layer.NeuronCount;
            }
            return count;
        }

        public long TotalDendriteCount()
        {
            long totalDendrites = 0;
            for (int l = 0; l < Layers.Count; l++)
            {
                Layer layer = Layers[l];
                totalDendrites += layer.Neurons.Count * layer.Neurons[0].DendriteCount;
            }

            return totalDendrites;
        }

        public Layer OutputLayer()
        {
            return Layers[Layers.Count - 1];
        }

        public NeuralNetwork(double learningRate, long inputNeurons, long hiddenNeurons, long outputNeurons)
        {
            learningIterations = 0;

            this.inputNeurons = inputNeurons;
            this.hiddenNeurons = hiddenNeurons;
            this.outputNeurons = outputNeurons;

            this.LearningRate = learningRate;
        }

        public NeuralNetwork(double learningRate, int[] layers, int numLayers)
        {
            learningIterations = 0;
            if (layers.Length < 2) return;

            inputNeurons = layers[0];
            hiddenNeurons = layers[1];
            outputNeurons = layers[layers.Length - 1];

            this.LearningRate = learningRate;
            this.Layers = new List<Layer>();

            for (int l = 0; l < numLayers; l++)
            {
                int numNeurons = (int)hiddenNeurons;
                if (l==0)
                {
                    numNeurons = (int)inputNeurons;
                }
                if (l == numLayers-1)       // last layer
                {
                    numNeurons = (int)outputNeurons;
                }
                Layer layer = new Layer(numNeurons);
                this.Layers.Add(layer);

                for (int n = 0; n < numNeurons; n++)
                    layer.Neurons.Add(new Neuron());

                layer.Neurons.ForEach((nn) =>
                {
                    if (l == 0)
                        nn.Bias = 0;
                    else
                        for (int d = 0; d < hiddenNeurons; d++)
                            nn.Dendrites.Add(new Dendrite());
                });
            }
        }

        public NeuralNetwork(double learningRate, int[] layers)
        {
            learningIterations = 0;
            if (layers.Length < 2) return;

            inputNeurons = layers[0];
            hiddenNeurons = layers[1];
            outputNeurons = layers[layers.Length-1];

            this.LearningRate = learningRate;
            this.Layers = new List<Layer>();

            for(int l = 0; l < layers.Length; l++)
            {
                Layer layer = new Layer(layers[l]);
                this.Layers.Add(layer);

                for (int n = 0; n < layers[l]; n++)
                    layer.Neurons.Add(new Neuron());

                layer.Neurons.ForEach((nn) =>
                {
                    if (l == 0)
                        nn.Bias = 0;
                    else
                        for (int d = 0; d < layers[l - 1]; d++)
                            nn.Dendrites.Add(new Dendrite());
                });
            }
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public void Train(List<List<double>> input, List<List<double>> output)
        {
            for (int k = 0; k < input.Count; k++)
            {
                Train(input[k], output[k]);
            }
        }

        public void Train(List<double> inputs, double output)
        {
            List<double> outputs = new List<double>();
            outputs.Add(output);

            Train(inputs, outputs);
        }

        public void Train(double input, double output)
        {
            List<double> inputs = new List<double>();
            inputs.Add(input);
            List<double> outputs = new List<double>();
            outputs.Add(output);
            Train(inputs, outputs);
        }

        public void Train(List<double> input, List<double> output)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            learningIterations++;

            Run(input);

            for (int i = 0; i < this.Layers[this.Layers.Count - 1].Neurons.Count; i++)
            {
                Neuron neuron = this.Layers[this.Layers.Count - 1].Neurons[i];

                neuron.Delta = neuron.Value * (1 - neuron.Value) * (output[i] - neuron.Value);

                for (int j = this.Layers.Count - 2; j >= 1; j--)
                {
                    for(int k = 0; k < this.Layers[j].Neurons.Count; k++)
                    {
                        Neuron n = this.Layers[j].Neurons[k];

                        n.Delta = n.Value *
                                  (1 - n.Value) *
                                  this.Layers[j + 1].Neurons[i].Dendrites[k].Weight *
                                  this.Layers[j + 1].Neurons[i].Delta;
                    }
                }
            }

            for (int i = this.Layers.Count - 1; i >= 1; i--)
            {
                for (int j=0; j < this.Layers[i].Neurons.Count; j++)
                {
                    Neuron n = this.Layers[i].Neurons[j];
                    n.Bias = n.Bias + (this.LearningRate * n.Delta);

                    for (int k = 0; k < n.Dendrites.Count; k++)
                        n.Dendrites[k].Weight = n.Dendrites[k].Weight + (this.LearningRate * this.Layers[i - 1].Neurons[k].Value * n.Delta);
                }
            }

            stopWatch.Stop();
            trainingTime += stopWatch.Elapsed;
        }

        public double[] Run(List<double> input)
        {
            if (input.Count != this.Layers[0].NeuronCount) return null;

            for (int l = 0; l < Layers.Count; l++)
            {
                Layer layer = Layers[l];

                for (int n = 0; n < layer.Neurons.Count; n++)
                {
                    Neuron neuron = layer.Neurons[n];

                    if (l == 0)
                        neuron.Value = input[n];
                    else
                    {
                        neuron.Value = 0;
                        for (int np = 0; np < this.Layers[l - 1].Neurons.Count; np++)
                            neuron.Value = neuron.Value + this.Layers[l - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;

                        neuron.Value = Sigmoid(neuron.Value + neuron.Bias);
                    }
                }
            }

            Layer last = this.Layers[this.Layers.Count - 1];
            int numOutput = last.Neurons.Count;
            double[] output = new double[numOutput];
            for (int i = 0; i < last.Neurons.Count; i++)
                output[i] = last.Neurons[i].Value;

            return output;
        }
    }
}
