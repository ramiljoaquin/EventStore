// Copyright (c) 2012, Event Store LLP
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
// 
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.
// Neither the name of the Event Store LLP nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
using System;
using EventStore.Core.Bus;
using EventStore.Core.Messages;
using EventStore.Transport.Http;
using EventStore.Transport.Http.EntityManagement;

namespace EventStore.Core.Services.Transport.Http.Controllers
{
    public class ReadEventDataController : CommunicationController
    {
        private static readonly ICodec[] SupportedCodecs = new ICodec[] {Codec.Json, Codec.Xml, Codec.Text};
        private static readonly ICodec DefaultResponseCodec = Codec.Json;

        public ReadEventDataController(IPublisher publisher) : base(publisher)
        {
        }

        protected override void SubscribeCore(IHttpService service, HttpMessagePipe pipe)
        {
            service.RegisterControllerAction(new ControllerAction("/streams/{stream}/event/{version}?resolve={resolve}", 
                                                                  HttpMethod.Get,
                                                                  Codec.NoCodecs,
                                                                  SupportedCodecs,
                                                                  DefaultResponseCodec), 
                                             OnGetRead);
        }

        private void OnGetRead(HttpEntity entity, UriTemplateMatch match)
        {
            var stream = match.BoundVariables["stream"];
            var versionString = match.BoundVariables["version"];
            var resolve = match.BoundVariables["resolve"] ?? "yes";
            //TODO: reply invalid ??? if neither NO nor YES
            int version;

            if (String.IsNullOrEmpty(stream) || !Int32.TryParse(versionString, out version))
            {
                SendBadRequest(entity, "Stream must bu non-empty string and id must be integer value");
                return;
            }

            var envelope = new SendToHttpEnvelope(entity, Format.ReadEventCompleted, Configure.ReadEventCompleted);
            Publish(
                new ClientMessage.ReadEvent(
                    Guid.NewGuid(), envelope, stream, version, resolve.Equals("yes", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
