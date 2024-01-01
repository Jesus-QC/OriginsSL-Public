using Discord.Interactions;

namespace OriginsHR.Commands;

public class ApplicationsModule : InteractionModuleBase<SocketInteractionContext>
{
    // [SlashCommand("review", "Reviews an application.")]
    // public async Task ReviewApplication([Summary(description:"Channel which you'd like to review application of.")] IChannel channel)
    // {
    //     (channel as ITextChannel).SendMessageAsync("# Welcome!\nWe are so proud of you to want to join the staff team, please click the button below to being with the application progress."
    //         , components: new ComponentBuilder().WithButton(ButtonHandler._applicationButton).Build()
    //         );
    // }
}