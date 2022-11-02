namespace AutoWorklog;

public class WorkLogTask
{
    public string name { get; set; }
    public List<string> pr { get; set; }
    public List<DailyLog> log { get; set; }

    public WorkLogTask(List<string> pr, List<DailyLog> log, string name)
    {
        this.pr = pr;
        this.log = log;
        this.name = name;
    }
}
