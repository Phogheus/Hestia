using System;
using Hestia.Base.Utilities;
using NUnit.Framework;

namespace Hestia.Base.Tests.UtilityTests
{
    public class PredictableGuidTests
    {
        [Test]
        public void NewGuidTests()
        {
            // Test name and default namespace
            var guid1 = PredictableGuid.NewGuid(nameof(PredictableGuidTests));
            var guid2 = PredictableGuid.NewGuid(nameof(PredictableGuidTests));
            var guid3 = PredictableGuid.NewGuid(nameof(PredictableGuidTests).ToLower());

            Assert.That(guid1, Is.EqualTo(guid2));
            Assert.That(guid1, Is.Not.EqualTo(guid3));

            var newNamespace = Guid.NewGuid();

            guid1 = PredictableGuid.NewGuid(newNamespace, nameof(PredictableGuidTests));
            guid2 = PredictableGuid.NewGuid(newNamespace, nameof(PredictableGuidTests));
            guid3 = PredictableGuid.NewGuid(nameof(PredictableGuidTests));
            var guid4 = PredictableGuid.NewGuid(newNamespace, nameof(PredictableGuidTests).ToLower());

            Assert.That(guid1, Is.EqualTo(guid2));
            Assert.That(guid1, Is.Not.EqualTo(guid3));
            Assert.That(guid1, Is.Not.EqualTo(guid4));
        }
    }
}
