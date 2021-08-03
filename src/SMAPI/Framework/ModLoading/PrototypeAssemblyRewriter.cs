using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using StardewModdingAPI.Toolkit.Framework.ModData;

namespace StardewModdingAPI.Framework.ModLoading
{
    public static class PrototypeAssemblyRewriter
    {
        private class ConsoleMonitor : IMonitor
        {
            public bool IsVerbose => true;

            public void Log(string message, LogLevel level = LogLevel.Trace)
            {
                Console.WriteLine(String.Format("{0}: {1}", level, message));
            }

            public void LogOnce(string message, LogLevel level = LogLevel.Trace)
            {
                Console.WriteLine($"{level}: {message}");
            }

            public void VerboseLog(string message)
            {
                Console.WriteLine($"Verbose: {message}");
            }
        }

        public static void RewriteAssembly(string assemblyPath)
        {
            var monitor = new ConsoleMonitor();
            var loader = new AssemblyLoader(
                Constants.Platform,
                Constants.GameFramework,
                monitor,
                paranoidMode: true,
                rewriteMods: true
            );

            monitor.Log("Starting");

            //var assemblyDef = AssemblyDefinition.ReadAssembly(assemblyPath);

            //var messages = new HashSet<string>();

            ModWarning warnings;
            //loader.Load("/home/dev/.local/share/Steam/steamapps/common/Stardew Valley/StardewValley.exe", assumeCompatible: true, out warnings);
            var result = loader.Load(assemblyPath, false, out warnings);
            var assembly = result.Item1;
            var assemblyDef = result.Item2;
            bool changed = result.Item3;

            if (changed)
            {
                string copyPath = assemblyPath + ".bak";
                if (File.Exists(copyPath))
                {
                    Console.WriteLine("Back up " + Path.GetFileName(copyPath) + " already exists");
                }
                else
                {
                    File.Copy(assemblyPath, copyPath);
                    Console.WriteLine("Backed up to " + Path.GetFileName(copyPath));
                }
                File.Delete(assemblyPath);
                assemblyDef.Write(assemblyPath);
                Console.WriteLine("Updated " + Path.GetFileName(assemblyPath));
            }
            else
            {
                Console.WriteLine("No change");
            }
        }
    }
}
