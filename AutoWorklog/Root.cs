namespace AutoWorklog;

public class Root
{
    public List<WorkLogTask> Tasks { get; set; }

    public Root(List<WorkLogTask> tasks)
    {
        Tasks = tasks;
    }
}
