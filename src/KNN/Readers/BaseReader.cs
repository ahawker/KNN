using System;
using System.IO;
using System.Collections.Generic;

namespace KNN.Readers {
    /// <summary>
    /// Maintains file I/O with minimal filtering for our higher level readers.
    /// </summary>
    class BaseReader {
        /// <summary>
        /// Opens file and yields appropriate lines to the readers.
        /// </summary>
        /// <param name="fname">Name of file to open/read.</param>
        /// <returns>IEnumerable[string]</returns>
        protected IEnumerable<string> ReadFromFile(string fname) {
            using(var sr = new StreamReader(fname)) {
                string line;
                while((line = sr.ReadLine()) != null){
                    if(line == string.Empty || line.IndexOf("//", 0, 2, System.StringComparison.Ordinal) == 0)
                        continue;
                    yield return line;
                }
            }
        }
    }
}
