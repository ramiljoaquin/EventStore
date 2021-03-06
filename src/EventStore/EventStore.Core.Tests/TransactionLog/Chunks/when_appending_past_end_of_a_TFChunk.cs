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
using System.IO;
using EventStore.Core.TransactionLog.LogRecords;
using NUnit.Framework;

namespace EventStore.Core.Tests.TransactionLog.Chunks
{
    [TestFixture]
    public class when_appending_past_end_of_a_TFChunk
    {
        readonly string filename = Path.Combine(Path.GetTempPath(), "foo");
        private Core.TransactionLog.Chunks.TFChunk _chunk;
        private Guid _corrId = Guid.NewGuid();
        private Guid _eventId = Guid.NewGuid();
        private bool _written;

        [SetUp]
        public void Setup()
        {
            var record = new PrepareLogRecord(15556, _corrId, _eventId, 15556, "test", 1, new DateTime(2000, 1, 1, 12, 0, 0),
                                              PrepareFlags.None, "Foo", new byte[12], new byte[15]);
            _chunk = Core.TransactionLog.Chunks.TFChunk.CreateNew(filename, 20, 0, 0);
            _written = _chunk.TryAppend(record).Success;
        }

        [Test]
        public void the_record_is_not_appended()
        {
            Assert.IsFalse(_written);
        }

        [TearDown]
        public void TD()
        {
            _chunk.Dispose();
            File.Delete(filename);
        }
    }
}