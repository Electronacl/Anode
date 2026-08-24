using System.Drawing;
using System.Drawing.Imaging;

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

namespace Anode.Common
{
    internal class Renderer
    {
        public Bitmap outputBitmap;
        Bitmap internalBitmap;
        BitmapData outputData;
        int stride;
        bool finishedFrame = true; // Juuust in case
        // Thanks to https://stackoverflow.com/questions/7768711/setpixel-is-too-slow-is-there-a-faster-way-to-draw-to-bitmap
        // for the code to speed up bitmap drawing

        public Renderer (int  width, int height)
        {
            internalBitmap = new Bitmap(width, height);
        }

        public void InitFrame()
        {
            if (!finishedFrame)
            {
                FinishFrame();
            }
            outputData = internalBitmap.LockBits(new Rectangle(0, 0, internalBitmap.Width, internalBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            stride = outputData.Stride;
            finishedFrame = false;
        }

        public void FinishFrame()
        {
            internalBitmap.UnlockBits(outputData);
            outputBitmap = Util.Clone(internalBitmap);
            finishedFrame = true;
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            unsafe
            {
                byte* ptr = (byte*)outputData.Scan0;
                ptr[(x * 3) + y * stride] = b;
                ptr[(x * 3) + y * stride + 1] = g;
                ptr[(x * 3) + y * stride + 2] = r;
            }
        }
    }
}
