# BotSandwich
This repo is my extension of Discord.Net that I use for all of my bots. It separate individual functionality into modules which can be loaded in when the bot is initialized.

Some features:
- Custom built attribute-based command system. [Source](https://github.com/CatSandwich/BotSandwich/blob/main/Data/Module.cs#L63) - [Example](https://github.com/CatSandwich/BotSandwich/blob/main/Modules/Utils/UtilsModule.cs#L21)
- Custom built attribute-based reaction menu. [Source](https://github.com/CatSandwich/BotSandwich/blob/main/Data/Input/ReactionMenu.cs) - [Example](https://github.com/CatSandwich/BotSandwich/blob/main/Modules/Utils/Embeds/TestEmbed.cs)
- Keyword-based auto response system. [Source](https://github.com/CatSandwich/BotSandwich/blob/main/Data/Module.cs#L131) - [Source 2](https://github.com/CatSandwich/BotSandwich/blob/main/Data/Module.cs#L161) - No example. Was used in a professional codebase I can't post.

This repository has over time been built when I wanted to learn new concepts (attribute / reflection mainly). I enjoy writing tools to make programming easier, and this repository is exactly that.
