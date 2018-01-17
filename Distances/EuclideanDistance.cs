using System;

namespace Classifiers
{
    public class EuclideanDistance : IDistance
    {
        public double GetDistance(Record a, Record b)
        {
            if (!a.HasTheSamePredictors(b))
                return double.PositiveInfinity;

            var euclidDistance = 0.0;

            foreach (var predictorKv in a.Attributes)
            {
                var thisPredictorsValue = Convert.ToInt32(predictorKv.Value);
                var otherRecordsPredictorValue = Convert.ToInt32(b.Attributes[predictorKv.Key]);

                euclidDistance += Math.Pow((thisPredictorsValue - otherRecordsPredictorValue), 2);
            }
            return Math.Sqrt(euclidDistance);
        }
    }
}