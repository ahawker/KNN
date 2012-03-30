using System;
using System.Collections.Generic;

namespace KNN.Readers {
    /// <summary>
    /// Class for maintaining functionality for reading and filtering entries for *.data files.
    /// </summary>
    class DataReader : BaseReader, IReader {
        public string Filename { get; set; }

        public DataReader(string fname) {
            Filename = fname;
        }

        /// <summary>
        /// Returns lines read from a file that have the correct number of entries per line.
        /// </summary>
        /// <returns>IEnumerable[string]</returns>
        public IEnumerable<string> ValidEntries() {
            foreach(string line in ReadFromFile(Filename)) {
                yield return line;
            }
        }
    }
}
