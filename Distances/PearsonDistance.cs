using System;

namespace Classifiers
{
    //Source: https://www.codeproject.com/Articles/49723/Linear-correlation-and-statistical-functions
    public class PearsonDistance : IDistance
    {
        public double GetDistance(Record a, Record b)
        {
            if (!a.HasTheSamePredictors(b))
                return double.PositiveInfinity;

            var tiny = double.MinValue;
            double yT, xT;
            Double syy = 0.0, sxy = 0.0, sxx = 0.0, ay = 0.0, ax = 0.0;

            var aMean = ComputeMean(a);
            var bMean = ComputeMean(b);

            foreach (var predictorKv in a.Attributes)
            {
                var thisPredictorsValue = Convert.ToInt32(predictorKv.Value);
                var otherRecordsPredictorValue = Convert.ToInt32(b.Attributes[predictorKv.Key]);

                xT = thisPredictorsValue - ax;
                yT = otherRecordsPredictorValue - ay;
                sxx += xT * xT;
                syy += yT * yT;
                sxy += xT * yT;
            }
            
            return sxy / (Math.Sqrt(sxx*syy) + tiny);
        }

        private double ComputeMean(Record r)
        {
            var mean = 0.0;

            foreach (var predictorKv in r.Attributes)
            {
                var thisPredictorsValue = Convert.ToInt32(predictorKv.Value);
                mean += thisPredictorsValue;
            }
            return mean / r.Attributes.Count;
        }

        /*
            /will regularize the unusual case of complete correlation
        
        for (j=0;j<n;j++) {
        // compute correlation coefficient
            xT=x[j]-ax;
            yT=y[j]-ay;
            sxx += xT*xT;
            syy += yT*yT;
            sxy += xT*yT;
        }
        r=sxy/(Math.Sqrt(sxx*syy)+TINY);
        //for a large n
        prob=erfcc(Math.Abs(z*Math.Sqrt(n-1.0))/1.4142136);
        */
    }
}
