using System;
using System.Globalization;
using System.Linq;
using KNN.Readers;

namespace KNN.Data {
    class DSBuilder {
        private readonly NamesReader m_NamesReader;
        private readonly DataReader m_DataReader;

        public DSBuilder(string[] args) {
            m_NamesReader = new NamesReader(args[0]);
            m_DataReader = new DataReader(args[1]);
        }

        /// <summary>
        /// Returns a filled DataSet containing all valid information read in
        /// from the *.names and *.data files.
        /// </summary>
        public DataSet BuildDataSet() {
            var dataSet = new DataSet();
            BuildNamesEntries(ref dataSet);
            BuildDataEntries(ref dataSet);
            return dataSet;
            
        }

        /// <summary>
        /// Reads all valid entries(determined by DataReader) and stores values into a List[object]
        /// in our DataSet; paired with the attributes read in from the names file.
        /// </summary>
        private void BuildDataEntries(ref DataSet dataSet) {
            foreach(string entry in m_DataReader.ValidEntries()) {
                string[] data = entry.Split(',');
                if(data.Length != dataSet.Features.Count){
                    Console.WriteLine("[Error]: Invalid # of data elements in {0}.", data.Select(s=>s.ToString(CultureInfo.InvariantCulture)));
                    continue;
                }
                var instance = new DataInstance();
                for(int i=0; i<data.Length; i++) {
                    if(IsValidValue(dataSet.Features[i].Type, data[i], dataSet.Features[i].PossibleValues.ToArray()))
                        instance.Add(data[i]);
                }
                if(instance.Count == dataSet.Features.Count)
                    dataSet.DataEntries.Add(instance);
            } 
        }

        /// <summary>
        /// Reads all valid entries (determined by NamesReader) and stores values into
        /// and attribute object that is stored in our DataSet.
        /// </summary>
        private void BuildNamesEntries(ref DataSet dataSet) {
            foreach(string line in m_NamesReader.ValidEntries()) {
                string[] features = line.Split(':');
                var feature = new Feature(features[0], (Types)Enum.Parse(typeof(Types), features[1]));
                foreach(string s in features[2].Split(',')) {
                    feature.PossibleValues.Add(s.Trim());
                }
                dataSet.Features.Add(feature);
                if(feature.Type == Types.Output){
                    dataSet.OutputIndex = dataSet.Features.Count-1;
                }
            }
        }

        /// <summary>
        /// Determines if the passed value is valid within the possible values of that attribute and type.
        /// </summary>
        /// <param name="type">FeatureType of the attribute we're on.</param>
        /// <param name="val">Value being checked against *.names file possible values.</param>
        /// <param name="possible">Array of possible values to check against val.</param>
        /// <returns>Bool</returns>
        private static bool IsValidValue(Types type, string val, params string[] possible) {
            bool isValid = false;
            switch(type) {
                case Types.Continuous:
                    double min, max, target;
                    if(double.TryParse(possible[0], out min) &&
                        double.TryParse(possible[1], out max) &&
                        double.TryParse(val, out target)){
                        isValid = (min <= target && target <= max);
                    }
                    break;
                case Types.Discrete:
                case Types.Output:
                    isValid = possible.Contains(val);
                    break;
            }
            if(!isValid)
                Console.WriteLine("[ERROR] Value: {0} of Type: {1} not in possible values of: {2}", val, type, string.Join(" ", possible.ToArray()));
            return isValid;
        }

        public string NamesFilename {
            set { m_NamesReader.Filename = value; }
        }
        public string DataFilename {
            set { m_DataReader.Filename = value; }
        }
    }
}
