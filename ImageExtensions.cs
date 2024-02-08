using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Bitmap = Avalonia.Media.Imaging.Bitmap;


namespace AAgIO
{
	public static class ImageExtensions
	{
	        
		 public static Bitmap ConvertToAvaloniaBitmap(this Image bitmap)
		 {
		    Avalonia.Media.Imaging.Bitmap bitmap1;
		    if (bitmap == null)
		        return null;
		    System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap);
		    using (var stream = new MemoryStream())
            {
               bitmapTmp.Save(stream, ImageFormat.Png);
               stream.Position = 0;

               bitmap1 = new Avalonia.Media.Imaging.Bitmap(stream);
             }
		     return bitmap1;
		 }
	}
}

 
