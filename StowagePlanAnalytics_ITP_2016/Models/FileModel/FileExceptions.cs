using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models.FileModel
{
    public class UnknownFileTypeException : Exception
    {
        public UnknownFileTypeException(string message) : base("Unknown File Type: " + message)
        {
        }
    }

    public class FileCorruptException : Exception
    {
        public FileCorruptException(string message) : base(message)
        {
        }
    }
}