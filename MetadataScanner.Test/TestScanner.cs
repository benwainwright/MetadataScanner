namespace MetadataScanner.Test
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class TestScanner : TestBase
    {
        [Test]
        public void TestScannerFindsSomeAssemblies()
        {
            var dir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(dir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            Assert.That(assemblies, Is.Not.Null);
            Assert.That(assemblies, Has.Count.EqualTo(4));
        }

        [Test]
        public void TestScannerFindsTheDummyAssembly()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(testDir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;
            Assert.That(assemblies, Is.Not.Null);
            Assert.That(assemblies, Has.Count.EqualTo(4));

            var core = from assembly
                       in assemblies
                       where assembly.Name == "CleanIoc.Core"
                       select assembly;

            Assert.That(core.FirstOrDefault(), Is.Not.Null);
        }

        [Test]
        public void TestDummyAssemblyContainsSomeDefinedTypes()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(testDir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            var core = from assembly
                       in assemblies
                       where assembly.Name == "CleanIoc.Core"
                       select assembly;

            var types = core.FirstOrDefault().TypeDefinitions;

            Assert.That(types, Is.Not.Null);
            Assert.That(types, Is.Not.Empty);
        }

        [Test]
        public void TestDummyAssemblyContainsSomeTypeReferences()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(testDir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            var core = from assembly
                       in assemblies
                       where assembly.Name == "CleanIoc.Core"
                       select assembly;

            var types = core.FirstOrDefault().TypeReferences;
            Assert.That(types, Is.Not.Null);
            Assert.That(types, Is.Not.Empty);
        }

        [Test]
        public void TestDummyAssemblyContainsSomeAssemblyReferences()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(testDir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            var core = from assembly
                       in assemblies
                       where assembly.Name == "CleanIoc.Core"
                       select assembly;

            var references = core.FirstOrDefault().AssemblyReferences;
            Assert.That(references, Is.Not.Null);
            Assert.That(references, Is.Not.Empty);
        }

        [Test]
        public void TestThatIRegistryShouldBeMarkedAsAnInterface()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(testDir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            var core = from assembly
                       in assemblies
                       where assembly.Name == "CleanIoc.Core"
                       select assembly;

            var definitions = core.FirstOrDefault().TypeDefinitions;
            Assert.That(definitions, Is.Not.Empty);

            var iregistryQuery = from definition
                                 in definitions
                                 where definition.Name == "IRegistry"
                                 select definition;

            var iregistry = iregistryQuery.FirstOrDefault();

            Assert.That(iregistry, Is.Not.Null);
            Assert.That(iregistry.IsInterface);
        }

        [Test]
        public void TestThatRegistryImplementsIRegistry()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(testDir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            var core = from assembly
                       in assemblies
                       where assembly.Name == "CleanIoc.Core"
                       select assembly;

            var definitions = core.FirstOrDefault().TypeDefinitions;
            Assert.That(definitions, Is.Not.Empty);

            var registryQuery = from definition
                                in definitions
                                where definition.Name == "Registry"
                                select definition;

            var registry = registryQuery.FirstOrDefault();

            Assert.That(registry, Is.Not.Null);

            var implementations = from implementation
                                  in registry.InterfaceImplementations
                                  where implementation.Name == "IRegistry"
                                  select implementation;

            Assert.That(implementations.FirstOrDefault(), Is.Not.Null);
        }

        [Test]
        public void TestThatMySimpleRegistryImplementsIRegistry()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(testDir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            var core = from assembly
                       in assemblies
                       where assembly.Name == "CleanIoc.Core"
                       select assembly;

            var iregistryQuery = from definition
                                 in core.FirstOrDefault().TypeDefinitions
                                 where definition.Name == "IRegistry"
                                 select definition;

            var iregistry = iregistryQuery.FirstOrDefault();

            var lib = from assembly
                      in assemblies
                      where assembly.Name == "CleanIoc.Sample.Library"
                      select assembly;

            var simpleRegistry = from type
                                 in lib.FirstOrDefault().TypeDefinitions
                                 where type.Name == "MySimpleRegistry"
                                 select type;

            Assert.That(simpleRegistry.FirstOrDefault(), Is.Not.Null);
            Assert.That(simpleRegistry.FirstOrDefault().ImplementsInterface(iregistry));
        }

        [Test]
        public void TestDirectInheritanceAcrossAssemblyBoundariesIsIdentifiedByIsSubclassOf()
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var scanner = AssemblyScanner.Create(testDir + "/Assets");
            scanner.Scan();

            var assemblies = scanner.Assemblies;

            var lib = from assembly
                      in assemblies
                      where assembly.Name == "CleanIoc.Sample.Library"
                      select assembly;

            var simpleRegistryQuery = from type
                                      in lib.FirstOrDefault().TypeDefinitions
                                      where type.Name == "MySimpleRegistry"
                                      select type;

           var core = from assembly
                      in assemblies
                      where assembly.Name == "CleanIoc.Core"
                      select assembly;

            var registryQuery = from definition
                                in core.FirstOrDefault().TypeDefinitions
                                where definition.Name == "Registry"
                                select definition;

            var simpleRegistry = simpleRegistryQuery.FirstOrDefault();
            var registry = registryQuery.FirstOrDefault();
            Assert.That(simpleRegistry.IsSubclassOf(registry));
        }

        [Ignore("Need to find some samples for this")]
        [Test]
        public void TestDirectInheritanceWithinAnAssemblyIsIdentifiedAsSubclassOf()
        {
        }

        [Test]
        public void TestIndirectInheritanceAcrossAssemblyBoundaries()
        {

        }
    }
}