using HandiPapi.DataAccess;
using HandiPapi.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HandiPapi.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> db;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            db = context.Set<T>();
        }
        public async Task Delete(int Id)
        {
            var entity = await db.FindAsync(Id);
            if (entity != null)
                _ = db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            db.RemoveRange(entities);
        }

        public async Task<T?> Get(Expression<Func<T, bool>> expression, List<string>? include = null)
        {
            IQueryable<T> query = db;
            if(include != null)
            {
                foreach(var includeValue in include)
                {
                    query = query.Include(includeValue);
                }
            }
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IOrderedQueryable<T>>? ordeyBy = null, List<string>? include = null)
        {
            IQueryable<T> query = db;
            if(expression != null)
            {

                query = query.Where(expression);
                
            }

            if(include != null)
            {
                foreach(var includeValue in include)
                {
                    query = query.Include(includeValue);
                }
            }

            if(ordeyBy != null)
            {
                query = ordeyBy(query);
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
