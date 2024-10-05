using InventoryHub.Context;
using InventoryHub.ServicesPatterns.Entities;
using InventoryHub.ServicesPatterns.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync(); 
        

        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        
        public async Task<bool> AddAsync(T entity)
        {
            try
            {
                if(entity is IAuditableEntity entityRepository)
                {
                    entityRepository.CreatedAt = DateTime.Now;
                    //entityRepository.DeletedByUserId = UserId;

                }

                _dbSet.Add(entity);
                return await Save();
            }
            catch (Exception ex)
            {
                // سجل الخطأ أو أعد إرسال رسالة تفصيلية
                throw new Exception("Error occurred while adding the entity: " + ex.Message);
            }
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Save();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            bool saveStatus = false;
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                if(entity is IAuditableEntity entityRepository)
                {
                    entityRepository.IsDeleted = true;  
                    entityRepository.DeletedAt = DateTime.Now;
                    //entityRepository.DeletedByUserId = UserId;
                }
                saveStatus =  await Save();
                return saveStatus;
            }
            return saveStatus;
        }
        public async Task<IEnumerable<T>> GetAllWithIncludeAsync(Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();//.FirstOrDefaultAsync(e => EF.Property<int>(e, "ProductId") == id);
        }
        public async Task<bool> Save() => (await _context.SaveChangesAsync() > 0) ? true : false;

       
    }
}
