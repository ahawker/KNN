using System;
using System.Linq;
using KNN.Classifiers.Selectors;
using KNN.Data;
using KNN.Classifiers.KNN;

namespace KNN {
    internal class Project3 {
        private static void Main(string[] args) {
            if (args.Length != 2) {
                Console.WriteLine("Project3.exe *.names *.data");
                return;
            }
            DateTime start = DateTime.Now;
            Console.WriteLine("Start Time: {0}", start);
            var builder = new DSBuilder(args);
            DataSet data = builder.BuildDataSet();

            var sets = data.RandomInstance(800);
            var knn = new KNearest(sets[0]);
            var fs = new FeatureSelector(knn);
            var optimal = fs.ForwardFeatureSelect(Enumerable.Range(0, data.Features.Count - 1).Where(x => x != data.OutputIndex).ToList());
            knn.K = optimal.Key;
            knn.Features = optimal.Value;
            Console.WriteLine("Final Result: {0:0.##}% with K:{1} using Features:{2}",
                              fs.Test(sets[1].DataEntries)*100.0,
                              knn.K,
                              string.Join(", ", knn.Features.Select(i => data.Features[i].Name).ToArray()));
            Console.WriteLine("Run-Time: {0}", DateTime.Now - start);
        }
    }
}