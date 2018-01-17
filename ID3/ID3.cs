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
            return "won";
        }

        //FOUND IT: The attributes param is never used!!!
        public Node CreateTree(List<Record> set, List<string> attributes)
        {
            Node rootNode = new Node();
            rootNode.Label = GetMostCommonClassification(set);

            //If all positive, return T with label 'won'
            if (set.Where(r => r.Classification == "won").Count() == set.Count)
            {
                rootNode.Label = "won";
                return rootNode;
            }
            else if (set.Where(r => r.Classification == "nowin").Count() == set.Count)
            {
                rootNode.Label = "nowin";
                return rootNode;
            }
            else if (attributes.Any() == false)
            {
                return rootNode;
            }
            else
            {
                //Computing the attribute with the most information gain.
                Dictionary<string, double> informationGainPerAttribute = GetAttributeWithMostInformationGain(set, attributes);
                var mostGainableAttribute = informationGainPerAttribute.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;


                //Setting T's decision attr to be that attribute.
                rootNode.DecisionAttribute = mostGainableAttribute;

                foreach (var possibleValue in GetPossibleValuesForAttribute(mostGainableAttribute, set))
                {
                    //Add a new tree branch below Root, corresponding to the test A = vi. ??
                    var subset = set.Where(r => r.Attributes[mostGainableAttribute] == possibleValue).ToList();

                    if(subset.Any())
                    {
                        //below this new branch add the subtree ID3 (Examples(vi), Target_Attribute, Attributes – {A})


                        //Check whether this (attributes.Where(a => a != mostGainableAttribute).ToList()) actually happens
                        //It should be empty after the first run, since we only use one bloody attribute!
                        rootNode.Children.Add(CreateTree(subset, attributes.Where(a => a != mostGainableAttribute).ToList()));
                    }
                    else
                    {
                        //below this new branch add a leaf node with label = most common classification in the current set
                        var leafNode = new Node();
                        leafNode.IsLeaf = true;
                        leafNode.Label = GetMostCommonClassification(set);

                        rootNode.Children.Add(leafNode);
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