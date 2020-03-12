﻿// <copyright file="Http2Scenario.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SampleClient.Scenarios
{
    internal class Http2Scenario : IScenario
    {
        public async Task ExecuteAsync(CommandLineArgs args, CancellationToken cancellation)
        {
            using (var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
                UseCookies = false,
                UseProxy = false,
            })
            using (var client = new HttpMessageInvoker(handler))
            {
                var targetUri = new Uri(new Uri(args.Target, UriKind.Absolute), "api/dump");
                var stopwatch = Stopwatch.StartNew();
                var request = new HttpRequestMessage(HttpMethod.Get, targetUri);
                request.Version = new Version(2, 0);
                Console.WriteLine($"Calling {targetUri} with HTTP/2");

                var response = await client.SendAsync(request, cancellation);
                Console.WriteLine($"Received response: {(int)response.StatusCode} in {stopwatch.ElapsedMilliseconds} ms");
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                JsonDocument json = JsonDocument.Parse(body);
                Console.WriteLine($"Received response:{Environment.NewLine}{JsonSerializer.Serialize(json.RootElement, new JsonSerializerOptions { WriteIndented = true })}");

                response.EnsureSuccessStatusCode();
            }
        }
    }
}