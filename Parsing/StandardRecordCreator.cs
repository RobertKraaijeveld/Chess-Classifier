using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace Classifiers
{
    public class StandardRecordCreator : AbstractRecordCreator
    {
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
                    var numericalValue = Parser.NumericalAttributeValues[attributeIndex][currentAttributeValue];

                    newRecord.Attributes.Add(attributeIndex.ToString(), numericalValue);
                }
                returnData.Add(newRecord);
            }
            return returnData;
        }
    }
}