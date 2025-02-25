---
uid: important_changes_ac
title: Application Commands
---

# What changed?

Discord [changed](https://discord.com/developers/docs/interactions/slash-commands) **SlashCommands** into **Application Commands** in favour of the new context menu stuff.
You can read about it [here](https://discord.com/developers/docs/interactions/application-commands).

## Upgrade from **DisCatSharp.SlashCommands** to **DisCatSharp.ApplicationCommands**

In **DisCatSharp.SlashCommands** you used Application Commands like this:
```cs
// Usage directives
using DisCatSharp.SlashCommands;
using DisCatSharp.SlashCommands.EventArgs;

// Extension
    public static SlashCommandsExtension Slash;

// Injecting SlashCommands
    Client.UseSlashCommands();
    Slash = Client.GetSlashCommands();

// Registration of commands and events
    Slash.SlashCommandErrored += Slash_SlashCommandErrored;
    Slash.SlashCommandExecuted += Slash_SlashCommandExecuted;

    Slash.RegisterCommands<SlashCommands.Main>();

// Events
    public static Task Slash_SlashCommandExecuted(SlashCommandsExtension sender, SlashCommandExecutedEventArgs e)
    {
        Console.WriteLine($"Slash/Info: {e.Context.CommandName}");
        return Task.CompletedTask;
    }

    public static Task Slash_SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
    {
        Console.WriteLine($"Slash/Error: {e.Exception.Message} | CN: {e.Context.CommandName} | IID: {e.Context.InteractionId}");
        return Task.CompletedTask;
    }

// Commands
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.SlashCommands;
using DisCatSharp.SlashCommands.Attributes;

namespace TestBot.SlashCommands
{
    internal class Main : SlashCommandModule
    {

        [SlashCommand("test", "A slash command made to test the DisCatSharp library!")]
        public static async Task TestCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await Task.Delay(5000);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("5 second delay complete!"));
        }
    }
}
```

In **DisCatSharp.ApplicationCommands** you use them like this:
```cs
// Usage directives
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.EventArgs;

// Extension
    public static ApplicationCommandsExtension ApplicationCommands;

// Injecting SlashCommands
    Client.UseApplicationCommands();
    ApplicationCommands = Client.GetApplicationCommands();

// Registration of commands and events
    ApplicationCommands.SlashCommandExecuted += Ac_SlashCommandExecuted;
    ApplicationCommands.SlashCommandErrored += Ac_SlashCommandErrored;
    /* New stuff - context menu */
    ApplicationCommands.ContextMenuExecuted += Ac_ContextMenuExecuted;
    ApplicationCommands.ContextMenuErrored += Ac_ContextMenuErrored;

    ApplicationCommands.RegisterCommands<SlashCommands.Main>();

// Events
    public static Task Ac_SlashCommandExecuted(ApplicationCommandsExtension sender, SlashCommandExecutedEventArgs e)
    {
        Console.WriteLine($"Slash/Info: {e.Context.CommandName}");
        return Task.CompletedTask;
    }

    public static Task Ac_SlashCommandErrored(ApplicationCommandsExtension sender, SlashCommandErrorEventArgs e)
    {
        Console.WriteLine($"Slash/Error: {e.Exception.Message} | CN: {e.Context.CommandName} | IID: {e.Context.InteractionId}");
        return Task.CompletedTask;
    }

    public static Task Ac_ContextMenuExecuted(ApplicationCommandsExtension sender, ContextMenuExecutedEventArgs e)
    {
        Console.WriteLine($"Slash/Info: {e.Context.CommandName}");
        return Task.CompletedTask;
    }

    public static Task Ac_ContextMenuErrored(ApplicationCommandsExtension sender, ContextMenuErrorEventArgs e)
    {
        Console.WriteLine($"Slash/Error: {e.Exception.Message} | CN: {e.Context.CommandName} | IID: {e.Context.InteractionId}");
        return Task.CompletedTask;
    }

// Commands
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;

namespace TestBot.ApplicationCommands
{
    internal class Main : ApplicationCommandsModule
    {

        [SlashCommand("test", "A slash command made to test the DisCatSharp library!")]
        public static async Task TestCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await Task.Delay(5000);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("5 second delay complete!"));
        }
    }
}
```
