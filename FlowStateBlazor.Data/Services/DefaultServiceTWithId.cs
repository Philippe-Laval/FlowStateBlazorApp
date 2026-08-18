using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace FlowStateBlazor.Data.Services
{
    public class DefaultServiceTWithId<T> : DefaultServiceT<T> where T : class, IId
    {
        public DefaultServiceTWithId(FlowStateContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return _dbContext.Set<T>().AnyAsync(l => l.Id == id, cancellationToken);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        public Task<T?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return _dbContext.Set<T>().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

    }
}
