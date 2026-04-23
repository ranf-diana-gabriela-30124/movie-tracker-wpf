namespace TEMA1_II.BusinessLogic
{
    public static class ValidationService
    {
        public static bool IsValidMovie(string title, string genre)
        {
            return !string.IsNullOrWhiteSpace(title)
                && !string.IsNullOrWhiteSpace(genre);
        }

        public static bool IsValidWatched(int stars, string platform)
        {
            return stars > 0 && !string.IsNullOrWhiteSpace(platform);
        }
    }
}