using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Classes
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly long _MaxFileSize = 5 * 1024 * 1024;
        private readonly string[] _AllowedExtension = { ".png", ".jpeg", ".jpg" };

        public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public bool Delete(string FileName, string FolderName)
        {
            var filepath = Path.Combine(_environment.ContentRootPath, FolderName, FileName);
            try
            {
                if (!File.Exists(filepath)) return false;
                File.Delete(filepath);
                return true;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed TO Delete File");
                return false;
            }
        }


        //this to call in in controller
        public (Stream stream, string ContantType)? GetFile(string FileName, string FolderName)
        {
            if (string.IsNullOrWhiteSpace(FileName) || string.IsNullOrWhiteSpace(FolderName)) return null;


            var fullpath = Path.Combine(_environment.ContentRootPath, FolderName, FileName);
            if (!File.Exists(fullpath)) return null;
            var stream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
            var extenstion = Path.GetExtension(fullpath).ToLower();
            var contenttype = extenstion switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream" // binary data
            };
            
            return (stream, contenttype);
        }
        // call it in memberserice in create member
        public async Task<string?> UploadAsync(Stream FileStream, string FileName, string FolderName, CancellationToken ct = default)
        {
            if (FileStream is null || !FileStream.CanRead) return null;
            if (FileStream.Length == 0) return null;

            //check file size
            if (FileStream.Length > _MaxFileSize)
            {
                _logger.LogError($"File Rejected : Too Large {FileStream.Length} Bytes");
                return null;
            }

            // check extensions
            var extension = Path.GetExtension(FileName);
            if (string.IsNullOrWhiteSpace(extension) || !_AllowedExtension.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogError("File Rejected : This Extension Not Allowed");
                return null;

            }



            //locate folder{MembersPhoto} in pl

            //D:\.NET Core\MVC\Projects\MyGym\MyGym\MyGym\MyGym.PL.csproj
            // use contentrootpath => road you to .pl , by IWebHostEnviroment  inject it above   [from also microsoft.aspnetcore.app] in bll
            var uploadsfolder = Path.Combine(_environment.ContentRootPath, FolderName);
            //if not exist create it
            Directory.CreateDirectory(uploadsfolder);

            //filename = user.jpg
            // dfsdfdfd-sdfdsf-dfsdfUser.jpg
            var StoredFileName = $"{Guid.NewGuid()}_{FileName}";

            var FilePath = Path.Combine(uploadsfolder, StoredFileName);
            //D:\.NET Core\MVC\Projects\MyGym\MyGym\MyGym\MembersPhoto\PhotoName.jpg

            try
            {
                //using cause it unmanaged stream // filestream
                using var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                await FileStream.CopyToAsync(fs, ct);
                return StoredFileName;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed To Upload Photo");
                return null;

            }


        }
    }
}
