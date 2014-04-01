namespace Cosi.Lab5.Core
{
    using System;

    public class CompetitiveNeuronNetwork
    {
        private double[] destributedLayer;

        private double[,] weightCoefficents;

        private int[] neuronsVictories;

        private double teachingSpeed;

        private int networkLength;

        private const double DistanceMinValue = 1.2;

        private const int NumberOfClusters = 3;

        private bool teachingReady = false;

        private double maxDistance = 0;

        private int iterations = 0;

        public CompetitiveNeuronNetwork(int vectorSize, double teachingSpeed)
        {
            destributedLayer = new double[vectorSize];
            weightCoefficents = new double[NumberOfClusters, vectorSize];
            neuronsVictories = new int[NumberOfClusters];

            networkLength = vectorSize;

            var rand = new Random();
            double result = 0;

            for (var j = 0; j < NumberOfClusters; j++)
            {
                for (var i = 0; i < vectorSize; i++)
                {
                        weightCoefficents[j, i] = rand.NextDouble();
                }

                for (var i = 0; i < networkLength; i++)
                {
                    result += Math.Pow(weightCoefficents[j, i], 2);
                }

                var norm = Math.Pow(result, 0.5);

                for (var i = 0; i < vectorSize; i++)
                {
                    weightCoefficents[j, i] = weightCoefficents[j, i]/ norm;
                }
            }

            this.teachingSpeed = teachingSpeed;
        }

        public void Teach(params double[][] inputVector)
        {
            while (!this.teachingReady)
            {
                foreach (var i in inputVector)
                {
                    MakeIteration(i);
                }
                
                if (iterations > 1000) 
                    break;
            }
        }

        public char DetermineClaster(double[] inputVector)
        {
            var tmp = 0d;
            var minDistance = 1000d;
            var winnerNumber = 0;

            for (var i = 0; i < inputVector.Length; i++)
            {
                tmp += Math.Pow(inputVector[i], 2); ;
            }

            tmp = Math.Pow(tmp, 0.5);

            for (var i = 0; i < inputVector.Length; i++)
            {
                destributedLayer[i] = inputVector[i] / tmp;
            }

            for (var i = 0; i < NumberOfClusters; i++)
            {
                var tmp3 = GetEuclidDistance(i) * (1 + neuronsVictories[i]);
                var currentDistance = tmp3;

                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    winnerNumber = i;
                }
            }

            if (winnerNumber == 0) return 'C';
            if (winnerNumber == 1) return 'A';
            if (winnerNumber == 2) return 'B';
            return 'A';
        }

        private void MakeIteration(double[] inputVector)
        {
            double minDistance = 1000000;
            var winnerNumber = 0;
            var tmp= 0d;

            for(var i = 0; i < inputVector.Length; i++)
            {
                tmp += Math.Pow(inputVector[i], 2); ;
            }

            tmp = Math.Pow(tmp, 0.5);

            for (var i = 0; i < inputVector.Length; i++)
            {
                destributedLayer[i] = inputVector[i]/tmp;
            }

            for (var i = 0; i < NumberOfClusters; i++)
            {
                var tmp3 = GetEuclidDistance(i);
                var currentDistance = tmp3*(1 + neuronsVictories[i]);

                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    winnerNumber = i;
                    if (tmp3 > maxDistance)
                    {
                        maxDistance = tmp3;
                    }
                }
            }

            if (this.maxDistance < DistanceMinValue)
            {
                teachingReady = true;
                this.maxDistance = 0;
                return;
            }

            UpdateWieghtCoefficent(winnerNumber);

            neuronsVictories[winnerNumber]++;

            iterations++;
        }

        private double GetEuclidDistance(int clusterNeuron)
        {
            double result = 0;

            for (var i = 0; i < networkLength; i++)
            {
                result += Math.Pow(destributedLayer[i] - weightCoefficents[clusterNeuron, i], 2);
            }

            var tmp = Math.Pow(result, 0.5);

            return tmp;
        }

        private void UpdateWieghtCoefficent(int clusterNeuron)
        {
            var devider = 0d;

            for (var i = 0; i < this.networkLength; i++)
            {
                devider += Math.Pow(weightCoefficents[clusterNeuron, i] + teachingSpeed*(destributedLayer[i] -
                    weightCoefficents[clusterNeuron, i]), 2);
            }

            devider = Math.Pow(devider, 0.5);

            for (var i = 0; i < this.networkLength; i++)
            {
                var tmp = ((weightCoefficents[clusterNeuron, i]
                            + this.teachingSpeed
                            * (this.destributedLayer[i]
                               - weightCoefficents[clusterNeuron, i])) / devider);

                this.weightCoefficents[clusterNeuron, i] = tmp;
            }
        }
    }
}
