using System.Collections.Generic;
using System.Linq;
using System;

namespace Classifiers
{
    public class ID3 : IClassifier
    {
        private List<Record> _trainingSet;
        private List<string> attributes;

        public Node ResultingTree;


        public ID3(List<Record> trainingSet)
        {
            this._trainingSet = trainingSet;

            //Setting the attributes
            attributes = _trainingSet[0].Attributes.Keys.ToList();

            ResultingTree = CreateTree(trainingSet, attributes);
        }

        public string GetName()
        {
            return "Iterative Dichotomiser 3";
        }

        public string GetClassification(Record r)
        {
            var classifyingNode = TraverseTree(r, ResultingTree);
            return classifyingNode.Label;
        }

        private Node TraverseTree(Record r, Node n)
        {
            if(n.IsLeaf)
                return n;
            else
            {
                var nodeAttribute = n.DecisionAttribute;
                var recordsValueForNodeAttribute = r.Attributes[nodeAttribute];

                return TraverseTree(r, n.Children[recordsValueForNodeAttribute]);
            }
        }

        private Node CreateTree(List<Record> set, List<string> attributes)
        {
            Node rootNode = new Node();

            //If all positive, return T with label 'won'
            if (set.Where(r => r.Classification == "won").Count() == set.Count)
            {
                rootNode.IsLeaf = true;                
                rootNode.Label = "won";
                return rootNode;
            }
            if (set.Where(r => r.Classification == "nowin").Count() == set.Count)
            {
                rootNode.IsLeaf = true;
                rootNode.Label = "nowin";
                return rootNode;
            }
            if (attributes.Any() == false)
            {
                rootNode.IsLeaf = true;                
                rootNode.Label = GetMostCommonClassification(set);
                return rootNode;
            }
            else
            {
                //Computing the attribute with the most information gain.
                Dictionary<string, double> informationGainPerAttribute = GetAttributeWithMostInformationGain(set, attributes);
                var bestAttribute = informationGainPerAttribute.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;


                //Setting T's decision attr to be that attribute.
                rootNode.DecisionAttribute = bestAttribute;

                foreach (var possibleValue in GetPossibleValuesForAttribute(bestAttribute, set))
                {
                    //Using the subset to either recurse, or add a leaf
                    var subset = set.Where(r => r.Attributes[bestAttribute] == possibleValue).ToList();

                    if (subset.Any())
                    {
                        //Add a new tree branch below Root, corresponding to the test attribute = possibleValue                        
                        rootNode.Children.Add(possibleValue, CreateTree(subset, attributes.Where(a => a != bestAttribute).ToList()));
                    }
                    else
                    {
                        Console.WriteLine("Adding a leaf");

                        var leafNode = new Node();
                        leafNode.IsLeaf = true;
                        leafNode.Label = GetMostCommonClassification(set);

                        rootNode.Children.Add(possibleValue, leafNode);
                        return leafNode;
                    }
                }
            }
            return rootNode;
        }



        /* MOST COMMON CLASSIFICATION / POSSIBLE VALUES PER ATTRIBUTE */
        private string GetMostCommonClassification(List<Record> set)
        {
            var amountOfPositiveClassifications = set.Where(r => r.Classification == "won").Count();
            var amountOfNegativeClassifications = set.Where(r => r.Classification == "nowin").Count();

            if (amountOfPositiveClassifications > amountOfNegativeClassifications)
                return "won";
            else
                return "nowin";
        }

        private List<int> GetPossibleValuesForAttribute(string attribute, List<Record> set)
        {
            var possibleValues = new HashSet<int>();

            foreach (var record in set)
            {
                var thisRecordsValueOfAttribute = record.Attributes[attribute];

                if (!possibleValues.Contains(thisRecordsValueOfAttribute))
                    possibleValues.Add(thisRecordsValueOfAttribute);
            }
            return possibleValues.ToList();
        }


        /* COMPUTING INFORMATION GAIN */

        private Dictionary<string, double> GetAttributeWithMostInformationGain(List<Record> set, List<string> allowedAttributes)
        {
            var returnDict = new Dictionary<string, double>();

            var setEntropy = ComputeEntropy(set);
            foreach (var attributeKV in set[0].Attributes.Where(a => allowedAttributes.Contains(a.Key)))
            {
                returnDict.Add(attributeKV.Key, ComputeInformationGain(attributeKV.Key, setEntropy));
            }
            return returnDict;
        }

        private double ComputeInformationGain(string currentAttribute, double entropyOfSet)
        {
            var informationGain = entropyOfSet;

            //Getting all the values of this attribute in the trainingset
            var allValuesOfThisAttribute = new List<int>();
            _trainingSet.ForEach(r => allValuesOfThisAttribute.Add(r.Attributes[currentAttribute]));

            foreach (var possibleValue in allValuesOfThisAttribute)
            {
                var recordsWithThisValueForCurrentAttribute = _trainingSet.Where(r => r.Attributes[currentAttribute] == possibleValue).ToList();

                informationGain -= (recordsWithThisValueForCurrentAttribute.Count / _trainingSet.Count) * ComputeEntropy(recordsWithThisValueForCurrentAttribute);
            }
            return informationGain;
        }


        /*
        Calculates a measure of how much the classifications of the dataset 'lean' to a certain classification label.
        Since we only use win and nowin, this is pretty easy.
        */
        private double ComputeEntropy(List<Record> set)
        {
            var total = set.Count;

            //Too literal, generalize this.
            var amountOfPositiveClassifications = set.Where(r => r.Classification == "won").Count();
            var amountOfNegativeClassifications = set.Where(r => r.Classification == "nowin").Count();

            double ratioPositive = (double)amountOfPositiveClassifications / total;
            double ratioNegative = (double)amountOfNegativeClassifications / total;

            if (ratioPositive != 0)
                ratioPositive = -(ratioPositive) * DoubleLog(ratioPositive);
            if (ratioNegative != 0)
                ratioNegative = -(ratioNegative) * DoubleLog(ratioNegative);

            return ratioPositive + ratioNegative;
        }

        private double DoubleLog(double x)
        {
            return Math.Log(x) / Math.Log(2);
        }
    }
}