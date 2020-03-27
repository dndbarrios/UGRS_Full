using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Core.Utility
{
    public class ImageUtility
    {
        public static BitmapImage LoadBitmapImageFromResource(Stream pObjData)
        {
            BitmapImage lObjBitmapImage = new BitmapImage();
            lObjBitmapImage.BeginInit();
            lObjBitmapImage.StreamSource = new MemoryStream(StreamToArray(pObjData));
            lObjBitmapImage.EndInit();
            lObjBitmapImage.Freeze();
            return lObjBitmapImage;
        }

        public static Image LoadImageFromResource(Stream pObjData)
        {
            return ArrayToImage(StreamToArray(pObjData));
        }

        private static byte[] StreamToArray(Stream pObjData)
        {
            using (Stream lObjStream = pObjData)
            {
                MemoryStream lObjBuffer = new MemoryStream();
                lObjStream.CopyTo(lObjBuffer);

                return lObjBuffer.ToArray();
            }
        }

        private static Image ArrayToImage(byte[] pArrBytData)
        {
            return (Bitmap)((new ImageConverter()).ConvertFrom(pArrBytData));
        }
    }
}
