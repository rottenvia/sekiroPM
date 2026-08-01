namespace SekiroParamMerger.Core
{
    /// <summary>
    /// Human-readable descriptions for param names and cell field names.
    /// Used by the conflict resolver UI to explain what each conflict means
    /// in plain language instead of raw internal names.
    /// </summary>
    public static class ParamDescriptions
    {
        // ─────────────────────────────────────────────────────────────────────
        // Param Categories — what each param file does in plain English
        // ─────────────────────────────────────────────────────────────────────
        private static readonly Dictionary<string, string> ParamCategories = new()
        {
            ["SpEffectParam"]              = "Status Effects & Buffs",
            ["SpEffectVfxParam"]           = "Status Effect Visuals",
            ["AtkParam_Pc"]                = "Player Attack Values",
            ["AtkParam_Npc"]               = "Enemy Attack Values",
            ["NpcParam"]                   = "Enemy Stats & Behaviour",
            ["NpcThinkParam"]              = "Enemy AI Patterns",
            ["EquipParamWeapon"]           = "Weapon Stats",
            ["EquipParamProtector"]        = "Armour Stats",
            ["EquipParamGoods"]            = "Item & Consumable Stats",
            ["EquipParamAccessory"]        = "Accessory Stats",
            ["MoveParam"]                  = "Movement & Speed",
            ["BehaviorParam"]              = "Player Behaviour Patterns",
            ["BehaviorParam_PC"]           = "Player Combat Behaviours",
            ["BulletParam"]                = "Projectile Stats",
            ["ThrowParam"]                 = "Grab Attack Stats",
            ["ThrowKindParam"]             = "Grab Attack Types",
            ["ThrowDirectionSfxParam"]     = "Grab Attack Sound Effects",
            ["ThrowDirectionSeParam"]      = "Grab Attack Sound Events",
            ["ThrowDirectionDecalParam"]   = "Grab Attack Decals",
            ["ItemLotParam"]               = "Item Drop Tables",
            ["ResourceItemLotParam"]       = "Resource Drop Tables",
            ["ShopLineupParam"]            = "Merchant Shop Items",
            ["GameAreaParam"]              = "Boss Rewards & Echoes",
            ["CalcCorrectGraph"]           = "Damage Scaling Curves",
            ["ReinforceParamWeapon"]       = "Weapon Upgrade Values",
            ["ReinforceParamProtector"]    = "Armour Upgrade Values",
            ["AttackElementCorrectParam"]  = "Elemental Damage Scaling",
            ["HitMtrlParam"]               = "Surface Hit Properties",
            ["HitEffectSeParam"]           = "Hit Sound Effects",
            ["HitEffectSfxParam"]          = "Hit Visual Effects",
            ["HitEffectSfxConceptParam"]   = "Hit Effect Concepts",
            ["KnockBackParam"]             = "Knockback Properties",
            ["StaminaControlParam"]        = "Stamina Regeneration",
            ["LockCamParam"]               = "Lock-on Camera Settings",
            ["CameraParam"]                = "Camera Behaviour",
            ["CameraSetParam"]             = "Camera Position Sets",
            ["CharaInitParam"]             = "Character Starting Stats",
            ["SkillParam"]                 = "Combat Arts Stats",
            ["SwordArtsParam"]             = "Sword Arts Definitions",
            ["RoleParam"]                  = "Player Role Stats",
            ["MapPartsParam"]              = "Map Object Properties",
            ["ObjectParam"]                = "World Object Properties",
            ["ObjActParam"]                = "Object Action Properties",
            ["GrassTypeParam"]             = "Grass Rendering Types",
            ["GrassLodRangeParam"]         = "Grass Draw Distance",
            ["DecalParam"]                 = "Surface Decal Properties",
            ["FootSfxParam"]               = "Footstep Sound Effects",
            ["ModelSfxParam"]              = "Character Sound Effects",
            ["TalkParam"]                  = "NPC Dialogue Triggers",
            ["GameProgressParam"]          = "Game Progress Flags",
            ["BonfireWarpParam"]           = "Sculptor Idol Warp Points",
            ["ClearCountCorrectParam"]     = "New Game Plus Scaling",
            ["MultiPlayCorrectionParam"]   = "Multiplayer Corrections",
            ["NetworkAreaParam"]           = "Multiplayer Zone Areas",
            ["NetworkMsgParam"]            = "Multiplayer Messages",
            ["EquipMtrlSetParam"]          = "Upgrade Material Sets",
            ["GemCategoryParam"]           = "Prosthetic Tool Categories",
            ["GemGenParam"]                = "Prosthetic Tool Generation",
            ["GemeffectParam"]             = "Prosthetic Tool Effects",
            ["GemDropDopingParam"]         = "Prosthetic Drop Bonuses",
            ["GemDropModifyParam"]         = "Prosthetic Drop Modifiers",
            ["FaceParam"]                  = "Character Face Parameters",
            ["WireSetParam"]               = "Grappling Hook Wire Sets",
            ["WirePointSearchParam"]       = "Grappling Hook Search",
            ["WireVariationParam"]         = "Grappling Hook Variations",
            ["ChrPhysicsHomingParam"]      = "Character Physics Homing",
            ["RagdollParam"]               = "Ragdoll Physics",
            ["CultSettingParam"]           = "Remnant Settings",
            ["PhantomParam"]               = "Remnant Properties",
            ["LoadBalancerParam"]          = "Performance Load Balancer",
            ["BudgetParam"]                = "Resource Budget Limits",
            ["MenuPropertyLayoutParam"]    = "Menu Layout Properties",
            ["MenuPropertySpecParam"]      = "Menu Specification",
            ["MenuTutorialParam"]          = "Tutorial Messages",
            ["MenuColorTableParam"]        = "Menu Color Definitions",
            ["NewMenuColorTableParam"]     = "Updated Menu Colors",
            ["KnowledgeLoadScreenItemParam"] = "Loading Screen Tips",
            ["ActionButtonParam"]          = "Interaction Button Actions",
            ["ActionGuideParam"]           = "On-screen Action Guides",
            ["ActionUnlockParam"]          = "Action Unlock Conditions",
            ["DefaultKeyAssignParam00"]    = "Default Keyboard Layout (PC)",
            ["DefaultKeyAssignParam01"]    = "Default Controller Layout 1",
            ["DefaultKeyAssignParam02"]    = "Default Controller Layout 2",
            ["DefaultKeyAssignParam03"]    = "Default Controller Layout 3",
            ["DefaultKeyAssignParam04"]    = "Default Controller Layout 4",
        };

        // ─────────────────────────────────────────────────────────────────────
        // Cell Field Descriptions — what each internal field name means
        // ─────────────────────────────────────────────────────────────────────
        private static readonly Dictionary<string, string> CellDescriptions = new()
        {
            // Attack & Damage
            ["damage"]                  = "Base damage dealt",
            ["guard_damage"]            = "Damage dealt through guard",
            ["stamina_damage"]          = "Stamina damage dealt",
            ["stamina_guard_damage"]    = "Stamina damage dealt through guard",
            ["correctRate"]             = "Scaling correction rate",
            ["atkPhysicsCorrect"]       = "Physical attack correction",
            ["atkMagicCorrect"]         = "Magic attack correction",
            ["atkFireCorrect"]          = "Fire attack correction",
            ["atkThunderCorrect"]       = "Lightning attack correction",
            ["atkDarkCorrect"]          = "Dark attack correction",
            ["atkPoisonCorrect"]        = "Poison buildup correction",
            ["atkBloodCorrect"]         = "Bleed buildup correction",
            ["atkPetrifyCorrect"]       = "Petrify buildup correction",
            ["atkFrostCorrect"]         = "Frostbite buildup correction",

            // Status Effects
            ["effectEndurance"]         = "How long this effect lasts (frames)",
            ["motionInterval"]          = "Time between effect ticks (frames)",
            ["registIllness"]           = "Poison buildup resistance threshold",
            ["registBlood"]             = "Bleed buildup resistance threshold",
            ["registPoison"]            = "Poison resistance",
            ["registFrost"]             = "Frostbite resistance threshold",
            ["registSleep"]             = "Sleep resistance threshold",
            ["registMadness"]           = "Madness resistance threshold",
            ["poizonAttackPower"]        = "Poison damage per tick",
            ["diseaseAttackPower"]      = "Rot damage per tick",
            ["bloodAttackPower"]        = "Bleed damage per tick",
            ["freezeAttackPower"]       = "Frostbite damage per tick",
            ["maxHpRate"]               = "Max HP multiplier",
            ["maxMpRate"]               = "Max Stamina multiplier",
            ["maxStaminaCutRate"]       = "Max stamina reduction rate",
            ["addLifeForce"]            = "Flat HP added",
            ["addStrength"]             = "Flat Strength added",
            ["addAgility"]              = "Flat Agility added",
            ["addMagic"]                = "Flat Magic added",
            ["addFaith"]                = "Flat Faith added",
            ["physicsDefRate"]          = "Physical defense multiplier",
            ["magicDefRate"]            = "Magic defense multiplier",
            ["fireDefRate"]             = "Fire defense multiplier",
            ["thunderDefRate"]          = "Lightning defense multiplier",
            ["slashDamageCutRate"]      = "Slash damage reduction",
            ["blowDamageCutRate"]       = "Strike damage reduction",
            ["thrustDamageCutRate"]     = "Thrust damage reduction",
            ["neutralDamageCutRate"]    = "Neutral damage reduction",

            // Movement & Speed
            ["walkSpeed"]               = "Walking speed",
            ["runSpeed"]                = "Running speed",
            ["dashSpeed"]               = "Dash speed",
            ["jumpHeightRate"]          = "Jump height multiplier",
            ["staminaRecoverRate"]      = "Stamina recovery rate",
            ["staminaAttack"]           = "Stamina cost of action",
            ["moveType"]                = "Movement type identifier",
            ["turnVellocityRate"]       = "Turning speed rate",

            // Enemy / NPC
            ["hp"]                      = "Base health points",
            ["mp"]                      = "Base stamina points",
            ["getSoul"]                 = "Echoes (souls) dropped on death",
            ["stamina"]                 = "Base stamina",
            ["baseVitality"]            = "Base vitality",
            ["baseEndurance"]           = "Base endurance",
            ["baseStrength"]            = "Base strength",
            ["baseDexterity"]           = "Base dexterity",
            ["baseIntelligence"]        = "Base intelligence",
            ["baseFaith"]               = "Base faith",
            ["itemLotId_map"]           = "Item drop table ID (map)",
            ["itemLotId_enemy"]         = "Item drop table ID (enemy)",
            ["defPhysics"]              = "Physical defense",
            ["defMagic"]                = "Magic defense",
            ["defFire"]                 = "Fire defense",
            ["defThunder"]              = "Lightning defense",
            ["defSlash"]                = "Slash defense",
            ["defBlow"]                 = "Strike defense",
            ["defThrust"]               = "Thrust defense",
            ["guardPhysics"]            = "Guard physical absorption",
            ["guardMagic"]              = "Guard magic absorption",
            ["guardFire"]               = "Guard fire absorption",
            ["guardThunder"]            = "Guard lightning absorption",
            ["guardStamina"]            = "Guard stamina cost",
            ["guardCutRate"]            = "Guard damage cut rate",
            ["toughness"]               = "Poise / toughness value",
            ["toughnessRecoverPoint"]   = "Poise recovery amount",

            // Weapons
            ["weight"]                  = "Item weight",
            ["weaponCategory"]          = "Weapon category type",
            ["properStrength"]          = "Required Strength",
            ["properAgility"]           = "Required Dexterity",
            ["properMagic"]             = "Required Intelligence",
            ["properFaith"]             = "Required Faith",
            ["physicsAtkRate"]          = "Physical attack rate multiplier",
            ["magicAtkRate"]            = "Magic attack rate multiplier",
            ["fireAtkRate"]             = "Fire attack rate multiplier",
            ["thunderAtkRate"]          = "Lightning attack rate multiplier",
            ["staminaAttackRate"]       = "Stamina attack rate multiplier",
            ["reinforceTypeId"]         = "Upgrade path type ID",
            ["originEquipWep"]          = "Base weapon ID (upgrade origin)",
            ["correctStrength"]         = "Strength scaling value",
            ["correctAgility"]          = "Dexterity scaling value",
            ["correctMagic"]            = "Intelligence scaling value",
            ["correctFaith"]            = "Faith scaling value",
            ["attackBasePhysics"]       = "Base physical attack",
            ["attackBaseMagic"]         = "Base magic attack",
            ["attackBaseFire"]          = "Base fire attack",
            ["attackBaseThunder"]       = "Base lightning attack",
            ["guardAbsorption"]         = "Guard absorption value",

            // Items / Goods
            ["maxNum"]                  = "Maximum carry quantity",
            ["goodsType"]               = "Item type identifier",
            ["refId_default"]           = "Reference ID (default)",
            ["sfxVariationId"]          = "Sound effect variation ID",
            ["vagrantItemLotId"]        = "Vagrant item lot ID",
            ["vagrantBonusEneDropItemLotId"] = "Vagrant bonus drop lot ID",

            // Camera
            ["camDistTarget"]           = "Camera target distance",
            ["rotRangeMin"]             = "Minimum rotation range",
            ["rotRangeMax"]             = "Maximum rotation range",
            ["fov"]                     = "Field of view angle",

            // Grab / Throw
            ["throwTypeId"]             = "Grab attack type ID",
            ["atkId"]                   = "Attack data ID",
            ["thrAttackId"]             = "Throw attack ID",
            ["escapeThrowDmgRate"]      = "Escape throw damage rate",

            // Stamina
            ["staminaConsume"]          = "Stamina consumed by action",
            ["recoveryTime"]            = "Stamina recovery time",

            // Misc / Padding (shown but noted as technical)
            ["pad"]                     = "Padding field (technical — safe to ignore)",
            ["pad0"]                    = "Padding field (technical — safe to ignore)",
            ["pad1"]                    = "Padding field (technical — safe to ignore)",
            ["pad2"]                    = "Padding field (technical — safe to ignore)",
            ["reserve"]                 = "Reserved field (technical — safe to ignore)",
        };

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a human-readable category name for a param.
        /// Falls back to the raw param name if not in dictionary.
        /// </summary>
        public static string GetParamCategory(string paramName)
        {
            return ParamCategories.TryGetValue(paramName, out string? category)
                ? category
                : paramName;
        }

        /// <summary>
        /// Returns a human-readable description for a cell field name.
        /// Falls back to a generic message if not in dictionary.
        /// </summary>
        public static string GetCellDescription(string cellName)
        {
            return CellDescriptions.TryGetValue(cellName, out string? desc)
                ? desc
                : $"Technical field: {cellName}";
        }

        /// <summary>
        /// Describes the direction of change between two values.
        /// e.g. "increased from 40 to 60" or "decreased from 60 to 40"
        /// </summary>
        public static string DescribeChange(object? vanillaVal, object? modVal)
        {
            if (vanillaVal == null || modVal == null)
                return $"changed to {modVal ?? "null"}";

            // Try numeric comparison for meaningful direction description
            if (TryGetDouble(vanillaVal, out double vNum) && TryGetDouble(modVal, out double mNum))
            {
                if (mNum > vNum)
                    return $"increased from {vanillaVal} to {modVal} (+{mNum - vNum})";
                if (mNum < vNum)
                    return $"decreased from {vanillaVal} to {modVal} ({mNum - vNum})";
                return $"unchanged ({modVal})";
            }

            // Non-numeric — just show changed
            return $"changed from {vanillaVal} to {modVal}";
        }

        /// <summary>
        /// Checks if this cell is a padding/technical field that users should not worry about.
        /// </summary>
        public static bool IsPaddingField(string cellName)
        {
            return cellName.StartsWith("pad", StringComparison.OrdinalIgnoreCase)
                || cellName.StartsWith("reserve", StringComparison.OrdinalIgnoreCase)
                || cellName.StartsWith("dummy", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryGetDouble(object val, out double result)
        {
            try
            {
                result = Convert.ToDouble(val);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }
    }
}
