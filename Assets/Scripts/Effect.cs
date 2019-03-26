﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnEffectAdd(Effect effect, Mob actor);
public delegate void OnEffectRemove(Effect effect, Mob actor);
public delegate void OnEffectTick(Effect effect, Mob actor);
public delegate float OnEffectMoveSpeed(Effect effect);

public enum EffectTypeEnum
{
    effectSprint, effectBlock, effectRegenerate, effectBlindness, effectDivineVengeance, effectInvisibility, effectBurdenOfSins, effectSyphonLight, effectBurning,
    effectFireAura, effectMinorRegeneration, effectFear, effectMeditate, effectImmobilize, effectDominateMind, effectSplitSoulTarget, effectSplitSoulSource,
    effectSilence
}

public class EffectType {
    public EffectTypeEnum id;
    public string name;
    public Color32 color;
    public OnEffectAdd OnEffectAdd;
    public OnEffectRemove OnEffectRemove;
    public OnEffectTick OnEffectTick;
    public OnEffectMoveSpeed OnEffectMoveSpeed;

}

public class Effect
{
    public const int CD_UNLIMITED = -100;

    public EffectTypeEnum idType;
    public Mob actor;
    public Mob target;
    public int cd;
}

public class EffectTypes
{
    public static Dictionary<EffectTypeEnum, EffectType> effectTypes;

    public static void InitializeEffects()
    {
        effectTypes = new Dictionary<EffectTypeEnum, EffectType>();

        Add(EffectTypeEnum.effectSprint, "Sprinting", new Color32(0, 255, 0, 255),
            (Effect effect) =>
            {
                return 0.5f;
            },
            (Effect effect, Mob actor) =>
            {
                actor.CalculateMoveSpeed();
                actor.CalculateFPRegen();
            },
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                actor.CalculateMoveSpeed();
                actor.CalculateFPRegen();
            }, null);

        Add(EffectTypeEnum.effectBlock, "Blocking", new Color32(255, 255, 0, 255),
            (Effect effect) =>
            {
                return -0.5f;
            },
            (Effect effect, Mob actor) =>
            {
                actor.CalculateMoveSpeed();
                actor.CalculateFPRegen();
                actor.CalculateArmor();
            },
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                actor.CalculateMoveSpeed();
                actor.CalculateFPRegen();
                actor.CalculateArmor();
            }, null);

        Add(EffectTypeEnum.effectRegenerate, "Regeneration", new Color32(0, 255, 0, 255),
            null,
            (Effect effect, Mob actor) =>
            {
                actor.CalculateHPRegen();
            },
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                actor.CalculateHPRegen();
            }, null);

        Add(EffectTypeEnum.effectBlindness, "Blindness", new Color32(100, 100, 100, 255),
            null,
            (Effect effect, Mob actor) =>
            {
                actor.CalculateVisionRadius();
            },
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                actor.CalculateVisionRadius();
            }, null);

        Add(EffectTypeEnum.effectDivineVengeance, "Divine Vengeance", new Color32(0, 255, 255, 255),
            null,
            null,
            null, 
            (Effect effect, Mob actor) =>
            {
                actor.curWP += 3;
                if (actor.curWP > Mob.MAX_WP) actor.curWP = Mob.MAX_WP;
            });

        Add(EffectTypeEnum.effectInvisibility, "Invisibility", new Color32(100, 100, 100, 255),
            null,
            (Effect effect, Mob actor) =>
            {
                Level level = BoardManager.instance.level;
                foreach (Mob mob in level.mobList)
                {
                    if (mob.visibleMobs.Contains(actor)) mob.visibleMobs.Remove(actor);
                }
            },
            null,
            null);

        Add(EffectTypeEnum.effectBurdenOfSins, "Burden of Sins", new Color32(0, 255, 255, 255),
            null,
            (Effect effect, Mob actor) =>
            {
                actor.CalculateArmor();
            },
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                actor.CalculateArmor();
            },
            null);

        Add(EffectTypeEnum.effectSyphonLight, "Syphon Light", new Color32(255, 0, 0, 255),
            null,
            null,
            null,
            null);

        Add(EffectTypeEnum.effectBurning, "Burning", new Color32(255, 168, 0, 255),
            null,
            null,
            null,
            (Effect effect, Mob actor) =>
            {
                int dmg = 0;
                dmg += Mob.InflictDamage(null, actor, 3, DmgTypeEnum.Fire, null);
                if (BoardManager.instance.level.visible[actor.x, actor.y])
                {
                    Vector3 pos = new Vector3(actor.x, actor.y, 0);
                    UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", pos);
                }
                BoardManager.instance.CreateBlooddrop(actor.x, actor.y);
                if (actor.CheckDead())
                {
                    actor.MakeDead(null, true, true, false);
                }
            });

        Add(EffectTypeEnum.effectFireAura, "Fire Aura", new Color32(255, 168, 0, 255),
            null,
            null,
            null,
            (Effect effect, Mob actor) =>
            {
                List<Mob> mobs = new List<Mob>();
                Level level = BoardManager.instance.level;
                level.CheckSurroundings(actor.x, actor.y, false,
                    (int x, int y) =>
                    {
                        if (level.mobs[x, y] != null && !actor.GetFactionRelation(level.mobs[x, y].faction)) 
                        {
                            mobs.Add(level.mobs[x, y]);
                        }
                    });

                foreach (Mob mob in mobs)
                {
                    int dmg = 0;
                    dmg += Mob.InflictDamage(actor, mob, 10, DmgTypeEnum.Fire,
                        (int dmg1) =>
                        {
                            string str;
                            if (dmg1 <= 0)
                            {
                                str = String.Format("{0} takes no fire dmg from the fire aura. ",
                                    mob.name);
                            }
                            else
                            {
                                str = String.Format("{0} takes {1} fire dmg from the fire aura. ",
                                    mob.name,
                                    dmg1);
                            }
                            return str;
                        });
                    mob.AddEffect(EffectTypeEnum.effectBurning, actor, 5);
                    if (BoardManager.instance.level.visible[mob.x, mob.y])
                    {
                        Vector3 pos = new Vector3(mob.x, mob.y, 0);
                        UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", pos);
                    }
                    BoardManager.instance.CreateBlooddrop(mob.x, mob.y);
                    if (mob.CheckDead())
                    {
                        mob.MakeDead(actor, true, true, false);
                    }
                }
                
            });

        Add(EffectTypeEnum.effectMinorRegeneration, "Minor Regeneration", new Color32(0, 255, 0, 255),
            null,
            (Effect effect, Mob actor) =>
            {
                actor.CalculateHPRegen();
            },
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                actor.CalculateHPRegen();
            }, null);

        Add(EffectTypeEnum.effectFear, "Fear", new Color32(255, 0, 255, 255),
            null,
            null,
            null, 
            null);

        Add(EffectTypeEnum.effectMeditate, "Meditation", new Color32(0, 255, 255, 255),
            null,
            null,
            null,
            (Effect effect, Mob actor) =>
            {
                actor.curHP += actor.regenHP;
                if (actor.curHP > actor.maxHP)
                    actor.curHP = actor.maxHP;
                actor.curFP += actor.regenFP;
                if (actor.curFP > actor.maxFP)
                    actor.curFP = actor.maxFP;
            });

        Add(EffectTypeEnum.effectImmobilize, "Immobilization", new Color32(255, 0, 255, 255),
            null,
            null,
            null,
            null);

        Add(EffectTypeEnum.effectDominateMind, "Dominate Mind", new Color32(0, 255, 255, 255),
            null,
            null,
            (Effect effect, Mob actor) =>
            {
                string str = String.Format("{0} no longer has its mind dominated. ", actor.name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
            },
            null);

        Add(EffectTypeEnum.effectSplitSoulSource, "Split Soul", new Color32(255, 0, 0, 255),
            null,
            null,
            null,
            null);

        Add(EffectTypeEnum.effectSplitSoulTarget, "Split Soul", new Color32(255, 0, 0, 255),
            null,
            null,
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                string str = String.Format("{0} ceases to exist. ", actor.name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
                actor.curHP = 0;
                actor.MakeDead(null, false, false, false);
            },
            null);

        Add(EffectTypeEnum.effectSilence, "Silence", new Color32(255, 0, 255, 255),
            null,
            null,
            null,
            null);
    }

    private static void Add(EffectTypeEnum _id, string _name, Color32 _color,
        OnEffectMoveSpeed _OnEffectMoveSpeed,
        OnEffectAdd _OnEffectAdd, OnEffectRemove _OnEffectRemove, OnEffectTick _OnEffectTick)
    {
        EffectType e = new EffectType
        {
            id = _id,
            name = _name,
            color = _color,
            OnEffectAdd = _OnEffectAdd,
            OnEffectRemove = _OnEffectRemove,
            OnEffectTick = _OnEffectTick,
            OnEffectMoveSpeed = _OnEffectMoveSpeed
        };
        effectTypes.Add(_id, e);
    }
}