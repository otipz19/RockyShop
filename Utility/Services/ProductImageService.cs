using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace RockyShop.Utility.Services
{
    public class ProductImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string _webFolderPath = "\\images\\products";
        private readonly IEnumerable<string> allowedFormats = new string[] { ".jpg", ".png", ".gif", ".bmp"};

        public ProductImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        ///<summary>Uploads given file to wwwroot</summary>
        /// <returns>Web path of uploaded image</returns>
        /// <exception cref="ApplicationException"></exception>
        public string UploadImage(IFormFile formFile)
        {
            string extension = Path.GetExtension(formFile.FileName);
            if (extension.IsNullOrEmpty())
                throw new ApplicationException("File name doesn't contain an extension");
            else if (!allowedFormats.Contains(extension))
                throw new ApplicationException("Not allowed format");

            string newFileName = Path.Join(_webFolderPath, Guid.NewGuid().ToString() + extension);
            string fullPath = Path.Join(_webHostEnvironment.WebRootPath, newFileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }

            return newFileName;
        }

        public void DeleteImage(string fileName)
        {
            string fullPath = Path.Join(_webHostEnvironment.WebRootPath, fileName);
            FileInfo fileInfo = new FileInfo(fullPath);
            string validFolderPath = Path.Join(_webHostEnvironment.WebRootPath, _webFolderPath);
            if (fileInfo.DirectoryName.Equals(validFolderPath) &&
                fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }
    }
}
