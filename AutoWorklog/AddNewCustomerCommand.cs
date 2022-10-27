﻿using CommandLine;
using Gazel.Client.CommandLine;
using Gazel.Logging;

namespace AutoWorklog;

public class AddNewCustomerCommand : CommandBase<AddNewCustomerOptions>
{
    private readonly ILogger log;
    private readonly DateTime dateTime;

    public AddNewCustomerCommand(ILogManager logger)
    {
        log = logger.Get<AddLogCommand>();
        dateTime = DateTime.Today;
    }

    public override async Task Execute()
    {
        if (!Directory.Exists(@"C:\Users\90533\source\repos\AutoWorklog\" + dateTime.Year + "-" + dateTime.Month))
        {
            log.Info("Folder for this month's work logs does not exists, do you want to create it ?");

            var options = new List<string>();
            options.Add("y");
            options.Add("n");

            foreach (var option in options)
            {
                Console.WriteLine(option);
            }

            var originalpos = Console.CursorTop;
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
                Directory.CreateDirectory(@"C:\Users\90533\source\repos\AutoWorklog\" + dateTime.Year + "-" + dateTime.Month);
            }
            else
            {
                log.Info("Without folder, file create will give error, stopping process...");

                return;
            }
        }

        if (Args.Customer == null)
        {
            log.Info("Customer is null, enter the customer as addCustomer -c <customer name> format.");

            return;
        }

        if (File.Exists(@"C:\Users\90533\source\repos\AutoWorklog\" + dateTime.Year + "-" + dateTime.Month + @"\" + Args.Customer + ".workreport.json"))
        {
            log.Info("File allready exists, stopping the addCustomer process.");

            return;
        }

        log.Info("Creating a new json file for customer " + Args.Customer + "...");

        string jsonOut = "{ }";

        await File.WriteAllTextAsync(@"C:\Users\90533\source\repos\AutoWorklog\" + dateTime.Year + "-" + dateTime.Month + @"\" + Args.Customer + ".workreport.json", jsonOut);

        if (File.Exists(@"C:\Users\90533\source\repos\AutoWorklog\" + dateTime.Year + "-" + dateTime.Month + @"\" + Args.Customer + ".workreport.json")) log.Info("File created succesfully.");
    }

    public void SetBackgroundColor()
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.White;
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

[Verb("addCustomer")]
public class AddNewCustomerOptions : IOptions
{
    [Option('c', "customer")]
    public string Customer { get; }

    public AddNewCustomerOptions(string customer)
    {
        Customer = customer;
    }
}
