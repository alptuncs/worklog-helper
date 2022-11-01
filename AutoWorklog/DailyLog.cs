using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AutoWorklog;

public class DailyLog
{
    public dynamic LogEntries { get; set; }
    public DailyLog(List<LogEntry> logEntries)
    {
        LogEntries = new System.Dynamic.ExpandoObject();
        LogEntries.logs = (logEntries);
    }
}
