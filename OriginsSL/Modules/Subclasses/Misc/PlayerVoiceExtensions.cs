using System.Collections.Concurrent;
using NorthwoodLib.Pools;
using VoiceChat;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;
using VoiceChat.Networking;

namespace OriginsSL.Modules.Subclasses.Misc;

public static class PlayerVoiceExtensions
{
    public class EncoderPool : IPool<OpusEncoder>
    {
        public static readonly EncoderPool Shared = new ();
        
        private readonly ConcurrentQueue<OpusEncoder> _pool = new ();
        
        public OpusEncoder Rent()
        {
            return _pool.TryDequeue(out OpusEncoder encoder) 
                ? encoder
                : new OpusEncoder(OpusApplicationType.Voip);
        }

        public void Return(OpusEncoder obj)
        {
            _pool.Enqueue(obj);
        }
    }
    
    public class DecoderPool : IPool<OpusDecoder>
    {
        public static readonly DecoderPool Shared = new ();
        
        private readonly ConcurrentQueue<OpusDecoder> _pool = new ();
        
        public OpusDecoder Rent()
        {
            return _pool.TryDequeue(out OpusDecoder encoder) 
                ? encoder
                : new OpusDecoder();
        }

        public void Return(OpusDecoder obj)
        {
            _pool.Enqueue(obj);
        }
    }
    
    public class PitchShifterPool : IPool<PitchShifter>
    {
        public static readonly PitchShifterPool Shared = new ();
        
        private readonly ConcurrentQueue<PitchShifter> _pool = new ();
        
        public PitchShifter Rent()
        {
            return _pool.TryDequeue(out PitchShifter encoder) 
                ? encoder
                : new PitchShifter();
        }

        public void Return(PitchShifter obj)
        {
            _pool.Enqueue(obj);
        }
    }
    
    public static VoiceMessage SetPitch(this VoiceMessage msg, PitchChangerSubclass subclass)
    {
        int len = subclass.Decoder.Decode(msg.Data, msg.DataLength, subclass.ReceiveBuffer);
        subclass.PitchShifter.PitchShift(subclass.Pitch, len, VoiceChatSettings.SampleRate, subclass.ReceiveBuffer);
        int length = subclass.Encoder.Encode(subclass.ReceiveBuffer, subclass.EncodedBuffer);
        msg.Data = subclass.EncodedBuffer;
        msg.DataLength = length;
        return msg;
    }
}