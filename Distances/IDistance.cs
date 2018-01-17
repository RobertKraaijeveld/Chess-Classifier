using System;

namespace Classifiers
{
    public interface IDistance
    {
        double GetDistance(Record a, Record b);
    }
}