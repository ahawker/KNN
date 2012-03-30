using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNN.Data {
    class Feature {
        public string Name { get; private set; }
        public Types Type { get; private set; }
        public List<string> PossibleValues { get; private set; }

        public Feature() { }
        public Feature(string name, Types type) {
            Name = name;
            Type = type;
            PossibleValues = new List<string>();
        }

        public override string ToString() {
            var sb = new StringBuilder(64);
            sb.AppendFormat("{0} ", Name);
            sb.AppendFormat("{0} ", Type);
            PossibleValues.Select(s=>sb.AppendFormat("{0} ", s));
            return sb.ToString();
        }
    }
}
