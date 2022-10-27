using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AutoWorklog;

public class DailyLog
{
    //[JsonProperty(PropertyName = "03")]
    public List<LogEntry> logEntries { get; set; }
    public DailyLog(List<LogEntry> logEntries)
    {
        this.logEntries = logEntries;
    }
}
