using System.Collections.Generic;
using System.Linq;
using System;

namespace Classifiers
{
    public class Node
    {
        public bool IsLeaf;
        public string Label = "";        
        public string DecisionAttribute; 
        public Dictionary<int, Node> Children = new Dictionary<int, Node>();

        public override string ToString()
        {
            var baseStr = "{ Label: " + Label + ", Attribute: " + DecisionAttribute 
                    + ", IsLeaf: " + IsLeaf; 
                    
            if(Children.Count >= 1)
            {
                baseStr += " }, Children: | ";
                foreach (var childKV in Children)
                {
                    baseStr += "== " + childKV.Key +" => " + childKV.Value.ToString();
                }
                baseStr += " | ";
            }
                
            else
                baseStr += " }; ";
            
            return baseStr;
        }
    }
}