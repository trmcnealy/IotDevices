
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using RaspberryPiDevices;

namespace RPiDevices;

internal class Program
{

    private static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.Sources.Clear();

        IConfigurationBuilder configurationBuilder = builder.Configuration.AddJsonFile("RPiSettings.json", optional: false, reloadOnChange: true);


        builder.Services.Configure<RPiSettings>(RPiSettings.Personalize, builder.Configuration.GetSection("Features:Personalize"));

        builder.Services.Configure<Features>(    Features.WeatherStation,    builder.Configuration.GetSection("Features:WeatherStation"));


        using IHost host = builder.Build();





        //host.Run();
        //await host.RunAsync();

        await Task.Delay(0);
    }
}


//HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

//builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
//    {
//        ["SecretKey"] = "Dictionary MyKey Value",
//        ["TransientFaultHandlingOptions:Enabled"] = bool.TrueString,
//        ["TransientFaultHandlingOptions:AutoRetryDelay"] = "00:00:07",
//        ["Logging:LogLevel:Default"] = "Warning"
//    });