namespace TEMA1_II.DataModels
{
    public class MovieItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int UserId { get; set; }
        public UserApp User { get; set; }
    }
}