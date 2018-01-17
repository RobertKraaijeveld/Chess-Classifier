using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Classifiers
{
    public static class AttributePicker
    {
        public static HashSet<int> GetPickedAttributeIndices()
        {
            HashSet<int> pickedAttributeIndices = new HashSet<int>();
            var attributeIndicesLines = File.ReadLines(@"attributesIndices.data");

            attributeIndicesLines.ToList().ForEach(a => pickedAttributeIndices.Add(Int32.Parse(a)));

            return pickedAttributeIndices;
        }
    }
}