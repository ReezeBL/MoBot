using System;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;
using MoBot.Scripts;
using MoBot.Structure;
using MoBot.Structure.Game;
using MoBot.Structure.Game.Items;
using NLog;

namespace MoBot
{
    public class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static Logger GetLogger()
        {
            return Log;
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoadScripts();

            GameBlock.LoadBlocks();
            Item.LoadItems();

            NetworkController model = NetworkController.GetInstance();
            Controller controller = new Controller();
            Viewer viewer = new Viewer { MainController = controller };
            model.Subscribe(viewer);
            model.Subscribe(viewer);
            Application.Run(viewer);
        }

        private static void LoadScripts()
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            Debug.Assert(path != null, "Invalid path");

            string[] dlls = Directory.GetFiles(path, "*.dll");
            string[] executables = Directory.GetFiles(path, "*.exe");


            foreach (var script in Directory.GetFiles(Path.Combine(path, Settings.ScriptsPath), "*.cs"))
            {
                CompilerParameters parameters = new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                    OutputAssembly = script
                };

                parameters.ReferencedAssemblies.AddRange(dlls);
                parameters.ReferencedAssemblies.AddRange(executables);
                parameters.ReferencedAssemblies.Add("System.dll");

                CompilerResults result = provider.CompileAssemblyFromFile(parameters, script);
                if (result.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Failed to compile {script}");
                    foreach (CompilerError error in result.Errors)
                    {
                        sb.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
                    }
                    Console.WriteLine(sb);
                }
                else
                {
                    Assembly assembly = result.CompiledAssembly;
                    var methods = assembly.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(ImportAttribue.PreInit), false).Length > 0)
                      .ToArray();
                    foreach (var method in methods)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
