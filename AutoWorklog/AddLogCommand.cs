using CommandLine;
using Gazel.Client.CommandLine;
using Gazel.Logging;
using Newtonsoft.Json;
using System;

namespace AutoWorklog;

public class AddLogCommand : CommandBase<AddLogOptions>
{
    private readonly ILogger log;

    private IConsole Console { get; }
    private string Path { get; set; }

    public AddLogCommand(ILogManager logger, SystemConsole console)
    {
        log = logger.Get<AddLogCommand>();
        Console = console;
        Path = Environment.CurrentDirectory;
    }

    public override async Task Execute()
    {
        if (Args.Customer == null) throw new Exception("Customer can not be null");

        if (Args.Path != null) Path = Args.Path;

        if (!File.Exists(Path + Args.Customer + ".workreport.json"))
        {
            log.Info("Customer workreport file could not be found, create one ?");

            var options = new List<string>();
            options.Add("y");
            options.Add("n");

            foreach (var option in options)
            {
                Console.WriteLine(option);
            }

            var originalpos = Console.CursorTop();
            var currentpos = originalpos;
            var k = Console.ReadKey();

            while (k.Key != ConsoleKey.Enter)
            {
                if (k.Key == ConsoleKey.UpArrow)
                {
                    currentpos--;
                }

                if (k.Key == ConsoleKey.DownArrow)
                {
                    currentpos++;
                }

                currentpos = Math.Min(originalpos - 1, Math.Max(originalpos - options.Count, currentpos));

                MoveCursor(originalpos, currentpos, options);

                Console.SetCursorPosition(0, originalpos);
                k = Console.ReadKey();
            }

            if (currentpos == originalpos - 2)
            {
                File.WriteAllText(Path + Args.Customer + ".workreport.json", "{}");
            }
            else
            {
                log.Info("Without a file you can't add log entries...");

                return;
            }
        }

        string text = await File.ReadAllTextAsync(Path + Args.Customer + ".workreport.json");

        dynamic MyDynamic = new System.Dynamic.ExpandoObject();

        var json = JsonConvert.DeserializeObject<WorkLogTask>(text);

        List<LogEntry> logEntryList = new List<LogEntry>();

        log.Info("Deneme");

        if (Args.Start != null)
        {
            logEntryList.Add(new(Args.Start, Args.End, Args.Task));
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
                    logEntryList.Add(logEntry);
                }
            }
        }

        var dailyLog = new DailyLog(logEntryList);
        List<DailyLog> dailyLogList = new List<DailyLog>();
        dailyLogList.Add(dailyLog);
        var prList = new List<string>();
        prList.Add("Github");
        WorkLogTask serializeTest = new WorkLogTask(prList, dailyLogList);
        MyDynamic.Worklog = serializeTest;



        string jsonOut = JsonConvert.SerializeObject(MyDynamic, Formatting.Indented);

        File.WriteAllText(Path + "test.workreport.json", jsonOut);

        Console.WriteLine(jsonOut);
    }
    public void SetBackgroundColor()
    {
        Console.SetForeGroundColor(ConsoleColor.Black);
        Console.SetBackgroundColor(ConsoleColor.White);
    }

    public void MoveCursor(int originalpos, int currentpos, List<string> options)
    {
        Console.SetCursorPosition(0, currentpos);

        HighlightCurrentPos(originalpos, currentpos, options);
    }

    public void HighlightCurrentPos(int originalpos, int currentpos, List<string> options)
    {
        SetBackgroundColor();
        Console.WriteLine(options[options.Count - (originalpos - currentpos)]);

        Console.ResetColor();

        Console.SetCursorPosition(0, (int)(currentpos + Math.Pow(-1, originalpos - currentpos)));
        Console.WriteLine(options[(originalpos - currentpos + 1) % 2]);

        Console.SetCursorPosition(0, currentpos);
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
    [Option('p', "path")]
    public string Path { get; }
    [Option('c', "customer")]
    public string Customer { get; }

    public AddLogOptions(string start, string end, string task, string path, string customer)
    {
        Start = start;
        End = end;
        Task = task;
        Path = path;
        Customer = customer;
    }
}
