using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;

namespace SpellbookMerge.Patches
{
    /// <summary>
    /// 法术进度补丁类
    /// 修改多个职业的法术位表，使其能够达到更高的法术等级
    /// 从而支持与神话道途合书后的完整施法能力
    /// </summary>
    public class SpellbookProgression
    {
        /// <summary>
        /// Harmony补丁：在蓝图缓存初始化时应用法术进度修改
        /// </summary>
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        private static class BlueprintsCacheInitPatch
        {
            /// <summary>防止重复初始化的标志位</summary>
            private static bool _initialized;

            /// <summary>
            /// 在蓝图缓存初始化完成后执行的后置补丁
            /// </summary>
            private static void Postfix()
            {
                // Mod未启用或已初始化则跳过
                if (!Main.Enabled || _initialized) return;
                _initialized = true;
                
                Main.LogHeader("Patching spellbook progressions");
                
                // 依次应用9个职业的法术位补丁
                PatchInquisitorSpellSlotProgression();      // 审判官
                PatchWarPriestSpellSlotProgression();       // 战斗祭司
                PatchMagusSpellSlotProgression();           // 魔战士
                PatchBardSpellSlotProgression();            // 诗人
                PatchSkaldSpellSlotProgression();           // 歌者
                PatchSwordSaintSpellSlotProgression();      // 剑圣
                PatchBloodRagerSpellSlotProgression();      // 血怒者
                PatchAlchemistSpellSlotProgression();       // 炼金术士
                PatchPaladinSpellSlotProgression();         // 圣武士
                PatchMagicDeceiverSpellSlotProgression(); 
               
            }

            /// <summary>
            /// 通用的混合施法者法术进度补丁
            /// 将6环施法者扩展为7环施法者
            /// </summary>
            /// <param name="hybridCasterSlots">职业的法术位表</param>
            private static void PatchHybridCasterSpellProgression(BlueprintSpellsTable hybridCasterSlots)
            {
                // 复制原版法术位列表（可变副本）
                List<SpellsLevelEntry> levels = new List<SpellsLevelEntry>(hybridCasterSlots.Levels);
                
                // 添加5个额外等级，逐步获得7环法术位
                // Count数组索引说明：
                // 索引0-6: 0环-6环法术位
                // 索引7: 7环法术位
                var additionalSlotTables = new List<int[]>
                {
                    new[] {0, 5, 5, 5, 5, 5, 5, 1},  // 获得1个7环法术位
                    new[] {0, 5, 5, 5, 5, 5, 5, 2},  // 获得2个7环法术位
                    new[] {0, 5, 5, 5, 5, 5, 5, 3},  // 获得3个7环法术位
                    new[] {0, 5, 5, 5, 5, 5, 5, 4},  // 获得4个7环法术位
                    new[] {0, 5, 5, 5, 5, 5, 5, 5},  // 获得5个7环法术位
                };
                
                // 将新等级添加到法术位表
                levels.AddRange(additionalSlotTables.Select(slots => new SpellsLevelEntry {Count = slots}));
                hybridCasterSlots.Levels = levels.ToArray();
            }


            private static void PatchMagicDeceiverSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["MagicDeceiver"]) return;
                
                var MagicDeceiverSpellSlots = Resources.SpellTableBlueprints.BardSpellsTable;
                PatchHybridCasterSpellProgression(MagicDeceiverSpellSlots);
                Main.Log($"Patched MagicDeceiver Spell Levels to {MagicDeceiverSpellSlots.Levels.Length}");
            }
            
            /// <summary>
            /// 审判官法术进度补丁
            /// 原版：6环施法者
            /// Mod后：7环施法者
            /// </summary>
            private static void PatchInquisitorSpellSlotProgression()
            {
                // 检查用户配置是否启用了审判官补丁
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["Inquisitor"]) return;
                
                var inquisitorSpellSlots = Resources.SpellTableBlueprints.InquisitorSpellsTable;
                PatchHybridCasterSpellProgression(inquisitorSpellSlots);
                Main.Log($"Patched Inquisitor Spell Levels to {inquisitorSpellSlots.Levels.Length}");
            }

            /// <summary>
            /// 战斗祭司法术进度补丁
            /// 原版：6环施法者
            /// Mod后：7环施法者
            /// </summary>
            private static void PatchWarPriestSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["WarPriest"]) return;
                
                var warPriestSpellSlots = Resources.SpellTableBlueprints.WarPriestSpellsTable;
                PatchHybridCasterSpellProgression(warPriestSpellSlots);
                Main.Log($"Patched WarPriest Spell Levels to {warPriestSpellSlots.Levels.Length}");
            }
            
            /// <summary>
            /// 诗人法术进度补丁
            /// 原版：6环施法者
            /// Mod后：7环施法者
            /// 注意：猎人也使用相同的法术位表
            /// </summary>
            private static void PatchBardSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["Bard"]) return;
                
                var bardSpellSlots = Resources.SpellTableBlueprints.BardSpellsTable;
                PatchHybridCasterSpellProgression(bardSpellSlots);
                Main.Log($"Patched Bard Spell Levels to {bardSpellSlots.Levels.Length}");
            }
            
            /// <summary>
            /// 魔战士法术进度补丁
            /// 原版：6环施法者
            /// Mod后：7环施法者
            /// </summary>
            private static void PatchMagusSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["Magus"]) return;
                
                var magusSpellSlots = Resources.SpellTableBlueprints.MagusSpellsTable;
                PatchHybridCasterSpellProgression(magusSpellSlots);
                Main.Log($"Patched Magus Spell Levels to {magusSpellSlots.Levels.Length}");
            }
            
            /// <summary>
            /// 歌者法术进度补丁
            /// 原版：6环施法者
            /// Mod后：7环施法者
            /// </summary>
            private static void PatchSkaldSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["Skald"]) return;
                
                var skaldSpellSlots = Resources.SpellTableBlueprints.SkaldSpellsTable;
                PatchHybridCasterSpellProgression(skaldSpellSlots);
                Main.Log($"Patched Skald Spell Levels to {skaldSpellSlots.Levels.Length}");
            }

            /// <summary>
            /// 剑圣法术进度补丁
            /// 原版：6环施法者
            /// Mod后：7环施法者
            /// 特殊：剑圣每级只有4个基础法术位（魔战士为5个），平衡性调整
            /// </summary>
            private static void PatchSwordSaintSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["SwordSaint"]) return;
                
                var swordSaintSpellSlots = Resources.SpellTableBlueprints.SwordSaintSpellsTable;
                List<SpellsLevelEntry> levels = new List<SpellsLevelEntry>(swordSaintSpellSlots.Levels);
                
                // 剑圣每级4个基础法术位（而非5个），保持职业特色
                var additionalSlotTables = new List<int[]>
                {
                    new[] {0, 4, 4, 4, 4, 4, 4, 1},  // 获得1个7环
                    new[] {0, 4, 4, 4, 4, 4, 4, 2},  // 获得2个7环
                    new[] {0, 4, 4, 4, 4, 4, 4, 3},  // 获得3个7环
                    new[] {0, 4, 4, 4, 4, 4, 4, 4},  // 获得4个7环
                };
                levels.AddRange(additionalSlotTables.Select(slots => new SpellsLevelEntry {Count = slots}));

                swordSaintSpellSlots.Levels = levels.ToArray();
                Main.Log($"Patched SwordSaint Spell Levels to {swordSaintSpellSlots.Levels.Length}");
            }
            
            /// <summary>
            /// 血怒者法术进度补丁
            /// 原版：4环施法者
            /// Mod后：7环施法者
            /// 特殊：渐进式获得新环位，精细平衡调整
            /// </summary>
            private static void PatchBloodRagerSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["BloodRager"]) return;
                
                var bloodRagerSpellSlots = Resources.SpellTableBlueprints.BloodRagerSpellsTable;
                List<SpellsLevelEntry> levels = new List<SpellsLevelEntry>(bloodRagerSpellSlots.Levels);
                
                // 血怒者渐进式法术位增长
                // Count数组索引说明：
                // [0-6]: 0-6环法术位, [7]: 7环法术位
                var additionalSlotTables = new List<int[]>
                {
                    new[] {0, 4, 4, 3, 3, 1},                              // 等级21: 获得5环
                    new[] {0, 4, 4, 4, 3, 2},                              // 等级22: 强化5环
                    new[] {0, 4, 4, 4, 4, 3, 1},                           // 等级23: 获得6环
                    new[] {0, 4, 4, 4, 4, 4, 2},                           // 等级24: 强化6环
                    new[] {0, 4, 4, 4, 4, 4, 3, 1},                        // 等级25: 获得7环
                    new[] {0, 4, 4, 4, 4, 4, 4, 2},                        // 等级26: 强化7环
                    new[] {0, 4, 4, 4, 4, 4, 4, 4},                        // 等级27: 满7环
                };
                levels.AddRange(additionalSlotTables.Select(slots => new SpellsLevelEntry {Count = slots}));
                bloodRagerSpellSlots.Levels = levels.ToArray();
                Main.Log($"Patched BloodRager Spell Levels to {bloodRagerSpellSlots.Levels.Length}");
            }
            
            /// <summary>
            /// 炼金术士法术进度补丁
            /// 原版：6环施法者
            /// Mod后：7环施法者
            /// </summary>
            private static void PatchAlchemistSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["Alchemist"]) return;
                
                var alchemistSpellSlots = Resources.SpellTableBlueprints.AlchemistSpellsTable;
                PatchHybridCasterSpellProgression(alchemistSpellSlots);
                Main.Log($"Patched Alchemist Spell Levels to {alchemistSpellSlots.Levels.Length}");
            }

            /// <summary>
            /// 圣武士法术进度补丁
            /// 原版：4环施法者
            /// Mod后：10环施法者（完整施法能力）
            /// 这是最复杂的补丁，因为圣武士需要从4环扩展到10环
            /// </summary>
            private static void PatchPaladinSpellSlotProgression()
            {
                if (!Main.ModSettings.PatchSettings.SpellProgressionPatches["Paladin"]) return;
                
                var paladinSpellSlots = Resources.SpellTableBlueprints.PaladinSpellsTable;
                
                // 第一步：修改原版最后几级的法术位（等级19-21）
                // 索引说明：Levels[18] = 等级19, Levels[19] = 等级20, Levels[20] = 等级21
                paladinSpellSlots.Levels[18].Count = new[] {0, 4, 4, 4, 4, 2, 2};      // 等级19: 获得6环
                paladinSpellSlots.Levels[19].Count = new[] {0, 4, 4, 4, 4, 4, 4};      // 等级20: 满6环
                paladinSpellSlots.Levels[20].Count = new[] {0, 4, 4, 4, 4, 4, 4, 2};   // 等级21: 获得7环
 
                List<SpellsLevelEntry> levels = new List<SpellsLevelEntry>(paladinSpellSlots.Levels);
                
                // 第二步：添加更多等级，逐步获得8、9、10环
                // Count数组索引：
                // 0-6: 0-6环, 7:7环, 8:8环, 9:9环, 10:10环
                var additionalSlotTables = new List<int[]>
                {
                    new[] {0, 4, 4, 4, 4, 4, 4, 4},                         // 等级22: 满7环
                    new[] {0, 4, 4, 4, 4, 4, 4, 4, 2},                      // 等级23: 获得8环
                    new[] {0, 4, 4, 4, 4, 4, 4, 4, 4},                      // 等级24: 满8环
                    new[] {0, 4, 4, 4, 4, 4, 4, 4, 4, 2},                   // 等级25: 获得9环
                    new[] {0, 4, 4, 4, 4, 4, 4, 4, 4, 4},                   // 等级26: 满9环
                    new[] {0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2},                // 等级27: 获得10环
                    new[] {0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},                // 等级28: 满10环
                };
                levels.AddRange(additionalSlotTables.Select(slots => new SpellsLevelEntry {Count = slots}));
                paladinSpellSlots.Levels = levels.ToArray();
                Main.Log($"Patched Paladin Spell Levels to {paladinSpellSlots.Levels.Length}");
            }
        }
    }
}
