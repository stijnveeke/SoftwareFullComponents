using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataModels;

namespace ProductComponent.Data
{
    public class ProductComponentContext : DbContext
    {
        public ProductComponentContext (DbContextOptions<ProductComponentContext> options)
            : base(options)
        {
        }

        public DbSet<DataModels.Product> Product { get; set; }
    }
}
