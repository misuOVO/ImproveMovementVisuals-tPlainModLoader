using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace ImproveMovementVisuals
{
    internal static class IMVFeature
    {
        private static readonly int[] Slimes = new int[]
        {
            1, 16, 59, 71, 81, 138, 122, 141, 147, 183,
            184, 204, 225, 244, 302, 304, 333, 335, 334, 336,
            535, 537
        };

        internal static void ApplyPlayer(Player p)
        {
            if (p == null || p.whoAmI != Main.myPlayer || Main.gameMenu) return;
            if (p.dead || p.sleeping.isSleeping || p.sitting.isSitting) return;
            if (p.isDisplayDollOrInanimate) return;
            if (p.mount.Active && p.mount.Type == 23) return;

            Setting.Data d = Setting.data;
            if (d == null) return;

            if (d.FaceCursor)
            {
                float dx = Main.MouseWorld.X - p.Center.X;
                if (dx > 6f) p.ChangeDir(1);
                else if (dx < -6f) p.ChangeDir(-1);
            }

            if (d.HeadStrength > 0.001f)
            {
                float rot = 0f;
                if (p.direction == 1 && Main.MouseWorld.X > p.position.X)
                    rot = Utils.ToRotation(Main.MouseWorld - p.Center);
                else if (p.direction == -1 && Main.MouseWorld.X < p.position.X)
                    rot = Utils.ToRotation(p.Center - Main.MouseWorld);

                p.headRotation = MathHelper.Clamp(rot, -0.5f, 0.4f) * d.HeadStrength;
            }

            if (d.LeanEnabled && !p.mount.Active)
            {
                p.fullRotationOrigin = new Vector2(p.width / 2f, p.height / 2f);
                float target = (p.velocity.X * p.runAcceleration
                    - p.velocity.Y * p.direction * 0.05f) * d.LeanScale;
                p.fullRotation = MathHelper.Clamp(target, -0.157f, 0.157f);
            }
        }

        internal static void ApplyNPC(NPC npc)
        {
            if (npc == null || !npc.active) return;
            Setting.Data d = Setting.data;
            if (d == null || !d.NPCLean) return;

            if (Array.IndexOf(Slimes, npc.type) >= 0)
            {
                npc.rotation = MathHelper.Clamp(
                    npc.velocity.Y * npc.direction / 4f, -0.3f, 0.3f);
            }
            else if (npc.aiStyle == 3)
            {
                npc.rotation = MathHelper.Clamp(
                    npc.velocity.X / 4f, -0.145f, 0.145f);
            }
            else if (npc.aiStyle == 7 && !npc.townNPC)
            {
                npc.rotation = MathHelper.Clamp(
                    npc.velocity.X / 4f, -0.1f, 0.1f);
            }
        }
        internal static void ApplyDrawHead(PlayerDrawSet drawInfo)
        {
            Player p = drawInfo.drawPlayer;
            if (p == null || p.whoAmI != Main.myPlayer || Main.gameMenu) return;
            if (p.dead || p.sleeping.isSleeping || p.sitting.isSitting) return;
            if (p.isDisplayDollOrInanimate) return;
            if (p.mount.Active && p.mount.Type == 23) return;
            if (drawInfo.shadow > 0f) return;

            Setting.Data d = Setting.data;
            if (d == null || d.HeadStrength <= 0.001f) return;

            float rot = 0f;
            if (p.direction == 1 && Main.MouseWorld.X > p.position.X)
                rot = Utils.ToRotation(Main.MouseWorld - p.Center);
            else if (p.direction == -1 && Main.MouseWorld.X < p.position.X)
                rot = Utils.ToRotation(p.Center - Main.MouseWorld);

            p.headRotation = MathHelper.Clamp(rot, -0.5f, 0.4f) * d.HeadStrength;
        }
    }

    [HarmonyPatch(typeof(Player), "Update", new Type[] { typeof(int) })]
    internal static class Patch_PlayerUpdate
    {
        private static void Postfix(Player __instance)
        {
            try { IMVFeature.ApplyPlayer(__instance); }
            catch (Exception ex) { IMVMod.LogError("Player.Update: " + ex.Message); }
        }
    }

    [HarmonyPatch(typeof(PlayerDrawSet), "BoringSetup")]
    internal static class Patch_PlayerDrawSetBoringSetup
    {
        private static void Postfix(PlayerDrawSet __instance)
        {
            try { IMVFeature.ApplyDrawHead(__instance); }
            catch (Exception ex) { IMVMod.LogError("PlayerDrawSet.BoringSetup: " + ex.Message); }
        }
    }

    [HarmonyPatch(typeof(NPC), "AI")]
    internal static class Patch_NPCAI
    {
        private static void Postfix(NPC __instance)
        {
            try { IMVFeature.ApplyNPC(__instance); }
            catch (Exception ex) { IMVMod.LogError("NPC.AI: " + ex.Message); }
        }
    }
}
