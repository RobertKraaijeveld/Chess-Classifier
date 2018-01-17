using System;

namespace Classifiers
{
    interface IClassifier
    {
        //Mostly used for pretty printing purposes
        string GetName();
        string GetClassification(Record r);
    }
}