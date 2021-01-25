using Localbanda.Models;
using System;
using System.Drawing;
using System.IO;

namespace Localbanda.Helpers
{
    public class ImageHelper
    {
        public static string UploadImage(ImageBase64 image, string folderPath, int PrimaryId, LocalbandaDbContext _context)
        {
            if (image != null && image.base64image != null && image.base64image != "" && image.fileName != null && image.fileExtention != null && image.fileExtention != "")
            {

                Guid obj = Guid.NewGuid();
                string ImageId = obj.ToString();
                string fileName = PrimaryId + "-" + ImageId + "." + image.fileExtention;

                try
                {


                    DirectoryInfo di = new DirectoryInfo(folderPath);
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string base64Extracted = image.base64image.Replace("data:image/png;base64,", String.Empty);
                    base64Extracted = base64Extracted.Replace("data:image/jpg;base64,", String.Empty);
                    base64Extracted = base64Extracted.Replace("data:image/jpeg;base64,", String.Empty);
                    base64Extracted = base64Extracted.Replace("data:image/bmp;base64,", String.Empty);
                    base64Extracted = base64Extracted.Replace("data:image/gif;base64,", String.Empty);
                    base64Extracted = base64Extracted.Replace("data:image/pdf;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(base64Extracted);


                    string file = Path.Combine(folderPath, fileName);
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }

                    try
                    {
                        if (bytes.Length > 0)
                        {
                            Stream stream = new MemoryStream(bytes);

                            //256
                            string Thumb_file = Path.Combine(folderPath, "thumb_" + fileName);

                            Image newImage = GetReducedImage(256, 256, stream);
                            newImage.Save(Thumb_file);

                            //128X
                            string Thumb_file128 = Path.Combine(folderPath, "thumb_128x128_" + fileName);
                            Image newImage128 = GetReducedImage(128, 128, stream);
                            newImage128.Save(Thumb_file128);

                            //64X
                            string Thumb_file64 = Path.Combine(folderPath, "thumb_64x64_" + fileName);
                            Image newImage64 = GetReducedImage(64, 64, stream);
                            newImage64.Save(Thumb_file64);

                            //32X
                            string Thumb_file32 = Path.Combine(folderPath, "thumb_32x32_" + fileName);
                            Image newImage32 = GetReducedImage(32, 32, stream);
                            newImage32.Save(Thumb_file32);

                        }
                    }
                    catch (Exception ex)
                    {
                        GNF.SaveException(ex, _context);
                    }




                }
                catch (Exception ex)
                {
                    GNF.SaveException(ex, _context);
                }
                return fileName;
            }
            else
            {
                return "";
            }
        }

        public static Image GetReducedImage(int width, int height, Stream resourceImage)
        {
            try
            {
                Image image = Image.FromStream(resourceImage);
                Image thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

                return thumb;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
