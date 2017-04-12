using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
                var applicationPath = Path.GetDirectoryName(Application.ExecutablePath);
                Debug.Assert(applicationPath != null, "Invalid path");

                var dllPath = Path.Combine(applicationPath, "lib");
                var extensionPath = Path.Combine(applicationPath, Settings.ScriptsPath);
                var binPath = Path.Combine(applicationPath, "bin");

                var dlls = Directory.GetFiles(dllPath, "*.dll");
                var executables = Directory.GetFiles(applicationPath, "*.exe");

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
                    var md5 = CreateMd5ForFolder(extension);
                    var extensionName = Path.GetFileNameWithoutExtension(extension);
                    if(extensionName == null)
                        continue;

                    var assemblyPath = Path.Combine(binPath, $"{extensionName}.dll");

                    if (!Settings.CompiledModules.TryGetValue(extensionName, out string storedMd5) || md5 != storedMd5 || !LoadExtensionFromFile(assemblyPath, out Assembly assembly))
                    {
                        parameters.OutputAssembly = assemblyPath;
                        Log.Info($"Compiling extension {extensionName}");
                        assembly = CompileExtension(extension, provider, parameters);
                        if (assembly == null)
                            continue;

                        Settings.CompiledModules[extensionName] = md5;
                        Settings.Serialize();
                    }
                    LoadExtension(assembly);
                }
            }
            catch (Exception e)
            {
                Log.Warn(e.ToString());
            }
        }

        private static bool LoadExtensionFromFile(string filename, out Assembly assembly)
        {
            try
            {
                assembly = Assembly.LoadFile(filename);
                return true;
            }
            catch (Exception)
            {
                assembly = null;
                return false;
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

        private static string CreateMd5ForFolder(string path)
        {
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .OrderBy(p => p).ToList();

            var md5 = MD5.Create();

            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var relativePath = file.Substring(path.Length + 1);
                var pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
                var contentBytes = File.ReadAllBytes(file);

                if (i == files.Count - 1)
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
        }
    }
}
