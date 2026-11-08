# 🚀 Darius's ModLoader (DML) for WorldBox

**Darius's ModLoader (DML)** is a lightweight, high-performance, and persistent modding framework designed specifically for **WorldBox**. Built around a robust `MonoBehaviour` host architecture (`DML_CoreHost`), DML ensures that your mods persist seamlessly across game states, world reloads, and menu transitions without altering the game's core assemblies directly.

---

## 📂 Ecosystem & Directory Structure

Upon its first execution inside the WorldBox directory, DML automatically generates a clean and organized workspace (`DML-ModLoader`) to handle all your mods, configurations, and shared libraries:

```text
DML-ModLoader/
├── Mods/
│   ├── SpeedMultiplierMod/
│   │   └── dml-script.dll  <-- Each sub-mod lives in its own dedicated folder
│   └── CustomUIMod/
│       └── dml-script.dll
├── Configs/                <-- Global configuration files and ini settings
├── System/                 <-- Shared helper libraries and internal API dependencies
├── dml_status.log          <-- Live initialization tracking and diagnostic logs
└── error.log               <-- Comprehensive exception logging for debugging

```

---

## 📥 Installation Guide (For Players)

1. **Obtain DML Core:** Download the latest release of **`DML.Core.dll`**.
2. **Install to Game Directory:** Move `DML.Core.dll` into your WorldBox managed folder:
* *Steam Path:* `C:\Program Files (x86)\Steam\steamapps\common\worldbox\worldbox_Data\Managed\` (or your custom injection target).


3. **Initialize Ecosystem:** Launch WorldBox once. DML will automatically inject its core, verify assemblies, and generate the `DML-ModLoader` directory tree.
4. **Install Sub-Mods:** Download any compatible sub-mod, create a subfolder inside `DML-ModLoader/Mods/` (e.g., `Mods/SpeedMod/`), and place its compiled **`dml-script.dll`** inside.

---

## 🛠️ Comprehensive Guide for Mod Developers

DML uses dynamic **Reflection** during its startup sequence. It scans every subfolder inside the `Mods/` directory looking for a compiled binary named **`dml-script.dll`**, inspects its types, and invokes a predefined static entry point.

### 1. Project Setup Requirements

To build a compatible sub-mod, configure your C# project (`.csproj`) with the following specifications:

* **Target Framework:** `.NET Framework 4.7.2` (matching WorldBox's runtime environment).
* **Required Assembly References:** (Imported from your game's `worldbox_Data/Managed/` directory):
* `UnityEngine.dll`
* `UnityEngine.CoreModule.dll`
* `Assembly-CSharp.dll`



### 2. Standard Sub-Mod Template (`SubModMain.cs`)

Your mod must expose a public static method named **`StartMod`** taking a `string` parameter representing its local folder path. This is what DML triggers upon discovery.

```csharp
using System;
using UnityEngine;

namespace DML.SubMods
{
    public class SpeedMultiplierMod
    {
        // Mandatory entry point called automatically by DML.Core via Reflection
        public static void StartMod(string modFolderPath)
        {
            try
            {
                Debug.Log("[DML.SubMod] Initializing SpeedMultiplierMod from path: " + modFolderPath);

                // Create a persistent root GameObject for your sub-mod logic
                GameObject modHost = new GameObject("DML_SpeedModHost");
                modHost.AddComponent<SpeedControllerComponent>();
                
                // Keep it alive across scene reloads
                UnityEngine.Object.DontDestroyOnLoad(modHost);

                Debug.Log("[DML.SubMod] SpeedMultiplierMod successfully injected!");
            }
            catch (Exception ex)
            {
                Debug.LogError("[DML.SubMod] Critical error during initialization: " + ex.Message);
            }
        }
    }

    public class SpeedControllerComponent : MonoBehaviour
    {
        private void Update()
        {
            try
            {
                // Example logic: Bypassing native limits to push simulation speed (e.g., 50x - 100x)
                Time.timeScale = 50f; 
            }
            catch (Exception ex)
            {
                Debug.LogError("[DML.SubMod] Error updating timeScale: " + ex.Message);
            }
        }
    }
}

```

### 3. Compilation & Deployment Workflow

1. Compile your mod project as a Class Library (`.dll`) targeting **.NET Framework 4.7.2** and name the output assembly **`dml-script.dll`**.
2. Navigate to your WorldBox installation folder: `DML-ModLoader/Mods/`.
3. Create a new folder named after your mod (e.g., `UltraSpeedMod`).
4. Copy your **`dml-script.dll`** directly inside that folder.
5. Boot up WorldBox. DML will load your script dynamically!

---

## 🔍 Troubleshooting & Diagnostic Logs

If a sub-mod fails to load or behave as expected, check the following files inside your installation directory:

* **`dml_status.log`**: Shows whether DML Core initialized correctly and lists discovered mod directories.
* **`error.log`**: Captures any runtime exceptions, missing references, or reflection failures encountered while loading `dml-script.dll` files.

---

## 🤝 Contributing & License

Darius's ModLoader is open-source. Feel free to open pull requests, suggest architecture expansions, report issues, or build extensions for the WorldBox community!

```

```