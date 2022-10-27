namespace AutoWorklog;

public class WorkLogTask
{
    public List<string> pr { get; set; }
    public List<DailyLog> log { get; set; }

    public WorkLogTask(List<string> pr, List<DailyLog> log)
    {
        this.pr = pr;
        this.log = log;
    }
}
