using System;

namespace Classifiers
{
    public class ManhattanDistance : IDistance
    {
        public double GetDistance(Record a, Record b)
        {
            if (!a.HasTheSamePredictors(b))
                return double.PositiveInfinity;

            var manhattanDistance = 0.0;

            foreach (var predictorKv in a.Attributes)
            {
                var thisPredictorsValue = Convert.ToInt32(predictorKv.Value);
                var otherRecordsPredictorValue = Convert.ToInt32(b.Attributes[predictorKv.Key]);

                manhattanDistance += Math.Abs((thisPredictorsValue - otherRecordsPredictorValue));
            }
            return manhattanDistance;
        }
    }
}