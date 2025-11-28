public class MovieResult
{
    public string Message { get; set; }
    public List<Movie> Movies { get; set; }
}

public class Movie
{
    public string Title { get; set; }
    public int Year { get; set; }
    public string Rated { get; set; }
    public string Released { get; set; }
    public string Runtime { get; set; }
}