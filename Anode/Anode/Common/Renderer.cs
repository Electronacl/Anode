using System.Drawing;
using System.Drawing.Imaging;
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
