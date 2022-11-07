using CommandLine;
using Gazel.Client.CommandLine;
using Gazel.Logging;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Management.Automation;

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

        if (!File.Exists(Path + @"\" + Args.Customer + ".workreport"))
        {
            var reply = YesNoOption();

            if (reply == "n")
            {
                log.Info("Without a file you can't add log entries...");
                return;
            }

            var work = CreateNewWork();

            var Tasks = new List<WorkLogTask>();
            Tasks.Add(work);

            var Root = new Root(Tasks);

            var jsonOut = JsonConvert.SerializeObject(Root, Formatting.Indented);

            File.WriteAllText(Path + @"\" + Args.Customer + ".workreport", jsonOut);
            log.Info("Added new file");
        }

        else
        {
            string text = await File.ReadAllTextAsync(Path + @"\" + Args.Customer + ".workreport");

            var json = JsonConvert.DeserializeObject<Root>(text);

            Console.WriteLine("Choose work for adding logs or create a new one");

            var options = new List<string>();
            foreach (var work in json.Tasks)
            {
                options.Add(work.name);
            }
            options.Add("Add New");

            var selected = ChooseFromOptions(options);

            var logEntryList = new List<LogEntry>();

            if (selected == options.Last())
            {
                json.Tasks.Add(CreateNewWork());
            }
            else
            {
                Console.WriteLine("Pick a Date or add new");
                var dateoptions = new List<string>();
                foreach (var work in json.Tasks)
                {
                    if (work.name == selected)
                    {
                        foreach (var date in work.log)
                        {
                            dateoptions.Add(date.Date);
                        }
                    }
                }
                dateoptions.Add("Add New");

                var selectedDate = ChooseFromOptions(dateoptions);

                if (selectedDate == "Add New")
                {
                    logEntryList = GetLogEntries(null);

                    Console.WriteLine("Enter the date");

                    foreach (var work in json.Tasks)
                    {
                        if (work.name == selected)
                        {
                            work.log.Add(new DailyLog(logEntryList, Console.ReadLine()));
                        }
                    }
                }
                else
                {
                    logEntryList = GetLogEntries(null);
                }

                foreach (var work in json.Tasks)
                {
                    if (work.name == selected)
                    {
                        foreach (var logDates in work.log)
                        {
                            if (logDates.Date == selectedDate)
                            {
                                logDates.LogEntries.AddRange(logEntryList);
                            }
                        }
                    }
                }
            }

            string jsonOut = JsonConvert.SerializeObject(json, Formatting.Indented);

            File.WriteAllText(Path + @"\" + Args.Customer + ".workreport", jsonOut);
            log.Info("Done.");

        }

        string fileContent = await File.ReadAllTextAsync(Path + @"\" + Args.Customer + ".workreport");

        var formattedJson = FormatWorklog(fileContent);

        File.WriteAllText(Path + @"\" + Args.Customer + ".workreport.json", formattedJson);

        log.Info("added formatted file");

        Commit();
    }

    private string ChooseFromOptions(List<string> options)
    {
        foreach (var option in options)
        {
            Console.WriteLine(option);
        }

        var originalpos = Console.CursorTop();
        var currentpos = originalpos;
        var count = 0;
        var k = Console.ReadKey();

        while (k.Key != ConsoleKey.Enter)
        {
            if (count != 0)
            {
                Console.SetCursorPosition(0, currentpos);
                Console.WriteLine(options[options.Count - (originalpos - currentpos)]);
            }

            if (k.Key == ConsoleKey.UpArrow)
            {
                currentpos--;
            }

            if (k.Key == ConsoleKey.DownArrow)
            {
                currentpos++;
            }

            currentpos = Math.Min(originalpos - 1, Math.Max(originalpos - options.Count, currentpos));

            Console.SetCursorPosition(0, currentpos);
            SetBackgroundColor();
            Console.WriteLine(options[options.Count - (originalpos - currentpos)]);
            Console.ResetColor();

            count++;

            Console.SetCursorPosition(0, originalpos);
            k = Console.ReadKey();
        }

        return options[options.Count - (originalpos - currentpos)];
    }

    public string YesNoOption()
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

        if (currentpos != originalpos - 2)
        {
            return "n";
        }
        else
        {
            return "y";
        }

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

    public List<LogEntry> GetLogEntries(List<LogEntry>? logEntryList)
    {
        if (logEntryList == null) logEntryList = new List<LogEntry>();

        Console.WriteLine("Enter log entries");

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

        return logEntryList;
    }

    public List<DailyLog> GetDate(List<LogEntry> logEntryList)
    {
        Console.WriteLine("Enter The Date");

        var dailyLog = new DailyLog(logEntryList, Console.ReadLine());
        List<DailyLog> dailyLogList = new List<DailyLog>();
        dailyLogList.Add(dailyLog);

        return dailyLogList;
    }

    public List<string> GetPr()
    {
        var prList = new List<string>();


        Console.WriteLine("Enter the pr for this work");

        while (true)
        {

            var input = Console.ReadLine();

            if (input == "done") break;

            if (input != null)
            {
                prList.Add(input);
            }
        }

        return prList;
    }

    public string GetWorkName()
    {

        Console.WriteLine("Enter the name of Work");
        return Console.ReadLine();
    }

    public string FormatWorklog(string fileContent)
    {
        var readFile = JsonConvert.DeserializeObject<Root>(fileContent);
        var root = new ExpandoObject();

        foreach (var work in readFile.Tasks)
        {
            var formatted = new ExpandoObject();
            var inner = new ExpandoObject();

            formatted.TryAdd("pr", work.pr);

            formatted.TryAdd("log", inner);

            foreach (var dailyLog in work.log)
            {
                var log = new List<LogEntry>();
                foreach (var logEntry in dailyLog.LogEntries)
                {
                    log.Add(logEntry);
                }
                inner.TryAdd(dailyLog.Date, log);
            }
            root.TryAdd(work.name, formatted);
        }

        return JsonConvert.SerializeObject(root, Formatting.Indented);
    }

    public WorkLogTask CreateNewWork()
    {
        var logEntryList = GetLogEntries(null);

        var dailyLogList = GetDate(logEntryList);

        var prList = GetPr();

        var workName = GetWorkName();

        return new WorkLogTask(prList, dailyLogList, workName);
    }

    public void Commit()
    {
        string directory = Path;

        PowerShell powershell = PowerShell.Create();

        powershell.AddScript($"cd {directory}");

        powershell.AddScript(@"git init");
        powershell.AddScript(@"git add .");
        powershell.AddScript(@"git commit -m 'worklog update'");
        powershell.AddScript(@"git push origin main");

        Collection<PSObject> results = powershell.Invoke();

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
