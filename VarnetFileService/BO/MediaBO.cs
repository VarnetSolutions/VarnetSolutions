using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VarnetFileService.DAO;
using VarnetFileService.Models;

namespace VarnetFileService.BO
{
    public class MediaBO
    {
        public static List<MediaTable> GetMediaById(string media_id)
        {
            if(media_id!=null)
                return MediaDAO.GetMediaById(media_id);
            else
                return new List<MediaTable>();
        }

        public static List<sp_get_filepaths_by_media_ids_Result> GetMediaByIds(string media_ids)
        {
            return MediaDAO.GetMediaByIds(media_ids);
        }

        public static List<MediaTable> CreateMedia(MediaTable mediaTable)
        {
            List<MediaTable> list = new List<MediaTable>();
            using (filedev_varnetsolutions_dbEntities db = new filedev_varnetsolutions_dbEntities())
            {
                db.MediaTables.Add(mediaTable);
                db.SaveChanges();
            }
            return list;
        }
    }
}