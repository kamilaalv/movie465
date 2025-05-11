using APP.Movies.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APP.Movies.Features
{
    public class MoviesDbHandler
    {
        private readonly MoviesDb _context;

        public MoviesDbHandler(MoviesDb context)
        {
            _context = context;
        }

        public async Task<T> Find<T>(int id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _context.Set<T>().AsQueryable();
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _context.Set<T>().Where(predicate);
        }

        public async Task<T> Add<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Update<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteById<T>(int id) where T : class
        {
            var entity = await Find<T>(id);
            if (entity == null)
                return false;

            return await Delete(entity);
        }
    }
}