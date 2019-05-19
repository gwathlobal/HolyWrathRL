using System;
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
    effectSilence, effectAbsorbingShield, effectReflectiveBlocking, effectRemoveAfterTime, effectAuraMinorProtection, effectMinorProtection, effectCoveredInTar,
    effectImmobilizeImmunity, effectPortalSummoned
}

public class EffectType {
    public EffectTypeEnum id;
    public string name;
    public string descr;
    public Color32 color;
    public OnEffectAdd OnEffectAdd;
    public OnEffectRemove OnEffectRemove;
    public OnEffectTick OnEffectTick;
    public OnEffectMoveSpeed OnEffectMoveSpeed;
    public int shieldValue;
    public bool reflectsProjectiles;
    public bool canBePurged;
    public int param1;
}

public class Effect
{
    public const int CD_UNLIMITED = -100;

    public EffectTypeEnum idType;
    public Mob actor;
    public Mob target;
    public int cd;
    public int SV;
    public int param1;
}

public class EffectTypes
{
    public static Dictionary<EffectTypeEnum, EffectType> effectTypes;

    public static void InitializeEffects()
    {
        effectTypes = new Dictionary<EffectTypeEnum, EffectType>();

        Add(EffectTypeEnum.effectSprint, "Sprinting", "Increases movement speed by 50%", new Color32(0, 255, 0, 255),
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

        Add(EffectTypeEnum.effectBlock, "Blocking", "Increases %DR from all damage types by 50%", new Color32(255, 255, 0, 255),
            null,
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

        Add(EffectTypeEnum.effectRegenerate, "Regeneration", "Increases HP regeneration by 4 pts", new Color32(0, 255, 0, 255),
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

        Add(EffectTypeEnum.effectBlindness, "Blindness", "Reduces vision radius to 0", new Color32(100, 100, 100, 255),
            null,
            (Effect effect, Mob actor) =>
            {
                actor.CalculateVisionRadius();
            },
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                actor.CalculateVisionRadius();
            }, null, 0, false, true);

        Add(EffectTypeEnum.effectDivineVengeance, "Divine Vengeance", "Gain 3 Wrath Points each turn", new Color32(0, 255, 255, 255),
            null,
            null,
            null, 
            (Effect effect, Mob actor) =>
            {
                actor.curWP += 3;
                if (actor.curWP > Mob.MAX_WP) actor.curWP = Mob.MAX_WP;
            });

        Add(EffectTypeEnum.effectInvisibility, "Invisibility", "Makes your invisible to enemies. Invisiblity remains even when you attack or use abilities", new Color32(100, 100, 100, 255),
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

        Add(EffectTypeEnum.effectBurdenOfSins, "Burden of Sins", "Reduces %DR for all damage types", new Color32(0, 255, 255, 255),
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
            null, 0, false, true);

        Add(EffectTypeEnum.effectSyphonLight, "Syphon Light", "Enemy gains 50% of the inflicted damage as HP", new Color32(255, 0, 0, 255),
            null,
            null,
            null,
            null, 0, false, true);

        Add(EffectTypeEnum.effectBurning, "Burning", "Deals 3 fire damage each turn", new Color32(255, 168, 0, 255),
            null,
            null,
            null,
            (Effect effect, Mob actor) =>
            {
                int dmg = 0;
                dmg += Mob.InflictDamage(null, actor, 
                    new Dictionary<DmgTypeEnum, int>()
                    {
                        { DmgTypeEnum.Fire, 3 }
                    },
                    null,
                    true);
                if (BoardManager.instance.level.visible[actor.x, actor.y])
                {
                    Vector3 pos = new Vector3(actor.x, actor.y, 0);
                    UIManager.instance.CreateFloatingText(dmg + " <i>DMG</i>", pos);
                }
                if (actor.CheckDead())
                {
                    actor.MakeDead(null, true, true, false);
                }
            }, 0, false, true);

        Add(EffectTypeEnum.effectFireAura, "Fire Aura", "Deals 5 fire damage to all enemies around you", new Color32(255, 168, 0, 255),
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
                    dmg += Mob.InflictDamage(actor, mob, 
                        new Dictionary<DmgTypeEnum, int>()
                        {
                            { DmgTypeEnum.Fire, 5 }
                        },
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

        Add(EffectTypeEnum.effectMinorRegeneration, "Minor Regeneration", "Increases HP regeneration by 3 pts", new Color32(0, 255, 0, 255),
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

        Add(EffectTypeEnum.effectFear, "Fear", "Makes move away from any enemy in sight", new Color32(255, 0, 255, 255),
            null,
            null,
            null,
            null, 0, false, true);

        Add(EffectTypeEnum.effectMeditate, "Meditation", "Doubles the FP and HP regeneration", new Color32(0, 255, 255, 255),
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

        Add(EffectTypeEnum.effectImmobilize, "Immobilization", "You can not move", new Color32(255, 0, 255, 255),
            null,
            null,
            null,
            null);

        Add(EffectTypeEnum.effectDominateMind, "Dominate Mind", "Your fight for someone else", new Color32(0, 255, 255, 255),
            null,
            null,
            (Effect effect, Mob actor) =>
            {
                string str = String.Format("{0} no longer has its mind dominated. ", actor.name);
                BoardManager.instance.msgLog.PlayerVisibleMsg(actor.x, actor.y, str);
            },
            null, 0, false, true);

        Add(EffectTypeEnum.effectSplitSoulSource, "Split Soul", "You can teleport to your split soul", new Color32(255, 0, 0, 255),
            null,
            null,
            null,
            null);

        Add(EffectTypeEnum.effectSplitSoulTarget, "Split Soul", "The source of this soul shard can teleport here", new Color32(255, 0, 0, 255),
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

        Add(EffectTypeEnum.effectSilence, "Silence", "You can not invoke any non-physical abilities", new Color32(255, 0, 255, 255),
            null,
            null,
            null,
            null, 0, false, true);

        Add(EffectTypeEnum.effectAbsorbingShield, "Absorbing Shield", "You have a shield that can absorb a maximum of 30 damage", new Color32(160, 160, 0, 255),
            null,
            (Effect effect, Mob actor) =>
            {
                actor.CalculateShieldValue();
            },
            (Effect effect, Mob actor) =>
            {
                actor.effects.Remove(effect.idType);
                actor.CalculateShieldValue();
            },
            null,
            30);

        Add(EffectTypeEnum.effectReflectiveBlocking, "Reflective Blocking", "You reflect projectiles back to the attacker", new Color32(255, 255, 0, 255),
           null,
           null,
           null, null, 0, true, false, 4);

        Add(EffectTypeEnum.effectRemoveAfterTime, "Temporary", "You will die once this effect expires", new Color32(255, 255, 0, 255),
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

        Add(EffectTypeEnum.effectMinorProtection, "Minor Protection", "Gives 2 DDR against all damage types", new Color32(255, 255, 0, 255),
            null,
            (Effect effect, Mob actor) =>
            {
                actor.CalculateArmor();
            },
            (Effect effect, Mob actor) =>
            {
                actor.CalculateArmor();
            },
            null);

        Add(EffectTypeEnum.effectAuraMinorProtection, "Minor Protection Aura", "Grants Minor Protection effect to nearby allies", new Color32(255, 255, 0, 255),
           null,
           (Effect effect, Mob actor) =>
           {
               actor.AddEffect(EffectTypeEnum.effectMinorProtection, actor, 2);

               Level level = BoardManager.instance.level;
               level.CheckSurroundings(actor.x, actor.y, false,
                   (int x, int y) =>
                   {
                       if (level.mobs[x, y] != null && actor.GetFactionRelation(level.mobs[x, y].faction))
                       {
                           Mob mob = level.mobs[x, y];

                           mob.AddEffect(EffectTypeEnum.effectMinorProtection, actor, 2);
                       }
                   });
           },
           null,
           (Effect effect, Mob actor) =>
           {
               actor.AddEffect(EffectTypeEnum.effectMinorProtection, actor, 2);

               Level level = BoardManager.instance.level;
               level.CheckSurroundings(actor.x, actor.y, false,
                   (int x, int y) =>
                   {
                       if (level.mobs[x, y] != null && actor.GetFactionRelation(level.mobs[x, y].faction))
                       {
                           Mob mob = level.mobs[x, y];

                           mob.AddEffect(EffectTypeEnum.effectMinorProtection, actor, 2);
                       }
                   });
           });

        Add(EffectTypeEnum.effectCoveredInTar, "Covered in Tar", "Give -50 %DR against fire damage", new Color32(132, 132, 132, 255),
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
            null, 0, false, false);

        Add(EffectTypeEnum.effectImmobilizeImmunity, "Immobilize Immunity", "You can not be immobilized", new Color32(255, 0, 255, 255),
            null,
            null,
            null,
            null, 0, false, false);

        Add(EffectTypeEnum.effectPortalSummoned, "Portal Summoned", "This is a summoned portal", new Color32(255, 0, 255, 255),
            null,
            null,
            null,
            null, 0, false, false);
    }

    private static void Add(EffectTypeEnum _id, string _name, string _descr, Color32 _color,
        OnEffectMoveSpeed _OnEffectMoveSpeed,
        OnEffectAdd _OnEffectAdd, OnEffectRemove _OnEffectRemove, OnEffectTick _OnEffectTick, int _sv = 0, bool _reflect = false, bool _purged = false, int _param1 = 0)
    {
        EffectType e = new EffectType
        {
            id = _id,
            name = _name,
            descr = _descr,
            color = _color,
            OnEffectAdd = _OnEffectAdd,
            OnEffectRemove = _OnEffectRemove,
            OnEffectTick = _OnEffectTick,
            OnEffectMoveSpeed = _OnEffectMoveSpeed,
            shieldValue = _sv,
            reflectsProjectiles = _reflect,
            canBePurged = _purged,
            param1 = _param1
        };
        effectTypes.Add(_id, e);
    }
}