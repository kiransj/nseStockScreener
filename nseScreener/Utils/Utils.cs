using System;

namespace Helper
{
    public class URLFormater
    {
        private static Logger log = Logger.GetLoggerInstance();
        private DateTime date;
        private string dd, MM, yy, yyyy, MMM, ddMMyy, ddMMMyyyy;
        public URLFormater(DateTime date)
        {
            this.date = date;
            dd = date.ToString("dd");
            MM = date.ToString("MM");
            yy = date.ToString("yy");
            MMM = date.ToString("MMM").ToUpper();
            ddMMyy = date.ToString("ddMMyy").ToUpper();
            ddMMMyyyy = date.ToString("ddMMMyyyy").ToUpper();
            yyyy = date.ToString("yyyy");
        }

        //Convert to the URL in json file to a format that can be downloaded.
        public string normalizeUrl(string url)
        {
            var result = url;
                        
            result = result.Replace("{ddMMMyyyy}", ddMMMyyyy);
            result = result.Replace("{ddMMyy}", ddMMyy);
            result = result.Replace("{yyyy}", yyyy);
            result = result.Replace("{MMM}", MMM);
            result = result.Replace("{yy}", yy);
            result = result.Replace("{MM}", MM);
            result = result.Replace("{dd}", dd);
            return result;
        }

        public static string fileNameInURL(string url)
        {
            return System.IO.Path.GetFileName(url);
        }

        public static string getFileExtension(string fileName)
        {
            try
            {
                var extn = System.IO.Path.GetExtension(fileName);                
                return extn;
            }
            catch(Exception)
            {
                return string.Empty;
            }
        }
    }
}