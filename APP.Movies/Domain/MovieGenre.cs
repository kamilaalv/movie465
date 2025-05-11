namespace APP.Movies.Domain
{
public class MovieGenre : CORE.APP.Domain.Entity
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int GenreId { get; set; }

    // Navigation properties
    public Movie Movie { get; set; }
    public Genre Genre { get; set; }
}
}