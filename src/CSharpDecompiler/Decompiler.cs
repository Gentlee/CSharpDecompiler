using System;
using System.IO;
using Mono.Cecil;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.Decompiler;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSharpDecompiler
{
    public class Decompiler
    {
        readonly string[] _inputFileExtensions = new [] { ".dll", ".exe" };

        public void DecompileFile(string input, string output)
        {
            DecompileFile(input, File.CreateText(output));
        }

        public void DecompileFile(string input, TextWriter writer)
        {
            var assembly = AssemblyDefinition.ReadAssembly(input, new ReaderParameters() {
                AssemblyResolver = new IgnoringExceptionsAssemblyResolver()
            });

            var decompiler = new AstBuilder(new DecompilerContext(assembly.MainModule));
            decompiler.AddAssembly(assembly);
            decompiler.GenerateCode(new PlainTextOutput(writer));
            writer.Close();
        }

        public void DecompileFilesFromDirectory(string input, string output, string[] except = null, string regex = null)
        {
            foreach (var file in EnumerateInputFiles(input, except, regex))
            {
                DecompileFile(file, GetNewFilePath(file, output));
            }
        }

        public async Task DecompileFilesFromDirectoryParallel(string input, string output, string[] except = null, string regex = null)
        {
            var tasks = EnumerateInputFiles(input, except, regex)
                .Select(file => Task.Run(() => DecompileFile(file, GetNewFilePath(file, output))));
            
            await Task.WhenAll(tasks);
        }

        IEnumerable<string> EnumerateInputFiles(string directory, string[] except, string regex)
        {
            var files = Directory.EnumerateFiles(directory)
                .Where(x => _inputFileExtensions.Contains(Path.GetExtension(x)));

            if (regex != null)
                files = files.Where(x => Regex.IsMatch(Path.GetFileName(x), regex));

            if (except != null && except.Any())
                files = files.Except(except);

            return files;
        }

        string GetNewFilePath(string oldFilePath, string newDirectory)
        {
            return Path.Combine(newDirectory, Path.GetFileNameWithoutExtension(oldFilePath) + ".cs");
        }
    }
}

