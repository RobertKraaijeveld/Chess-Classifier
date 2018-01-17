using System;
using System.Linq;
using System.Collections.Generic;

namespace Classifiers
{
    public class Table<T, J>
    {
        //Key is classification name, tablenetry key is factors name, tableentry value is factors count for this classification. 
        public Dictionary<T, Dictionary<T, J>> rowsPerColumn { get; set; }

        public void PotentiallyAddClassification(T classification)
        {
            if (!rowsPerColumn.ContainsKey(classification))
                rowsPerColumn.Add(classification, new Dictionary<T, J>());
        }

        public Table()
        {
            rowsPerColumn = new Dictionary<T, Dictionary<T, J>>();
        }
    }
}
