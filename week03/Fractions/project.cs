using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ScriptureMemorizer
{
    /// <summary>
    /// Represents a scripture reference, e.g. "John 3:16" or "Proverbs 3:5-6"
    /// </summary>
    public class Reference
    {
        public string Book { get; private set; }
        public int Chapter { get; private set; }
        public int StartVerse { get; private set; }
        public int? EndVerse { get; private set; } // nullable for single verse

        // Constructor for single verse
        public Reference(string book, int chapter, int verse)
        {
            Book = book;
            Chapter = chapter;
            StartVerse = verse;
            EndVerse = null;
        }

        // Constructor for verse range
        public Reference(string book, int chapter, int startVerse, int endVerse)
        {
            if (endVerse < startVerse)
                throw new ArgumentException("End verse must be greater or equal to start verse.");
            Book = book;
            Chapter = chapter;
            StartVerse = startVerse;
            EndVerse = endVerse;
        }

        public override string ToString()
        {
            if (EndVerse.HasValue)
                return $"{Book} {Chapter}:{StartVerse}-{EndVerse}";
            else
                return $"{Book} {Chapter}:{StartVerse}";
        }
    }

    /// <summary>
    /// Represents a single word in the scripture text.
    /// </summary>
    public class Word
    {
        private string _text;
        private bool _hidden;

        public Word(string text)
        {
            _text = text;
            _hidden = false;
        }

        public bool IsHidden => _hidden;

        public void Hide()
        {
            _hidden = true;
        }

        public override string ToString()
        {
            if (_hidden)
            {
                // Replace letters with underscores, keep punctuation intact
                // For simplicity, consider only letters replaced by underscores
                var sb = new StringBuilder();
                foreach (char c in _text)
                {
                    if (char.IsLetter(c))
                        sb.Append('_');
                    else
                        sb.Append(c);
                }
                return sb.ToString();
            }
            else
            {
                return _text;
            }
        }
    }

    /// <summary>
    /// Represents a scripture, including reference and text.
    /// </summary>
    public class Scripture
    {
        private Reference _reference;
        private List<Word> _words;

        private static readonly Random _random = new Random();

        /// <summary>
        /// Constructor for single verse scripture.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="text"></param>
        public Scripture(Reference reference, string text)
        {
            _reference = reference;
            _words = ParseTextIntoWords(text);
        }

        /// <summary>
        /// Parses the scripture text into Word objects, preserving punctuation.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private List<Word> ParseTextIntoWords(string text)
        {
            // Split on spaces, keep punctuation attached to words
            // We want to keep punctuation as part of the word so it can be displayed properly.
            // For example: "God," should be one word with comma.
            // We'll split on spaces only.
            var splitWords = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var words = new List<Word>();
            foreach (var w in splitWords)
            {
                words.Add(new Word(w));
            }
            return words;
        }

        /// <summary>
        /// Returns true if all words are hidden.
        /// </summary>
        public bool AllWordsHidden()
        {
            return _words.All(w => w.IsHidden);
        }

        /// <summary>
        /// Hides a few random words that are not yet hidden.
        /// </summary>
        /// <param name="count">Number of words to hide</param>
        public void HideRandomWords(int count)
        {
            var notHiddenWords = _words.Where(w => !w.IsHidden).ToList();
            if (notHiddenWords.Count == 0)
                return;

            int toHide = Math.Min(count, notHiddenWords.Count);

            // Select random words to hide without repeats
            var indices = new HashSet<int>();
            while (indices.Count < toHide)
            {
                int idx = _random.Next(notHiddenWords.Count);
                indices.Add(idx);
            }

            foreach (int idx in indices)
            {
                notHiddenWords[idx].Hide();
            }
        }

        /// <summary>
        /// Returns the scripture text with hidden words replaced by underscores.
        /// </summary>
        /// <returns></returns>
        public string GetDisplayText()
        {
            return string.Join(" ", _words.Select(w => w.ToString()));
        }

        public override string ToString()
        {
            return $"{_reference}\n{GetDisplayText()}";
        }
    }

    class Program
    {
        /// <summary>
        /// Clears the console screen.
        /// </summary>
        private static void ClearScreen()
        {
            Console.Clear();
        }

        static void Main(string[] args)
        {
            // Example scripture with multiple verses
            // You can change this to any scripture you want
            Reference reference = new Reference("Proverbs", 3, 5, 6);
            string scriptureText = "Trust in the Lord with all your heart and lean not on your own understanding; in all your ways submit to him, and he will make your paths straight.";

            Scripture scripture = new Scripture(reference, scriptureText);

            ClearScreen();
            Console.WriteLine(scripture);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Press Enter to hide words or type 'quit' to exit.");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "quit")
                    break;

                // Hide a few random words (e.g., 3 words per iteration)
                scripture.HideRandomWords(3);

                ClearScreen();
                Console.WriteLine(scripture);

                if (scripture.AllWordsHidden())
                {
                    // All words hidden, end program
                    Console.WriteLine();
                    Console.WriteLine("All words are hidden. Program will now exit.");
                    break;
                }
            }
        }
    }
}