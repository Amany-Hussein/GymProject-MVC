using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<string?> UploadAsync(Stream FileStream, string FileName, string FolderName, CancellationToken ct = default);

        bool Delete(string FileName, string FolderName);
        (Stream stream, string ContantType)? GetFile(string FileName, string FolderName);
    }
}
