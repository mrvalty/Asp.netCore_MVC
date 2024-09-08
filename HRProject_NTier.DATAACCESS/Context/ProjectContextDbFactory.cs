using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRProject_NTier.DATAACCESS.Context
{
    public class ProjectContextDbFactory : IDesignTimeDbContextFactory<ProjectContext>
    {
        public ProjectContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectContext>();
            
            optionsBuilder.UseSqlServer("Server=MERVE; database=HRManagementDB; Integrated Security = true;");
            return new ProjectContext(optionsBuilder.Options);
        }
    }
}
