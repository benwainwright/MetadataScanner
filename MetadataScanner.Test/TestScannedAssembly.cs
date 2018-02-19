namespace MetadataScanner.Test
{
    using MetadataScanner.Enums;
    using MetadataScanner.Lib;
    using NUnit.Framework;

    [TestFixture]
    public class TestScannedAssembly
    {
        [Test]
        public void TestScannedAssemblyWithTokenOnlyProducesAnUnresolvedAssembly()
        {
            var assembly = new ScannedAssembly(token: 1);

            Assert.That(assembly.Token, Is.EqualTo(1));
            Assert.That(assembly.ResolutionStatus, Is.EqualTo(ResolutionStatus.UnResolved));
        }
    }
}
