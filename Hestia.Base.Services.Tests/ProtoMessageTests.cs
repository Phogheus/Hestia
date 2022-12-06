using Hestia.Base.Services.ProtoContracts;
using NUnit.Framework;

namespace Hestia.Base.Services.Tests
{
    public class ProtoMessageTests
    {
        private enum TestingEnumType
        {
            ValueOne,
            ValueTwo
        }

        private record TestingRecord(int ValueOne, int ValueTwo);

        [Test]
        public void ConstructorTests()
        {
            Assert.DoesNotThrow(() => new ProtoMessage<object?>(null));
            Assert.DoesNotThrow(() => new ProtoMessage<TestingEnumType>(TestingEnumType.ValueTwo));
            Assert.DoesNotThrow(() => new ProtoMessage<int>(1));
            Assert.DoesNotThrow(() => new ProtoMessage<string>("Hello World"));
            Assert.DoesNotThrow(() => new ProtoMessage<char>('c'));
            Assert.DoesNotThrow(() => new ProtoMessage<TestingRecord>(new TestingRecord(3, 4)));
        }

        [Test]
        public void GetMessageTests()
        {
            Assert.Multiple(() =>
            {
                Assert.That(new ProtoMessage<TestingEnumType>(TestingEnumType.ValueTwo).GetMessage(), Is.EqualTo(TestingEnumType.ValueTwo));
                Assert.That(new ProtoMessage<int>(1).GetMessage(), Is.EqualTo(1));
                Assert.That(new ProtoMessage<string>("Hello World").GetMessage(), Is.EqualTo("Hello World"));
                Assert.That(new ProtoMessage<char>('c').GetMessage(), Is.EqualTo('c'));
            });

            var record = new TestingRecord(3, 4);
            var recordProtoMessage = new ProtoMessage<TestingRecord>(record).GetMessage()!;

            Assert.Multiple(() =>
            {
                Assert.That(record.ValueOne, Is.EqualTo(recordProtoMessage.ValueOne));
                Assert.That(record.ValueTwo, Is.EqualTo(recordProtoMessage.ValueTwo));
            });
        }
    }
}