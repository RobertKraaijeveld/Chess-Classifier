using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace Classifiers
{
    public class NaiveBayesRecordCreator : AbstractRecordCreator
    {
        //Naive bayes records: all the possible values of each attribute are in separate records. 1 means this value was present, 0 means it wasnt
        public override List<Record> CreateRecords(IEnumerable<string> lines)
        {
            var returnData = new List<Record>();

            foreach (var line in lines)
            {
                var attributeValues = line.Split(',');
                var classificationLabel = attributeValues[attributeValues.Length - 1];

                Record newRecord = new Record(classificationLabel);

                foreach(var attributeIndex in AttributePicker.GetPickedAttributeIndices())
                {
                    var currentAttributeValue = attributeValues[attributeIndex];
                    var allPossibleValuesForThisAttribute = Parser.NumericalAttributeValues[attributeIndex].Keys.ToList();

                    foreach (var possibleValue in allPossibleValuesForThisAttribute)
                    {
                        //We insert a record for each possible value, containing whether or not this row has that value. We do this for each attribute. 
                        var oneOrZero = currentAttributeValue.Equals(possibleValue) ? 1 : 0;
                        newRecord.Attributes.Add(attributeIndex + ": " + possibleValue, oneOrZero);
                    }
                }
                returnData.Add(newRecord);
            }
            return returnData;
        }
    }
}