using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreMvcUnitTest.MVC.Data.Impl
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();

        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            // state modified olarak işaretlendiğinde ve saveChanges metodu çağırıldığında ilgili entity nin tüm property lerini güncelleyecek sorguyu db ye gönderir.
            _context.Entry(entity).State = EntityState.Modified;

            //aşağıdaki update metodu çağırıldığında ise sadece değişen property nin güncelleneceği sorguyu hazırlar. Performans açısından bu daha faydalıdır 
            //_context.Update(entity);

            _context.SaveChanges();
        }
    }
}
