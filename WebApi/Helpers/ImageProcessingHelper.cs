using Microsoft.AspNetCore.WebUtilities;
using SixLabors.ImageSharp.Formats.Webp;
using static System.Net.Mime.MediaTypeNames;

namespace WebApi.Helpers
{
    public static class ImageProcessingHelper
    {
        public static byte[] ResizeImage(byte[] bytes, int width, int height)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(bytes))
            {
                image.Mutate(x =>
                {
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(width, height),
                        Mode = ResizeMode.Max
                    });
                });
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, new WebpEncoder());
                    return ms.ToArray();
                }
            }
        }
    }
}