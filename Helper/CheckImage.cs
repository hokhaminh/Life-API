using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Life_API.Helper
{
    public class CheckImage
    {
        public static Tuple<bool, string> CheckImageExtension(IFormFile file) => CheckFileExtension(file,
        new[] {
                    ".jpeg", ".png", ".jpg"
        },
        new[] {
                    "image/jpeg", "image/png"
        }
    );
        public static Tuple<bool, string> CheckFileExtension(IFormFile file, string[] fileTypes, string[] mimeTypes)
        {
            if (file == null || file.Length <= 0)
            {
                return Tuple.Create(false, "No upload file");
            }

            bool success;

            // Check file extension
            var fileType = Path.GetExtension(file.FileName);
            success = false;
            foreach (var type in fileTypes)
            {
                if (fileType.Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    success = true;
                    break;
                }
            }
            if (!success)
            {
                return Tuple.Create(false, "Not support file extension");
            }

            // Check MIME type
            Stream fs = file.OpenReadStream();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);
            var mimeType = HeyRed.Mime.MimeGuesser.GuessMimeType(bytes);
            success = false;
            foreach (var type in mimeTypes)
            {
                if (mimeType.Equals(type))
                {
                    success = true;
                    break;
                }
            }
            if (!success)
            {
                return Tuple.Create(false, "Not support fake extension");
            }

            return Tuple.Create(true, "Validated");
        }
    }
}
