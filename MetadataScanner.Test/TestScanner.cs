namespace MetadataScanner.Test
{
    using System.Collections.Generic;
    using MetadataScanner.Interfaces;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class TestScanner : TestBase
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

        [Test]
        public void TestScannerFindsTheDummyAssembly()
        {
            var dir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(dir);
            scanner.Scan();

            var mockAssembly = new Mock<IAssembly>();
            mockAssembly.Setup(x => x.Name).Returns("CleanIoc.Core");
            mockAssembly.Setup(x => x.PublicKey).Returns(new List<byte>());
            mockAssembly.Setup(x => x.Version).Returns(new System.Version(1, 0, 0, 0));

            var assemblies = scanner.Assemblies;
            Assert.That(assemblies, Is.Not.Null);
            Assert.That(assemblies, Contains.Item(mockAssembly).Using<IAssembly>(this));
        }
    }
}