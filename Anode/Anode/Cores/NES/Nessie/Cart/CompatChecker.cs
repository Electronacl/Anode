using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anode.Cores.NES.Nessie.Cart
{
    internal class CompatChecker
    {
        public static bool CheckCartCompat(ushort mapper, byte expansion, byte nesversion, byte inesversion, byte[] Header)
        {
            // Code is a little funky as it's taken straight from PartialNES

            bool incompatible = false;
            // Check compatibility with the emulator
            if ((Header[6] & 2) != 0 && inesversion == 1)
            {
                // Header[6] denotes PRG RAM
                MessageBox.Show("This emulator is incompatible with PRG RAM cartridges.", "Compatibility error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                incompatible = true;
            }
            if (((Header[12] & 0x3) == 3) && inesversion == 2)
            {
                // I think only NES 2.0 is compatible with the dendy
                MessageBox.Show("This emulator is incompatible with the \"Dendy\" console.", "Compatibility error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                incompatible = true;
            }

            // Console type compat
            // If you're wondering why these don't just return incompatible, I leave them like this just in case the extended functionality isn't used or isn't important.
            // Although, that can cause unexpected behaviour in certain cases.
            // I have made some incompatible if I think they might not have a chance of working.
            switch (nesversion)
            {
                case 0:
                    // Standard NES is compatible
                    break;
                case 1:
                    MessageBox.Show(
                        "The cartridge loaded is a Vs. System game. These are coin op, so may not be able to start games",
                        "Compatibility warning: console", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    break;
                case 2:
                    MessageBox.Show(
                        "The cartridge loaded is a PlayChoice-10 game. Compatibility cannot be guarunteed.",
                        "Compatibility warning: console", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    break;
                case 3:
                    MessageBox.Show(
                        "The cartridge used is for a NES with decimal mode. This emulator does not support decimal mode, so calculations may be incorrect.",
                        "Compatibility warning: console", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    break;
                case 4:
                    MessageBox.Show(
                        "The cartridge used is a cartridge with EPSM or plug-through device, which are not supported by the emulator.",
                        "Compatibility error: console", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 0xA:
                    MessageBox.Show(
                        "The cartridge used is for a VTxx NES-on-a-chip. This emulator is not compatible with the extended function of these devices.",
                        "Compatibility warning: console", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    break;
                case 0xB:
                    MessageBox.Show(
                        "The cartridge used is for a UMC UM6578 famiclone. This emulator is not compatible with the extended function of the device.",
                        "Compatibility warning: console", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    break;
                case 0xC:
                    MessageBox.Show(
                        "The cartridge used is for the Famicom Network System. This emulator is not compatible with this device.",
                        "Compatibility error: console", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                default:
                    MessageBox.Show(
                        $"Unknown device compatibility: {nesversion:X}",
                        "Invalid console error", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
            }

            // Expansion compat
            switch (expansion)
            {
                case 0:
                case 1:
                    // standard controllers
                    break;
                case 2:
                    MessageBox.Show(
                        "Incompatible with the Four Score/Satellite",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 3:
                    MessageBox.Show(
                        "Incompatible with the 4 player addapter",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 4:
                case 5:
                    MessageBox.Show(
                        "Incompatible with the Vs. System input",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 6:
                    MessageBox.Show(
                        "Input 6 is reserved, and unknown.",
                        "Invalid expansion error", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 7:
                    MessageBox.Show(
                        "Incompatible with the Vs. Zapper",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 8:
                    MessageBox.Show(
                        "Incompatible with the Zapper",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 9:
                    MessageBox.Show(
                        "Incompatible with 2 Zappers",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0xA:
                    MessageBox.Show(
                        "Incompatible with the Hyper Shot Lightgun",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0xB:
                case 0xC:
                    MessageBox.Show(
                        "Incompatible with the Power Pad",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0xD:
                case 0xE:
                    MessageBox.Show(
                        "Incompatible with the Family Trainer",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0xF:
                case 0x10:
                    MessageBox.Show(
                        "Incompatible with the Arkanoid Vaus Controller",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x11:
                    MessageBox.Show(
                        "Incompatible with 2 Arkanoid Vaus Controllers and a Famicom Data Recorder",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x12:
                    MessageBox.Show(
                        "Incompatible with the Konami Hyper Shot",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x13:
                    MessageBox.Show(
                        "Incompatible with the Coconuts Pachinko",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x14:
                    MessageBox.Show(
                        "Incompatible with the Exciting Boxing Punching Bag",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x15:
                    MessageBox.Show(
                        "Incompatible with the Jissen Mahjong Controller",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x16:
                    MessageBox.Show(
                        "Incompatible with the Yonezawa Party Tap",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x17:
                    MessageBox.Show(
                        "Incompatible with the Oeka Kids Tablet",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x18:
                    MessageBox.Show(
                        "Incompatible with the Sunsoft Barcode Battler",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x19:
                    MessageBox.Show(
                        "Incompatible with the Miracle Piano Keyboard",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x1A:
                    MessageBox.Show(
                        "Incompatible with the Pokkun Maguraa Tap-tap Mat",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x1B:
                    MessageBox.Show(
                        "Incompatible with the Top Rider",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x1C:
                    MessageBox.Show(
                        "Incompatible with the Double Fisted",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x1D:
                    MessageBox.Show(
                        "Incompatible with the Famicom 3D System",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x1E:
                    MessageBox.Show(
                        "Incompatible with the Doremikko Keyboard",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x1F:
                    MessageBox.Show(
                        "Incompatible with the R.O.B Gyromite",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x20:
                    MessageBox.Show(
                        "Incompatible with the Famicom Data Recorder",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x21:
                    MessageBox.Show(
                        "Incompatible with the ASCII Turbo File",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x22:
                    MessageBox.Show(
                        "Incompatible with the IGS Storage Battle Box",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x23:
                    MessageBox.Show(
                        "Incompatible with the Family BASIC Keyboard plus Famicom Data Recorder",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x24:
                    MessageBox.Show(
                        "Incompatible with the Dongda PEC Keyboard",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x25:
                    MessageBox.Show(
                        "Incompatible with the Puze Bit-79 Keyboard",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x26:
                case 0x27:
                case 0x28:
                case 0x36:
                case 0x44:
                case 0x4D:
                case 0x4F:
                    MessageBox.Show(
                        "Incompatible with the Xiaobawang Keyboard (+ any mouse requirements)",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x29:
                case 0x48:
                    MessageBox.Show(
                        "Incompatible with the SNES Mouse",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x2A:
                    MessageBox.Show(
                        "Incompatible with Multicarts",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x2B:
                    MessageBox.Show(
                        "Incompatible with 2 SNES Controllers",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x2C:
                    MessageBox.Show(
                        "Incompatible with the RacerMate Bicycle",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x2D:
                    MessageBox.Show(
                        "Incompatible with the U-Force",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x2E:
                    MessageBox.Show(
                        "Incompatible with the ROB Stack-up",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x2F:
                    MessageBox.Show(
                        "Incompatible with the City Patrolman Lightgun",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x30:
                    MessageBox.Show(
                        "Incompatible with the Sharp C1 Cassette Interface",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x31:
                    MessageBox.Show(
                        "Incompatible with swapped input standard contollers",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x32:
                    MessageBox.Show(
                        "Incompatible with the Excalibur Sudoku Pad",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x33:
                    // I'm sorry, but:
                    // PINBALLLLLLLLL!!!!!!!

                    // i dobut many got that reference.
                    MessageBox.Show(
                        "Incompatible with ABL Pinball",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x34:
                    MessageBox.Show(
                        "Incompatible with the Golden Nugget Casino extra buttons",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x35:
                    MessageBox.Show(
                        "Incompatible with the Keda Keyboard",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x37:
                    MessageBox.Show(
                        "Incompatible with the Port test controller",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x38:
                    MessageBox.Show(
                        "Incompatible with the Bandai Multi Game Player Gamepad buttons",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x39:
                    MessageBox.Show(
                        "Incompatible with the Venom TV Dance Mat",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x40:
                    MessageBox.Show(
                        "Incompatible with the LG TV Remote Control",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x41:
                    MessageBox.Show(
                        "Incompatible with the Famicom Network Controller",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x42:
                    MessageBox.Show(
                        "Incompatible with the King Fishing Controller",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x43:
                    MessageBox.Show(
                        "Incompatible with the Yuxing mouse",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x45:
                    MessageBox.Show(
                        "Incompatible with the Giggle TV Pump",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x46:
                    MessageBox.Show(
                        "Incompatible with the Bubugao keyboard and PS/2 mouse",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x47:
                    MessageBox.Show(
                        "Incompatible with Magical Cooking",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                case 0x4E:
                    MessageBox.Show(
                        "Incompatible with the IBM PC/XT Keyboard",
                        "Compatibility error: expansion", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
                default:
                    MessageBox.Show(
                        $"Unknown expansion: {expansion:X}",
                        "Invalid expansion error", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
            }
            // Mapper compat
            switch (mapper)
            {
                case 0:
                    // NROM
                    break;
                default:
                    MessageBox.Show(
                        $"Incompatible with mapper: {mapper}",
                        "Compatibility error: mapper", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    incompatible = true;
                    break;
            }

            // Incorrect file format
            if (mapper == 0 && (0x4000 * Header[4]) > 0x8000)
            {
                // If an NROM is bigger than it should be
                incompatible = true;
                MessageBox.Show("ROM size is too big to be an NROM, but the cartridge registers as NROM.", "Corrupt cartridge error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return incompatible;
        }
    }
}
