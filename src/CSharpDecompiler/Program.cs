using System;
using Mono.Cecil.Cil;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.Decompiler;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Cecil;
using NDesk.Options;
using System.Collections.Generic;

namespace CSharpDecompiler
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                MainImpl(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `CSharpDecompiler --help' for more information.");
            }
        }

        public static void MainImpl(string[] args)
        {
            string output = null;
            string regex = null;
            bool parallel = false;
            bool help = false;

            var options = new OptionSet() {
                { "o|output=", "Output file or directory", x => output = x },
                { "r|regex=", "Filter input files", x => regex = x },
                { "p|parallel", "Decompile files parallely (for directories)", x => parallel = x != null },
                { "h|?|help", "Help", x => help = x != null },
            };

            List<string> extra = options.Parse(args);

            if (help)
            {
                ShowHelp(options);
                return;
            }

            if (extra.Count == 0)
                throw new ArgumentException("Provide file or directory path");

            if (extra.Count != 1)
                throw new ArgumentException("Too many extra arguments");

            string input = extra[0];

            bool inputIsFile = File.Exists(input);
            bool inputIsDirectory = Directory.Exists(input);
            if (!inputIsFile && !inputIsDirectory)
                throw new ArgumentException("Provided file or directory does not exist");
            
            if (inputIsDirectory)
            {
                if (output == null)
                    output = Directory.GetCurrentDirectory();

                if (parallel)
                {
                    new Decompiler().DecompileFilesFromDirectoryParallel(input, output, null, regex).Wait();
                }
                else
                {
                    new Decompiler().DecompileFilesFromDirectory(input, output, null, regex);
                }
            }
            else
            {
                if (output == null)
                {
                    new Decompiler().DecompileFile(input, Console.Out);
                }
                else
                {
                    new Decompiler().DecompileFile(input, output);
                }
            }
        }

        static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: CSharpDecompiler [OPTIONS]+ (file|directory)");
            Console.WriteLine("Decompiles .dll and .exe files to .cs");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }
    }
}
