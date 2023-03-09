using Microsoft.Extensions.Options;

public static class ConfigurationHelper<T> where T : class
{
    public static IOptions<T> _options { get; set; }
    public static void Initialize(IOptions<T> options)
    {
        _options = options;
    }
}