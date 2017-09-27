using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.IO
{
    public class FileHelper
    {
        public static String FormatFileSize(Int64 fileSize)
        {
            if (fileSize < 0)
            {
                throw new ArgumentOutOfRangeException("fileSize");
            }
            else if (fileSize >= 1024 * 1024 * 1024)
            {
                return string.Format("{0} GB", Math.Round(fileSize / 1024.0 / 1024.0 / 1024.0, 2));
            }
            else if (fileSize >= 1024 * 1024)
            {
                return string.Format("{0} MB", Math.Round(fileSize / 1024.0 / 1024.0, 2));
            }
            else if (fileSize >= 1024)
            {
                return string.Format("{0} KB", Math.Round(fileSize / 1024.0, 2));
            }
            else
            {
                return string.Format("{0} B", fileSize);
            }
        }

    }
}
