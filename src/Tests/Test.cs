using NUnit.Framework;
using System;
using CSharpDecompiler;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class Test
    {
        static readonly string TestInputDirectory = Path.Combine("..", "..", "TestFiles");
        static readonly string TestOutputDirectory = Path.Combine("..", "..", "TestOutput");
        static readonly string TextInputFile = Path.Combine(TestInputDirectory, "Newtonsoft.Json.dll");
        static readonly string TestOutputFile = "output.cs";

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(TestOutputFile))
                File.Delete(TestOutputFile);

            foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.cs"))
                File.Delete(file);

            foreach (var file in Directory.EnumerateFiles(TestOutputDirectory, "*.cs"))
                File.Delete(file);
        }

        [Test]
        public void FileToConsole()
        {
            Run("{0}", TextInputFile);
        }

        [Test]
        public void FileToFile()
        {
            Run("-o {0} {1}", TestOutputFile, TextInputFile);

            Assert.True(File.Exists(TestOutputFile));
            Assert.True(new FileInfo(TestOutputFile).Length != 0);
        }

        [Test]
        public void DirectoryToCurrentDirectory()
        {
            Run("{0}", TestInputDirectory);

            Assert.True(DecompileDirectorySuccessed(TestInputDirectory, Directory.GetCurrentDirectory()));
        }

        [Test]
        public void DirectoryToDirectory()
        {
            Run("-o {0} {1}", TestOutputDirectory, TestInputDirectory);

            Assert.True(DecompileDirectorySuccessed(TestInputDirectory, TestOutputDirectory));
        }

        [Test]
        public void DirectoryToDirectoryParallel()
        {
            Run("-p -o {0} {1}", TestOutputDirectory, TestInputDirectory);

            Assert.True(DecompileDirectorySuccessed(TestInputDirectory, TestOutputDirectory));
        }

        [Test]
        public void DirectoryToDirectoryParallelWithRegex()
        {
            Run("{0} -p --output={1} --regex={2}", TestInputDirectory, TestOutputDirectory, @"^(?!Xamarin\.|System\.|Mono\.|mscorlib\.dll$)");

            Assert.True(Directory.EnumerateFiles(TestOutputDirectory).Any());
            Assert.True(Directory.EnumerateFiles(TestOutputDirectory, "*.cs").Select(x => Path.GetFileName(x)).All(x => !x.StartsWith("Xamarin.") && !x.StartsWith("System.") && !x.StartsWith("Mono.") && x != "mscorlib.cs"));
        }

        void Run(string format, params object[] args)
        {
            MainClass.MainImpl(String.Format(format, args).Split(' '));
        }

        bool DecompileDirectorySuccessed(string input, string output)
        {
            var testFiles = Directory.EnumerateFiles(input, "*.dll").Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
            var outputFiles = Directory.EnumerateFiles(output, "*.cs").Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();

            return testFiles.SequenceEqual(outputFiles);
        }
    }
}

