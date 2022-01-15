using System;
using System.IO;
using Microsoft.Extensions.Configuration;

public static class TestResources
{
    private static readonly Lazy<IConfiguration> _configuration = new(BuildConfiguration);

    public static IConfiguration Configuration => _configuration.Value;

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables()
            .Build();
    }
}