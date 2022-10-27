using CommandLine;
using Gazel.Client.CommandLine;
using Gazel.Logging;
using Newtonsoft.Json;

namespace AutoWorklog;

public class AddWorkItemCommand : CommandBase<AddWorkItemOptions>
{

}

[Verb("add")]
public class AddWorkItemOptions : IOptions
{

    public AddWorkItemOptions()
    {
    
    }
}
