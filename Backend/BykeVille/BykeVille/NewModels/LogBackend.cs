namespace BykeVille.NewModels
{
    public class LogBackend
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string Exception { get; set; }

        public LogBackend()
        {

        }

        public LogBackend(int id, DateOnly date, TimeOnly time, string className, string methodName, string exception)
        {
            Id = id;
            Date = date;
            Time = time;
            ClassName = className;
            MethodName = methodName;
            Exception = exception;
        }
    }
}
