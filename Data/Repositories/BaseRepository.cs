using Data.Contexts;
using Business.Models;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace Data.Repositories;

public interface IBaseRepository<TEntity, T> where TEntity : class
{
    Task<RepositoryResult<bool>> AddAsync(TEntity entity);
    Task<RepositoryResult<bool>> DeleteAsync(TEntity entity);
    Task<RepositoryResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> findBy);
    Task<RepositoryResult<IEnumerable<T>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<IEnumerable<TEntity>>> GetAllRawAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes);

    Task<RepositoryResult<T>> GetAsync(Expression<Func<TEntity, bool>> where = null!, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<bool>> UpdateAsync(TEntity entity);
}

public abstract class BaseRepository<TEntity, T> : IBaseRepository<TEntity, T> where TEntity : class
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _table;

    protected BaseRepository(DataContext context)
    {
        _context = context;
        _table = _context.Set<TEntity>();
    }

    public virtual async Task<RepositoryResult<bool>> AddAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult<bool> { Succeeded = false, StatusCode = 400, Error = "Entity can't be null." };
        try
        {
            _table.Add(entity);
            await _context.SaveChangesAsync();
            Debug.WriteLine("Entity successfully saved.");
            return new RepositoryResult<bool> { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error in AddAsync: " + ex.Message);
            Debug.WriteLine("Stack trace: " + ex.StackTrace);
            return new RepositoryResult<bool> { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }



    public virtual async Task<RepositoryResult<IEnumerable<T>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _table;

        if (where != null)
            query = query.Where(where);

        if (includes != null && includes.Length != 0)
            foreach (var include in includes)
                query = query.Include(include);

        if (sortBy != null)
            query = orderByDescending
                ? query.OrderByDescending(sortBy)
                : query.OrderBy(sortBy);

        var entities = await query.ToListAsync();

        var result = entities.Select(entity => entity.MapTo<T>());
        return new RepositoryResult<IEnumerable<T>> { Succeeded = true, StatusCode = 200, Result = result };
    }

    public virtual async Task<RepositoryResult<IEnumerable<TEntity>>> GetAllRawAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _table;

        if (where != null)
            query = query.Where(where);

        if (includes != null && includes.Length != 0)
            foreach (var include in includes)
                query = query.Include(include);

        if (sortBy != null)
            query = orderByDescending
                ? query.OrderByDescending(sortBy)
                : query.OrderBy(sortBy);

        var entities = await query.ToListAsync();

        return new RepositoryResult<IEnumerable<TEntity>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = entities
        };
    }

    public virtual async Task<RepositoryResult<T>> GetAsync(Expression<Func<TEntity, bool>> where = null!, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _table;

        if (includes != null && includes.Length != 0)
            foreach (var include in includes)
                query = query.Include(include);

        var entity = await query.FirstOrDefaultAsync(where);
        if (entity == null)
            return new RepositoryResult<T> { Succeeded = false, StatusCode = 404, Error = "Entity not found." };

        var result = entity.MapTo<T>();
        return new RepositoryResult<T> { Succeeded = true, StatusCode = 200, Result = result };

    }

    public virtual async Task<RepositoryResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> findBy)
    {
        var exists = await _table.AnyAsync(findBy);
        return !exists
            ? new RepositoryResult<bool> { Succeeded = false, StatusCode = 404, Error = "Entity not found." }
            : new RepositoryResult<bool> { Succeeded = true, StatusCode = 200 };
    }
    public virtual async Task<RepositoryResult<bool>> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult<bool> { Succeeded = false, StatusCode = 400, Error = "Entity can't be null." };

        try
        {
            _table.Update(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult<bool> { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool> { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult<bool>> DeleteAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult<bool> { Succeeded = false, StatusCode = 400, Error = "Entity can't be null." };

        try
        {
            _table.Remove(entity);
            await _context.SaveChangesAsync();
            return new RepositoryResult<bool> { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool> { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }
}
