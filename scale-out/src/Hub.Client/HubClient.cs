namespace Hub.Client;

public class HubClient
{
    private readonly HttpClient _client;

    public HubClient(HttpClient client)
    {
        _client = client;
    }

    public async Task GreetEveryone()
    {
        using var response = await _client.PostAsync("greet/all", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task GreetGroup(string groupName)
    {
        using var response = await _client.PostAsync($"greet/group/{groupName}", null);
        response.EnsureSuccessStatusCode();
    }
}