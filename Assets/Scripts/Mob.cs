using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum AttemptMoveResultEnum
{
    moveClear, moveBlockedbyMob, moveBlockedByTerrain, moveOutOfBounds
}

public struct AttemptMoveResult
{
    public AttemptMoveResultEnum result;
    public TerrainType terrain;
    public Mob mob;

    public AttemptMoveResult(AttemptMoveResultEnum _result, TerrainType _terrain, Mob _mob)
    {
        result = _result;
        terrain = _terrain;
        mob = _mob;
    }
}

public delegate string dmg_string(int dmg);

public class Mob  
{
    public MobTypeEnum idType;
    public int id = 0;
    public int x = 0;
    public int y = 0;

    public int curHP = 1;
    public int maxHP
    {
        get { return MobTypes.mobTypes[idType].maxHP; }
    }
    public int curFP = 1;
    public int maxFP
    {
        get { return MobTypes.mobTypes[idType].maxFP; }
    }
    public int curSH = 0;
    public int regenHP = 1;
    public int regenHPBase
    {
        get { return MobTypes.mobTypes[idType].regenHP; }
    }
    public int regenFP = 1;
    public int regenFPBase
    {
        get { return MobTypes.mobTypes[idType].regenFP; }
    }
    public int visionRadius = 10;
    public int curWP = 0;
    public const int MAX_WP = 200;

    public string name;
    public bool hasPersonalName;

    public string typeName
    {
        get
        {
            return MobTypes.mobTypes[idType].name;
        }
    }

    public bool alreadyDied;
    public string killedBy;
    public float curAP;
    public float curMoveSpeed;

    public GameObject go;
    public MovingObject mo;

    public List<Mob> visibleMobs;
    public List<Item> visibleItems;
    public List<Feature> visibleFeatures;

    public Vector2Int pathDst;

    public Dictionary<AbilityTypeEnum, bool> abilities;
    public List<AbilityTypeEnum> curAbils;
    public AbilityTypeEnum meleeAbil;
    public AbilityTypeEnum rangedAbil;
    public AbilityTypeEnum dodgeAbil;
    public AbilityTypeEnum blockAbil;

    public Dictionary<DmgTypeEnum,int> armorPR;
    public Dictionary<DmgTypeEnum, int> armorDR;

    public Dictionary<EffectTypeEnum, Effect> effects;

    public List<Vector2Int> path;

    public List<AiPackageEnum> aiPackages
    {
        get { return MobTypes.mobTypes[idType].aiPackages; }
    }

    public FactionEnum faction
    {
        get
        {
            if (GetEffect(EffectTypeEnum.effectDominateMind) != null) return GetEffect(EffectTypeEnum.effectDominateMind).actor.faction;
            else return MobTypes.mobTypes[idType].faction;
        }
    }

    public Mob(MobTypeEnum _idType, int _x, int _y)
    {
        idType = _idType;
        x = _x;
        y = _y;
        if (MobTypes.mobTypes != null)
        {
            go = GameObject.Instantiate(MobTypes.mobTypes[idType].prefab, new Vector3(x, y, 0f), Quaternion.identity);
            mo = go.GetComponent<MovingObject>();
        }

        visibleMobs = new List<Mob>();
        visibleItems = new List<Item>();
        visibleFeatures = new List<Feature>();

        abilities = new Dictionary<AbilityTypeEnum, bool>();
        foreach (AbilityTypeEnum abilityType in MobTypes.mobTypes[idType].abilities)
            abilities.Add(abilityType, true);

        curAbils = new List<AbilityTypeEnum>();
        for (int i = 0; i < 10; i++)
            curAbils.Add(AbilityTypeEnum.abilNone);

        name = MobTypes.mobTypes[idType].name;

        meleeAbil = MobTypes.mobTypes[idType].meleeAbil;
        rangedAbil = MobTypes.mobTypes[idType].rangedAbil;
        dodgeAbil = MobTypes.mobTypes[idType].dodgeAbil;
        blockAbil = MobTypes.mobTypes[idType].blockAbil;

        if (meleeAbil != AbilityTypeEnum.abilNone && !abilities.ContainsKey(meleeAbil)) abilities.Add(meleeAbil, true);
        if (rangedAbil != AbilityTypeEnum.abilNone && !abilities.ContainsKey(rangedAbil)) abilities.Add(rangedAbil, true);
        if (dodgeAbil != AbilityTypeEnum.abilNone && !abilities.ContainsKey(dodgeAbil)) abilities.Add(dodgeAbil, true);
        if (blockAbil != AbilityTypeEnum.abilNone && !abilities.ContainsKey(blockAbil)) abilities.Add(blockAbil, true);

        effects = new Dictionary<EffectTypeEnum, Effect>();

        armorDR = new Dictionary<DmgTypeEnum, int>();
        foreach (DmgTypeEnum dmgType in MobTypes.mobTypes[idType].armorDR.Keys)
        {
            armorDR[dmgType] = MobTypes.mobTypes[idType].armorDR[dmgType];
        }
        armorPR = new Dictionary<DmgTypeEnum, int>();
        foreach (DmgTypeEnum dmgType in MobTypes.mobTypes[idType].armorPR.Keys)
        {
            armorPR[dmgType] = MobTypes.mobTypes[idType].armorPR[dmgType];
        }

        hasPersonalName = false;
        if (abilities.ContainsKey(AbilityTypeEnum.abilNamed))
        {
            if (abilities.ContainsKey(AbilityTypeEnum.abilAngel))
            {
                int r = UnityEngine.Random.Range(0, Nemesis.angelNames.Count);
                name = Nemesis.angelNames[r];
                Nemesis.angelNames.RemoveAt(r);
                hasPersonalName = true;
            }
            else if (abilities.ContainsKey(AbilityTypeEnum.abilDemon))
            {
                int r = UnityEngine.Random.Range(0, Nemesis.demonNames.Count);
                name = Nemesis.demonNames[r];
                Nemesis.demonNames.RemoveAt(r);
                hasPersonalName = true;
            }
        }

        if (GetAbility(AbilityTypeEnum.abilAngel) != null)
            GetAbility(AbilityTypeEnum.abilAngel).AbilityInvoke(this, new TargetStruct());

        path = new List<Vector2Int>();
        pathDst = new Vector2Int(x, y);
        curHP = maxHP;
        curFP = maxFP;
        regenFP = regenFPBase;
        regenHP = regenHPBase;
        curAP = MobType.NORMAL_AP;
        curMoveSpeed = MobTypes.mobTypes[idType].moveSpeed;

        CalculateArmor();
    }
    
    public virtual void AiFunction()
    {
        //Debug.Log("AiFuncation " + name + " [" + id + "]");
        GetFOV();
        visibleMobs.Remove(this);

        List<Mob> allies = new List<Mob>();
        List<Mob> enemies = new List<Mob>();
        Mob nearestEnemy = null;
        Mob nearestAlly = null;

        foreach (Mob mob in visibleMobs)
        { 
            if (!GetFactionRelation(mob.faction)) enemies.Add(mob);
            else allies.Add(mob);
        }

        foreach (Mob mob in enemies)
        {
            if (nearestEnemy == null) nearestEnemy = mob;
            if (Level.GetDistance(mob.x, mob.y, this.x, this.y) <
                Level.GetDistance(nearestEnemy.x, nearestEnemy.y, this.x, this.y))
                nearestEnemy = mob;
        }

        foreach (Mob mob in allies)
        {
            if (nearestAlly == null) nearestAlly = mob;
            if (Level.GetDistance(mob.x, mob.y, this.x, this.y) <
                Level.GetDistance(nearestAlly.x, nearestAlly.y, this.x, this.y))
                nearestAlly = mob;
        }

        if (GetEffect(EffectTypeEnum.effectFear) != null && nearestEnemy != null)
        {
            int dx = this.x - nearestEnemy.x, dy = this.y - nearestEnemy.y;

            if (dx > 0) dx = 1;
            else if (dx < 0) dx = -1;

            if (dy > 0) dy = 1;
            else if (dy < 0) dy = -1;

            if (CanMoveToPos(this.x + dx, this.y + dy).result == AttemptMoveResultEnum.moveClear)
            {
                Move(dx, dy);
                pathDst.x = this.x;
                pathDst.y = this.y;
                path.Clear();
                return;
            }
            else
            {
                MakeRandomMove();
                pathDst.x = this.x;
                pathDst.y = this.y;
                path.Clear();
                return;
            }

        }

        Dictionary<int, List<AiPackageEnum>> sortedAiPackages = new Dictionary<int, List<AiPackageEnum>>();
        List<AiPackageEnum> value;

        foreach (AiPackageEnum ai in aiPackages)
        {
            if (!sortedAiPackages.TryGetValue(AIs.aiPackages[ai].priority, out value)) sortedAiPackages[AIs.aiPackages[ai].priority] = new List<AiPackageEnum>();
            sortedAiPackages[AIs.aiPackages[ai].priority].Add(ai);
        }

        for (int priority = 9; priority >= 1; priority--)
        {
            if (sortedAiPackages.TryGetValue(priority, out value))
            {
                
                for (int i = sortedAiPackages[priority].Count - 1; i >= 0; i--)
                {
                    //Debug.Log(sortedAiPackages[priority][i].ToString());
                    if (!AIs.aiPackages[sortedAiPackages[priority][i]].OnCheckAI(this, nearestEnemy, nearestAlly, enemies, allies))
                        sortedAiPackages[priority].RemoveAt(i);
                }
                if (sortedAiPackages[priority].Count > 0)
                {
                    int r = UnityEngine.Random.Range(0, sortedAiPackages[priority].Count - 1);
                    
                    AIs.aiPackages[sortedAiPackages[priority][r]].OnInvokeAI(this, nearestEnemy, nearestAlly, enemies, allies);
                    return;
                }
            }
        }

        MakeRandomMove();

    }

    public virtual void GetFOV()
    {
        Level level = BoardManager.instance.level;

        visibleMobs.Clear();
        visibleItems.Clear();
        visibleFeatures.Clear();
        LOS_FOV.DrawFOV(x, y, visionRadius,
            (int dx, int dy, int pdx, int pdy) =>
            {
                if (level.mobs[dx, dy] != null && !visibleMobs.Contains(level.mobs[dx, dy]) && level.mobs[dx, dy].GetEffect(EffectTypeEnum.effectInvisibility) == null)
                {
                    visibleMobs.Add(level.mobs[dx, dy]);
                }

                if (level.items[dx,dy].Count > 0)
                {
                    foreach (Item item in level.items[dx, dy])
                    {
                        if (!visibleItems.Contains(item)) visibleItems.Add(item);
                    }
                }
                if (level.features[dx, dy].Count > 0)
                {
                    foreach (Feature feature in level.features[dx, dy])
                    {
                        if (!visibleFeatures.Contains(feature)) visibleFeatures.Add(feature);
                    }
                }

                if (TerrainTypes.terrainTypes[level.terrain[dx, dy]].blocksVision) return false;
                return true;
            });
    }

    public AttemptMoveResult CanMoveToPos(int x, int y)
    {
        Level level = BoardManager.instance.level;

        if ((x < 0) || (y < 0) || (x >= level.maxX) || (y >= level.maxY))
        {
            return new AttemptMoveResult(AttemptMoveResultEnum.moveOutOfBounds, null, null);
        }

        if (TerrainTypes.terrainTypes[level.terrain[x, y]].blocksMovement)
        {
            return new AttemptMoveResult(AttemptMoveResultEnum.moveBlockedByTerrain, TerrainTypes.terrainTypes[level.terrain[x, y]], null);
        }

        if (level.mobs[x, y] != null && level.mobs[x, y] != this)
        {
            return new AttemptMoveResult(AttemptMoveResultEnum.moveBlockedbyMob, null, level.mobs[x, y]);
        }

        return new AttemptMoveResult(AttemptMoveResultEnum.moveClear, null, null);
    }

    public bool Move(int xDir, int yDir)
    {
        if (this == BoardManager.instance.player)
            Debug.Log(String.Format("Player Move ({0}, {1}) + ({2}, {3})", x, y, xDir, yDir));
        if (GetEffect(EffectTypeEnum.effectImmobilize) != null)
        {
            Vector2 end = new Vector2(x, y);
            Level level = BoardManager.instance.level;
            bool visible = level.visible[x, y];

            mo.Move(end, visible,
                () =>
                {
                    SetPosition(x, y);
                });
            MakeAct(curMoveSpeed);
            return true;
        }

        AttemptMoveResult attemptMoveResult = CanMoveToPos(x + xDir, y + yDir);
        bool result = false;

        //if (GameManager.instance.player == this)
        //    Debug.Log("attemptMoveResult = " + attemptMoveResult.result.ToString());

        switch (attemptMoveResult.result)
        {
            case AttemptMoveResultEnum.moveClear:
                Vector2 end = new Vector2(x + xDir, y + yDir);
                Level level = BoardManager.instance.level;
                bool visibleStart = level.visible[x, y];
                bool visibleEnd = level.visible[x + xDir, y + yDir];

                mo.Move(end, (visibleStart || visibleEnd),
                    () =>
                    {
                        SetPosition(x + xDir, y + yDir);
                    });
                MakeAct(curMoveSpeed);
                result = true;
                break;
            case AttemptMoveResultEnum.moveBlockedbyMob:
                OnBump(attemptMoveResult.mob);
                result = true;
                break;
            default:
                result = false;
                break;
        }

        return result;
    }

    public void OnBump(Mob target)
    {
        MeleeDamage(target);
    }

    public void MakeAct(float spd)
    {
        if (GetEffect(EffectTypeEnum.effectSprint) != null)
            curFP -= 20;
        if (curFP <= 0)
        {
            curFP = 0;
            RemoveEffect(EffectTypeEnum.effectSprint);
        }
        curAP -= spd;
        CalculateFPRegen();
    }

    public bool SetPosition(int nx, int ny)
    {
        Level level = BoardManager.instance.level;
        level.mobs[x, y] = null;
        x = nx;
        y = ny;
        level.mobs[x, y] = this;

        // check if there is an onstep function
        if (TerrainTypes.terrainTypes[level.terrain[x,y]].TerrainOnStep != null)
        {
            TerrainTypes.terrainTypes[level.terrain[x, y]].TerrainOnStep(level, this);
        }
        return true;
    }

    public bool CheckVisibilityByPlayer()
    {
        Level level = BoardManager.instance.level;
        PlayerMob player = BoardManager.instance.player;
        if (level.visible[x, y] && player.visibleMobs.Contains(this)) return true;
        else return false;
    }

    public virtual string Description()
    {
        string str = "";
        bool noAbilities;
        str += GetFullName() + "\n";
        str += String.Format("{0}\n", MobTypes.mobTypes[idType].name);
        str += String.Format("HP: {0}/{1}\n", curHP, maxHP);
        str += String.Format("FP: {0}/{1}\n", curFP, maxFP);
        if (GetAbility(AbilityTypeEnum.abilDivineVengeance) != null)
            str += String.Format("WP: {0}\n", curWP);

        bool hasArmor = false;
        str += "\nDamage Reduction (DR):\n";
        foreach (DmgType dmgType in DmgTypes.dmgTypes.Values)
        {
            if (armorDR[dmgType.dmgType] > 0 || armorPR[dmgType.dmgType] > 0)
            {
                hasArmor = true;
                str += String.Format("   {0}: {1} pts/{2}%\n", dmgType.name.Substring(0, 1).ToUpper() + dmgType.name.Substring(1), armorDR[dmgType.dmgType], armorPR[dmgType.dmgType]);
            }
        }
        if (!hasArmor)
            str += "   None.\n";

        str += "\n";
        str += "-----------------------------------\n";
        str += "ACTIVE ABILITIES\n";
        str += "-----------------------------------\n";
        str += "\n";
        noAbilities = true;
        foreach (AbilityTypeEnum abilityType in abilities.Keys)
        {
            Ability ability = GetAbility(abilityType);
            if (ability.id != AbilityTypeEnum.abilNone &&
                (!ability.passive ||
                 (ability.passive && ability.slot == AbilitySlotEnum.abilMelee)))
            {
                str += ability.GetFullDescription(this);
                str += "\n\n";
                noAbilities = false;
            }
        }

        if (noAbilities) str += "No abilities.\n\n";

        str += "\n";
        str += "-----------------------------------\n";
        str += "PASSIVE ABILITIES\n";
        str += "-----------------------------------\n";
        str += "\n";
        noAbilities = true;
        foreach (AbilityTypeEnum abilityType in abilities.Keys)
        {
            Ability ability = GetAbility(abilityType);
            if (ability.passive && ability.slot != AbilitySlotEnum.abilMelee)
            {
                str += GetAbility(abilityType).GetFullDescription(this);
                str += "\n\n";
                noAbilities = false;
            }
        }

        if (noAbilities) str += "No abilities.\n\n";

        str += "\n";
        str += "-----------------------------------\n";
        str += "EFFECTS\n";
        str += "-----------------------------------\n";
        str += "\n";
        noAbilities = true;
        foreach (Effect eff in effects.Values)
        {

            str += String.Format("<color=#{1}>{0}</color>\n", eff.GetEffectFullLine(),
                        ColorUtility.ToHtmlStringRGBA(EffectTypes.effectTypes[eff.idType].color));
            str += String.Format("{0}.", EffectTypes.effectTypes[eff.idType].descr);
            str += "\n\n";
            noAbilities = false;
        }

        if (noAbilities) str += "No active effects.\n\n";

        return str;
    }

    public static int InflictDamage(Mob attacker, Mob target, Dictionary<DmgTypeEnum, int> dmgDict, dmg_string dmg_string, bool createBlood = true)
    {
        string str = "";
        int finaldmg = 0;
        foreach (DmgTypeEnum dmgType in dmgDict.Keys)
        {
            int dmg = dmgDict[dmgType];

            dmg = (int)(dmg * (100f - target.armorPR[dmgType]) / 100);
            dmg -= target.armorDR[dmgType];
            dmg = (dmg < 0) ? 0 : dmg;

            if (attacker != null && dmg > 0 && dmg_string == null)
            {
                str = String.Format("{0} hits {1} for {2} {3} dmg. ",
                    attacker.name,
                    target.name,
                    dmg,
                    DmgTypes.dmgTypes[dmgType].name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(attacker.x, attacker.y, str);
            }
            else if (attacker != null && dmg == 0 && dmg_string == null)
            {
                str = String.Format("{0} hits {1}, but {1} takes no {2} damage. ",
                    attacker.name,
                    target.name,
                    DmgTypes.dmgTypes[dmgType].name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(attacker.x, attacker.y, str);
            }
            else if (attacker == null && dmg > 0 && dmg_string == null)
            {
                str = String.Format("{0} takes {1} {2} dmg. ",
                    target.name,
                    dmg,
                    DmgTypes.dmgTypes[dmgType].name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(target.x, target.y, str);
            }
            else if (attacker == null && dmg == 0 && dmg_string == null)
            {
                str = String.Format("{0} takes no {1} dmg. ",
                    target.name,
                    DmgTypes.dmgTypes[dmgType].name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(target.x, target.y, str);
            }
            else if (dmg_string != null)
            {
                str = dmg_string(dmg);
                BoardManager.instance.msgLog.PlayerVisibleMsg(target.x, target.y, str);
            }

            int curDmg = dmg;
            finaldmg += dmg;
            List<Effect> shieldsToRemove = new List<Effect>();
            foreach (Effect effect in target.effects.Values)
            {
                int SV = effect.SV;
                if (SV > 0)
                {
                    if (curDmg >= SV)
                    {
                        curDmg -= SV;
                        effect.SV = 0;
                        shieldsToRemove.Add(effect);
                    }
                    else
                    {
                        effect.SV -= curDmg;
                        curDmg = 0;
                    }
                }
            }
            for (int i = shieldsToRemove.Count - 1; i >= 0; i--)
            {
                target.effects.Remove(shieldsToRemove[i].idType);
            }
            shieldsToRemove.Clear();
            target.CalculateShieldValue();

            target.curHP -= curDmg;

        }

        // increase WP with Divine Vengeance
        if (attacker != null && attacker.GetAbility(AbilityTypeEnum.abilDivineVengeance) != null)
            attacker.GetAbility(AbilityTypeEnum.abilDivineVengeance).AbilityInvoke(attacker, new TargetStruct(new Vector2Int(attacker.x, attacker.y), attacker));
        if (target.GetAbility(AbilityTypeEnum.abilDivineVengeance) != null)
            target.GetAbility(AbilityTypeEnum.abilDivineVengeance).AbilityInvoke(target, new TargetStruct(new Vector2Int(target.x, target.y), target));

        // remove Blindness
        if (finaldmg > 0 && target.GetEffect(EffectTypeEnum.effectBlindness) != null)
            target.RemoveEffect(EffectTypeEnum.effectBlindness);

        // transfer health from Syphon Light
        if (finaldmg > 0 && target.GetEffect(EffectTypeEnum.effectSyphonLight) != null &&
            attacker != null && target.GetEffect(EffectTypeEnum.effectSyphonLight).actor == attacker)
        {
            attacker.curHP += finaldmg / 2;
            if (target.curHP > target.maxHP) attacker.curHP = attacker.maxHP;
        }

        // invoke Teleport on hit
        if (attacker != null && target.GetAbility(AbilityTypeEnum.abilTeleportOnHit) != null)
        {
            target.InvokeAbility(target.GetAbility(AbilityTypeEnum.abilTeleportOnHit), new TargetStruct(new Vector2Int(0, 0), null));
        }

        if (createBlood && finaldmg > 0)
            BoardManager.instance.CreateBlooddrop(target.x, target.y);

        return finaldmg;
    }

    public bool CheckDead()
    {
        if (curHP <= 0 || alreadyDied) return true;
        else return false;
    }

    public void MakeDead(Mob attacker, bool makeMsg, bool leaveCorpse, bool severLimbs, string killedByStr)
    {
        if (alreadyDied) return;
        alreadyDied = true;

        Level level = BoardManager.instance.level;
        string msg = this.name + " dies. ";

        if (attacker != null && killedByStr == "")
        {
            if (attacker.hasPersonalName)
                killedBy = String.Format("Killed by {0} the {1}.", attacker.name, attacker.typeName);
            else
                killedBy = String.Format("Killed by an unnamed {0}.", attacker.name.ToLower());
        }
        else if (attacker != null && killedByStr != "")
        {
            killedBy = killedByStr;
        }
        else if (attacker == null && killedByStr != "")
        {
            killedBy = killedByStr;
        }
        else
        {
            killedBy = String.Format("Killed by unknown forces.", killedByStr);
        }

        if (leaveCorpse)
        {
            Item corpsePrim;
            Item corpseSec;
            int r;

            if (severLimbs) r = UnityEngine.Random.Range(0, 5);
            else r = 0;

            switch (r)
            {
                case 1:
                    // sever head
                    corpsePrim = new Item(ItemTypeEnum.itemCorpseMutilated, this.x, this.y);
                    corpseSec = new Item(ItemTypeEnum.itemCorpseHead, this.x, this.y);
                    if (attacker != null)
                        msg = String.Format("{0} chops off {1}'s head. ", attacker.name, this.name);
                    break;
                case 2:
                    // sever arm
                    corpsePrim = new Item(ItemTypeEnum.itemCorpseMutilated, this.x, this.y);
                    corpseSec = new Item(ItemTypeEnum.itemCorpseArm, this.x, this.y);
                    if (attacker != null)
                        msg = String.Format("{0} severs {1}'s arm. ", attacker.name, this.name);
                    break;
                case 3:
                    // sever leg
                    corpsePrim = new Item(ItemTypeEnum.itemCorpseMutilated, this.x, this.y);
                    corpseSec = new Item(ItemTypeEnum.itemCorpseLeg, this.x, this.y);
                    if (attacker != null)
                        msg = String.Format("{0} severs {1}'s leg. ", attacker.name, this.name);
                    break;
                case 4:
                    // sever torso
                    corpsePrim = new Item(ItemTypeEnum.itemCorpseLower, this.x, this.y);
                    corpseSec = new Item(ItemTypeEnum.itemCorpseUpper, this.x, this.y);
                    if (attacker != null)
                        msg = String.Format("{0} cuts {1} in half. ", attacker.name, this.name);
                    break;
                default:
                    // do not sever anything
                    corpsePrim = new Item(ItemTypeEnum.itemCorpseFull, this.x, this.y);
                    corpseSec = null;
                    break;
            }

            level.AddItemToLevel(corpsePrim, corpsePrim.x, corpsePrim.y);

            Feature bloodPool = new Feature(FeatureTypeEnum.featBloodPool, this.x, this.y);
            BoardManager.instance.level.AddFeatureToLevel(bloodPool, bloodPool.x, bloodPool.y);

            if (!level.visible[corpsePrim.x, corpsePrim.y])
            {
                //corpsePrim.go.GetComponent<Renderer>().enabled = false;
                //if (bloodPool.go != null) bloodPool.go.GetComponent<Renderer>().enabled = false;
            }

            // if the second body part is not null, find the final destination for it and determine how far it will go (up to 4 tiles)
            if (corpseSec != null)
            {
                int tx = corpseSec.x - 4 + UnityEngine.Random.Range(0, 9);
                int ty = corpseSec.y - 4 + UnityEngine.Random.Range(0, 9);
                int fx = corpseSec.x;
                int fy = corpseSec.y;
                List<Vector2Int> path = new List<Vector2Int>();

                LOS_FOV.DrawLine(corpseSec.x, corpseSec.y, tx, ty,
                    (int x, int y, int prev_x, int prev_y) =>
                    {
                        if (level.IsTerrainImpassable(x, y)) return false;
                        else
                        {
                            path.Add(new Vector2Int(x, y));
                            fx = x;
                            fy = y;
                            return true;
                        }
                    });

                corpseSec.x = fx;
                corpseSec.y = fy;

                foreach (Vector2Int pos in path)
                {
                    Feature pathBlood = new Feature(FeatureTypeEnum.featBloodDrop, pos.x, pos.y);
                    BoardManager.instance.level.AddFeatureToLevel(pathBlood, pathBlood.x, pathBlood.y);
                    //if (pathBlood.go != null) pathBlood.go.GetComponent<Renderer>().enabled = false;
                }

                level.AddItemToLevel(corpseSec, corpseSec.x, corpseSec.y);
                corpseSec.go.GetComponent<Transform>().position = new Vector3(this.x, this.y);
                //if (!level.visible[corpseSec.x, corpseSec.y]) corpseSec.go.GetComponent<Renderer>().enabled = false;

                Vector2 end = new Vector2(corpseSec.x, corpseSec.y);
                bool visibleStart = level.visible[this.x, this.y];
                bool visibleEnd = level.visible[corpseSec.x, corpseSec.y];

                corpseSec.mo.Move(end, (visibleStart || visibleEnd),
                    () =>
                    {
                        return;
                    });
            }
            
        }

        if (makeMsg)
        {
            BoardManager.instance.msgLog.PlayerVisibleMsg(this.x, this.y, msg);
        }

        if (GetAbility(AbilityTypeEnum.abilNamed) != null)
            GetAbility(AbilityTypeEnum.abilNamed).AbilityInvoke(this, new TargetStruct(new Vector2Int(this.x, this.y), this));

        level.RemoveMobFromLevel(this);

        if (this is PlayerMob) PlayerMob.QuitGame();
    }

    public void OnTick()
    {
        List<Effect> effectsToRemove = new List<Effect>();

        foreach(EffectTypeEnum effectType in effects.Keys.ToList())
        {
            if (EffectTypes.effectTypes[effectType].OnEffectTick != null)
            {
                EffectTypes.effectTypes[effectType].OnEffectTick(effects[effectType], this);
            }
            if (effects[effectType].cd != Effect.CD_UNLIMITED)
            {
                effects[effectType].cd--;
                if (effects[effectType].cd == 0) effectsToRemove.Add(effects[effectType]);
            }
        }

        for (int i = effectsToRemove.Count - 1; i >= 0; i--)
        {
            RemoveEffect(effectsToRemove[i].idType);
        }
        curAP += MobType.NORMAL_AP;

        curHP += regenHP;
        if (curHP > maxHP)
            curHP = maxHP;
        curFP += regenFP;
        if (curFP > maxFP)
            curFP = maxFP;

        CalculateArmor();
        CalculateFPRegen();
        CalculateHPRegen();
        CalculateMoveSpeed();

        if (this is PlayerMob)
            UIManager.instance.RightPanel.ShowEffects();
    }

    public void MeleeDamage(Mob target)
    {
        if (this.meleeAbil != AbilityTypeEnum.abilNone)
        {
            TargetStruct targetStruct = new TargetStruct(new Vector2Int(target.x, target.y), target);
            Ability ability = GetAbility(meleeAbil);
            ability.AbilityInvoke(this, targetStruct);
            curFP -= ability.Cost(this);
            MakeAct(ability.Spd(this));
        }
        else
        {
            MakeAct(MobType.NORMAL_AP);
        }
    }

    public Ability GetAbility(AbilityTypeEnum abilType)
    {
        if (abilities.ContainsKey(abilType)) return AbilityTypes.abilTypes[abilType];
        else return null;
    }

    public void InvokeAbility(Ability ability, TargetStruct target)
    {
        switch (ability.costType)
        {
            case AbilityCostType.wp:
                curWP = 0;
                break;
            default:
                curFP -= ability.Cost(this);
                break;
        }

        ability.AbilityInvoke(this, target);
        
        MakeAct(ability.Spd(this));

    }

    public void AddEffect(EffectTypeEnum effectType, Mob _actor, int _cd)
    {
        if (effects.ContainsKey(effectType))
        {
            effects[effectType].cd = _cd;
            effects[effectType].actor = _actor;
            effects[effectType].SV = EffectTypes.effectTypes[effectType].shieldValue;
            effects[effectType].param1 = EffectTypes.effectTypes[effectType].param1;
        }
        else
        {
            Effect effect = new Effect
            {
                idType = effectType,
                actor = _actor,
                target = this,
                cd = _cd,
                SV = EffectTypes.effectTypes[effectType].shieldValue,
                param1 = EffectTypes.effectTypes[effectType].param1
            };
            effects.Add(effectType, effect);
            if (EffectTypes.effectTypes[effectType].OnEffectAdd != null)
                EffectTypes.effectTypes[effectType].OnEffectAdd(effect, this);
        }
        if (this is PlayerMob)
            UIManager.instance.RightPanel.ShowEffects();
    }

    public Effect GetEffect(EffectTypeEnum effectType)
    {
        Effect effect;
        if (effects.TryGetValue(effectType, out effect))
            return effect;
        else
            return null;
    }

    public void RemoveEffect(EffectTypeEnum effectType)
    {
        if (GetEffect(effectType) == null) return;

        if (EffectTypes.effectTypes[effectType].OnEffectRemove != null)
            EffectTypes.effectTypes[effectType].OnEffectRemove(GetEffect(effectType), this);
        effects.Remove(effectType);
        if (this is PlayerMob)
            UIManager.instance.RightPanel.ShowEffects();
    }

    public void CalculateMoveSpeed()
    {
        float init_ms = MobTypes.mobTypes[idType].moveSpeed;
        float bonus = 1;

        foreach (EffectTypeEnum effectType in effects.Keys)
        {
            if (EffectTypes.effectTypes[effectType].OnEffectMoveSpeed != null)
            {
                bonus += EffectTypes.effectTypes[effectType].OnEffectMoveSpeed(effects[effectType]);
            } 
        }
        float ms = init_ms * bonus;
        curMoveSpeed = (init_ms / ms) * init_ms;

    }

    public void CalculateFPRegen()
    {
        int regen = MobTypes.mobTypes[idType].regenFP;

        if (GetEffect(EffectTypeEnum.effectSprint) != null)
            regen = 0;
        if (GetEffect(EffectTypeEnum.effectBlock) != null)
            regen = 0;
        if (GetEffect(EffectTypeEnum.effectReflectiveBlocking) != null)
            regen = 0;

        if (GetEffect(EffectTypeEnum.effectMeditate) != null)
            regen *= 2;

        regenFP = regen;
    }

    public void CalculateHPRegen()
    {
        int regen = MobTypes.mobTypes[idType].regenHP;

        if (GetEffect(EffectTypeEnum.effectRegenerate) != null)
            regen += 4;
        if (GetEffect(EffectTypeEnum.effectMinorRegeneration) != null)
            regen += 3;

        if (GetEffect(EffectTypeEnum.effectMeditate) != null)
            regen *= 2;

        regenHP = regen;
    }

    public void CalculateArmor()
    {
        foreach (DmgTypeEnum dmgType in MobTypes.mobTypes[idType].armorDR.Keys)
        {
            armorDR[dmgType] = MobTypes.mobTypes[idType].armorDR[dmgType];
        }
        foreach (DmgTypeEnum dmgType in MobTypes.mobTypes[idType].armorPR.Keys)
        {
            armorPR[dmgType] = MobTypes.mobTypes[idType].armorPR[dmgType];
        }

        // Mindless
        if (GetAbility(AbilityTypeEnum.abilMindless) != null)
        {
            armorPR[DmgTypeEnum.Mind] += 50;
        }

        // Blocking
        if (GetEffect(EffectTypeEnum.effectBlock) != null)
        {
            foreach (DmgTypeEnum dmgType in MobTypes.mobTypes[idType].armorPR.Keys)
            {
                armorPR[dmgType] += 50;
            }
        }

        // Reflective Blocking
        if (GetEffect(EffectTypeEnum.effectReflectiveBlocking) != null)
        {
            foreach (DmgTypeEnum dmgType in MobTypes.mobTypes[idType].armorPR.Keys)
            {
                armorPR[dmgType] += 50;
            }
        }

        // Burden of Sins
        if (GetEffect(EffectTypeEnum.effectBurdenOfSins) != null)
        {
            foreach (DmgTypeEnum dmgType in MobTypes.mobTypes[idType].armorPR.Keys)
            {
                armorPR[dmgType] -= 50;
            }
        }

        // Minor Protection
        if (GetEffect(EffectTypeEnum.effectMinorProtection) != null)
        {
            foreach (DmgTypeEnum dmgType in MobTypes.mobTypes[idType].armorDR.Keys)
            {
                armorDR[dmgType] += 2;
            }
        }

        // Covered in Tar
        if (GetEffect(EffectTypeEnum.effectCoveredInTar) != null)
        {
            armorPR[DmgTypeEnum.Fire] -= 50;
        }
    }

    public void CalculateVisionRadius()
    {
        int vision = 10;

        if (GetEffect(EffectTypeEnum.effectBlindness) != null) vision = 0;

        visionRadius = vision;
    }

    public void SetPathDst(Vector2Int dst)
    {
        pathDst = dst;
        path.Clear();
    }

    public void PlotPathToDst(Vector2Int dst)
    {
        path = Astar.FindPath(this.x, this.y, dst.x, dst.y,
            delegate (int dx, int dy, int prevx, int prevy)
            {
                AttemptMoveResult attemptMoveResult = CanMoveToPos(dx, dy);

                if (attemptMoveResult.result == AttemptMoveResultEnum.moveClear || attemptMoveResult.result == AttemptMoveResultEnum.moveBlockedbyMob) return true;
                return false;
            },
            delegate (int dx, int dy)
            {
                return 10;
            });
    }

    public void MakeRandomMove()
    {
        //Debug.Log("Make Random Move");
        List<Vector2Int> availCells = new List<Vector2Int>();
        BoardManager.instance.level.CheckSurroundings(this.x, this.y, true,
            (int dx, int dy) =>
            {
                if (CanMoveToPos(dx, dy).result == AttemptMoveResultEnum.moveClear) availCells.Add(new Vector2Int(dx, dy));
            });
        int r = UnityEngine.Random.Range(0, availCells.Count - 1);
        Move(availCells[r].x - this.x, availCells[r].y - this.y);
    }

    public bool MoveAlongPath()
    {
        bool result = false;
        
        if (path.Count > 0)
        {
            result = Move(path[0].x - this.x, path[0].y - this.y);
            path.RemoveAt(0);
        }
        return result;
    }

    public bool CanInvokeAbility(Ability ability)
    {
        if (ability.id == AbilityTypeEnum.abilNone) return false;

        if (!ability.IsApplicByCost(this)) return false;

        if (!ability.AbilityCheckApplic(ability, this)) return false;

        return true;
    }

    public bool GetFactionRelation(FactionEnum mobFaction)
    {
        return Factions.factions[faction].GetRelation(mobFaction);
    }

    public void CalculateShieldValue()
    {
        curSH = 0;
        foreach (Effect effect in effects.Values)
        {
            if (effect.SV > 0)
            {
                curSH += effect.SV;
            }
        }
    }

    public string GetFullName()
    {
        string str = "";
        if (hasPersonalName)
            str += String.Format("{0} the {1}", name, typeName);
        else
            str += String.Format("{0}", name);
        return str;
    }
}
