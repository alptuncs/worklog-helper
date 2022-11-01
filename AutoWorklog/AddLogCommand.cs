using CommandLine;
using Gazel.Client.CommandLine;
using Gazel.Logging;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks.Sources;
using System.Transactions;

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
                log.Info("Without a file you can't add log entries...");

                return;
            }
            else
            {
                List<LogEntry> logEntryList = new List<LogEntry>();

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

                Console.WriteLine("Enter The Date");

                var dailyLog = new DailyLog(logEntryList, Console.ReadLine());
                List<DailyLog> dailyLogList = new List<DailyLog>();
                dailyLogList.Add(dailyLog);

                var prList = new List<string>();

                Console.WriteLine("Enter the pr for this work");
                prList.Add(Console.ReadLine());

                Console.WriteLine("Enter the name of Work");
                WorkLogTask serializeTest = new WorkLogTask(prList, dailyLogList, Console.ReadLine());

                var Tasks = new List<WorkLogTask>();
                Tasks.Add(serializeTest);

                var Root = new Root(Tasks);

                string jsonOut = JsonConvert.SerializeObject(Root, Formatting.Indented);

                File.WriteAllText(Path + @"\" + Args.Customer + ".workreport", jsonOut);
                log.Info("Added new file");
            }
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

            var selected = options[options.Count - (originalpos - currentpos)];

            var logEntryList = new List<LogEntry>();

            if (selected == options.Last())
            {
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

                Console.WriteLine("Enter The Date");

                var dailyLog = new DailyLog(logEntryList, Console.ReadLine());
                List<DailyLog> dailyLogList = new List<DailyLog>();
                dailyLogList.Add(dailyLog);

                var prList = new List<string>();

                Console.WriteLine("Enter the pr for this work");
                prList.Add(Console.ReadLine());

                Console.WriteLine("Enter the name of Work");
                WorkLogTask serializeTest = new WorkLogTask(prList, dailyLogList, Console.ReadLine());

                json.Tasks.Add(serializeTest);
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

                foreach (var option in dateoptions)
                {
                    Console.WriteLine(option);
                }

                var secondOriginalpos = Console.CursorTop();
                var secondCurrentpos = secondOriginalpos;
                var secondCount = 0;
                var c = Console.ReadKey();

                while (c.Key != ConsoleKey.Enter)
                {
                    if (secondCount != 0)
                    {
                        Console.SetCursorPosition(0, secondCurrentpos);
                        Console.WriteLine(dateoptions[dateoptions.Count - (secondOriginalpos - secondCurrentpos)]);
                    }

                    if (c.Key == ConsoleKey.UpArrow)
                    {
                        secondCurrentpos--;
                    }

                    if (c.Key == ConsoleKey.DownArrow)
                    {
                        secondCurrentpos++;
                    }

                    secondCurrentpos = Math.Min(secondOriginalpos - 1, Math.Max(secondOriginalpos - dateoptions.Count, secondCurrentpos));

                    Console.SetCursorPosition(0, secondCurrentpos);
                    SetBackgroundColor();
                    Console.WriteLine(dateoptions[dateoptions.Count - (secondOriginalpos - secondCurrentpos)]);
                    Console.ResetColor();

                    secondCount++;

                    Console.SetCursorPosition(0, secondOriginalpos);
                    c = Console.ReadKey();
                }

                var selectedDate = dateoptions[dateoptions.Count - (secondOriginalpos - secondCurrentpos)];

                if (selectedDate == dateoptions.Last())
                {
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
            log.Info("Done");
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
