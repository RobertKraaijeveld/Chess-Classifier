using System.Collections.Generic;
using System.Linq;

namespace Classifiers
{
    public class KNN : IClassifier
    {
        private List<Record> _trainingSet;
        private readonly int K;
        private readonly IDistance distanceMeasure;

        public KNN(List<Record> trainingSet, int K, IDistance distanceMeasure)
        {
            this.K = K;
            _trainingSet = trainingSet;
            this.distanceMeasure = distanceMeasure;
        }

        public string GetName()
        {
            return "K Nearest Neighbours";
        }

        public string GetClassification(Record newRecord)
        {
            //Getting all the distances
            Dictionary<Record, double> distancesToNeighbours = new Dictionary<Record, double>();
            foreach (var alreadyClassifiedRecord in _trainingSet)
            {
                var distanceToThisClassifiedRecord = distanceMeasure.GetDistance(newRecord, alreadyClassifiedRecord);

                distancesToNeighbours.Add(alreadyClassifiedRecord, distanceToThisClassifiedRecord);
            }

            //Getting the K amount of closest neighbours
            var KNeighbours = distancesToNeighbours.OrderByDescending(n => n.Value).Take(K);

            //Counting the classifications of the neighbours
            var classificationsCounts = new Dictionary<string, int>();
            foreach (var neighbour in KNeighbours)
            {
                var neighboursClassification = neighbour.Key.Classification;

                if (classificationsCounts.ContainsKey(neighboursClassification) == false)
                    classificationsCounts.Add(neighboursClassification, 1);
                else
                    classificationsCounts[neighboursClassification] += 1;
            }

            //Returning the classification which happens most among the neighbours
            return classificationsCounts.ToList().OrderByDescending(c => c.Value).First().Key;
        }
    }
}