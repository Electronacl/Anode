using Anode.Common;
using Anode.Cores.NES.Nessie.Cart;
using System;
using System.IO;
using System.Net;
using System.Windows.Input;

namespace Anode.Cores.NES.Nessie
{
    internal class NESIO
    {
        byte[] InternalROM;
        byte[] ROM = new byte[0x8000];
        public byte[] RAM = new byte[0x800]; // Random Access Memory

        byte[] HeaderedROM;
        byte cartSize;

        byte[] InternalCHRData;
        public byte[] CHRData = new byte[0x2000];
        public bool CHRDataUpdate;

        public byte[] Header = new byte[0x10]; // iNES header

        public bool compatible;

        public bool region;

        byte inesversion;
        ushort mapper;
        byte mapper_sub;
        byte nesversion;
        byte expansion;
        byte ext_nesversion;

        public bool OAMDMA;
        public byte OAMDMAInit;
        public byte OAMDMAAddr;
        public byte OAM_POS;

        // ----- Controller
        byte controller1;
        byte Controller1ShiftRegister;

        void Update_Controller()
        {
            // This function can still get input, even if the application is minimised.
            // Therefore, I need to check focus
            if (Util.ApplicationIsActivated())
            {
                // Hopefully I can make custom control schemes somewhen
                controller1 = 0;
                if (Keyboard.IsKeyDown(Key.X)) { controller1 |= 0x80; }
                if (Keyboard.IsKeyDown(Key.Z)) { controller1 |= 0x40; }
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) { controller1 |= 0x20; }
                if (Keyboard.IsKeyDown(Key.Enter)) { controller1 |= 0x10; }
                if (Keyboard.IsKeyDown(Key.Up)) { controller1 |= 0x08; }
                if (Keyboard.IsKeyDown(Key.Down)) { controller1 |= 0x04; }
                if (Keyboard.IsKeyDown(Key.Left)) { controller1 |= 0x02; }
                if (Keyboard.IsKeyDown(Key.Right)) { controller1 |= 0x01; }
            }
        }

        public void LoadCart(string path)
        {
            compatible = true;
            // Load the ROM from the file
            HeaderedROM = File.ReadAllBytes(path);
            cartSize = 0;

            if (HeaderedROM.Length < 16)
            {
                // The header is 16 bytes long
                compatible = false;
                Util.ThrowError("Corrupt cartridge error", "File too short to be a cartridge");
            }
            else
            {
                // Copy the header of the cart into a separate array
                Array.Copy(HeaderedROM, Header, 0x10);
                cartSize = Header[4]; // Amount of banks

                if (Header[0] != 0x4E || Header[1] != 0x45 || Header[2] != 0x53 || Header[3] != 0x1A)
                {
                    // iNES headers start with "NES<eof>"
                    compatible = false;
                    Util.ThrowError("Corrupt cartridge error", "No cartridge detected");
                }
                else
                {
                    if (HeaderedROM.Length < 0x4000 * cartSize + 0x10 + ((Header[5] != 0) ? 0x2000 : 0))
                    {
                        // Check that there's actually enough bytes available
                        compatible = false;
                        Util.ThrowError("Corrupt cartridge error", "Cart was too small for the size provided");
                    }
                }
            }
            if (!compatible)
            {
                return;
            }

            if ((Header[7] & 0x0C) == 0x0C)
            {
                // Presumably NES 2.0
                inesversion = 2;
            }
            else if ((Header[7] & 0x0C) == 0x04)
            {
                // Presumably archaic
                inesversion = 0;
            }
            else
            {
                // Presumably iNES or iNES 0.7 (or archaic). iNES is assumed.
                inesversion = 1;
            }

            // Get the mapper used
            // All iNES versions have the low nybble
            mapper = (byte)(Header[6] >> 4);
            if (inesversion >= 1)
            {
                // iNES 0.7 and later have a high nybble
                mapper |= (byte)(Header[7] & 0xF0);
            }
            if (inesversion == 2)
            {
                // NES 2.0 has another nybble and a subtype
                mapper |= (ushort)((Header[8] & 0xF) << 8);
                mapper_sub = (byte)(Header[8] >> 4);
            }

            // Check console used
            if (inesversion >= 1)
            {
                nesversion = (byte)(Header[7] & 0x3);
                // 0 = famicom/NES
                // 1 = Vs. System
                // 2 = PlayChoice 10
                // 3 = Extended console type
                if (nesversion == 3 && inesversion == 2)
                {
                    // Only available on NES 2.0
                    // More console types are available
                    ext_nesversion = (byte)(Header[13] & 0xF);
                    nesversion = ext_nesversion;
                }
            }

            if (inesversion == 2)
            {
                expansion = (byte)(Header[15] & 0x7F);
            }

            compatible = !CompatChecker.CheckCartCompat(mapper, expansion, nesversion, inesversion, Header);

            if (!compatible)
            {
                return;
            }

            InitBanks();

            // Auto detecting region
            if (inesversion == 1)
            {
                // In iNES, the region is in header 10.
                // This isn't supported by iNES 0.7 and isn't widely used either.
                region = (Header[10] & 0x2) == 0;
            }
            else if (inesversion == 2)
            {
                // When it's multi-region, NTSC is used as 60Hz is the standard for most monitors now, so that will be the standard.
                // Maybe add a user option for this though?
                region = (Header[12] & 0x3) != 1;
            }
            else
            {
                // Archaic iNES has absolutely no region support, so NTSC is just assumed
                region = true;
            }
        }

        private void InitBanks()
        {
            if (cartSize <= 2)
            {
                Array.Copy(HeaderedROM, 0x10, ROM, 0, 0x4000 * cartSize);
            }
            else
            {
                Array.Copy(HeaderedROM, 0x10, ROM, 0, 0x8000);
            }

            // Does the ROM support graphics?
            if (Header[5] != 0)
            {
                Array.Copy(HeaderedROM, 0x4000 * cartSize + 0x10, CHRData, 0, 0x2000); // Load graphics pattern data
            }
        }

        public byte ReadCPU(ushort AddressBus, byte DataBus)
        {
            if (AddressBus < 0x2000)
            {
                // Returns mirrored RAM
                return RAM[AddressBus & 0x7FF];
            }
            else if (AddressBus == 0x4016)
            {
                // Read from the controller
                byte controllerBit = (byte)((Controller1ShiftRegister & 0x80) >> 7);
                Controller1ShiftRegister <<= 1;
                controllerBit |= (byte)(DataBus & 0b11100000);
                return controllerBit;
            }
            else if (AddressBus == 0x4017)
            {
                // P2 controller isn't implemented, so it's just open bus in the upper bits and 0 in lower
                return (byte)(DataBus & 0b11100000);
            }
            else if (AddressBus >= 0x8000)
            {
                if (cartSize <= 2)
                {
                    return ROM[(AddressBus & 0x7FFF) & ((Header[4] * 0x4000) - 1)];
                }
            }
            return DataBus;
        }

        public void WriteCPU(ushort AddressBus, byte DataBus)
        {
            if (AddressBus < 0x2000)
            {
                // Write to RAM
                RAM[AddressBus & 0x7FF] = DataBus;
            }
            else if (AddressBus == 0x4014)
            {
                OAM_POS = DataBus;
                OAMDMAAddr = 0;
                OAMDMAInit = 0;
                OAMDMA = true;
            }
            else if (AddressBus == 0x4016)
            {
                // Controller write ("Joystick strobe")
                if ((DataBus & 1) != 0)
                {
                    Update_Controller();
                    Controller1ShiftRegister = controller1;
                }
            }
        }
    }
}
