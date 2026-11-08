using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DML.Core
{
    public class WorldBoxMod : MonoBehaviour
    {
        private static bool _isInitialized = false;

        public static void init()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            try
            {
                // Creăm un GameObject root curat direct din codul de inițializare
                GameObject coreHost = new GameObject("DML_CoreHost");
                coreHost.AddComponent<WorldBoxMod>();
                DontDestroyOnLoad(coreHost);
            }
            catch (Exception ex)
            {
                try
                {
                    File.WriteAllText("dml_core_init_error.txt", ex.ToString());
                }
                catch { }
            }
        }

        private void Awake()
        {
            // Asigurăm persistența și pe instanța atașată
            DontDestroyOnLoad(gameObject);

            // Rulăm sistemul de generare a folderelor și scanare a sub-modurilor
            DMLMasterCore.InitializeSystem();
        }
    }

    public static class DMLMasterCore
    {
        public static void InitializeSystem()
        {
            try
            {
                string assemblyPath = typeof(WorldBoxMod).Assembly.Location;
                string assemblyDir = Path.GetDirectoryName(assemblyPath);

                string rootDir = Path.Combine(assemblyDir, "DML-ModLoader");
                string modsDir = Path.Combine(rootDir, "Mods");
                string configsDir = Path.Combine(rootDir, "Configs");
                string systemDir = Path.Combine(rootDir, "System");

                Directory.CreateDirectory(rootDir);
                Directory.CreateDirectory(modsDir);
                Directory.CreateDirectory(configsDir);
                Directory.CreateDirectory(systemDir);

                File.WriteAllText(Path.Combine(rootDir, "dml_status.log"), 
                    $"DML Core a pornit curat la: {DateTime.Now}\nDirector: {assemblyDir}");

                ScanAndExecuteSubMods(modsDir, rootDir);
            }
            catch (Exception ex)
            {
                try
                {
                    File.WriteAllText("dml_fatal_error.txt", ex.ToString());
                }
                catch { }
            }
        }

        private static void ScanAndExecuteSubMods(string modsDirectory, string rootFolder)
        {
            if (!Directory.Exists(modsDirectory)) return;

            string[] subFolders = Directory.GetDirectories(modsDirectory);
            foreach (string folder in subFolders)
            {
                string scriptPath = Path.Combine(folder, "dml-script.dll");
                if (File.Exists(scriptPath))
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(scriptPath);
                        foreach (Type t in asm.GetTypes())
                        {
                            MethodInfo m = t.GetMethod("StartMod", BindingFlags.Public | BindingFlags.Static);
                            if (m != null)
                            {
                                m.Invoke(null, new object[] { folder });
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Path.Combine(rootFolder, "error.log"), ex.Message + "\n");
                    }
                }
            }
        }
    }
}