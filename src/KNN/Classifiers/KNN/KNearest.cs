using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KNN.Data;

namespace KNN.Classifiers.KNN {
    class KNearest : IClassifier {
        private readonly DataSet m_DataSet;
        public int K { get; set; }
        public List<int> Features { get; set; }

        public KNearest(DataSet data) {
            m_DataSet = data;
        }
        public void AppendData(List<DataInstance> data) {
            m_DataSet.DataEntries.AddRange(data);
        }
        public void AppendFeatures(List<Feature> features) {
            m_DataSet.Features.AddRange(features);
        }
        public void ClearData() {
            m_DataSet.DataEntries.Clear();
        }
        public double Test(List<DataInstance> testData) {
            var outputValues = m_DataSet.OutputValues;
            return Estimate(testData, Features, K, outputValues).Value;
        }

        /// <summary>
        /// Iterates through all possible values of K, finding K nearest neighbors
        /// and returning the optimal K for the given attributes.
        /// </summary>
        /// <param name="indices">Attributes to use in distance computation</param>
        /// <returns>KVP[int,double]</returns>
        public KeyValuePair<int,double> Tune(List<int> indices) {
            Console.WriteLine("Tuning Feature Set: <{0}>", string.Join(", ", indices.Select(i=>m_DataSet.Features[i].Name).ToArray()));
            DateTime start = DateTime.Now;
            var optimal = new KeyValuePair<int, double>(0,0);
            var outputValues = m_DataSet.OutputValues;
            for(int i=1, k=1; k<m_DataSet.DataEntries.Count; i++, k=(int)Math.Pow(2, i)-1) {
                var estimate = Estimate(m_DataSet.DataEntries, indices, k, outputValues);
                if(estimate.Value > optimal.Value){
                    optimal = estimate;
                }
                Console.WriteLine("K:{0} -- Estimate:{1:0.##}%", estimate.Key, estimate.Value * 100.0);
            }
            Console.WriteLine("Best K={0} with Estimate:{1:0.##}% found in {2}", optimal.Key, optimal.Value * 100.0, DateTime.Now.Subtract(start));
            return optimal;
        }

        /// <summary>
        /// Given a list of attributes, k neighbors and the two target concept values,
        /// return a KVP(int,double) containing the estimate based on the k value.
        /// </summary>
        /// <param name="indices">Indices of attributes to use in our estimate</param>
        /// <param name="k">Number of nearest neighbors to find</param>
        /// <param name="outputValues">String representation of our two target concept values</param>
        /// <returns>KVP(int,double)</returns>
        private KeyValuePair<int,double> Estimate(List<DataInstance> data, List<int> indices, int k, string[] outputValues) {
            double correct = 0;
            foreach(DataInstance instance in data) {
                var neighbors = FindNearestNeighbors(instance, indices, k);
                int output1 = neighbors.Count(n => n.Value[m_DataSet.OutputIndex] == outputValues[0]);
                int output2 = neighbors.Count(n => n.Value[m_DataSet.OutputIndex] == outputValues[1]);
                if((output1 > output2 && instance[m_DataSet.OutputIndex] == outputValues[0]) ||
                    (output2 > output1 && instance[m_DataSet.OutputIndex] == outputValues[1])){
                    correct++;
                }
            }
            return new KeyValuePair<int,double>(k, correct/(double)data.Count);
        }

        /// <summary>
        /// Given a single tuning data instance and attribute indices, find the
        /// k nearest neighbors based on Euclidean distance.
        /// </summary>
        /// <param name="tune">Single tuning instance</param>
        /// <param name="indices">Indices of features used</param>
        /// <param name="k">Number of neighbors to find</param>
        /// <returns>KVP(double,DataInstance)</returns>
        private List<KeyValuePair<double, DataInstance>> FindNearestNeighbors(DataInstance tune, List<int> indices, int k) {
            var neighbors = new List<KeyValuePair<double, DataInstance>>();
            foreach(DataInstance trainingInstance in m_DataSet.DataEntries) {
                if(trainingInstance == tune) continue;
                double distance = ComputeDistance(tune, trainingInstance, indices);
                neighbors.Add(new KeyValuePair<double, DataInstance>(distance, trainingInstance));
            }
            return neighbors.OrderBy(n=>n.Key).Take(k).ToList();
        }

        /// <summary>
        /// Computes the Euclidean distance between the DataInstances tune/train using
        /// the features located at the given indices.
        /// </summary>
        /// <param name="indices">Indices of the features used</param>
        /// <param name="tune">Single tuning instance</param>
        /// <param name="train">Single training instance</param>
        /// <returns>Double</returns>
        private double ComputeDistance(DataInstance tune, DataInstance train, IEnumerable<int> indices) {
            double d = 0;
            foreach(int i in indices) {
                switch(m_DataSet.Features[i].Type) {
                    case Types.Continuous:
                        d += Distance(tune[i], train[i]);
                        break;
                    case Types.Discrete:
                        d += (tune[i] == train[i]) ? 0 : 1;
                        break;
                }
            }
            return Math.Sqrt(d);
        }

        /// <summary>
        /// Given two values, compute (x - y)^2.
        /// Subroutine for Euclidean Distance computation.
        /// </summary>
        /// <param name="tune">Value from our local tuning set.</param>
        /// <param name="train">Value from our local training set.</param>
        /// <returns>Double</returns>
        private static double Distance(string tune, string train) {
            double x = double.Parse(tune);
            double y = double.Parse(train);
            return Math.Pow(x - y, 2);
        }
    }
}
