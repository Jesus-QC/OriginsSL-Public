using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace OriginsHR.Commands;

public class ButtonHandler(DiscordSocketClient discord)
{
    public static readonly ButtonBuilder _applicationButton = ButtonBuilder.CreateSecondaryButton("Create application!", "create_app_btn");

    private readonly ModalBuilder _applicationModal =
        new ("Create Application", "new_app", new ModalComponentBuilder()
            .WithTextInput("What position are you applying for?", "position", placeholder: "Discord Moderator", required: true)
            .WithTextInput("What is your age?", "age", placeholder: "18", required: true)
            .WithTextInput("What is your timezone?", "timezone", placeholder: "CET + 0", required: true)
            .WithTextInput("Why would you like to apply?", "question", TextInputStyle.Paragraph, placeholder: "Because...", required: true)
        );
    
    public void Initialize()
    {
        discord.ButtonExecuted += HandleButton;
        discord.ModalSubmitted += HandleModal;
    }

    private async Task HandleButton(SocketMessageComponent component)
    {
        if (component.Data.CustomId == _applicationButton.CustomId)
            await HandleApplicationButton(component);
    }
    
    private async Task HandleApplicationButton(SocketMessageComponent component)
    {
        if (component.User is SocketGuildUser user && user.Roles.Any(x => x.Id == 1187390765844942898))
        {
            await component.RespondAsync("You have already applied!", ephemeral: true);
            return;
        }        
        
        await component.RespondWithModalAsync(_applicationModal.Build());
    }

    private async Task HandleModal(SocketModal modal)
    {
        if (modal.Data.CustomId == _applicationModal.CustomId)
            await HandleApplicationCreateModal(modal);
    }

    private async Task HandleApplicationCreateModal(SocketModal modal)
    {
        IReadOnlyCollection<SocketMessageComponentData> components = modal.Data.Components;

        string position = components.First(x => x.CustomId == "position").Value;
        string age = components.First(x => x.CustomId == "age").Value;
        string timezone = components.First(x => x.CustomId == "timezone").Value;
        string question = components.First(x => x.CustomId == "question").Value;
        
        SocketGuild guild = discord.GetGuild(1187381119935582238);
        
        RestTextChannel channel = await guild.CreateTextChannelAsync(modal.User.Username, properties =>
        {
            properties.CategoryId = 1187391671328059512;
            properties.PermissionOverwrites = new List<Overwrite>
            {
                new(guild.EveryoneRole.Id, PermissionTarget.Role, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Deny)),
                new(modal.User.Id, PermissionTarget.User, OverwritePermissions.InheritAll.Modify(viewChannel: PermValue.Allow))
            };
        });
        
        await channel.SendMessageAsync("# New Application <@&1187390622886281297>\n- Applicant: " + modal.User.Mention + "\n- Position Applied for: `" + position + "`\n- Age: `" + age + "`\n- Country and Timezone: `" + timezone + "`\n- Why do you want to apply?: \n```"+ question + "```");
        await (modal.User as SocketGuildUser)!.AddRoleAsync(1187390765844942898);

        await modal.RespondAsync("Thank you for applying! There should be a new channel created to handle your app.", ephemeral: true);
    }
}