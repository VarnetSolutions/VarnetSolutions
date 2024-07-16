using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VarnetFileService.BO;
using VarnetFileService.Entity;
using VarnetFileService.Models;

namespace VarnetFileService.helper
{
    public class MediaHelper
    {
        public static string Mergepdfs(List<string> files)
        {
            // Create an output document
            PdfDocument outputDocument = new PdfDocument();
            var fileName = $"{Guid.NewGuid().ToString()}_merged.pdf";
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/uploads"), fileName);
            foreach (string file in files)
            {
                var mediaBytes = FileEncryption.DecryptFileAndGetBytes(file);//File.ReadAllBytes(media[0].file_path);
                var mediaStream = new MemoryStream(mediaBytes);
                // Open the document to import pages from it
                PdfDocument inputDocument = PdfReader.Open(mediaStream, PdfDocumentOpenMode.Import);

                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document
                    PdfPage page = inputDocument.Pages[idx];

                    // Add the page to the output document
                    outputDocument.AddPage(page);
                }
            }

            //return outputDocument.ToString();
            // Save the document
            //const string filename = "MergedDocument.pdf";
            outputDocument.Save(filePath);
            return EncryptAndSaveFile(filePath, fileName);
            //Console.WriteLine("Document merged successfully!");
        }

        public static string EncryptAndSaveFile(string filePath, string filename)
        {
            filePath = FileEncryption.EncryptFile(filePath, filePath);
            string fileClass = FileCategoryClassifier.GetFileCategory(".pdf");
            MediaTable mediaTable = new MediaTable();
            mediaTable.application_code = string.Empty;
            mediaTable.tenant_code = string.Empty;
            mediaTable.user_id = 0;
            mediaTable.file_path = filePath;
            mediaTable.media_id = Guid.NewGuid().ToString();
            mediaTable.file_nm = filename;

            mediaTable.file_size = (int)FileHelper.GetFileSize(filePath);

            mediaTable.mime_type = FileTypeResolver.GetContentTypeByExtension(Path.GetExtension(filePath));
            mediaTable.created_at = DateTime.Now;

            /*MediaUploadInternalEntity media = MediaBO.UploadMedia(filePath, 0, ContstantsHelper.MEDIA_FILE_USED_FOR_BULK_MESSAGE);
            if (media.MediaId.Contains("error"))
            {
                throw new Exception(media.MediaId);
            }

            //WARestClient.SendTextMessage(number, media.SaveMediaId, userId, fileClass, media.MediaId);
            httpResponseEntity.Message = media.SaveMediaId;
            httpResponseEntity.Data = media;
            httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, httpResponseEntity);*/
            // Save the new instance to the database

            using (var context = new filedev_varnetsolutions_dbEntities())
            {
                mediaTable = context.MediaTables.Add(mediaTable);
                context.SaveChanges();
            }

            return mediaTable.media_id;

        }
    }

}