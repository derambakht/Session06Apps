using Infrastructure.Common;
using Infrastructure.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Utility;

public class FileUtility
{
    private readonly IHostingEnvironment webHostEnvironment;

    public FileUtility(IHostingEnvironment webHostEnvironment)
    {
        this.webHostEnvironment = webHostEnvironment;
    }

    public async Task<byte[]?> ConvertToByteArray(IFormFile file)
    {
        if (file.Length == 0) return null;
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            return stream.ToArray();
        }
    }

    public async Task<MellatActionResult<string>> SaveInDisk(IFormFile? file, string entityName, FileType fileType = FileType.Image)
    {   
        var result = new MellatActionResult<string>();

        if(file == null || file.Length == 0) {
            result.IsSuccess = false;
            result.Message = Messages.FIleEmpty;
        };

        //validation
        if(!CheckFileType(file.ContentType, fileType)) 
        {
            // throw new Exception("Invalid Fortmat Type");
            result.IsSuccess = false;
            result.Message = Messages.InvalidFileFormat;
            return result;
        }
        
        var rootPath = webHostEnvironment.ContentRootPath;
        var mediaFolderPath = Path.Combine(rootPath, "Media") ;
        var entityFolderPath = Path.Combine(mediaFolderPath, entityName);
        if(!Directory.Exists(entityFolderPath))
        {
            Directory.CreateDirectory(entityFolderPath);
        }

        string fileExtension = GetFileExtenstion(file.FileName);
        var newFileName = $"{Guid.NewGuid().ToString()}.{fileExtension}";

        var filePath = Path.Combine(entityFolderPath, newFileName);

        using(var stream = new FileStream(filePath, FileMode.Create))
        {
           await file.CopyToAsync(stream);
        }

        result.IsSuccess = true;
        result.Data =  newFileName;
        return result;
    }

    public string ConvertToBase64(byte[]? content)
    {
        if(content is null || content.Length ==0) return string.Empty;
        return Convert.ToBase64String(content);
    }

    private bool CheckFileType(string contentType, FileType fileType)
    {
       switch (fileType)
       {
            case FileType.Image :
            {
                if(contentType == "image/png" || contentType == "image/jpg") return true;
                return false;
            }
             case FileType.Document :
            {
                if(contentType == "word" || contentType == "pdf") return true;
                return false;
            }
       }

       return false;
    }

    private string GetFileExtenstion(string fileName)
    {
        var temp = fileName.Split('.');
        return temp[temp.Length - 1];
    }

    public string GenerateFileUrl(string fileName, string entityName)
    {
        return $"/Media/{entityName}/{fileName}";
    }
}