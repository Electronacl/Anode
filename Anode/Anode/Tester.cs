using System;
using System.Windows.Forms;

/*
Copyright © 2026 Electronacl

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the “Software”), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace Anode
{
    internal class Tester
    {
        // Taken from my old NES Emulator
        // 2_ReadWrite
        /*uint[,] ram_test = {
            { 0, 0x5A },
            { 1, 0x5A },
            { 2, 0x80 },
            { 0x550, 0x80 }
        };*/
        // 3_Branches & 4_TheStack
        /*uint[,] ram_test = {
            { 0, 1 }
        };*/
        // 5_Instructions1
        /*uint[,] ram_test = {
            { 0, 0x02 },
            { 1, 0x01 },
            { 2, 0xfd },
            { 3, 0x3d },
            { 4, 0x34 },
            { 5, 0x05 },
            { 6, 0x53 },
            { 7, 0x11 },
            { 8, 0x90 },
            { 9, 0xf0 },
            { 10, 0x70 },
            { 11, 0x01 },
            { 12, 0x01 }
        };*/
        // 6_Instructions2
        /*uint[,] ram_test = {
            { 0, 0x01 },
            { 1, 0x01 },
            { 2, 0x02 },
            { 3, 0x03 },
            { 4, 0x05 },
            { 5, 0x08 },
            { 6, 0x0d },
            { 7, 0x15 },
            { 8, 0x22 },
            { 9, 0x37 },
            { 10, 0x59 },
            { 11, 0x90 },
            { 12, 0xe9 },
            { 16, 0x48 },
            { 17, 0x65 },
            { 18, 0x6c },
            { 19, 0x6c },
            { 20, 0x6f },
            { 21, 0x20 },
            { 22, 0x77 },
            { 23, 0x6f },
            { 24, 0x72 },
            { 25, 0x6c },
            { 26, 0x64 },
            { 27, 0x21 },
            { 0x20, 0x00 },
            { 0x21, 0x01 },
            { 0x22, 0x02 },
            { 0x23, 0x03 },
            { 0x24, 0x04 },
            { 0x25, 0x05 },
            { 0x26, 0x06 },
            { 0x27, 0x07 },
            { 0x28, 0x08 },
            { 0x29, 0x09 },
            { 0x2A, 0x0A },
            { 0x2B, 0x0B },
            { 0x2C, 0x0C },
            { 0x2D, 0x0D },
            { 0x2E, 0x0E },
            { 0x2F, 0x0F },
            { 48, 0x80 },
            { 49, 0x10 },
            { 50, 0x90 },
            { 51, 0x01 }
        };*/
        // nestest.nes
        uint[,] ram_test = {
            { 0x2, 0x0 },
            { 0x3, 0x0 }
        };

        uint[] ram_list = { };
        uint fails = 0;
        uint passes = 0;


        public void Test_Ram(Emulator emulator)
        {
            for (int i = 0; i < ram_test.GetLength(0); i++)
            {
                if (emulator.RAM[ram_test[i, 0]] != ram_test[i, 1])
                {
                    Console.WriteLine($"Failed - address {ram_test[i, 0]} (0x{ram_test[i, 0]:X}) is 0x{emulator.RAM[ram_test[i, 0]]:X} instead of 0x{ram_test[i, 1]:X}");
                    fails++;
                }
                else
                {
                    Console.WriteLine($"Pass - address {ram_test[i, 0]} (0x{ram_test[i, 0]:X}) is 0x{ram_test[i, 1]:X}");
                    passes++;
                }
            }
            for (int i = 0; i < ram_list.Length; i++)
            {
                Console.WriteLine($"RAM View: 0x{ram_list[i]:X}: 0x{emulator.RAM[ram_list[i]]:X}");
            }

            string result = fails > 0 ? "failed" : "passed";
            Console.WriteLine($"Test {result}, with {passes} pass(es) and {fails} fail(s)");
            MessageBox.Show($"Test {result}, with {passes} pass(es) and {fails} fail(s).\nMore info in console",
                "Test results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
