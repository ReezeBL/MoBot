using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using MoBot.Scripts;
using MoBot.Structure;
using MoBot.Structure.Game;
using MoBot.Structure.Game.Items;
using MoBot.Structure.Windows;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MoBot
{
    public class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static Logger GetLogger()
        {
            return Log;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AllocConsole();

            Console.WriteLine("Loading scripts...");
            LoadScripts();

            Console.WriteLine("Loading blocks...");
            Block.LoadBlocks();

            Console.WriteLine("Loading items...");
            Item.LoadItems();

            Console.WriteLine("Loading entites...");
            Entity.LoadEntities();

            using (var connection = new SQLiteConnection("Data Source=GameInfo.db3;"))
            {
                connection.Open();
                Block.WriteBlocksToDb(connection);
            }

            

            Console.WriteLine("Everything is done! Application is ready to launch!");

            var model = NetworkController.Instance;
            var controller = new Controller();
            var viewer = new Viewer { MainController = controller };
            model.Subscribe(viewer);
            FreeConsole();

            Application.Run(viewer);
        }

        private static void LoadScripts()
        {
            try
            {
                CodeDomProvider provider = new CSharpCodeProvider();
                var path = Path.GetDirectoryName(Application.ExecutablePath);
                Debug.Assert(path != null, "Invalid path");

                var dllPath = Path.Combine(path, "lib");
                var dlls = Directory.GetFiles(dllPath, "*.dll");
                var executables = Directory.GetFiles(path, "*.exe");


                var scripts = Directory.GetFiles(Path.Combine(path, Settings.ScriptsPath), "*.cs",
                    SearchOption.AllDirectories);
                {
                    var parameters = new CompilerParameters
                    {
                        GenerateExecutable = false,
                        GenerateInMemory = true,
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

                    CompilerResults result = provider.CompileAssemblyFromFile(parameters, scripts);
                    if (result.Errors.HasErrors)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"Failed to compile");
                        foreach (CompilerError error in result.Errors)
                        {
                            sb.AppendLine($"Error in {error.FileName} ({error.ErrorNumber}): {error.ErrorText}");
                        }
                        Log.Error(sb);
                    }
                    else
                    {
                        var assembly = result.CompiledAssembly;
                        var methods = assembly.GetTypes()
                            .SelectMany(t => t.GetMethods())
                            .Where(m => m.GetCustomAttributes(typeof(ImportHandler.PreInit), false).Length > 0)
                            .ToArray();
                        foreach (var method in methods)
                        {
                            method.Invoke(null, null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
               Log.Warn(e.ToString());
            }
        }
    }
}
