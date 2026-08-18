using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace FlowStateBlazor.Data.Services
{
    public class DefaultServiceTWithIdAndName<T> : DefaultServiceTWithId<T> where T : class, IIdAndNamed
    {
        public DefaultServiceTWithIdAndName(FlowStateContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                return _dbContext.Set<T>().AnyAsync(l => l.Name == name, cancellationToken);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        public Task<T?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                return _dbContext.Set<T>().FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

    }
}
