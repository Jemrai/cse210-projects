/*
Exceeding Requirements:
- Implemented statistics tracking for the number of sessions performed and total time spent for each activity type.
- In the Reflection Activity, modified the question selection to ensure no question is repeated until all questions have been used at least once during the session.
- Added a more dynamic countdown in the Breathing Activity that adjusts the final breath phase if necessary to avoid exceeding the specified duration.
- Enhanced the Listing Activity to approximate the duration by starting a timer after the preparation countdown and checking elapsed time after each user input.
*/

using System;
using System.Collections.Generic;
using System.Threading;

public abstract class MindfulnessActivity
{
    protected string _name;
    protected string _description;
    protected int _duration;
    protected Random _random;

    public MindfulnessActivity(string name, string description)
    {
        _name = name;
        _description = description;
        _random = new Random();
    }

    public int Duration => _duration;

    public virtual void StartActivity()
    {
        Console.WriteLine();
        Console.WriteLine($"Starting the {_name} Activity");
        Console.WriteLine(_description);
        Console.WriteLine("How long, in seconds, would you like for your session?");
        _duration = int.Parse(Console.ReadLine());
        Console.WriteLine("Get ready...");
        DotsPause(3);
    }

    public virtual void EndActivity()
    {
        Console.WriteLine();
        Console.WriteLine("Well done!!");
        DotsPause(2);
        Console.WriteLine($"You've completed the {_name} Activity for {_duration} seconds.");
        DotsPause(3);
    }

    protected void DotsPause(int seconds)
    {
        for (int i = 0; i < seconds; i++)
        {
            Console.Write(".");
            Thread.Sleep(1000);
        }
        Console.WriteLine();
    }

    protected void CountdownPause(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            Console.Write($"{i} ");
            Thread.Sleep(1000);
        }
        Console.WriteLine();
    }

    protected void SpinnerPause(int seconds)
    {
        char[] spinner = { '|', '/', '-', '\\' };
        for (int s = 0; s < seconds; s++)
        {
            for (int j = 0; j < 4; j++)
            {
                Console.Write(spinner[j]);
                Thread.Sleep(250);
                Console.Write("\b");
            }
        }
        Console.WriteLine();
    }

    public abstract void PerformActivity();
}

public class BreathingActivity : MindfulnessActivity
{
    public BreathingActivity() : base("Breathing", "This activity will help you relax by walking you through breathing in and out slowly. Clear your mind and focus on your breathing.")
    {
    }

    public override void PerformActivity()
    {
        DateTime startTime = DateTime.Now;
        bool breatheIn = true;
        int breathSeconds = 4;

        while ((DateTime.Now - startTime).TotalSeconds < _duration)
        {
            string action = breatheIn ? "Breathe in..." : "Breathe out...";
            Console.WriteLine(action);

            int elapsed = (int)(DateTime.Now - startTime).TotalSeconds;
            int remaining = _duration - elapsed;
            int thisPause = Math.Min(breathSeconds, remaining);

            CountdownPause(thisPause);

            breatheIn = !breatheIn;
        }
    }
}

public class ReflectionActivity : MindfulnessActivity
{
    private readonly string[] _prompts = {
        "Think of a time when you stood up for someone else.",
        "Think of a time when you did something really difficult.",
        "Think of a time when you helped someone in need.",
        "Think of a time when you did something truly selfless."
    };

    private readonly string[] _questions = {
        "Why was this experience meaningful to you?",
        "Have you ever done anything like this before?",
        "How did you get started?",
        "How did you feel when it was complete?",
        "What made this time different than other times when you were not as successful?",
        "What is your favorite thing about this experience?",
        "What could you learn from this experience that applies to other situations?",
        "What did you learn about yourself through this experience?",
        "How can you keep this experience in mind in the future?"
    };

    public ReflectionActivity() : base("Reflection", "This activity will help you reflect on times in your life when you have shown strength and resilience. This will help you recognize the power you have and how you can use it in other aspects of your life.")
    {
    }

    public override void PerformActivity()
    {
        string prompt = _prompts[_random.Next(_prompts.Length)];
        Console.WriteLine("Consider the following prompt:");
        Console.WriteLine($"--- {prompt} ---");
        DotsPause(2);

        List<string> availableQuestions = new List<string>(_questions);
        DateTime startTime = DateTime.Now;
        int questionPauseSeconds = 10;

        while ((DateTime.Now - startTime).TotalSeconds < _duration)
        {
            if (availableQuestions.Count == 0)
            {
                availableQuestions = new List<string>(_questions);
            }

            int randomIndex = _random.Next(availableQuestions.Count);
            string question = availableQuestions[randomIndex];
            availableQuestions.RemoveAt(randomIndex);

            Console.WriteLine(question);

            int elapsed = (int)(DateTime.Now - startTime).TotalSeconds;
            int remaining = _duration - elapsed;
            if (remaining > 0)
            {
                int thisPause = Math.Min(questionPauseSeconds, remaining);
                SpinnerPause(thisPause);
            }
            else
            {
                break;
            }
        }
    }
}

public class ListingActivity : MindfulnessActivity
{
    private readonly string[] _prompts = {
        "Who are people that you appreciate?",
        "What are personal strengths of yours?",
        "Who are people that you have helped this week?",
        "When have you felt the Holy Ghost this month?",
        "Who are some of your personal heroes?"
    };

    public ListingActivity() : base("Listing", "This activity will help you reflect on the good things in your life by having you list as many things as you can in a certain area.")
    {
    }

    public override void PerformActivity()
    {
        string prompt = _prompts[_random.Next(_prompts.Length)];
        Console.WriteLine($"--- {prompt} ---");
        Console.WriteLine("Get ready to start listing...");
        CountdownPause(3);

        DateTime startTime = DateTime.Now;
        Console.WriteLine($"You have {_duration} seconds. Start listing now! (Empty line to finish early)");
        int count = 0;
        string entry;
        do
        {
            Console.Write(">> ");
            entry = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(entry))
            {
                count++;
            }
        } while (!string.IsNullOrWhiteSpace(entry) && (DateTime.Now - startTime).TotalSeconds < _duration);

        Console.WriteLine($"\nYou listed {count} items!");
        DotsPause(2);
    }
}

class Program
{
    private static int _breathingCount = 0;
    private static int _breathingTotalTime = 0;
    private static int _reflectionCount = 0;
    private static int _reflectionTotalTime = 0;
    private static int _listingCount = 0;
    private static int _listingTotalTime = 0;

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Mindfulness Program!");
        Console.WriteLine("This program helps you engage in mindfulness activities to reduce stress and reflect.");

        bool quit = false;
        while (!quit)
        {
            Console.WriteLine("\nMenu Options:");
            Console.WriteLine("  1 - Start Breathing Activity");
            Console.WriteLine("  2 - Start Reflection Activity");
            Console.WriteLine("  3 - Start Listing Activity");
            Console.WriteLine("  4 - View Statistics");
            Console.WriteLine("  5 - Quit");
            Console.Write("Select a choice: ");
            string choiceStr = Console.ReadLine();
            if (int.TryParse(choiceStr, out int choice))
            {
                switch (choice)
                {
                    case 1:
                        var breathing = new BreathingActivity();
                        breathing.StartActivity();
                        breathing.PerformActivity();
                        breathing.EndActivity();
                        _breathingCount++;
                        _breathingTotalTime += breathing.Duration;
                        Console.WriteLine("Press Enter to continue to menu...");
                        Console.ReadLine();
                        break;
                    case 2:
                        var reflection = new ReflectionActivity();
                        reflection.StartActivity();
                        reflection.PerformActivity();
                        reflection.EndActivity();
                        _reflectionCount++;
                        _reflectionTotalTime += reflection.Duration;
                        Console.WriteLine("Press Enter to continue to menu...");
                        Console.ReadLine();
                        break;
                    case 3:
                        var listing = new ListingActivity();
                        listing.StartActivity();
                        listing.PerformActivity();
                        listing.EndActivity();
                        _listingCount++;
                        _listingTotalTime += listing.Duration;
                        Console.WriteLine("Press Enter to continue to menu...");
                        Console.ReadLine();
                        break;
                    case 4:
                        Console.WriteLine("\n--- Statistics ---");
                        Console.WriteLine($"Breathing Activity: {_breathingCount} sessions, {_breathingTotalTime} seconds total");
                        Console.WriteLine($"Reflection Activity: {_reflectionCount} sessions, {_reflectionTotalTime} seconds total");
                        Console.WriteLine($"Listing Activity: {_listingCount} sessions, {_listingTotalTime} seconds total");
                        int totalSessions = _breathingCount + _reflectionCount + _listingCount;
                        int totalTime = _breathingTotalTime + _reflectionTotalTime + _listingTotalTime;
                        Console.WriteLine($"Total: {totalSessions} sessions, {totalTime} seconds");
                        Console.WriteLine("Press Enter to continue to menu...");
                        Console.ReadLine();
                        break;
                    case 5:
                        quit = true;
                        Console.WriteLine("\nThank you for using the Mindfulness Program! Take care.");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please select 1-5.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number 1-5.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }
    }
}
