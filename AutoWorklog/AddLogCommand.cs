using CommandLine;
using Gazel.Client.CommandLine;
using Gazel.Logging;
using Newtonsoft.Json;

namespace AutoWorklog;

public class AddLogCommand : CommandBase<AddLogOptions>
{
    private readonly ILogger log;

    public AddLogCommand(ILogManager logger)
    {
        log = logger.Get<AddLogCommand>();
    }
    public override async Task Execute()
    {
        string text = await File.ReadAllTextAsync(@"C:\Users\90533\Documents\GitHub\file-location\test.workreport.json");

        var json = JsonConvert.DeserializeObject<WorkLogTask>(text);

        List<LogEntry> logEntryList = new List<LogEntry>();

        log.Info("Deneme");

        if (Args.Start != null)
        {
            json.log.FirstOrDefault().logEntries.Add(new(Args.Start, Args.End, Args.Task));
        }
        else
        {



            while (true)
            {
                var input = Console.ReadLine();

                if (input == "done") break;

                if (input != null)
                {
                    var logEntry = new LogEntry(input.Substring(0, 5), input.Substring(6, 5), input.Substring(11));
                    json.log.FirstOrDefault().logEntries.Add(logEntry);
                }
            }
        }

        var dailyLog = new DailyLog(logEntryList);
        List<DailyLog> dailyLogList = new List<DailyLog>();
        dailyLogList.Add(dailyLog);
        var prList = new List<string>();
        prList.Add("Github");
        WorkLogTask serializeTest = new WorkLogTask(prList, dailyLogList);



        string jsonOut = JsonConvert.SerializeObject(serializeTest, Formatting.Indented);

        File.WriteAllText(@"C:\Users\90533\source\repos\AutoWorklog\Test.json", jsonOut);

        Console.WriteLine(json);



    }
}

[Verb("add")]
public class AddLogOptions : IOptions
{
    [Value(0, MetaName = "start")]
    public string Start { get; }
    [Value(1, MetaName = "end")]
    public string End { get; }
    [Option('t', "task")]
    public string Task { get; }

    public AddLogOptions(string start, string end, string task)
    {
        Start = start;
        End = end;
        Task = task;
    }
}
