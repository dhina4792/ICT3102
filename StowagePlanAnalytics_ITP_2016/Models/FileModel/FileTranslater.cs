using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.Models.FileModel
{
    public class FileTranslater
    {
        /**
         * This method determines the strategy to process files based on the defined translation strategies.
         * Translation strategies must be sub-classed or inheriting from the FileTranslationBase class.
         * @param files UploadedFile type files
         * @return returns null if file extension is undefined
         *         returns model object dependent on translation strategy deployed
         */
        public static object TranslateFiles(params UploadedFile[] files)
        {
            // Determine file type and check file type consistency for all files
            string fileType = GetFileTypeAndCheckExtensionConsistency(files);

            // If file type is not available or not consistent across files,
            if (fileType == null)
                return null; // exit function / throw exception?

            // Get file contents
            //string[] filesContents = GetFileContents(files);

            // Obtain list of file translation strategies
            IEnumerable<Type> translationStrategies = typeof(FileTranslationBase).Assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(FileTranslationBase)));

            foreach (Type translationStrategy in translationStrategies)
            {
                // Instantiate strategy class
                FileTranslationBase strategy = (FileTranslationBase)Activator.CreateInstance(translationStrategy);

                // If input file type matches strategy file type(s),
                if (strategy.fileExtensionType().Contains(fileType.ToUpper()))
                {
                    // Execute Translation Strategy
                    return strategy.Translate(files);   // Exit and return results from translation
                }
            }
            // Unknown file type
            throw new UnknownFileTypeException(fileType);
        }

        private static string GetFileTypeAndCheckExtensionConsistency(params UploadedFile[] files)
        {
            // Check if there is at least 1 file
            if (files.Length == 0)
                return null;

            // Set file iterator to last file index
            int fileIterator = files.Length - 1;
            // Get file type from first file
            string fileType = Path.GetExtension(files[0].FileName).ToUpper();
            // Check if file type is available
            if (fileType == null)
                return null;

            // While file iterator has not iterated through all files,
            while (fileIterator > 0)
            {
                // Get file types of iterator file and next file
                string fileTypeIteratorFile = Path.GetExtension(files[fileIterator].FileName).ToUpper();
                string fileTypeNextFile = Path.GetExtension(files[fileIterator - 1].FileName).ToUpper();

                // If file types do not match,
                if (!fileTypeIteratorFile.Equals(fileTypeNextFile))
                {
                    return null;    // Exit function and return null
                }
                fileIterator--;
            }
            // File types are consistent across all files, return file type
            return fileType;
        }

        public static UploadedFile[] HttpPostedFileBaseToUploadedFile(params HttpPostedFileBase[] files)
        {
            // Create list to store UploadedFile(s)
            List<UploadedFile> uploadedFileList = new List<UploadedFile>();

            // For each file,
            for (int i = 0; i < files.Length; i++)
            {
                var uploadedFile = new UploadedFile(files[i]);
                uploadedFileList.Add(uploadedFile);
            }
            return uploadedFileList.ToArray();
        }
    }
}