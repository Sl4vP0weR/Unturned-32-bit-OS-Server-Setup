using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib;
using dnlib.DotNet;
using dnlib.IO;
using dnlib.DotNet.Emit;

namespace Unturned_32BitServerSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            var defEx = new Exception("Drag & Drop Unturned direcory or Unturned.exe on this application to install/recover!");
            var quiet = args.Length == 2 && args[1].Contains("q");
            try
            {
#if DEBUG
                args = new string[] { @"F:\Steam\steamapps\common\Unturned", "q" };
#endif
                if (args.Length > 2 || args.Length < 1)
                    throw defEx;
                var path = args[0];
                if (Path.GetFileName(path) == "Unturned.exe" && File.Exists(path))
                    path = Path.GetDirectoryName(path);
                if (Directory.Exists(path))
                {
                    var untPath = path;
                    var file = Path.Combine(path, "Unturned_Data", "Managed", "Assembly-CSharp.dll");
                    var savFile = $"{file}.sav";
                    if (!File.Exists(savFile)) // UNTOUCHABLE BACKUP
                        File.Copy(file, savFile);
                    if (!File.Exists(file))
                        throw defEx;
                    var backup = $"{file}.bak";
                    string act = "patched";
                    patching:
                    if (!File.Exists(backup))
                    {
                        var fileData = File.ReadAllBytes(file);
                        var ctx = new ModuleContext();
                        var mod = ModuleDefMD.Load(fileData);
                        var clt = mod.CorLibTypes;
                        var type = mod.Find("SDG.Unturned.Assets", false);
                        var method = type.FindMethod("Start");
                        var refresh = type.FindMethod("refresh");
                        var instrs = (method.Body = new CilBody()).Instructions;
                        instrs.Add(OpCodes.Call.ToInstruction(refresh));
                        instrs.Add(OpCodes.Ret.ToInstruction());
                        File.Copy(file, backup);
                        mod.Write(file);
                    }
                    else
                    {
                        Console.Write("Recover ?[y/N]: ");
                        if (quiet || Console.ReadKey(true).Key != ConsoleKey.Y)
                        {
                            Console.WriteLine("N");
                            File.Delete(backup);
                            goto patching;
                        }
                        Console.WriteLine("Y");
                        File.Delete(file);
                        File.Copy(backup, file);
                        File.Delete(backup);
                        act = "recovered";
                    }
                    Console.WriteLine($"Unturned main dll {act}...");
                    file = Path.Combine(untPath, "steam_appid.txt");
                    if (!File.Exists(file))
                        throw defEx;
                    var appid = "304930";
                    if (act == "patched")
                    {
                        appid = "1110390";
                        act = "set";
                    }
                    File.WriteAllText(file, appid);
                    throw new Exception($"App id was {act} successfully...\nInstallation/recovery progress completed.");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#else
                Console.WriteLine(ex.Message);
#endif
                if(!quiet)
                    Console.ReadKey();
            }
        }
    }
}
