namespace MetadataScanner.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TestScanner
    {
        [Test]
        public void TestScannerFindsSomeAssemblies()
        {
            var dir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(dir);
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            Assert.That(assemblies, Is.Not.Null);
            Assert.That(assemblies, Is.Not.Empty);
        }
    }
}