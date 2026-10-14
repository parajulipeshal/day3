
using System;
using System.Collections.Generic;
using System.Threading;

namespace day3InClass;

class Course
{
    public Dictionary<int, int> course_availseats = new();
    public readonly object seatLock = new();
    public readonly object anotherLock = new();

    public Course(int courses = 100, int availseats = 10_000)
    {
        for (int i = 1; i <= courses; i++)
        {
            course_availseats[i] = availseats;
        }
    }
}

class Student
{
    public int id;
    public List<int> courses;

    public Student(int in_id)
    {
        id = in_id;
        courses = new();
    }

    public void SelectCoursesForRegistration(int num)
    {
        while (courses.Count < num)
        {
            int pick = Random.Shared.Next(100) + 1;
            if (!courses.Contains(pick))
                courses.Add(pick);
        }
    }

    public bool RegisterMyCourses()
    {
        bool allRegistered = true;

        if (Program.approach == 0)
        {
            lock (Program.c.seatLock)
            {
                foreach (var c in courses)
                {
                    if (Program.c.course_availseats[c] > 0)
                        Program.c.course_availseats[c]--;
                    else
                        allRegistered = false;
                }
            }
        }
        else if (Program.approach == 1)
        {
            if (id % 2 == 0)
            {
                lock (Program.c.seatLock)
                {
                    lock (Program.c.anotherLock)
                    {
                        foreach (var c in courses)
                        {
                            if (Program.c.course_availseats[c] > 0)
                                Program.c.course_availseats[c]--;
                            else
                                allRegistered = false;
                        }
                    }
                }
            }
            else
            {
                lock (Program.c.anotherLock)
                {
                    lock (Program.c.seatLock)
                    {
                        foreach (var c in courses)
                        {
                            if (Program.c.course_availseats[c] > 0)
                                Program.c.course_availseats[c]--;
                            else
                                allRegistered = false;
                        }
                    }
                }
            }
        }
        else if (Program.approach == 2)
        {
            lock (Program.c.seatLock)
            {
                lock (Program.c.anotherLock)
                {
                    foreach (var c in courses)
                    {
                        if (Program.c.course_availseats[c] > 0)
                            Program.c.course_availseats[c]--;
                        else
                            allRegistered = false;
                    }
                }
            }
        }

        return allRegistered;
    }
}

class Program
{
    public static Course c = new();

    // 0 = original, 1 = deadlock, 2 = fixed
    public static int approach = 2;

    static void Main(string[] args)
    {
        List<Thread> myThreads = new();

        for (int i = 1; i <= 200; i++)
        {
            myThreads.Add(new Thread(() =>
            {
                for (int j = 1; j <= 1000; j++)
                {
                    Student s = new Student(j);
                    s.SelectCoursesForRegistration(5);

                    if (s.RegisterMyCourses() == false)
                        Console.WriteLine($"Student {s.id} failed to register course list");
                }
            }));
        }

        foreach (var x in myThreads)
        {
            x.Start();
        }

        foreach (var x in myThreads)
        {
            x.Join();
        }

        Console.WriteLine("All seats assigned");

        foreach (var x in c.course_availseats)
        {
            Console.WriteLine($"Course {x.Key} has {x.Value} seats still available");
        }
    }
}
