using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;


namespace Classifiers
{
    class Program
    {
        static void Main(string[] args)
        {
            //Parsing
            Parser parser = new Parser();

            var standardTrainingData = parser.GetTrainingData(new StandardRecordCreator());
            var standardTestData = parser.GetTestData(new StandardRecordCreator());
            var trainingDataForNaiveBayes = parser.GetTrainingData(new NaiveBayesRecordCreator());
            var testDataForNaiveBayes = parser.GetTestData(new NaiveBayesRecordCreator());


            //KNN
            var knnConstructionTimeStopwatch = new Stopwatch();
            knnConstructionTimeStopwatch.Start();
            var K = 5;
            var knnClassifier = new KNN(standardTrainingData, K, new EuclideanDistance());
            knnConstructionTimeStopwatch.Stop();

            //NAIVE BAYES
            var naiveBayesConstructionTimeStopwatch = new Stopwatch();
            naiveBayesConstructionTimeStopwatch.Start();
            var naiveBayesClassifier = new NaiveBayesClassifier(trainingDataForNaiveBayes);
            naiveBayesConstructionTimeStopwatch.Stop();

            //ID3
            var id3ConstructionTimeStopwatch = new Stopwatch();
            id3ConstructionTimeStopwatch.Start();
            var ID3Classifier = new ID3(standardTrainingData);
            id3ConstructionTimeStopwatch.Stop();


            //Testing the classifiers
            TestClassifier(knnClassifier, standardTestData, knnConstructionTimeStopwatch.ElapsedMilliseconds);
            TestClassifier(naiveBayesClassifier, testDataForNaiveBayes, naiveBayesConstructionTimeStopwatch.ElapsedMilliseconds);
            TestClassifier(ID3Classifier, standardTestData, id3ConstructionTimeStopwatch.ElapsedMilliseconds);
        }

        private static void TestClassifier(IClassifier classifier, List<Record> testData, double constructionTimeInMs)
        {
            //Classifying all the testData, seeing whether they were right or not.
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int correctClassificationCount = 0;
            var predictedClassifications = new List<Record>();
            foreach (var testRecord in testData)
            {
                var actualClassification = testRecord.Classification;
                var predictedClassification = classifier.GetClassification(testRecord);

                if (predictedClassification.Equals(actualClassification))
                {
                    correctClassificationCount++;
                    testRecord.Classification = predictedClassification;
                    predictedClassifications.Add(testRecord);
                }
            }
            sw.Stop();

            var elapsedSeconds = (sw.ElapsedMilliseconds + constructionTimeInMs) / 1000;
            var correctnessPercentage = Math.Round((((double)correctClassificationCount / testData.Count) * 100.0), 2);

            Console.WriteLine("========================================");
            Console.WriteLine("Done classifying using " + classifier.GetName() + ". Elapsed time (s): " + elapsedSeconds);
            Console.WriteLine("Correct classification percentage: " + correctnessPercentage + "%");
            Console.WriteLine("========================================");

            WriteToFile(predictedClassifications, classifier.GetName());
        }

        public static void WriteToFile(List<Record> classifiedRecords, string classifierName)
        {
            var strings = new List<string>();
            var fileName = @classifierName + "_RESULT.data";

            foreach (var record in classifiedRecords)
            {
                strings.Add(record.ToString());
            }

            using (var tw = new StreamWriter(fileName, true))
            {
                foreach (var line in strings)
                {
                    tw.WriteLine(line);
                }
            }
        }
    }
}