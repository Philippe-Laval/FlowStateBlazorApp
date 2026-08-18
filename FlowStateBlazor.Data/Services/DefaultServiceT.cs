using FlowStateBlazor.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace FlowStateBlazor.Data.Services
{
    public class DefaultServiceT<T> where T : class
    {
        protected FlowStateContext _dbContext;

        public DefaultServiceT(FlowStateContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Insert many objects using a transaction and batching to improve performance.
        /// Change tracking off during load.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="batchSize"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task InsertManyAsync(IEnumerable<T> items, int batchSize = 5_000, CancellationToken ct = default)
        {
            // 1) Change tracking off during load
            var oldAutoDetect = _dbContext.ChangeTracker.AutoDetectChangesEnabled;
            _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            // 2) One transaction for the whole load (or per chunk if very large)
            await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                foreach (var batch in items.Chunk(batchSize))
                {
                    _dbContext.AddRange(batch);
                    // Don't call AcceptAllChanges automatically (slightly faster)
                    await _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: false, ct);

                    // Clear tracking so memory doesn't balloon
                    _dbContext.ChangeTracker.Clear();
                }

                await tx.CommitAsync(ct);
            }
            finally
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = oldAutoDetect;
            }
        }


        public async Task AddAsync(T instance, CancellationToken cancellationToken = default)
        {
            try
            {
                _dbContext.Set<T>().Add(instance);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        public async Task AddRangeAsync(T[] instances, CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.Set<T>().AddRangeAsync(instances);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        public async Task UpdateAsync(T instance, CancellationToken cancellationToken = default)
        {
            try
            {
                _dbContext.Set<T>().Update(instance);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        public Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return _dbContext.Set<T>().ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return _dbContext.Set<T>().CountAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        public async Task RemoveAsync(T instance, CancellationToken cancellationToken = default)
        {
            try
            {
                _dbContext.Set<T>().Remove(instance);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        public async Task RemoveAllAsync(bool useCustomSql = false, CancellationToken cancellationToken = default)
        {
            try
            {

                if (useCustomSql)
                {
                    try
                    {
                        string tableName = GetTableName();
                        string sql = $"DELETE FROM {tableName}";
                        await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        // Take the long way
                        await RemoveAllAsync(false, cancellationToken).ConfigureAwait(false);
                    }
                }
                else
                {
                    // Added in EF Core 7.0
                    await _dbContext.Set<T>().ExecuteDeleteAsync(cancellationToken);

                    //T? instance;
                    //
                    //do
                    //{
                    //    instance = await _dbContext.Set<T>().FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    //    if (instance != null)
                    //    {
                    //        _dbContext.Set<T>().Remove(instance);
                    //        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    //    }
                    //
                    //} while (instance != null);
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }

        private string GetTableName()
        {
            // https://stackoverflow.com/questions/45667126/how-to-get-table-name-of-mapped-entity-in-entity-framework-core

            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            //var schema = entityType.GetSchema();
            var tableName = entityType!.GetTableName();

            return tableName!;
        }


    }
}
