using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace VarnetFileService.Entity
{
    public class FileCategoryClassifier
    {
        private static readonly Dictionary<string, string> FileExtensionToCategoryMap = new Dictionary<string, string>
    {
        // Audio
        { ".mp3", "audio" },
        { ".mpeg", "audio" },
        { ".aac", "audio" },
        { ".ogg", "audio" },
        // Documents
        { ".pdf", "document" },
        { ".doc", "document" },
        { ".docx", "document" },
        { ".ppt", "document" },
        { ".pptx", "document" },
        { ".xls", "document" },
        { ".xlsx", "document" },
        { ".txt", "document" },
        // Images
        { ".jpeg", "image" },
        { ".jpg", "image" },
        { ".png", "image" },
        { ".gif", "image" },
        { ".bmp", "image" },
        { ".webp", "image" },
        //{ ".webp", "sticker" },
        // Video
        { ".mp4", "video" },
        { ".3gpp", "video" },
    };
        public static string GetFileCategory(string fileExtension)
        {
            if (FileExtensionToCategoryMap.TryGetValue(fileExtension.ToLower(), out var category))
            {
                return category;
            }

            // Default to "unknown" for unsupported file extensions
            return "";
        }
    }

    public class FileTypeResolver
    {
        private static readonly Dictionary<string, string> FileExtensionToContentTypeMap = new Dictionary<string, string>
        {
        { ".aac", "audio/aac" },
        //{ ".mp4", "audio/mp4" },
        { ".mp3", "audio/mpeg" },
        { ".mpeg", "audio/mpeg" },
        { ".amr", "audio/amr" },
        { ".ogg", "audio/ogg" },
        { ".opus", "audio/opus" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".xls", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".pdf", "application/pdf" },
        { ".txt", "text/plain" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".bmp", "image/png" },
        { ".webp", "image/webp" },
        { ".mp4", "video/mp4" },
        { ".gif", "image/jpeg" },
        { ".3gp", "video/3gpp" }
    };

        /// <summary>
        /// This method returns content type based on extenstion value
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns>string</returns>
        public static string GetContentTypeByExtension(string fileExtension)
        {
            if (FileExtensionToContentTypeMap.TryGetValue(fileExtension.ToLower(), out var contentType))
            {
                return contentType;
            }

            // Default to binary/octet-stream for unknown file extensions
            return "";
        }
    }

    public class FileHelper
    {
        public static long GetFileSize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                return fileInfo.Length;
            }
            else
            {
                // File does not exist
                return -1;
            }
        }
    }
}