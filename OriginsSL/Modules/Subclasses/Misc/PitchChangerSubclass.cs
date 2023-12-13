using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using VoiceChat;
using VoiceChat.Codec;

namespace OriginsSL.Modules.Subclasses.Misc;

public abstract class PitchChangerSubclass  : SubclassBase
{ 
    public virtual float Pitch { get; } = 1f;

    private bool _enabled = true;
    
    public readonly float[] ReceiveBuffer = new float[VoiceChatSettings.BufferLength];
    public readonly byte[] EncodedBuffer = new byte[VoiceChatSettings.MaxEncodedSize];
    
    public OpusDecoder Decoder = PlayerVoiceExtensions.DecoderPool.Shared.Rent();
    public OpusEncoder Encoder = PlayerVoiceExtensions.EncoderPool.Shared.Rent();
    
    public PitchShifter PitchShifter = PlayerVoiceExtensions.PitchShifterPool.Shared.Rent();

    public override void OnDeath(CursedPlayer player)
    {
        _enabled = false;
        
        PlayerVoiceExtensions.DecoderPool.Shared.Return(Decoder);
        Decoder = null;
        PlayerVoiceExtensions.EncoderPool.Shared.Return(Encoder);
        Encoder = null;
        PlayerVoiceExtensions.PitchShifterPool.Shared.Return(PitchShifter);
        PitchShifter = null;
        
        base.OnDeath(player);
    }

    public class PitchChangerSubclassHandler : ISubclassEventsHandler
    {
        public void OnLoaded()
        {
            CursedPlayerEventsHandler.UsingVoiceChat += OnPlayerUsingVoiceChat;
        }
        
        private static void OnPlayerUsingVoiceChat(PlayerUsingVoiceChatEventArgs args)
        {
            if (!args.Player.TryGetSubclass(out ISubclass subclass) || subclass is not PitchChangerSubclass { _enabled: true } pitchChangerSubclass)
                return;

            args.VoiceMessage = args.VoiceMessage.SetPitch(pitchChangerSubclass);
        }
    }
    
}