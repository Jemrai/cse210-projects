using System;
using System.Collections.Generic;

public class Comment
{
    public string Name { get; set; }
    public string Text { get; set; }

    public Comment(string name, string text)
    {
        Name = name;
        Text = text;
    }
}

public class Video
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Length { get; set; } 
    public List<Comment> Comments { get; set; } = new List<Comment>();

    public Video(string title, string author, int length)
    {
        Title = title;
        Author = author;
        Length = length;
    }

    public void AddComment(Comment comment)
    {
        Comments.Add(comment);
    }

    public int GetNumberOfComments()
    {
        return Comments.Count;
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Video> videos = new List<Video>();

        Video video1 = new Video("How to Bake Cookies", "Chef John", 300);
        video1.AddComment(new Comment("Alice", "Great recipe!"));
        video1.AddComment(new Comment("Bob", "I burned mine :("));
        video1.AddComment(new Comment("Charlie", "Thanks for the tips!"));
        videos.Add(video1);

        Video video2 = new Video("Gaming Tutorial", "GamerPro", 600);
        video2.AddComment(new Comment("Dave", "Helped a lot!"));
        video2.AddComment(new Comment("Eve", "Too fast paced"));
        video2.AddComment(new Comment("Frank", "Awesome!"));
        video2.AddComment(new Comment("Gina", "Will try this"));
        videos.Add(video2);

        Video video3 = new Video("Fitness Workout", "FitLife", 450);
        video3.AddComment(new Comment("Hank", "Good workout"));
        video3.AddComment(new Comment("Ivy", "Challenging"));
        video3.AddComment(new Comment("Jack", "Motivating"));
        videos.Add(video3);

        Video video4 = new Video("Tech Review", "TechGuru", 900);
        video4.AddComment(new Comment("Kyle", "Informative"));
        video4.AddComment(new Comment("Lila", "Boring"));
        video4.AddComment(new Comment("Mike", "Buy it!"));
        video4.AddComment(new Comment("Nora", "Detailed"));
        videos.Add(video4);

        foreach (Video video in videos)
        {
            Console.WriteLine($"Title: {video.Title}");
            Console.WriteLine($"Author: {video.Author}");
            Console.WriteLine($"Length: {video.Length} seconds");
            Console.WriteLine($"Number of comments: {video.GetNumberOfComments()}");
            Console.WriteLine("Comments:");
            foreach (Comment comment in video.Comments)
            {
                Console.WriteLine($"  - {comment.Name}: {comment.Text}");
            }
            Console.WriteLine(); 
        }
    }
}
