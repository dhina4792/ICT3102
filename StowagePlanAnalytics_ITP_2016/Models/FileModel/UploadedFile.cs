using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models.FileModel
{
    public class UploadedFile
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }

        public UploadedFile()
        {
            FileName = null;
            FileContent = null;
        }

        public UploadedFile(HttpPostedFileBase file)
        {
            // Create a Binary Reader for the file input stream
            var reader = new System.IO.BinaryReader(file.InputStream);
            // Create byte array to store all data from file, using binary reader to read bytes
            FileName = file.FileName;
            FileContent = reader.ReadBytes(file.ContentLength);
        }

        public string FileBytesToString(System.Text.Encoding encoding)
        {
            return encoding.GetString(FileContent);
        }

        public string FileBytesToHexString()
        {
            var hex = new System.Text.StringBuilder(FileContent.Length * 2);
            foreach (byte b in FileContent)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}