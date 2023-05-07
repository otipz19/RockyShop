using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace RockyShop.Services
{
    public class ProductImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

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
            if(extension.IsNullOrEmpty())
                throw new ApplicationException("File name doesn't contain an extension");

            string newFileName = Path.Combine("images", "products", Guid.NewGuid().ToString() + extension);
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, newFileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }

            return "\\" + newFileName;
        }

        public void DeleteImage(string fileName)
        {
            string fullPath = Path.Join(_webHostEnvironment.WebRootPath, fileName);
            FileInfo fileInfo = new FileInfo(fullPath);
            if (fileInfo.DirectoryName.Equals(Path.Combine(_webHostEnvironment.WebRootPath, "images", "products")) &&
                fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }
    }
}
