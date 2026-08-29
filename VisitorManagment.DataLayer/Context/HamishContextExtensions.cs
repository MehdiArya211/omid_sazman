using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Context
{
    public static class HamiContextExtensions
    {
        public static string GetSqlServerTableName<TEntity>(this DbContext context) where TEntity : class, new()
        {
            var metaData = context.Model
                .FindEntityType(typeof(TEntity).FullName);

            var schema = string.IsNullOrEmpty(metaData.GetSchema()) ? "dbo" : metaData.GetSchema();
            return $"{schema}.{metaData.GetTableName()}";
        }
    }
}
