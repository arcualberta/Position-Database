using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PD.Test.Db
{
    public class SqlServerDb
    {
        public ApplicationDbContext Db { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public SqlServerDb()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            Configuration = config;

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(config.GetConnectionString("DefaultConnection"));

            Db = new ApplicationDbContext(builder.Options);
        }
    }
}
