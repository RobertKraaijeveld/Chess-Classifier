using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;


namespace Classifiers
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();

            var standardTrainingData = parser.GetTrainingData(new StandardRecordCreator());
            var standardTestData = parser.GetTestData(new StandardRecordCreator());
            var K = 14; 
            var knnClassifier = new KNN(standardTrainingData, K, new PearsonDistance());

            var trainingDataForNaiveBayes = parser.GetTrainingData(new NaiveBayesRecordCreator());
            var testDataForNaiveBayes = parser.GetTestData(new NaiveBayesRecordCreator());
            var naiveBayesClassifier = new NaiveBayesClassifier(trainingDataForNaiveBayes);


            TestClassifier(knnClassifier, standardTestData);
            TestClassifier(naiveBayesClassifier, testDataForNaiveBayes);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var ID3Classifier = new ID3(standardTrainingData);
            Console.WriteLine("Done with ID3 in " + sw.ElapsedMilliseconds / 1000 + "s. ");

            /*
            8
            20
            31
            32
            */
        }

        private static void TestClassifier(IClassifier classifier, List<Record> testData)
        {
            //Classifying all the testData, seeing whether they were right or not.
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int correctClassificationCount = 0;
            foreach (var testRecord in testData)
            {
                var actualClassification = testRecord.Classification;
                var predictedClassification = classifier.GetClassification(testRecord);

                if (predictedClassification.Equals(actualClassification))
                    correctClassificationCount++;
            }
            sw.Stop();

            var elapsedSeconds = sw.ElapsedMilliseconds / 1000;
            var correctnessPercentage = Math.Round((((double)correctClassificationCount / testData.Count) * 100.0), 2);

            Console.WriteLine("========================================");
            Console.WriteLine("Done classifying using " + classifier.GetName()  + ". Elapsed time (s): " + elapsedSeconds);
            Console.WriteLine("Correct classification percentage: " + correctnessPercentage + "%");
            Console.WriteLine("========================================");
        }
    }
}