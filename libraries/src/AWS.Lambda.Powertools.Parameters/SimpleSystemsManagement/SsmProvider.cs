/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using AWS.Lambda.Powertools.Parameters.Configuration;
using AWS.Lambda.Powertools.Parameters.Provider;
using AWS.Lambda.Powertools.Parameters.Internal.SimpleSystemsManagement;

namespace AWS.Lambda.Powertools.Parameters.SimpleSystemsManagement;

public class SsmProvider : ParameterProvider<SsmProviderConfigurationBuilder>
{
    private IAmazonSimpleSystemsManagement? _client;
    private IAmazonSimpleSystemsManagement Client => _client ??= new AmazonSimpleSystemsManagementClient();
    
    public SsmProvider UseClient(IAmazonSimpleSystemsManagement client)
    {
        _client = client;
        return this;
    }
    
    public SsmProviderConfigurationBuilder WithDecryption()
    {
        return NewConfigurationBuilder().WithDecryption();
    }

    public SsmProviderConfigurationBuilder Recursive()
    {
        return NewConfigurationBuilder().Recursive();
    }

    protected override SsmProviderConfigurationBuilder NewConfigurationBuilder()
    {
        return new SsmProviderConfigurationBuilder(this);
    }
    
    protected override async Task<string?> GetAsync(string key, ParameterProviderConfiguration? config)
    {
        var configuration = config as SsmProviderConfiguration;
        var response = await Client.GetParameterAsync(
            new GetParameterRequest
            {
                Name = key,
                WithDecryption = (configuration?.WithDecryption).GetValueOrDefault()
            }).ConfigureAwait(false);

        return response?.Parameter?.Value;
    }

    protected override async Task<IDictionary<string, string>> GetMultipleAsync(string path, ParameterProviderConfiguration? config)
    {
        var configuration = config as SsmProviderConfiguration;
        var retValues = new Dictionary<string, string>();

        string? nextToken = default;
        do
        {
            // Query AWS Parameter Store
            var response = await Client.GetParametersByPathAsync(
                new GetParametersByPathRequest
                {
                    Path = path,
                    WithDecryption = (configuration?.WithDecryption).GetValueOrDefault(),
                    Recursive = (configuration?.Recursive).GetValueOrDefault(),
                    NextToken = nextToken
                }).ConfigureAwait(false);

            // Store the keys/values that we got back into the protected Data dictionary
            foreach (var parameter in response.Parameters)
            {
                retValues.TryAdd(parameter.Name, parameter.Value);
            }

            // Possibly get more
            nextToken = response.NextToken;
        } while (!string.IsNullOrEmpty(nextToken));

        return retValues;
    }
}