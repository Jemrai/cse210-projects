// Creativity: I added a gamification element with a leveling system.

using System;
using System.Collections.Generic;
using System.IO;

namespace EternalQuest
{

    public abstract class Goal
    {
        protected string _description;
        protected int _points;

        public Goal(string description, int points)
        {
            _description = description;
            _points = points;
        }

        public abstract int RecordEvent();
        public abstract bool IsCompleted();
        public abstract string GetSaveString();

        public virtual string GetCompletionMark() => IsCompleted() ? "[X]" : "[ ]";
        public virtual string GetProgressString() => "";
        public string Description => _description;
    }

  
    public class SimpleGoal : Goal
    {
        private bool _completed;

        public SimpleGoal(string description, int points, bool initialCompleted = false) : base(description, points)
        {
            _completed = initialCompleted;
        }

        public override int RecordEvent()
        {
            if (_completed) return 0;
            _completed = true;
            return _points;
        }

        public override bool IsCompleted() => _completed;

        public override string GetSaveString() => $"Simple|{_description}|{_points}|{_completed}";
    }


    public class EternalGoal : Goal
    {
        public EternalGoal(string description, int points) : base(description, points)
        {
        }

        public override int RecordEvent() => _points;

        public override bool IsCompleted() => false;

        public override string GetSaveString() => $"Eternal|{_description}|{_points}";
    }

  
    public class ChecklistGoal : Goal
    {
        private int _target;
        private int _bonus;
        private int _completedCount;

        public ChecklistGoal(string description, int points, int target, int bonus, int initialCompleted = 0) : base(description, points)
        {
            _target = target;
            _bonus = bonus;
            _completedCount = initialCompleted;
        }

        public override int RecordEvent()
        {
            if (_completedCount >= _target) return 0;
            _completedCount++;
            int pointsGained = _points;
            if (_completedCount == _target)
            {
                pointsGained += _bonus;
            }
            return pointsGained;
        }

        public override bool IsCompleted() => _completedCount >= _target;

        public override string GetProgressString() => $"Completed {_completedCount}/{_target}";

        public override string GetSaveString() => $"Checklist|{_description}|{_points}|{_target}|{_bonus}|{_completedCount}";
    }

    public class EternalQuest
    {
        private List<Goal> _goals = new List<Goal>();
        private int _score = 0;
        private int _level = 0;
        private const int POINTS_PER_LEVEL = 1000;

        public void AddGoal(Goal goal)
        {
            _goals.Add(goal);
        }

        public int GetGoalCount() => _goals.Count;

        public int RecordEvent(int goalIndex)
        {
            if (goalIndex < 0 || goalIndex >= _goals.Count) return 0;
            int points = _goals[goalIndex].RecordEvent();
            if (points > 0)
            {
                _score += points;
                int oldLevel = _level;
                UpdateLevel();
                if (_level > oldLevel)
                {
                    Console.WriteLine($"\nCongratulations! You've reached level {_level}!");
                }
            }
            return points;
        }

        public void DisplayGoals()
        {
            Console.WriteLine("\n--- Goals ---");
            for (int i = 0; i < _goals.Count; i++)
            {
                string mark = _goals[i].GetCompletionMark();
                string progress = _goals[i].GetProgressString();
                string display = $"{i + 1}. {mark} {progress} - {_goals[i].Description}";
                Console.WriteLine(display);
            }
            Console.WriteLine("---");
        }

        public void DisplayScore()
        {
            UpdateLevel();
            Console.WriteLine("\n--- Score ---");
            Console.WriteLine($"You have {_score} points.");
            Console.WriteLine($"Level: {_level}");
            Console.WriteLine("---");
        }

        private void UpdateLevel()
        {
            int newLevel = _score / POINTS_PER_LEVEL;
            if (newLevel > _level)
            {
                _level = newLevel;
            }
        }

        public void Save(string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine(_score);
                writer.WriteLine(_goals.Count);
                foreach (var goal in _goals)
                {
                    writer.WriteLine(goal.GetSaveString());
                }
            }
        }

        public void Load(string filename)
        {
            if (!File.Exists(filename)) return;
            try
            {
                string[] lines = File.ReadAllLines(filename);
                if (lines.Length < 2) return;

                int lineIdx = 0;
                _score = int.Parse(lines[lineIdx++]);
                int numGoals = int.Parse(lines[lineIdx++]);
                _goals.Clear();

                for (int i = 0; i < numGoals; i++)
                {
                    if (lineIdx >= lines.Length) break;
                    string[] parts = lines[lineIdx++].Split('|');
                    if (parts.Length < 3) continue;

                    string type = parts[0];
                    string desc = parts[1];
                    int points = int.Parse(parts[2]);
                    Goal g = null;

                    switch (type)
                    {
                        case "Simple":
                            if (parts.Length >= 4)
                            {
                                bool completed = bool.Parse(parts[3]);
                                g = new SimpleGoal(desc, points, completed);
                            }
                            break;
                        case "Eternal":
                            g = new EternalGoal(desc, points);
                            break;
                        case "Checklist":
                            if (parts.Length >= 6)
                            {
                                int target = int.Parse(parts[3]);
                                int bonus = int.Parse(parts[4]);
                                int count = int.Parse(parts[5]);
                                g = new ChecklistGoal(desc, points, target, bonus, count);
                            }
                            break;
                    }

                    if (g != null) _goals.Add(g);
                }

                UpdateLevel();
            }
            catch
            {
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            EternalQuest quest = new EternalQuest();
            bool quit = false;
            Console.WriteLine("Welcome to Eternal Quest!");

            while (!quit)
            {
                Console.WriteLine("\nMenu Options:");
                Console.WriteLine("1. Create New Goal");
                Console.WriteLine("2. Record Event");
                Console.WriteLine("3. Display Goals");
                Console.WriteLine("4. Display Score");
                Console.WriteLine("5. Save Progress");
                Console.WriteLine("6. Load Progress");
                Console.WriteLine("7. Quit");

                Console.Write("Select option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Goal type (simple/eternal/checklist): ");
                        string type = Console.ReadLine().ToLower();
                        Console.Write("Description: ");
                        string desc = Console.ReadLine();
                        Console.Write("Points per completion: ");
                        int points = int.Parse(Console.ReadLine());
                        Goal newGoal = null;

                        if (type == "simple")
                        {
                            newGoal = new SimpleGoal(desc, points);
                        }
                        else if (type == "eternal")
                        {
                            newGoal = new EternalGoal(desc, points);
                        }
                        else if (type == "checklist")
                        {
                            Console.Write("Target completions: ");
                            int target = int.Parse(Console.ReadLine());
                            Console.Write("Bonus points: ");
                            int bonus = int.Parse(Console.ReadLine());
                            newGoal = new ChecklistGoal(desc, points, target, bonus);
                        }

                        if (newGoal != null)
                        {
                            quest.AddGoal(newGoal);
                            Console.WriteLine("Goal created!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid type.");
                        }
                        break;

                    case "2":
                        if (quest.GetGoalCount() == 0)
                        {
                            Console.WriteLine("No goals yet. Create some first!");
                            break;
                        }
                        quest.DisplayGoals();
                        Console.Write("Which goal (number)? ");
                        if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= quest.GetGoalCount())
                        {
                            int pts = quest.RecordEvent(idx - 1);
                            if (pts > 0)
                            {
                                Console.WriteLine($"You earned {pts} points!");
                            }
                            else
                            {
                                Console.WriteLine("No points awarded (goal already complete or invalid).");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid selection.");
                        }
                        break;

                    case "3":
                        if (quest.GetGoalCount() == 0)
                        {
                            Console.WriteLine("No goals yet.");
                        }
                        else
                        {
                            quest.DisplayGoals();
                        }
                        break;

                    case "4":
                        quest.DisplayScore();
                        break;

                    case "5":
                        quest.Save("eternal_quest.txt");
                        Console.WriteLine("Progress saved!");
                        break;

                    case "6":
                        quest.Load("eternal_quest.txt");
                        Console.WriteLine("Progress loaded!");
                        break;

                    case "7":
                        quit = true;
                        Console.WriteLine("Thanks for playing Eternal Quest!");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
