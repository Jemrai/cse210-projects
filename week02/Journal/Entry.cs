using System;

public class Entry
{
    private string _date;
    private string _prompt;
    private string _response;

    public Entry(string date, string prompt, string response)
    {
        _date = date;
        _prompt = prompt;
        _response = response;
    }

    public string Date
    {
        get { return _date; }
    }

    public string Prompt
    {
        get { return _prompt; }
    }

    public string Response
    {
        get { return _response; }
    }

    public override string ToString()
    {
        return $"Date: {_date}\nPrompt: {_prompt}\nResponse: {_response}\n";
    }

    public string Serialize()
    {
        return $"{_date}~|~{_prompt}~|~{_response}";
    }

    public static Entry Deserialize(string line)
    {
        string[] parts = line.Split(new string[] { "~|~" }, StringSplitOptions.None);
        if (parts.Length == 3)
        {
            return new Entry(parts[0], parts[1], parts[2]);
        }
        else
        {
            throw new FormatException("Invalid entry format.");
        }
    }
}
