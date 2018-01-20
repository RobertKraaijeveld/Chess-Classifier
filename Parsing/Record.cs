using System;
using System.Collections.Generic;
using System.Linq;

namespace Classifiers
{
    public class Record
    {
        public string Classification;
        public readonly Dictionary<string, int> Attributes = new Dictionary<string, int>();


        public Record(string classification)
        {
            this.Classification = classification;
        }

        public Record(string classification, Dictionary<string, int> predictors)
        {
            this.Classification = classification;
            this.Attributes = predictors;
        }

        
        public bool HasTheSamePredictors(Record anotherRecord)
        {
            foreach (var predictorKey in this.Attributes.Keys)
            {
                if (anotherRecord.Attributes.ContainsKey(predictorKey) == false)
                    return false;
            }
            return true;
        }


        public override string ToString()
        {
            var predictorValueString = "";
            Attributes.ToList().ForEach(p => predictorValueString += (p.Key + " = " + p.Value + "\n"));

            return "Attributes: { " + predictorValueString + " }, Classification: " + Classification + "\n";
        }
    }
}