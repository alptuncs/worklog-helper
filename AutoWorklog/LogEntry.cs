namespace AutoWorklog;

public class LogEntry
{
    public string start { get; set; }
    public string end { get; set; }
    public string task { get; set; }

    public LogEntry(string start, string end, string task)
    {
        this.start = start;
        this.end = end;
        this.task = task;
    }
}
