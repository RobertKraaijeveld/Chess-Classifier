using System;
using System.Linq;
using System.Collections.Generic;

namespace Classifiers
{
    public class NaiveBayesClassifier : IClassifier
    {
        public List<Record> classifiedSet;
        private Dictionary<string, int> classificationCounts = new Dictionary<string, int>();
        private Table<string, double> FrequencyTable = new Table<string, double>();
        private Table<string, double> LikelihoodTable = new Table<string, double>();

        public NaiveBayesClassifier(List<Record> classifiedSet)
        {
            this.classifiedSet = classifiedSet;

            CreateFrequencyTableAndCountClassifications();
            CreateLikelihoodTable();
        }

        public string GetName()
        {
            return "Naive bayesian";
        }

        public string GetClassification(Record recordToBeClassified)
        {
            var likelihoodsPerClassification = new Dictionary<string, double>();

            foreach (var possibleClassification in FrequencyTable.rowsPerColumn.Keys)
            {
                var thisClassificationsProbability = GetProbabilityOfGivenClassification(possibleClassification, recordToBeClassified); 
                likelihoodsPerClassification.Add(possibleClassification, thisClassificationsProbability);
            }

            var sortedLikelihoodsPerClassification = likelihoodsPerClassification.OrderByDescending(kv => kv.Value).ToList();
            var mostLikelyClassification = sortedLikelihoodsPerClassification.First();

            return mostLikelyClassification.Key; 
        }

        private double GetProbabilityOfGivenClassification(string currentPossibleClassification,
                                                           Record recordToBeClassified)
        {
            var priorClassProbability = GetClassPriorProbability(currentPossibleClassification);    
            double finalProbability = priorClassProbability;

            foreach(var predictorKV in LikelihoodTable.rowsPerColumn[currentPossibleClassification])
            {
                var predictorsName = predictorKV.Key;
                var predictorLikelihood = predictorKV.Value;

                if(recordToBeClassified.Attributes[predictorsName] == 1)
                    finalProbability = finalProbability * predictorLikelihood;
            }
            return finalProbability;
        }


        /**
        Table creation
        **/

        private void CreateFrequencyTableAndCountClassifications()
        {
            foreach (var record in classifiedSet)
            {
                //populating count dict
                if(!classificationCounts.ContainsKey(record.Classification))
                    classificationCounts.Add(record.Classification, 1);
                else
                    classificationCounts[record.Classification] += 1;

                //populating frequency table
                FrequencyTable.PotentiallyAddClassification(record.Classification);

                foreach (var predictor in record.Attributes)
                {
                    if (!FrequencyTable.rowsPerColumn[record.Classification].ContainsKey(predictor.Key))
                        FrequencyTable.rowsPerColumn[record.Classification].Add(predictor.Key, 0);

                    if (predictor.Value == 1)
                    {
                        FrequencyTable.rowsPerColumn[record.Classification][predictor.Key] += 1;
                    }
                }
            }
        }

        private void CreateLikelihoodTable()
        {
            foreach (var record in FrequencyTable.rowsPerColumn)
            {
                var classificationOfItem = record.Key;
                LikelihoodTable.PotentiallyAddClassification(classificationOfItem);

                foreach (var predictor in record.Value)
                {
                    var likelihoodOfThisPredictor = predictor.Value / GetCountsOfClassification(FrequencyTable, classificationOfItem);

                    if (!LikelihoodTable.rowsPerColumn[classificationOfItem].ContainsKey(predictor.Key))
                        LikelihoodTable.rowsPerColumn[classificationOfItem].Add(predictor.Key, likelihoodOfThisPredictor);
                    else
                        LikelihoodTable.rowsPerColumn[classificationOfItem][predictor.Key] = likelihoodOfThisPredictor;
                }
            }
        }

        private int GetCountsOfClassification(Table<string, double> table, string classification)
        {
            return classificationCounts[classification];
        }


        /**
        Formula computation
        **/
        private double GetClassPriorProbability(string classification)
        {
            return (double) classificationCounts[classification] / classifiedSet.Count();
        }      
    }
}
