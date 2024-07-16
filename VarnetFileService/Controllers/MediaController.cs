
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using VarnetFileService.Auth;
using VarnetFileService.BO;
using VarnetFileService.constant;
using VarnetFileService.Entity;
using VarnetFileService.Models;
using VarnetFileService.helper;

using WebApplication3.Auth;

namespace VarnetFileService.Controllers
{
    [EnableCors("*", "*", "*")]
    public class MediaController : ApiController
    {

        [JwtAuth("VFS_MO_UPLOAD")]
        [HttpPost]
        [ActionName("UploadMedia")]
        //[Authorize]
        public HttpResponseMessage UploadMedia()
        {

        
            // Get the token from the request header
           //string token = httpContext.Request.Headers["Authorization"];

            // Verify token authorization
           // string username = UserValidateToken.OpenToken(token);




            
            HttpResponseMessage httpResponseMessage = null;
            var httpRequest = HttpContext.Current.Request;

            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,MediaTypeConstantHelper.UNSUPPORTED_MEDIA_TYPE);
            }

            //string application_code = httpRequest.Form.Get("application_code");
            //string tanant_code = httpRequest.Form.Get("tanant_code");
            //int user_id =Convert.ToInt32(httpRequest.Form.Get("user_id"));

            int user_id = Convert.ToInt32(((System.Security.Claims.ClaimsPrincipal)HttpContext.Current.User).Claims.Where(x => x.Type == "azp").First().Value);

            HttpResponseEntity httpResponseEntity = new HttpResponseEntity();

            try
            {

                if (httpRequest.Files.Count > 0)
                {
                    var postedFile = httpRequest.Files[0];
                    var originalFileName = postedFile.FileName;
                    var extension = Path.GetExtension(originalFileName);
                    var fileName = $"{Guid.NewGuid().ToString()}_{postedFile.FileName}";
                    var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/uploads"), fileName);




                    try
                    {
                        postedFile.SaveAs(filePath);
                    }
                    catch (Exception e)
                    {
                        httpResponseEntity = new HttpResponseEntity();
                        httpResponseEntity.Status = HttpStatusCode.InternalServerError;
                        httpResponseEntity.Message = e.Message + e.InnerException;
                        httpResponseEntity.Data = e;
                        httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, httpResponseEntity);
                    }

                    try
                    {
                        filePath = FileEncryption.EncryptFile(filePath, filePath);
                        string fileClass = FileCategoryClassifier.GetFileCategory(extension);
                        if (fileClass == string.Empty)
                        {
                            httpResponseEntity.Status = HttpStatusCode.BadRequest;
                            httpResponseEntity.Message = MediaTypeConstantHelper.FILE_TYPE_NOT_SUPPORTED;
                             
                            httpResponseEntity.Data = null;// ContactBO.GetContacts();
                            httpResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest, httpResponseEntity);
                        }
                        else
                        {
                            MediaTable mediaTable = new MediaTable();
                            mediaTable.application_code = string.Empty;
                            mediaTable.tenant_code = string.Empty;
                            mediaTable.user_id = user_id;
                            mediaTable.file_path = filePath;
                            mediaTable.media_id=Guid.NewGuid().ToString();
                            mediaTable.file_nm = postedFile.FileName;

                            mediaTable.file_size = (int)FileHelper.GetFileSize(filePath);

                            mediaTable.mime_type= FileTypeResolver.GetContentTypeByExtension(Path.GetExtension(filePath));
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


                            httpResponseEntity.Status = HttpStatusCode.OK;
                            httpResponseEntity.Message = MediaTypeConstantHelper.SUCCESSFULLY_UPLOADED;
                            httpResponseEntity.Data = mediaTable.media_id;
                            httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, httpResponseEntity);
                        }
                    }
                    catch (Exception e)
                    {
                        httpResponseEntity = new HttpResponseEntity();
                        httpResponseEntity.Status = HttpStatusCode.InternalServerError;
                        httpResponseEntity.Message = e.Message + e.InnerException;
                        httpResponseEntity.Data = e;
                        httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, httpResponseEntity);
                    }

                }
                else
                {
                    httpResponseEntity.Status = HttpStatusCode.BadRequest;
                    httpResponseEntity.Message = MediaTypeConstantHelper.FILES_NOT_ATTACHED;
                    httpResponseEntity.Data = MediaTypeConstantHelper.FILES_NOT_ATTACHED;// ContactBO.GetContacts();
                    httpResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest, httpResponseEntity);
                }


            }
            catch (Exception e)
            {
                httpResponseEntity = new HttpResponseEntity();
                httpResponseEntity.Status = HttpStatusCode.InternalServerError;
                httpResponseEntity.Message = e.Message + e.InnerException;
                httpResponseEntity.Data = e.Message;
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, httpResponseEntity);

            }
            return httpResponseMessage;

        }

        [HttpGet]
        [ActionName("getmedia")]
        public HttpResponseMessage GetMedia(string media_id,string hash)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            var defaultNotfoundImage = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/defaultImage/how-to-fix-error-404-1.png"));
            try
            {
                //check token

                //end check token

                var media = MediaBO.GetMediaById(media_id);
                
                if (media.Any() && File.Exists(media[0].file_path))
                {
                    var mediaBytes = FileEncryption.DecryptFileAndGetBytes(media[0].file_path);//File.ReadAllBytes(media[0].file_path);
                    var mediaStream = new MemoryStream(mediaBytes);
                  
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(mediaStream)
                    };

                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(media[0].mime_type);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = media[0].file_nm != null ? media[0].file_nm : Guid.NewGuid().ToString() // Replace with your actual file name and extension
                    };
                    
                    httpResponseMessage = response;
                }
                else
                {
                    httpResponseMessage = getErrorImage();                   
                }
            }
            catch (Exception e)
            {

                httpResponseMessage = getErrorImage();
            }

            //var filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Images"), imageName);

            return httpResponseMessage;
        }
        [JwtAuth("VFS_MO_MERGE_MEDIA")]
        [HttpGet]
        [ActionName("mergeMedia")]
        public HttpResponseMessage mergeMedia(string media_ids)
        {
            HttpResponseEntity httpResponseEntity;
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            var defaultNotfoundImage = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/defaultImage/how-to-fix-error-404-1.png"));
            try
            {
                //check token

                //end check token

                var media = MediaBO.GetMediaByIds(media_ids);
                string id = MediaHelper.Mergepdfs(media.Select(x => x.file_path).ToList());
                
                httpResponseEntity = new HttpResponseEntity();
                httpResponseEntity.Status = HttpStatusCode.OK;
                httpResponseEntity.Message = "Merged successfully!";
                httpResponseEntity.Data =id;
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, httpResponseEntity);
            }
            catch (Exception e)
            {
                httpResponseEntity = new HttpResponseEntity();
                httpResponseEntity.Status = HttpStatusCode.InternalServerError;
                httpResponseEntity.Message = e.Message;
                httpResponseEntity.Data = e;
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, httpResponseEntity);
            }

            //var filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Images"), imageName);

            return httpResponseMessage;
        }


        private static HttpResponseMessage getErrorImage()
        {
            var mediaBytes = File.ReadAllBytes(Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/defaultImage/how-to-fix-error-404-1.png")));
            var mediaStream = new MemoryStream(mediaBytes);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(mediaStream)
            };

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png"); // Change the content type based on your image type

            return response;
        }




    }
}