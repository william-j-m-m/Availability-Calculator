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
        DateTime start = DateTime.Now;
        const string textFiles = @".\textFiles";
        var delta = new TimeSpan(1, 0, 0);

        (List<PersonAvailability> peopleAvailabilities, DateTime earliest, DateTime latest) = ReadDataIn(textFiles);

        Console.Write('\n');
        Console.WriteLine(earliest);
        Console.WriteLine(latest);
        Console.Write('\n');

        Console.Write("Enter number of people you want at any one time: ");
        int minNumOfPeeps = 2;
        // minNumOfPeeps = int.Parse(Console.ReadLine());

        Console.Write("Enter minimum length of streak (hours): ");
        //var minStreak = new TimeSpan(2, 0, 0);
        var minStreak = new TimeSpan(int.Parse(Console.ReadLine()), 0, 0);
        
        List<(DateTime start, DateTime end)> validStreaks =
            ProcessData(peopleAvailabilities, delta, earliest, latest, minNumOfPeeps, minStreak);
        Console.Write('\n');
        foreach (var n in validStreaks)
        {
            Console.WriteLine($"{n.start} -> {n.end}");
        }

        Console.WriteLine($"Time to run: {(DateTime.Now - start).TotalMilliseconds} ms");
    }

    private static (DateTime start, DateTime end) EncodeDateTime(string line)
    {
        // (DateTime start, DateTime end) output = (DateTime.Now, DateTime.Now);

        string[] splitLine = line.Split(",");

        string[] splitStartDate = splitLine[0].Split("/");
        string[] splitEndDate = splitLine[2].Split("/");

        string[] startTime = splitLine[1].Split(':');
        string[] endTime = splitLine[3].Split(':');

        DateTime startDateTime = new(int.Parse(splitStartDate[2]) + 2000, int.Parse(splitStartDate[1]),
            int.Parse(splitStartDate[0]), int.Parse(startTime[0]), int.Parse(startTime[1]), 0);
        DateTime endDateTime = new(int.Parse(splitEndDate[2]) + 2000, int.Parse(splitEndDate[1]), int.Parse(splitEndDate[0]),
            int.Parse(endTime[0]), int.Parse(endTime[1]), 0);

        Console.WriteLine($"{startDateTime} -> {endDateTime}");
        return (startDateTime, endDateTime);
    }


    private static (
        List<PersonAvailability> availability, DateTime earliest, DateTime latest)
        ReadDataIn(
            string folderPath
        )
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

    private static List<(DateTime start, DateTime end)> ProcessData(
        List<PersonAvailability> peopleAvailabilities,
        TimeSpan delta,
        DateTime earliest,
        DateTime latest,
        int minNumOfPeeps,
        TimeSpan minStreak
    )
    {
        List<(DateTime start, DateTime end)> validStreaks = [];

        var onStreak = false;
        DateTime streakStart = DateTime.MinValue;
        for (DateTime currentInterval = earliest; currentInterval <= latest; currentInterval += delta)
        {
            var numPeopleStreak = 0;
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

            if (numPeopleStreak >= minNumOfPeeps)
            {
                // IS VALID
                if (!onStreak)
                {
                    Console.WriteLine(" ==== STARTING STREAK ==== ");

                    onStreak = true;
                    streakStart = currentInterval;
                }

                Console.WriteLine(currentInterval);
                
            }
            else if (onStreak)
            {
                // NOT VALID, END STREAK
                onStreak = false;
                DateTime streakEnd = currentInterval - delta;
                if ((streakEnd - streakStart) >= minStreak)
                {
                    validStreaks.Add((streakStart, streakEnd));
                }
                
                Console.WriteLine(" ==== ENDING STREAK ==== ");
            }
            
        }

        return validStreaks;
    }
}