using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anode.Cores.NES.Nessie.Component
{
    internal class _2a03_APU
    {
        // Tables (╯°□°)╯︵ ┻━┻
        readonly byte[] LengthTable =
        {
            10, 254, 20, 2, 40, 4, 80, 6, 160, 8, 60, 10, 14, 12, 26, 14,
            12, 16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30
        };

        // External stuff
        bool NTSC = true; // PAL is not supported atm.

        // Frame counter timing values
        public uint apuFrameIndex;

        // ----- APU Registers
        // sq1 registers
        byte sq1_duty;
        bool sq1_loop;
        bool sq1_constant;
        byte sq1_vol;
        bool sq1_sweepEnabled;
        byte sq1_sweepPeriod;
        bool sq1_sweepNegate;
        byte sq1_sweepShift;
        ushort sq1_timer_lo;
        byte sq1_timer_hi;
        byte sq1_lengthCounter;
        uint sq1_ftime;
        bool sq1_playing;

        // sq2 registers
        byte sq2_duty;
        bool sq2_loop;
        bool sq2_constant;
        byte sq2_vol;
        bool sq2_sweepEnabled;
        byte sq2_sweepPeriod;
        bool sq2_sweepNegate;
        byte sq2_sweepShift;
        byte sq2_timer_lo;
        byte sq2_timer_hi;
        byte sq2_lengthCounter;

        // tri registers
        bool tri_count;
        byte tri_linear;
        byte tri_timer_lo;
        byte tri_timer_hi;
        byte tri_lengthCounter;

        // noise registers
        bool noise_loop;
        bool noise_constant;
        byte noise_vol;
        bool noise_mode;
        byte noise_period;
        byte noise_lengthCounter;

        // DMC registers
        byte apuDMCFrequency;
        bool apuDMCLoops;
        byte apuDMCLoadCounter;
        ushort apuDMCSampleAddress;
        ushort apuDMCSampleLength;
        ushort apuDMCBytesRemaining;
        byte apuDMCBuffer;
        byte apuDMCBufferStep;
        ushort apuDMCPosition;
        //byte apuDMCOutput;

        // IRQ For DMC and Frame Counter
        public bool IRQLevelDetector; // Inverse of what actually happens.

        // Registers used in DMC DMA
        bool apuDMCRequired;
        byte apuDMCDMAStep;
        bool DMCDMACycle;
        byte DMCDMATempVal;

        // Enable registers
        bool apuEnable_DMC;
        bool apuEnable_Noise;
        bool apuEnable_Tri;
        bool apuEnable_sq2;
        bool apuEnable_sq1;

        // APU Flags
        bool apuFlag_DMCInterrupt;
        bool apuFlag_frameInterrupt;
        bool apuFlag_IRQInhibit; // Frame Counter IRQ
        bool apuFlag_IRQEnable; // DMC DMA IRQ

        // 0 = 4 step, 1 = 5 step
        bool apuFrameCounterMode;

        bool apuCycle;
        byte apuFrameResetCountdown;
        uint apuFrameCycle;

        public byte Read(byte ExternalDataBus)
        {
            // Sound channel and IRQ status
            byte apuFlags = 0;
            apuFlags |= (byte)(apuFlag_DMCInterrupt ? 0x80 : 0);
            apuFlags |= (byte)(apuFlag_frameInterrupt ? 0x40 : 0);
            apuFlags |= (byte)(ExternalDataBus & 0x20);
            apuFlags |= (byte)(apuDMCBytesRemaining > 0 ? 0x10 : 0);
            apuFlags |= (byte)(noise_lengthCounter > 0 ? 8 : 0);
            apuFlags |= (byte)(tri_lengthCounter > 0 ? 4 : 0);
            apuFlags |= (byte)(sq2_lengthCounter > 0 ? 2 : 0);
            apuFlags |= (byte)(sq1_lengthCounter > 0 ? 1 : 0);

            // Clears this flag
            apuFlag_frameInterrupt = false;
            IRQLevelDetector = false;

            return apuFlags;
        }

        public void Write(ushort Address, byte Value)
        {
            switch (Address)
            {
                // Pulse channel 1
                case 0x4000:
                    // SQ1_VOL
                    sq1_duty = (byte)(Value >> 6);
                    sq1_loop = (Value & 0x20) != 0;
                    sq1_constant = (Value & 0x10) != 0;
                    sq1_vol = (byte)(Value & 0xF);
                    break;
                case 0x4001:
                    // SQ1_SWEEP
                    sq1_sweepEnabled = (Value & 0x80) != 0;
                    sq1_sweepPeriod = (byte)((Value & 0xB0) >> 4);
                    sq1_sweepNegate = (Value & 0x8) != 0;
                    sq1_sweepShift = (byte)(Value & 0xB);
                    break;
                case 0x4002:
                    // SQ1_LO
                    sq1_timer_lo = Value;
                    break;
                case 0x4003:
                    // SQ1_HI
                    if (apuEnable_sq1)
                    {
                        sq1_lengthCounter = LengthTable[Value >> 3];
                    }
                    sq1_timer_hi = (byte)(Value & 0xB);
                    break;

                // Pulse channel 2
                case 0x4004:
                    // SQ2_VOL
                    sq2_duty = (byte)(Value >> 6);
                    sq2_loop = (Value & 0x20) != 0;
                    sq2_constant = (Value & 0x10) != 0;
                    sq2_vol = (byte)(Value & 0xF);
                    break;
                case 0x4005:
                    // SQ2_SWEEP
                    sq2_sweepEnabled = (Value & 0x80) != 0;
                    sq2_sweepPeriod = (byte)((Value & 0xB0) >> 4);
                    sq2_sweepNegate = (Value & 0x8) != 0;
                    sq2_sweepShift = (byte)(Value & 0xB);
                    break;
                case 0x4006:
                    // SQ2_LO
                    sq2_timer_lo = Value;
                    break;
                case 0x4007:
                    // SQ2_HI
                    if (apuEnable_sq2)
                    {
                        sq2_lengthCounter = LengthTable[Value >> 3];
                    }
                    sq2_timer_hi = (byte)(Value & 0xB);
                    break;

                // Triangle channel
                // TRI has no sweep function
                case 0x4008:
                    // TRI_LINEAR
                    tri_count = (Value & 0x80) != 0;
                    tri_linear = (byte)(Value & 0xBF);
                    break;
                case 0x400A:
                    // TRI_LO
                    tri_timer_lo = Value;
                    break;
                case 0x400B:
                    // TRI_HI
                    if (apuEnable_Tri)
                    {
                        tri_lengthCounter = LengthTable[Value >> 3];
                    }
                    tri_timer_hi = (byte)(Value & 0xB);

                    break;

                // Noise channel
                // Nor does NOISE
                case 0x400C:
                    // NOISE_VOL
                    noise_loop = (Value & 0x20) != 0;
                    noise_constant = (Value & 0x10) != 0;
                    noise_vol = (byte)(Value & 0xF);
                    break;
                case 0x400E:
                    // NOISE_LO
                    noise_mode = (Value & 0x80) != 0;
                    noise_period = (byte)(Value & 0xF);
                    break;
                case 0x400F:
                    // NOISE_HI
                    if (apuEnable_Noise)
                    {
                        noise_lengthCounter = LengthTable[Value >> 3];
                    }
                    break;
            }
        }

        public void RunAPUFrame()
        {
            // Value which all the channels will be added to
            //float this_APU_frame = 0;

            apuCycle = !apuCycle;
            if (apuCycle)
            {
                apuFrameCycle++;
            }

            if (apuFrameResetCountdown > 0)
            {
                apuFrameResetCountdown--;
                if (apuFrameResetCountdown == 0)
                {
                    apuFrameCycle = 0;
                }
            }

            // Update the frame counter if that needs to happen
            if (NTSC)
            {
                switch (apuFrameCycle)
                {
                    case 3728:
                        APUFrameQuarterStep();
                        break;
                    case 7456:
                        APUFrameQuarterStep();
                        APUFrameHalfStep();
                        break;
                    case 11185:
                        APUFrameQuarterStep();
                        break;
                    case 14913:
                        if (!apuFrameCounterMode)
                        {
                            apuFlag_frameInterrupt = !apuFlag_IRQInhibit;
                        }
                        break;
                    case 14914:
                        if (!apuFrameCounterMode)
                        {
                            APUFrameQuarterStep();
                            APUFrameHalfStep();
                            apuFlag_frameInterrupt = !apuFlag_IRQInhibit;
                            IRQLevelDetector |= apuFlag_frameInterrupt;
                        }
                        break;
                    case 14915:
                        if (!apuFrameCounterMode)
                        {
                            apuFrameCycle = 0;
                            apuFlag_frameInterrupt = !apuFlag_IRQInhibit;
                            IRQLevelDetector |= apuFlag_frameInterrupt;
                        }
                        break;
                    case 18640:
                        APUFrameQuarterStep();
                        APUFrameHalfStep();
                        break;
                    case 18641:
                        apuFrameCycle = 0;
                        break;
                    default:
                        break;
                }
            }

            // Pulse channel 1
            if (sq1_lengthCounter > 0 && sq1_vol > 0 && apuEnable_sq1)
            {
                uint sq1_freq = (uint)(sq1_timer_lo | (sq1_timer_hi << 8));
                if (sq1_ftime > sq1_freq)
                {
                    sq1_ftime = 0;
                }
                // this_APU_frame += GetPulseChannel(sq1_duty, sq1_freq, sq1_ftime) * (sq1_vol / 15f) * 0.2f;
                sq1_ftime++;
            }

            // DMC
            /*if (apuDMCBytesRemaining > 0 || apuDMCLoops)
            {
                if (DMCCountdown == 0)
                {
                    // A change occurs
                    apuDMCBuffer >>= 1;
                    apuDMCBufferStep++;
                    if (apuDMCBufferStep == 8)
                    {
                        apuDMCBytesRemaining--;
                        apuDMCSampleAddress++;
                        if (apuDMCBytesRemaining == 0 && apuDMCLoops)
                        {
                            Begin_DMC();
                        }
                        else if (apuDMCBytesRemaining == 0)
                        {
                            if (apuFlag_IRQEnable)
                            {
                                // Perform an IRQ
                                apuFlag_DMCInterrupt = true;
                                IRQLevelDetector |= apuFlag_DMCInterrupt;
                            }
                        }
                        apuDMCRequired = true;
                        apuDMCDMAStep = 0;
                        apuDMCBufferStep = 0;
                    }
                    DMCCountdown = DMCRate[apuDMCFrequency];
                }
                DMCCountdown--;
            }*/

            // Send the APU frame off to the buffer to be processed later
            //rawAPUBuffer[apuFrameIndex] = (byte)this_APU_frame;
            apuFrameIndex++;
        }
        void APUFrameQuarterStep()
        {
            // Envelopes
            // Also linear counter
            if (tri_linear > 0)
            {
                tri_linear--;
            }
        }

        void APUFrameHalfStep()
        {
            // Length counters
            Clock_Length_Counters();
            // Sweep units
        }

        void Clock_Length_Counters()
        {
            if (sq1_lengthCounter > 0 && !sq1_loop)
            {
                sq1_lengthCounter--;
            }

            if (sq2_lengthCounter > 0 && !sq2_loop)
            {
                sq2_lengthCounter--;
            }

            if (tri_lengthCounter > 0 && !tri_count)
            {
                tri_lengthCounter--;
            }

            if (noise_lengthCounter > 0 && !noise_loop)
            {
                noise_lengthCounter--;
            }
        }
    }
}
