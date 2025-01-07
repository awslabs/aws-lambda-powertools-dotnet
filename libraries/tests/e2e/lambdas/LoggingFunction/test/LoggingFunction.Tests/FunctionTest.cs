using Amazon.Lambda;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Amazon.Lambda.TestUtilities;
using Xunit.Abstractions;

namespace LoggingFunction.Tests;

public class FunctionTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AmazonLambdaClient _lambdaClient;

    public FunctionTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _lambdaClient = new AmazonLambdaClient();
    }
    
    [Theory]
    [InlineData("E2ETestLambda_X86_NET6")]
    [InlineData("E2ETestLambda_ARM_NET6")]
    [InlineData("E2ETestLambda_X86_NET8")]
    [InlineData("E2ETestLambda_ARM_NET8")]
    public async Task TestToUpperFunction(string functionName)
    {
        var request = new InvokeRequest
        {
            FunctionName = functionName,
            InvocationType = InvocationType.RequestResponse,
            Payload = "\"hello world\"",
            LogType = LogType.Tail
        };

        var response = await _lambdaClient.InvokeAsync(request);

        var payload = System.Text.Encoding.UTF8.GetString(response.Payload.ToArray());
        Assert.Equal("\"HELLO WORLD\"", payload);
        
        if(string.IsNullOrEmpty(response.LogResult))
        {
            Assert.Fail("No LogResult field returned in the response of Lambda invocation. This should not happen.");
        }
        
        var logResult = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(response.LogResult));
        _testOutputHelper.WriteLine("Log output: " + logResult);
    }
}
