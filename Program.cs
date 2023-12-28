﻿// See https://aka.ms/new-console-template for more information

using System;

namespace PersonAvailabilityCalculator
{
    class Program
    {
        struct PersonAvailability
        {
            public PersonAvailability(string name, List<(DateTime start, DateTime end)> availability)
            {
                Name = name;
                Availability = availability;
            }

            public string Name;
            public List<(DateTime start, DateTime end)> Availability;
        }
        
        public static void Main(string[] args)
        {
            const string textFiles = @".\textFiles";
            ReadDataIn(textFiles);
        }

        static (DateTime start, DateTime end) EncodeDateTime(string line)
        {
            // (DateTime start, DateTime end) output = (DateTime.Now, DateTime.Now);

            string[] splitLine = line.Split(",");

            string[] splitDate = splitLine[0].Split("/");

            string[] startTime = splitLine[1].Split(':');
            string[] endTime = splitLine[2].Split(':');

            DateTime startDateTime = new(int.Parse(splitDate[2]) + 2000, int.Parse(splitDate[1]),
                int.Parse(splitDate[0]), int.Parse(startTime[0]), int.Parse(startTime[1]), 0);
            DateTime endDateTime = new(int.Parse(splitDate[2]) + 2000, int.Parse(splitDate[1]), int.Parse(splitDate[0]),
                int.Parse(endTime[0]), int.Parse(endTime[1]), 0);

            Console.WriteLine(startDateTime);
            return (startDateTime, endDateTime);
        }


        static void ReadDataIn(string folderPath)
        {
            string[] fileNames = Directory.GetFiles(folderPath);
            foreach (string fileName in fileNames)
            {
                using (var reader = new StreamReader(fileName))
                {
                    string name = fileName.Split(@"\")[^1].Split(".").First();
                    Console.WriteLine($"Reading data for {name}");
                    var listOfDates = new List<(DateTime Start, DateTime End)>();

                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        (DateTime Start, DateTime End) dates = EncodeDateTime(line);
                        listOfDates.Add(dates);
                    }

                    var personAvailability = new PersonAvailability(name, listOfDates);
                }
            }
        }

        static void ProcessData()
        {
        }
    }
}