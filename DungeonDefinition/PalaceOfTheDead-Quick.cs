﻿using System.Collections.Generic;
using System.Linq;
using Clio.Utilities;
using Deep.DungeonDefinition.Base;
using Deep.Helpers;
using Deep.TaskManager.Actions;
using ff14bot;
using ff14bot.Directors;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Deep.DungeonDefinition
{
    public class PalaceOfTheDeadQuick : PalaceOfTheDead
    {
        public PalaceOfTheDeadQuick(DeepDungeonData deepDungeon) : base(deepDungeon)
        {
        }

        public override string DisplayName => base.DisplayName + "-Quick";

        public override List<GameObject> GetObjectsByWeight()
        {
            if (DeepDungeonManager.PortalActive)
                return GameObjectManager.GameObjects
                    .Where(Filter)
                    .OrderByDescending(SortComplete)
                    .ToList();

            return GameObjectManager.GameObjects
                .Where(Filter)
                .OrderByDescending(Sort)
                .ToList();
        }

        public override float Sort(GameObject obj)
        {
            var weight = 150f;

            if (PartyManager.IsInParty && !PartyManager.IsPartyLeader && !DeepDungeonManager.BossFloor)
            {
                if (PartyManager.PartyLeader.IsInObjectManager && PartyManager.PartyLeader.CurrentHealth > 0)
                {
                    if (PartyManager.PartyLeader.BattleCharacter.HasTarget)
                        if (obj.ObjectId == PartyManager.PartyLeader.BattleCharacter.TargetGameObject.ObjectId)
                            weight += 600;
                    weight -= obj.Distance2D(PartyManager.PartyLeader.GameObject);
                }
                else
                {
                    weight -= obj.Distance2D();
                }
            }
            else
            {
                weight -= obj.Distance2D();
            }

            switch (obj.Type)
            {
                case GameObjectType.BattleNpc:
                    weight /= 2;
                    if ((obj as BattleCharacter).IsTargetingMyPartyMember())
                        weight += 100;
                    break;
                case GameObjectType.Treasure:
                    //weight += 10;
                    break;
            }

            return weight;
        }

        private float SortComplete(GameObject obj)
        {
            var weight = 150f;

            if (PartyManager.IsInParty && !PartyManager.IsPartyLeader && !DeepDungeonManager.BossFloor)
            {
                if (PartyManager.PartyLeader.IsInObjectManager && PartyManager.PartyLeader.CurrentHealth > 0)
                {
                    if (PartyManager.PartyLeader.BattleCharacter.HasTarget)
                        if (obj.ObjectId == PartyManager.PartyLeader.BattleCharacter.TargetGameObject.ObjectId)
                            weight += 600;
                    weight -= obj.Distance2D(PartyManager.PartyLeader.GameObject);
                }
                else
                {
                    weight -= obj.Distance2D();
                }
            }
            else
            {
                if (FloorExit.location != Vector3.Zero)
                    weight -= Core.Me.Distance2D(Vector3.Lerp(obj.Location, FloorExit.location, 0.25f));
                else
                    weight -= obj.Distance2D();
            }

            switch (obj.Type)
            {
                case GameObjectType.BattleNpc when !PartyManager.IsInParty:
                    return weight / 2;

                case GameObjectType.BattleNpc:
                    weight /= 2;
                    break;
                case GameObjectType.Treasure:
                    break;
            }

            if (DeepDungeonManager.PortalActive && Settings.Instance.GoForTheHoard && obj.NpcId == EntityNames.Hidden)
                weight += 5;

            return weight;
        }

        public override bool Filter(GameObject obj)
        {
            //Blacklists
            if (Blacklist.Contains(obj) || Constants.TrapIds.Contains(obj.NpcId) ||
                Constants.IgnoreEntity.Contains(obj.NpcId))
                return false;

            if (obj.Location == Vector3.Zero)
                return false;

            switch (obj.Type)
            {
                case GameObjectType.Treasure:
                    return !(HaveMainPomander() && DeepDungeonManager.PortalActive &&
                             FloorExit.location != Vector3.Zero);
                case GameObjectType.EventObject:
                    return true;
                case GameObjectType.BattleNpc:
                    return !((BattleCharacter) obj).IsDead;
                default:
                    return false;
            }
        }

        private bool HaveMainPomander()
        {
            return DeepDungeonManager.GetInventoryItem(Pomander.Lust).Count > 0 &&
                   DeepDungeonManager.GetInventoryItem(Pomander.Strength).Count > 0 &&
                   DeepDungeonManager.GetInventoryItem(Pomander.Steel).Count > 0;
        }

        public override string GetDDType()
        {
            return "PotD-Quick";
        }
    }
}