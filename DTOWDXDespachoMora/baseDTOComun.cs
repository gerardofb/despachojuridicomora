using System;
namespace DTOWDXDespachoMora
{
    public class baseDTOComun : IDisposable
    {
        private static string server = "localhost";
        private static string database = "WDXDespachoMora";
        private static string user = "usr_despacho";
        private static string password = "Jerry200346602";
        public static string Conexion = $"Server={server};Database={database};user={user};password={password}";

        private bool disposed;
        public baseDTOComun()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                
            }
        }
    }
}
