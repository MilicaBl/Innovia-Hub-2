using System.Net;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly string _response;
    private readonly HttpStatusCode _statusCode;

    public FakeHttpMessageHandler(string response, HttpStatusCode statusCode)
    {
        _response = response;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var message = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_response)
        };
        return Task.FromResult(message);
    }
}