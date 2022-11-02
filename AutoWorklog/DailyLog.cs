namespace AutoWorklog;

public class DailyLog
{
    public string Date { get; set; }
    public List<LogEntry> LogEntries { get; set; }
    public DailyLog(List<LogEntry> logEntries, string date)
    {
        LogEntries = (logEntries);
        Date = date;
    }
}
