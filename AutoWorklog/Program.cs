using Gazel.Client;
using Gazel.Configuration.Features;
using Gazel.IoC;
using Gazel.Logging;

await Run.App<Global>(with => with.TerminalMode = true);

public class Global : CommandLineApplication
{
    protected override CommandLineConfiguration CommandLine(CommandLineConfigurer configure) =>
        configure.CommandLineParser(AssemblyType.All);

    protected override LoggingFeature Logging(LoggingConfigurer configure) => configure
        .Level("Gazel", LogLevel.Off)
        .Level("Gazel.Cli", LogLevel.Debug)
        .Log4Net(LogLevel.Info, c => c
                .DefaultFileAppenders()
                .DefaultConsoleAppenders(),
            handledExceptionsAreErrors: true
        )
    ;
}