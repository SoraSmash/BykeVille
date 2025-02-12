namespace BykeVille.NewModels
{
    public class LogFrontend
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Exception { get; set; }

        public LogFrontend()
        {
        }

        public LogFrontend(int id, DateOnly date, TimeOnly time, string exception)
        {
            Id = id;
            Date = date;
            Time = time;
            Exception = exception;
        }
    }
}
