using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WDXWebApiDespachoJuridico
{
    public class ModeloWDXDespacho : IdentityDbContext
    {
        public ModeloWDXDespacho()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                /*
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build()
                   */
                string server = "localhost";
                string database = "WDXDespachoMora";
                string user = "usr_despacho";
                string password = "Jerry200346602";
                string connection = $"Server={server};Database={database};user={user};password={password}";
                var connectionString = connection;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
