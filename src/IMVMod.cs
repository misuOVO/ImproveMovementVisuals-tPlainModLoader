using System;
using HarmonyLib;
using tContentPatch;
using tContentPatch.ModLoad;
using tContentPatch.Patch;

namespace ImproveMovementVisuals
{
    /// <summary>
    /// 更好的移动视觉 (Improved Movement Visuals) for tPlainModLoader
    /// 参考 Workshop "Improved Movement Visuals" (id 2816557864, 2024.9 版本)。
    /// </summary>
    public sealed class IMVMod : Mod
    {
        private const string PatchId = "ImproveMovementVisuals.patch";
        private Harmony harmony = null;
        private static string modDir = null;

        internal static string ModDir { get { return modDir; } }

        internal static void LogError(string message)
        {
            if (modDir == null || message == null) return;
            try
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(modDir, "patch-errors.txt"),
                    "\n[运行时] " + message + "\n");
            }
            catch { }
        }

        internal static void LogInfo(string message)
        {
            if (modDir == null || message == null) return;
            try
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(modDir, "draw-log.txt"),
                    DateTime.Now.ToString("HH:mm:ss.fff") + " " + message + "\n");
            }
            catch { }
        }

        public override void Load(ModObject mo)
        {
            modDir = mo?.modPath;
            LogInfo("IMV loaded");
        }

        public override void AddPatch(IAddPatch addPatch)
        {
            harmony = new Harmony(PatchId);
            System.Text.StringBuilder failures = new System.Text.StringBuilder();
            foreach (System.Type type in typeof(IMVMod).Assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(HarmonyPatch), true).Length == 0) continue;
                try
                {
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch (System.Exception ex)
                {
                    System.Exception e = (ex is HarmonyException && ex.InnerException != null) ? ex.InnerException : ex;
                    failures.AppendLine(type.Name + ": " + e.Message);
                }
            }
            if (failures.Length > 0 && modDir != null)
            {
                try
                {
                    System.IO.File.WriteAllText(
                        System.IO.Path.Combine(modDir, "patch-errors.txt"),
                        "以下补丁类应用失败:\n" + failures);
                }
                catch { }
            }
        }

        public override void Unload()
        {
            harmony?.UnpatchAll(PatchId);
            harmony = null;
        }
    }
}
