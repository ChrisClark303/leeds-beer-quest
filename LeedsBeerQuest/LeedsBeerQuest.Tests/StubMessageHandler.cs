﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LeedsBeerQuest.Tests
{
    public class StubMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage? _response;
        private readonly List<HttpRequestMessage> _messages = new List<HttpRequestMessage>();
        public Uri? LastRequestUri;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _messages.Add(request);
            LastRequestUri = request.RequestUri;
            return Task.FromResult(_response ?? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("") });
        }

        public static StubMessageHandler WithResponse(HttpResponseMessage response)
        {
            return new StubMessageHandler()
            {
                _response = response
            };
        }

        public static StubMessageHandler WithResponse(HttpStatusCode statusCode)
        {
            return new StubMessageHandler()
            {
                _response = new HttpResponseMessage(statusCode)
            };
        }
    }
}
