using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VarnetFileService.Models;

namespace VarnetFileService.DAO
{
    public class MediaDAO
    {
        public static List<MediaTable> GetMediaById(string media_id)
        {
            List<MediaTable> list = new List<MediaTable>();
            using (filedev_varnetsolutions_dbEntities db = new filedev_varnetsolutions_dbEntities())
            {
                list = db.MediaTables.Where(x=>x.media_id.Equals(media_id,StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return list;
        }

        public static List<sp_get_filepaths_by_media_ids_Result> GetMediaByIds(string media_ids)
        {
            List<sp_get_filepaths_by_media_ids_Result> list = new List<sp_get_filepaths_by_media_ids_Result>();
            using (filedev_varnetsolutions_dbEntities db = new filedev_varnetsolutions_dbEntities())
            {
                list = db.sp_get_filepaths_by_media_ids(media_ids).ToList();
            }
            return list;
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