using System;
using System.Collections.Generic;
using KNN.Data;

namespace KNN.Classifiers {
    interface IClassifier {
        void AppendData(List<DataInstance> trainData);
        void AppendFeatures(List<Feature> features);
        void ClearData();

        double Test(List<DataInstance> testData);
        KeyValuePair<int, double> Tune(List<int> indices);
    }
}
