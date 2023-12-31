﻿#define manual

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

        (List<PersonAvailability> peopleAvailabilities, DateTime earliest, DateTime latest) =
            ReadDataIn(textFiles, delta);

        // Console.Write('\n');
        // Console.WriteLine(earliest);
        // Console.WriteLine(latest);
        // Console.Write('\n');

        // const bool manual = true;


        Console.Write("Enter number of people you want at any one time: ");

#if manual
        int minNumOfPeeps = int.Parse(Console.ReadLine());
#else
        var minNumOfPeeps = 2;
        Console.WriteLine(minNumOfPeeps);
#endif

        Console.Write("Enter minimum length of streak (hours): ");

#if manual
        var minStreak = new TimeSpan(int.Parse(Console.ReadLine()), 0, 0);
#else
            var minStreak = new TimeSpan(2, 0, 0);
            Console.WriteLine(minStreak.TotalHours);
#endif


        List<string> people = [];
        foreach (PersonAvailability personAvailability in peopleAvailabilities)
        {
            people.Add(personAvailability.Name);
        }

        var shownPeople = new List<string>(people);

        List<string> requiredPeople = [];

        if (minNumOfPeeps != peopleAvailabilities.Count)
        {
#if manual
            Console.WriteLine("Enter the number of the people you'd like to require to be there:");
            for (var i = 0; i < people.Count; i++)
            {
                var counter = 1;

                foreach (string name in shownPeople)
                {
                    Console.WriteLine($"[{counter++}] {name}");
                }

                Console.WriteLine($"[{counter}] Finish");
                Console.Write(" -> ");
                int selected = int.Parse(Console.ReadLine());

                if (selected > counter || selected <= 0)
                {
                    // Invalid
                    continue;
                }

                if (selected == counter)
                {
                    // They selected finish
                    break;
                }

                // They selected a name
                requiredPeople.Add(shownPeople[selected - 1]);
                shownPeople.RemoveAt(selected - 1);
            }
#else
            requiredPeople.Add(people[0]);
#endif

            foreach (var requiredPerson in requiredPeople)
            {
                Console.Write($"{requiredPerson}, ");
                if (!people.Contains(requiredPerson))
                {
                    throw new Exception("You're trying to require a person who does not exist");
                }
            }

            Console.Write('\n');
        }

        List<(DateTime start, DateTime end)> validStreaks = ProcessData(
            peopleAvailabilities,
            delta,
            earliest,
            latest,
            minNumOfPeeps,
            minStreak,
            activeHours,
            requiredPeople
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
        (DateTime start, DateTime end) activeHours,
        List<string> requiredPeople)
    {
        var numPeopleStreak = 0;
        foreach (PersonAvailability person in peopleAvailabilities)
        {
            bool isAvailable = false;
            foreach ((DateTime start, DateTime end) availableInterval in person.Availability)
            {
                if (currentInterval >= availableInterval.start && currentInterval <= availableInterval.end)
                {
                    numPeopleStreak++;
                    isAvailable = true;
                }
            }

            if (requiredPeople.Contains(person.Name) && !isAvailable)
            {
                return false;
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

        // foreach (string requiredPerson in requiredPeople)
        // {
        //     for (int numPeople = 0; numPeople < peopleAvailabilities.Count; numPeople++)
        //     {
        //         if (requiredPerson == peopleAvailabilities[numPeople].Name)
        //         {
        //             bool meetsCriteria = false;
        //             foreach ((DateTime start, DateTime end) interval in peopleAvailabilities[numPeople].Availability)
        //             {
        //                 if (currentInterval >= interval.start && currentInterval <= interval.end)
        //                 {
        //                     meetsCriteria = true;
        //                 }
        //             }
        //
        //             if (!meetsCriteria)
        //             {
        //                 return false;
        //             }
        //         }
        //     }
        // }

        return true;
    }

    private static List<(DateTime start, DateTime end)> ProcessData(
        List<PersonAvailability> peopleAvailabilities,
        TimeSpan delta,
        DateTime earliest,
        DateTime latest,
        int minNumOfPeeps,
        TimeSpan minStreak,
        (DateTime start, DateTime end) activeHours,
        List<string> requiredPeople
    )
    {
        List<(DateTime start, DateTime end)> validStreaks = [];

        var onStreak = false;
        var streakStart = DateTime.MinValue;
        for (DateTime currentInterval = earliest; currentInterval <= latest; currentInterval += delta)
        {
            if (IsValid(currentInterval, peopleAvailabilities, minNumOfPeeps, activeHours, requiredPeople))
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