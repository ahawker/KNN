using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KNN.Data;

namespace KNN.Readers {
    /// <summary>
    /// Class for maintaining functionality for reading and filtering *.names files.
    /// </summary>
    class NamesReader : BaseReader, IReader {
        public string Filename { get; set; }
        private StringBuilder m_Sb;

        public NamesReader(string fname) {
            Filename = fname;
        }

        /// <summary>
        /// Captures required info from the *.names file and returns it in a ':' seperated string.
        /// </summary>
        /// <returns>IEnumerable[string]</returns>
        public IEnumerable<string> ValidEntries() {
            foreach(string line in ReadFromFile(Filename)) {
                Match isValid = Regex.Match(line, @"^([\w\-]+)\s+(\w+)\s+([\w\d\-]+,?\s?)+");
                if(isValid.Success) {
                    m_Sb = new StringBuilder(100);
                    m_Sb.AppendFormat("{0}:", isValid.Groups[1].ToString());
                    m_Sb.AppendFormat("{0}:", (Types)Enum.Parse(typeof(Types), isValid.Groups[2].ToString()));
                    foreach(Capture c in isValid.Groups[3].Captures)
                        m_Sb.Append(c.ToString());
                    yield return m_Sb.ToString();
                }
            }
        }
    }
}
