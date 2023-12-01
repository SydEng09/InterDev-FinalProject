using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesSystem.DAL;
using SalesSystem.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem
{
    public static class SaleExtentions
    {
        public static void AddSaleBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<eBikeContext>(options);

            services.AddTransient<SaleServices>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<eBikeContext>();
                return new SaleServices(context);
            });
        }
            }
}
