using System;
using System.Collections.Generic;
using KNN.Data;

namespace KNN.Classifiers.Selectors {
    class FeatureSelector : IClassifier {
        private readonly IClassifier m_Classifier;
        
        public FeatureSelector(IClassifier classifier) {
            m_Classifier = classifier;
        }
        public void AppendData(List<DataInstance> trainData) {
            m_Classifier.AppendData(trainData);
        }
        public void AppendFeatures(List<Feature> features) {
            m_Classifier.AppendFeatures(features);
        }
        public void ClearData() {
            m_Classifier.ClearData();
        }
        public double Test(List<DataInstance> testData) {
            return m_Classifier.Test(testData);
        }
        public KeyValuePair<int,double> Tune(List<int> indices) {
            return m_Classifier.Tune(indices);
        }

        /// <summary>
        /// Iterates through all the given feature indices to find the optimal attributes to use for the best KNN estimate.
        /// </summary>
        /// <param name="features">List of Attribute indices</param>
        /// <returns>KVP[int,List[int]]</returns>
        public KeyValuePair<int, List<int>> ForwardFeatureSelect(List<int> features) {
            Console.WriteLine("Starting Forward Feature Select using {0} features.", features.Count + 1);
            var optimal = new KeyValuePair<int, double>(0,0);
            var optimalFeatures = new List<int>();
            bool foundAdditionalFeature;
            do {
                int bestIndex = 0;
                foundAdditionalFeature = false;
                foreach(int i in features) {
                    if(optimalFeatures.Contains(i)) continue;
                    var estimate = m_Classifier.Tune(new List<int>(optimalFeatures) { i } );
                    if(estimate.Value > optimal.Value) {
                        optimal = estimate;
                        bestIndex = i;
                        foundAdditionalFeature = true;
                    }
                }
                if(foundAdditionalFeature)
                    optimalFeatures.Add(bestIndex);
                Console.WriteLine("Optimal -- K:{0} Estimate:{1}%", optimal.Key, optimal.Value * 100.0);
            }
            while(foundAdditionalFeature);
            Console.WriteLine("Forward Feature Selection complete.");
            return new KeyValuePair<int, List<int>>(optimal.Key, optimalFeatures);
        }
    }
}
