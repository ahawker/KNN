using System;
using System.Collections.Generic;

namespace KNN.Data {
    class DataInstance : List<string> {
        public DataInstance() { }
        public DataInstance(IEnumerable<string> data) {
            this.AddRange(data);
        }

        public override string ToString() {
            return string.Join(",", this.ToArray());
        }
    }
}
