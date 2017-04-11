using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using NLog;

namespace MoBot.Scripts
{
    public class ScriptLoader
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static void LoadScripts()
        {
            try
            {
                CodeDomProvider provider = new CSharpCodeProvider();
                var path = Path.GetDirectoryName(Application.ExecutablePath);
                Debug.Assert(path != null, "Invalid path");

                var dllPath = Path.Combine(path, "lib");
                var dlls = Directory.GetFiles(dllPath, "*.dll");
                var executables = Directory.GetFiles(path, "*.exe");
                var extensionPath = Path.Combine(path, Settings.ScriptsPath);


                var parameters = new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = false,
                };

                parameters.ReferencedAssemblies.AddRange(dlls);
                parameters.ReferencedAssemblies.AddRange(executables);
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("System.ComponentModel.Composition.dll");
                parameters.ReferencedAssemblies.Add("System.ComponentModel.DataAnnotations.dll");
                parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");


                var extensions = Directory.GetDirectories(extensionPath);
                foreach (var extension in extensions)
                {
                    //parameters.OutputAssembly = $"{Path.GetFileNameWithoutExtension(extension)}.dll";
                    var assembly = CompileExtension(extension, provider, parameters);
                    if (assembly == null)
                        continue;
                    LoadExtension(assembly);
                }
            }
            catch (Exception e)
            {
                Log.Warn(e.ToString());
            }
        }

        private static Assembly CompileExtension(string folder, CodeDomProvider provider, CompilerParameters parameters)
        {
            var files = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);
            var compilerResults = provider.CompileAssemblyFromFile(parameters, files);
            if (!compilerResults.Errors.HasErrors) return compilerResults.CompiledAssembly;


            var errorDescription = new StringBuilder();
            errorDescription.AppendLine($"Failed to compile {folder}:");
            foreach (CompilerError error in compilerResults.Errors)
            {
                errorDescription.AppendLine($"Error in {error.FileName} ({error.ErrorNumber}): {error.ErrorText}");
            }
            Log.Error(errorDescription);
            return null;
        }

        private static void LoadExtension(Assembly extensionAssembly)
        {
            var extensionLoadClass = extensionAssembly.GetExportedTypes()
                .FirstOrDefault(type => type.GetCustomAttribute<MoBotExtension>() != null);

            if (extensionLoadClass == null)
            {
                Log.Error("Failed to find loader class!");
                return;
            }

            var extensionInfo = extensionLoadClass.GetCustomAttribute<MoBotExtension>();
            var instance = Activator.CreateInstance(extensionLoadClass);
            if (instance == null)
            {
                Log.Error($"Failed to create loader instance for {extensionInfo.Id}");
                return;
            }

            var initMethod = extensionLoadClass.GetMethods()
                .FirstOrDefault(method => method.GetCustomAttribute<Initialisation>() != null);
            if (initMethod == null)
            {
                Log.Error($"Failed to find init method for {extensionInfo.Id}");
                return;
            }
            initMethod.Invoke(instance, null);
            Console.WriteLine($"Succesfully loaded {extensionInfo.Id}: {extensionInfo.Version}");
        }
    }
}
