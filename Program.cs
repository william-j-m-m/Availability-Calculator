namespace PersonAvailabilityCalculator;

public abstract class Program
{
    private struct PersonAvailability(string name, List<(DateTime start, DateTime end)> availability)
    {
        public readonly string Name = name;
        public readonly List<(DateTime start, DateTime end)> Availability = availability;
    }

    public static void Main(string[] args)
    {
        const string textFiles = @".\textFiles";
        var delta = new TimeSpan(1, 0, 0);
        
        (List<PersonAvailability> peopleAvailabilities, DateTime earliest, DateTime latest) = ReadDataIn(textFiles);
        // Console.WriteLine(peopleAvailabilities[0].Name);
        Console.WriteLine('\n');
        Console.WriteLine(earliest);
        Console.WriteLine(latest);
        ProcessData(peopleAvailabilities, delta, earliest, latest);
    }

    private static (DateTime start, DateTime end) EncodeDateTime(string line)
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

        Console.WriteLine($"{startDateTime} -> {endDateTime.TimeOfDay}");
        return (startDateTime, endDateTime);
    }


    private static (List<PersonAvailability> availability, DateTime earliest, DateTime latest) ReadDataIn(string folderPath)
    {
        var earliest = DateTime.MaxValue;
        var latest = DateTime.MinValue;
        
        var peopleAvailabilities = new List<PersonAvailability>();
        string[] fileNames = Directory.GetFiles(folderPath);
        foreach (string fileName in fileNames)
        {
            using (var reader = new StreamReader(fileName))
            {
                string name = fileName.Split(@"\")[^1].Split(".").First();
                Console.WriteLine($"Reading data for {name}");
                var listOfDates = new List<(DateTime Start, DateTime End)>();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    (DateTime Start, DateTime End) dates = EncodeDateTime(line);
                    listOfDates.Add(dates);

                    if (dates.Start < earliest)
                    {
                        earliest = dates.Start;
                    }

                    if (dates.End > latest)
                    {
                        latest = dates.End;
                    }
                }

                var personAvailability = new PersonAvailability(name, listOfDates);
                peopleAvailabilities.Add(personAvailability);
            }
        }

        return (peopleAvailabilities, earliest, latest);
    }

    private static void ProcessData(List<PersonAvailability> peopleAvailabilities, TimeSpan delta, DateTime earliest, DateTime latest)
    {
        for (DateTime currentInterval = earliest; currentInterval <= latest; currentInterval += delta)
        {
            int numPeopleStreak = 0;
            foreach (PersonAvailability person in peopleAvailabilities)
            {
                foreach ((DateTime start, DateTime end) availableInterval in person.Availability)
                {
                    if (currentInterval >= availableInterval.start && currentInterval <= availableInterval.end)
                    {
                        numPeopleStreak++;
                    }
                }
            }

            if (numPeopleStreak >= 2)
            {
                Console.WriteLine(currentInterval);
                
            }
            
        }
    }
}