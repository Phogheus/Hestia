using System;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using NUnit.Framework;

namespace Hestia.Base.ServiceBus.Tests
{
    public class ServiceBusSettingsTests
    {
        [Test]
        public void ConstructorTests()
        {
            Assert.DoesNotThrow(() => new ServiceBusSettings());
            _ = Assert.Throws<InvalidOperationException>(() => new ServiceBusSettings(string.Empty, string.Empty, 0, null));
            _ = Assert.Throws<InvalidOperationException>(() => new ServiceBusSettings("Some string", string.Empty, 0, null));
            _ = Assert.Throws<InvalidOperationException>(() => new ServiceBusSettings("Some string", "Some other string", 0, null));
            Assert.DoesNotThrow(() => new ServiceBusSettings("Some string", "Some other string", ServiceBusSettings.MINIMUM_BATCH_SIZE, null));
        }

        [Test]
        public void SerializationTests()
        {
            // Test without options
            var settings = new ServiceBusSettings("some string", "some other string", 100, null);
            var serialized = JsonSerializer.Serialize(settings);
            var deserialized = JsonSerializer.Deserialize<ServiceBusSettings>(serialized);

            Assert.That(deserialized, Is.Not.Null);
            Assert.That(settings.PrimaryConnectionString, Is.EqualTo(deserialized.PrimaryConnectionString));
            Assert.That(settings.SecondaryConnectionString, Is.EqualTo(deserialized.SecondaryConnectionString));
            Assert.That(settings.DefaultMaxSizeInBytes, Is.EqualTo(deserialized.DefaultMaxSizeInBytes));

            // Test with options
            settings = new ServiceBusSettings("another string", "another other string", 255, new ServiceBusProcessorOptions { SubQueue = SubQueue.TransferDeadLetter });
            serialized = JsonSerializer.Serialize(settings);
            deserialized = JsonSerializer.Deserialize<ServiceBusSettings>(serialized);

            Assert.That(deserialized, Is.Not.Null);
            Assert.That(settings.PrimaryConnectionString, Is.EqualTo(deserialized.PrimaryConnectionString));
            Assert.That(settings.SecondaryConnectionString, Is.EqualTo(deserialized.SecondaryConnectionString));
            Assert.That(settings.DefaultMaxSizeInBytes, Is.EqualTo(deserialized.DefaultMaxSizeInBytes));
            Assert.That(settings.ProcessorOptions, Is.Not.Null);
            Assert.That(settings.ProcessorOptions.SubQueue, Is.EqualTo(SubQueue.TransferDeadLetter));
        }
    }
}
