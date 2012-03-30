using System;
using System.Collections.Generic;

namespace KNN.Readers {
    /// <summary>
    /// Interface to implement rules for readers.
    /// </summary>
    interface IReader {
        string Filename { get; set; }

        IEnumerable<string> ValidEntries();
    }
}
