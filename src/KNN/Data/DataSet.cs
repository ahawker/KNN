using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNN.Data {
    class DataSet {
        private static readonly Random Rng = new Random();
        public List<Feature> Features { get; private set; }
        public List<DataInstance> DataEntries { get; private set; }
        public int OutputIndex { get; set; }

        public DataSet() {
            Features = new List<Feature>();
            DataEntries = new List<DataInstance>();
        }
        public DataSet(List<DataInstance> data) {
            Features = new List<Feature>();
            DataEntries = new List<DataInstance>(data);
        }

        /// <summary>
        /// Returns a new dataset with a shuffled order of data instances.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet RandomInstance() {
            return Shuffle();
        }

        /// <summary>
        /// Returns a training/testing set split based on the given trainingSize.
        /// </summary>
        /// <param name="trainingSize">Number of data instances we want in our training set.</param>
        /// <returns>List[DataSet] containing training/test sets</returns>
        public List<DataSet> RandomInstance(double trainingSize) {
            DataSet instance = Shuffle();
            var training = new DataSet { Features = new List<Feature>(this.Features) };
            training.DataEntries.AddRange(instance.DataEntries.Take((int)trainingSize));
            var test = new DataSet(instance.DataEntries.Except(training.DataEntries).ToList());
            test.Features = new List<Feature>(this.Features);
            training.OutputIndex = test.OutputIndex = this.OutputIndex;
            return new List<DataSet>(){training, test};
        }

        /// <summary>
        /// Shuffles this dataset and returns a new dataset containing the random instances.
        /// </summary>
        /// <returns>DataSet</returns>
        private DataSet Shuffle() {
            var instance = new DataSet { Features = new List<Feature>(this.Features) };
            for(int i=DataEntries.Count-1; i>=0; i--) {
                int index = Rng.Next(DataEntries.Count);
                while(instance.DataEntries.Contains(DataEntries[index]))
                    index = Rng.Next(DataEntries.Count);
                instance.DataEntries.Add(DataEntries[index]);
            }
            return instance;
        }

        /// <summary>
        /// Returns string[] containing our two possible output(target concept) variables.
        /// </summary>
        /// <returns>string[]</returns>
        public string[] OutputValues {
            get{
                return Features.Where(i => i.Type == Types.Output).SelectMany(i => i.PossibleValues).ToArray();
            }
        }

        /// <summary>
        /// Returns integer count of the number of entries that contain the given output string.
        /// </summary>
        /// <param name="output">Target concept value</param>
        /// <returns>Int</returns>
        public int OutputValueCount(string output) {
           return DataEntries.SelectMany(i => i).Count(i => i == output);
        }
    }
}
