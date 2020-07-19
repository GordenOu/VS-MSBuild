using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.Cli.Utils;

namespace VS_MSBuild
{
    class Program
    {
        private static string[] Run(string commandName, params string[] args)
        {
            var lines = new List<string>();
            var command = Command
                .Create(commandName, args);
            Console.WriteLine($"{command.CommandName} {command.CommandArgs}");
            var result = command.OnOutputLine(lines.Add).Execute();
            if (result.ExitCode != 0)
            {
                Environment.Exit(result.ExitCode);
            }
            return lines.ToArray();
        }

        static void Main(string[] args)
        {
            string vswherePath = Path.Combine(
                Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
                "Microsoft Visual Studio",
                "Installer",
                "vswhere.exe");
            if (!File.Exists(vswherePath))
            {
                Console.WriteLine($"vswhere not found: {vswherePath}.");
            }
            var lines = Run(vswherePath,
                "-latest",
                "-requires", "Microsoft.Component.MSBuild",
                "-find", @"MSBuild\**\Bin\MSBuild.exe");
            if (lines.Length == 0)
            {
                Console.WriteLine("MSBuild not found.");
            }
            Run(lines[0], args);
        }
    }
}
