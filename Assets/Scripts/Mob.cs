﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public bool ifDead;
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
    public AbilityTypeEnum sprintAbil;
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
        get { return MobTypes.mobTypes[idType].faction; }
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
        sprintAbil = MobTypes.mobTypes[idType].sprintAbil;
        dodgeAbil = MobTypes.mobTypes[idType].dodgeAbil;
        blockAbil = MobTypes.mobTypes[idType].blockAbil;

        if (meleeAbil != AbilityTypeEnum.abilNone && !abilities.ContainsKey(meleeAbil)) abilities.Add(meleeAbil, true);
        if (rangedAbil != AbilityTypeEnum.abilNone && !abilities.ContainsKey(rangedAbil)) abilities.Add(rangedAbil, true);
        if (sprintAbil != AbilityTypeEnum.abilNone && !abilities.ContainsKey(sprintAbil)) abilities.Add(sprintAbil, true);
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

        if (abilities.ContainsKey(AbilityTypeEnum.abilNamed))
        {
            int r = UnityEngine.Random.Range(0, Nemesis.names.Count);
            name = Nemesis.names[r];
            Nemesis.names.RemoveAt(r);
        }

        path = new List<Vector2Int>();
        pathDst = new Vector2Int(x, y);
        curHP = maxHP;
        curFP = maxFP;
        regenFP = regenFPBase;
        regenHP = regenHPBase;
        curAP = MobType.NORMAL_AP;
        curMoveSpeed = MobTypes.mobTypes[idType].moveSpeed;
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
        AttemptMoveResult attemptMoveResult = CanMoveToPos(x + xDir, y + yDir);
        bool result = false;

        switch (attemptMoveResult.result)
        {
            case AttemptMoveResultEnum.moveClear:
                SetPosition(x + xDir, y + yDir);
                mo.Move(x, y);
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
        return true;
    }

    public bool CheckVisibilityByPlayer()
    {
        Level level = BoardManager.instance.level;
        PlayerMob player = BoardManager.instance.player;
        if (level.visible[x, y] && player.visibleMobs.Contains(this)) return true;
        else return false;
    }

    public static int InflictDamage(Mob attacker, Mob target, int initDmg, DmgTypeEnum dmgType)
    {
        string str = "";
        int dmg = initDmg;

        dmg = (int)(dmg * (100f - target.armorPR[dmgType]) / 100);
        dmg -= target.armorDR[dmgType];
        dmg = (dmg < 0) ? 0 : dmg;

        if (attacker != null && dmg > 0)
        {
            str = String.Format("{0} hits {1} for {2} {3} dmg. ",
                attacker.name,
                target.name,
                dmg,
                DmgTypes.dmgTypes[dmgType].name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(attacker.x, attacker.y, str);
        }
        else if (attacker != null && dmg == 0)
        {
            str = String.Format("{0} hits {1}, but {1} takes no {2} damage. ",
                attacker.name,
                target.name,
                DmgTypes.dmgTypes[dmgType].name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(attacker.x, attacker.y, str);
        }
        else if (attacker == null && dmg > 0)
        {
            str = String.Format("{0} takes {1} {2} dmg. ",
                target.name,
                dmg,
                DmgTypes.dmgTypes[dmgType].name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(target.x, target.y, str);
        }
        else if (attacker == null && dmg == 0)
        {
            str = String.Format("{0} takes no {1} dmg. ",
                target.name,
                DmgTypes.dmgTypes[dmgType].name);
            BoardManager.instance.msgLog.PlayerVisibleMsg(target.x, target.y, str);
        }

        target.curHP -= dmg;

        // increase WP with Divine Vengeance
        if (attacker != null && attacker.GetAbility(AbilityTypeEnum.abilDivineVengeance) != null)
            attacker.GetAbility(AbilityTypeEnum.abilDivineVengeance).AbilityInvoke(attacker, new TargetStruct(new Vector2Int(attacker.x, attacker.y), attacker));
        if (target.GetAbility(AbilityTypeEnum.abilDivineVengeance) != null)
            target.GetAbility(AbilityTypeEnum.abilDivineVengeance).AbilityInvoke(target, new TargetStruct(new Vector2Int(target.x, target.y), target));

        // remove Blindness
        if (dmg > 0 && target.GetEffect(EffectTypeEnum.effectBlindness) != null)
            target.RemoveEffect(EffectTypeEnum.effectBlindness);
            

        return dmg;
    }

    public bool CheckDead()
    {
        if (curHP <= 0) return true;
        else return false;
    }

    public void MakeDead(Mob attacker, bool makeMsg, bool leaveCorpse, bool severLimbs)
    {
        Level level = BoardManager.instance.level;
        string msg = this.name + " dies. ";


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

                corpseSec.mo.Move(corpseSec.x, corpseSec.y);
            }
            
        }

        if (makeMsg)
        {
            BoardManager.instance.msgLog.PlayerVisibleMsg(this.x, this.y, msg);
        }

        ifDead = true;
        level.RemoveMobFromLevel(this);

        if (this is PlayerMob) PlayerMob.QuitGame();
    }

    public void OnTick()
    {
        List<Effect> effectsToRemove = new List<Effect>();

        foreach(EffectTypeEnum effectType in effects.Keys)
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
            curFP -= ability.cost;
            MakeAct(ability.spd);
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
        ability.AbilityInvoke(this, target);
        switch (ability.costType)
        {
            case AbilityCostType.wp:
                curWP = 0;
                break;
            default:
                curFP -= ability.cost;
                break;
        }
        
        MakeAct(ability.spd);

    }

    public void AddEffect(EffectTypeEnum effectType, Mob _actor, int _cd)
    {
        if (effects.ContainsKey(effectType))
        {
            effects[effectType].cd = _cd;
            effects[effectType].actor = _actor;
        }
        else
        {
            Effect effect = new Effect
            {
                idType = effectType,
                actor = _actor,
                target = this,
                cd = _cd
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
        float ms = MobTypes.mobTypes[idType].moveSpeed;
        float bonus = 1;

        foreach (EffectTypeEnum effectType in effects.Keys)
        {
            if (EffectTypes.effectTypes[effectType].OnEffectMoveSpeed != null)
            {
                bonus += EffectTypes.effectTypes[effectType].OnEffectMoveSpeed(effects[effectType]);
            } 
        }
        ms = ms * bonus;
        curMoveSpeed = (MobType.NORMAL_AP / ms) * MobType.NORMAL_AP;

    }

    public void CalculateFPRegen()
    {
        int regen = MobTypes.mobTypes[idType].regenFP;

        if (GetEffect(EffectTypeEnum.effectSprint) != null)
            regen = 0;
        if (GetEffect(EffectTypeEnum.effectBlock) != null)
            regen = 0;

        regenFP = regen;
    }

    public void CalculateHPRegen()
    {
        int regen = MobTypes.mobTypes[idType].regenHP;

        if (GetEffect(EffectTypeEnum.effectRegenerate) != null)
            regen += 4;

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

        // Blocking
        if (GetEffect(EffectTypeEnum.effectBlock) != null)
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
}
