using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace Classifiers
{
    public class Parser
    {
        public int LineCount;
        private readonly string[] Lines;
        //First key is the attribute index, second key is the value of the attribute in the file, last is the numerical value.
        public static Dictionary<int, Dictionary<string, int>> NumericalAttributeValues = new Dictionary<int, Dictionary<string, int>>();


        public Parser()
        {
            Lines = File.ReadAllLines(@"chess.data");
            LineCount = Lines.Length;

            for (int i = 0; i < 36; i++)
            {
                var numericCounter = 0;
                NumericalAttributeValues.Add(i, new Dictionary<string, int>());

                //Going through this attribute depth first             
                foreach (var line in Lines)
                {
                    var currentAttributeValue = line.Split(',')[i];

                    //If weve never encountered this attributes value for this attr before, add it
                    if (NumericalAttributeValues[i].ContainsKey(currentAttributeValue) == false)
                    {
                        NumericalAttributeValues[i].Add(currentAttributeValue, numericCounter);
                        numericCounter++;
                    }
                }
            }
        }


        public List<Record> GetTrainingData(AbstractRecordCreator recordsCreator)
        {
            var trainingData = new List<Record>();

            var twoThirdOfLines = Lines.Take((int)Math.Round(Lines.Length / 1.5));
            trainingData = recordsCreator.CreateRecords(twoThirdOfLines);

            return trainingData;
        }

        public List<Record> GetTestData(AbstractRecordCreator recordsCreator)
        {
            var testData = new List<Record>();

            var oneThirdOfLines = Lines.Skip(Lines.Length / 3 * 2 +1);
            testData = recordsCreator.CreateRecords(oneThirdOfLines);    

            return testData;
        }
    }
}