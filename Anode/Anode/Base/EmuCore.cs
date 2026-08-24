using Anode.Common;

namespace Anode.Base
{
    internal interface EmuCore
    {
        Renderer GetRenderer();

        byte[] GetAudioBuffer();

        string GetTitle();

        // Byte 0
        // - Supports soft reset
        // - Has audio
        // - Has video
        byte[] GetCompatibleFeatures();

        void AdvanceFrame();

        void HardReset(string ROM);

        void SoftReset();

        bool CanEmulatorRun();

        float GetSpeed();
    }
}
