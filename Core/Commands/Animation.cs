﻿using BetterSecondBot.bottypes;
using BetterSecondBot.Static;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;
using Newtonsoft.Json;
using System.Reflection;

namespace BetterSecondBot.HttpService
{
    public class HTTP_Animation : WebApiControllerWithTokens
    {
        public HTTP_Animation(SecondBot mainbot, TokenStorage setuptokens) : base(mainbot, setuptokens) { }

        [About("Toggles if animation requests from this avatar (used for remote poseballs) are accepted")]
        [ReturnHints("Granted perm animation")]
        [ReturnHints("Removed perm animation")]
        [ReturnHintsFailure("avatar lookup")]
        [ArgHints("avatar", "URLARG", "UUID (or Firstname Lastname)")]
        [Route(HttpVerbs.Get, "/AddToAllowAnimations/{avatar}/{token}")]
        public object AddToAllowAnimations(string avatar, string token)
        {
            if (tokens.Allow(token, "animation", "addtoallowanimations", handleGetClientIP()) == false)
            {
                return BasicReply("Token not accepted", "addtoallowanimations", new [] { avatar });
            }
            ProcessAvatar(avatar);
            if (avataruuid == UUID.Zero)
            {
                return BasicReply("avatar lookup", "addtoallowanimations", new [] { avatar });
            }
            if (bot.Accept_action_from("animation", avataruuid) == false)
            {
                bot.Add_action_from("animation", avataruuid, bot.FindAvatarKey2Name(avataruuid));
                return BasicReply("Granted perm animation", "addtoallowanimations", new [] { avatar });
            }
            bot.Remove_action_from("animation", avataruuid, true);
            return BasicReply("Removed perm animation", "addtoallowanimations", new [] { avatar });
        }

        [About("Attempts to play a gesture")]
        [ReturnHintsFailure("Error with gesture")]
        [ReturnHints("Accepted")]
        [ArgHints("gesture", "URLARG", "Inventory UUID of the gesture")]
        [Route(HttpVerbs.Get, "/PlayGesture/{gesture}/{token}")]
        public object PlayGesture(string gesture, string token)
        {
            if (tokens.Allow(token, "animation", "gesture", handleGetClientIP()) == false)
            {
                return BasicReply("Token not accepted", "gesture", new [] { gesture });
            }
            if (UUID.TryParse(gesture, out UUID gestureUUID) == false)
            {
                return BasicReply("Error with gesture", "gesture", new [] { gesture });
            }
            InventoryItem itm = bot.GetClient.Inventory.FetchItem(gestureUUID, bot.GetClient.Self.AgentID, (3 * 1000));
            bot.GetClient.Self.PlayGesture(itm.AssetUUID);
            return BasicReply("Accepted", "gesture", new [] { gesture });
        }

        [About("Resets the animation stack for the bot")]
        [ReturnHints("Accepted")]
        [Route(HttpVerbs.Get, "/ResetAnimations/{token}")]
        public object ResetAnimations(string token)
        {
            if (tokens.Allow(token, "animation", "resetanimations", handleGetClientIP()) == false)
            {
                return BasicReply("Token not accepted", "resetanimations");
            }
            bot.ResetAnimations();
            return BasicReply("Accepted", "resetanimations");
        }

    }
}
