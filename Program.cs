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

        (DateTime start, DateTime end) activeHours = (
            new DateTime(new DateOnly(), new TimeOnly(9, 0)),
            new DateTime(new DateOnly(), new TimeOnly(23, 0))
        );

        (List<PersonAvailability> peopleAvailabilities, DateTime earliest, DateTime latest) = ReadDataIn(textFiles, delta);

        // Console.Write('\n');
        // Console.WriteLine(earliest);
        // Console.WriteLine(latest);
        // Console.Write('\n');

        const bool manual = false;

        Console.Write("Enter number of people you want at any one time: ");

        var minNumOfPeeps = 2;
        if (manual)
        {
            minNumOfPeeps = int.Parse(Console.ReadLine());
        }
        else
        {
            Console.WriteLine(minNumOfPeeps);
        }

        Console.Write("Enter minimum length of streak (hours): ");
        var minStreak = new TimeSpan(2, 0, 0);
        if (manual)
        {
            minStreak = new TimeSpan(int.Parse(Console.ReadLine()), 0, 0);
        }
        else
        {
            Console.WriteLine(minStreak.TotalHours);
        }

        List<(DateTime start, DateTime end)> validStreaks = ProcessData(
            peopleAvailabilities,
            delta,
            earliest,
            latest,
            minNumOfPeeps,
            minStreak,
            activeHours
        );

        Console.Write('\n');
        
        foreach ((DateTime start, DateTime end) streak in validStreaks)
        {
            string prevPeopleAvailableAtHour = "";
            Console.Write($"{streak.start:g} -> ");
            if (streak.start.Date == streak.end.Date)
            {
                Console.WriteLine(streak.end.ToString("t"));
            }
            else
            {
                Console.WriteLine(streak.end.ToString("g"));
            }
            
            for (DateTime interval = streak.start; interval <= streak.end; interval += delta)
            {
                Console.Write($"\t| {interval:HH:mm}");

                string peopleAvailableAtHour = PeopleAvailableAtHour(interval, peopleAvailabilities);
                
                if (peopleAvailableAtHour != prevPeopleAvailableAtHour)
                {
                    Console.WriteLine($" {peopleAvailableAtHour}");
                }
                else
                {
                    Console.Write('\n');
                }
                    
                prevPeopleAvailableAtHour = peopleAvailableAtHour;
            }
            Console.Write('\n');
        }

        Console.WriteLine($"Time to run: {(DateTime.Now - start).TotalMilliseconds} ms");
    }

    private static (DateTime start, DateTime end) EncodeDateTime(string line)
    {
        // (DateTime start, DateTime end) output = (DateTime.Now, DateTime.Now);

        string[] splitLine = line.Split(',');

        string[] splitStartDate = splitLine[0].Split('/');
        string[] splitEndDate = splitLine[2].Split('/');

        string[] startTime = splitLine[1].Split(':');
        string[] endTime = splitLine[3].Split(':');

        DateTime startDateTime = new(int.Parse(splitStartDate[2]) + 2000, int.Parse(splitStartDate[1]),
            int.Parse(splitStartDate[0]), int.Parse(startTime[0]), int.Parse(startTime[1]), 0);
        DateTime endDateTime = new(int.Parse(splitEndDate[2]) + 2000, int.Parse(splitEndDate[1]),
            int.Parse(splitEndDate[0]),
            int.Parse(endTime[0]), int.Parse(endTime[1]), 0);

        // Console.WriteLine($"{startDateTime} -> {endDateTime}");
        return (startDateTime, endDateTime);
    }


    private static (
        List<PersonAvailability> availability, DateTime earliest, DateTime latest)
        ReadDataIn(
            string folderPath,
            TimeSpan delta
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
                // Console.WriteLine($"Reading data for {name}");
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
        
        DateTime now = DateTime.Now;
        if (earliest < now)
        {
            long ticks = now.Ticks / delta.Ticks;

            earliest = new DateTime(ticks * delta.Ticks, now.Kind);
        }

        return (peopleAvailabilities, earliest, latest);
    }

    private static bool IsValid(
        DateTime currentInterval,
        List<PersonAvailability> peopleAvailabilities,
        int minNumOfPeeps,
        (DateTime start, DateTime end) activeHours
    )
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

        // If the minimum number of people are not available
        if (numPeopleStreak < minNumOfPeeps)
        {
            return false;
        }

        if (currentInterval.TimeOfDay < activeHours.start.TimeOfDay)
        {
            return false;
        }

        if (currentInterval.TimeOfDay > activeHours.end.TimeOfDay)
        {
            return false;
        }


        return true;
    }

    private static List<(DateTime start, DateTime end)> ProcessData(
        List<PersonAvailability> peopleAvailabilities,
        TimeSpan delta,
        DateTime earliest,
        DateTime latest,
        int minNumOfPeeps,
        TimeSpan minStreak,
        (DateTime start, DateTime end) activeHours
    )
    {
        List<(DateTime start, DateTime end)> validStreaks = [];

        var onStreak = false;
        var streakStart = DateTime.MinValue;
        for (DateTime currentInterval = earliest; currentInterval <= latest; currentInterval += delta)
        {
            if (IsValid(currentInterval, peopleAvailabilities, minNumOfPeeps, activeHours))
            {
                // IS VALID
                if (!onStreak)
                {
                    // Console.WriteLine(" ==== STARTING STREAK ==== ");

                    onStreak = true;
                    streakStart = currentInterval;
                }

                // Console.WriteLine(currentInterval);
            }
            else
            {
                if (!onStreak) continue;
                // NOT VALID, END STREAK
                onStreak = false;
                DateTime streakEnd = currentInterval - delta;
                if ((streakEnd - streakStart) >= minStreak)
                {
                    validStreaks.Add((streakStart, streakEnd));
                }

                // Console.WriteLine(" ==== ENDING STREAK ==== ");
            }
        }

        return validStreaks;
    }

    private static string PeopleAvailableAtHour(DateTime current, List<PersonAvailability> peopleAvailabilities)
    {
        string peopleHour = "";
        foreach (PersonAvailability person in peopleAvailabilities)
        {
            foreach ((DateTime start, DateTime end) availableInterval in person.Availability)
            {
                if (current >= availableInterval.start && current <= availableInterval.end)
                {
                    if (peopleHour == "")
                    {
                        peopleHour += person.Name;
                    }
                    else
                    {
                        peopleHour += $", {person.Name}";
                    }
                }
            }
        }

        return peopleHour;
    }
}