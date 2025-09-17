using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ScriptureMemorizer
{
    // Clase que representa la referencia de la escritura (ej. "John 3:16" o "Proverbs 3:5-6")
    public class Reference
    {
        public string Book { get; private set; }
        public int Chapter { get; private set; }
        public int StartVerse { get; private set; }
        public int? EndVerse { get; private set; }

        // Constructor para versículo único
        public Reference(string book, int chapter, int verse)
        {
            Book = book;
            Chapter = chapter;
            StartVerse = verse;
            EndVerse = null;
        }

        // Constructor para rango de versículos
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

    // Clase que representa una palabra en la escritura
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
                // Reemplaza solo letras por guiones bajos, mantiene puntuación
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

    // Clase que representa la escritura completa
    public class Scripture
    {
        private Reference _reference;
        private List<Word> _words;
        private static readonly Random _random = new Random();

        public Scripture(Reference reference, string text)
        {
            _reference = reference;
            _words = ParseTextIntoWords(text);
        }

        private List<Word> ParseTextIntoWords(string text)
        {
            // Divide el texto en palabras separadas por espacios
            var splitWords = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var words = new List<Word>();
            foreach (var w in splitWords)
            {
                words.Add(new Word(w));
            }
            return words;
        }

        public bool AllWordsHidden()
        {
            return _words.All(w => w.IsHidden);
        }

        // Oculta un número dado de palabras aleatorias que NO estén ocultas aún (reto adicional)
        public void HideRandomWords(int count)
        {
            var notHiddenWords = _words.Where(w => !w.IsHidden).ToList();
            if (notHiddenWords.Count == 0)
                return;

            int toHide = Math.Min(count, notHiddenWords.Count);

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
        static void Main(string[] args)
        {
            // Puedes cambiar esta escritura o agregar más para extender el programa
            Reference reference = new Reference("Proverbs", 3, 5, 6);
            string scriptureText = "Trust in the Lord with all your heart and lean not on your own understanding; in all your ways submit to him, and he will make your paths straight.";

            Scripture scripture = new Scripture(reference, scriptureText);

            while (true)
            {
                Console.Clear();
                Console.WriteLine(scripture);
                Console.WriteLine();
                if (scripture.AllWordsHidden())
                {
                    Console.WriteLine("All words are hidden. Program will now exit.");
                    break;
                }
                Console.WriteLine("Press Enter to hide words or type 'quit' to exit.");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "quit")
                    break;

                scripture.HideRandomWords(3);
            }
        }
    }
}