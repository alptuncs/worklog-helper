using CommandLine;
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

            Console.WriteLine("y");
            Console.WriteLine("n");

            var input = "";

            var originalpos = Console.CursorTop;
            var currentpost = originalpos;

            var k = Console.ReadKey();

            while (k.Key != ConsoleKey.Enter)
            {

                if (k.Key == ConsoleKey.UpArrow)
                {
                    currentpost--;

                    Console.SetCursorPosition(0, currentpost);

                    if (currentpost == originalpos - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("n");
                    }

                    if (currentpost == originalpos - 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("y");

                        Console.ResetColor();

                        Console.SetCursorPosition(0, originalpos - 1);
                        Console.WriteLine("n");

                        Console.SetCursorPosition(0, originalpos - 2);
                    }

                }

                if (k.Key == ConsoleKey.DownArrow)
                {
                    currentpost++;

                    Console.SetCursorPosition(0, currentpost);

                    if (currentpost == originalpos - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("n");

                        Console.ResetColor();

                        Console.SetCursorPosition(0, originalpos - 2);
                        Console.WriteLine("y");

                        Console.SetCursorPosition(0, originalpos - 1);
                    }

                    if (currentpost == originalpos - 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("y");

                        Console.ResetColor();

                        Console.SetCursorPosition(0, originalpos - 1);
                        Console.WriteLine("n");

                        Console.SetCursorPosition(0, originalpos - 2);
                    }
                }

                Console.SetCursorPosition(0, originalpos);
                k = Console.ReadKey();
            }

            if (currentpost == originalpos - 2) input = "y";
            if (currentpost == originalpos - 1) input = "n";

            if (input == "y")
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
