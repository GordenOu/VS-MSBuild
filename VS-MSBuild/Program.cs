using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.Cli.Utils;

namespace VS_MSBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            // Find vswhere
            string programFilesPath = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (string.IsNullOrEmpty(programFilesPath))
            {
                Console.WriteLine($"ProgramFiles(x86) not found.");
                return;
            }
            string vswherePath = Path.Combine(
                programFilesPath,
                "Microsoft Visual Studio",
                "Installer",
                "vswhere.exe");
            if (!File.Exists(vswherePath))
            {
                Console.WriteLine($"vswhere not found: {vswherePath}.");
                return;
            }

            // Find latest MSBuild.
            var lines = new List<string>();
            var command = Command.Create(
                vswherePath,
                new[]
                {
                    "-latest",
                    "-requires", "Microsoft.Component.MSBuild",
                    "-find", @"MSBuild\**\Bin\MSBuild.exe"
                });
            Console.WriteLine($"{command.CommandName} {command.CommandArgs}");
            var result = command
                .OnOutputLine(lines.Add)
                .OnErrorLine(lines.Add)
                .Execute();
            if (result.ExitCode != 0)
            {
                Environment.Exit(result.ExitCode);
            }
            if (lines.Count == 0)
            {
                Console.WriteLine("MSBuild not found.");
                return;
            }

            // Invoke MSBuild
            command = Command.Create(lines[0], args);
            result = command
                .OnOutputLine(Console.WriteLine)
                .OnErrorLine(Console.WriteLine)
                .Execute();
            Environment.Exit(result.ExitCode);
        }
    }
}
