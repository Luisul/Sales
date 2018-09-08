namespace Sales.Backend.Helpers
{
    using System.IO;
    using System.Web;
    public static class  FileHelpers
    {
        public static string UploadPhoto(HttpPostedFileBase file,string folder)
        {
            string path = string.Empty;
            string pic = string.Empty;

            if (file !=null)
            {
                pic = Path.GetFileName(file.FileName);
                path = Path.Combine(HttpContext.Current.Server.MapPath(folder), pic);
            }

            return pic;

        }
    }
}