using System.Collections.Generic;
using System.Linq;
using System;

namespace Classifiers
{
    public class Node
    {
        public bool IsLeaf;
        public string Label;        
        public string DecisionAttribute; 
        public List<Node> Children = new List<Node>();
    }
}