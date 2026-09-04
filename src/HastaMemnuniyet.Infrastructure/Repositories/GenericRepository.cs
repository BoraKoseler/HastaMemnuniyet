using System.Linq.Expressions;
using HastaMemnuniyet.Domain.Interfaces;
using HastaMemnuniyet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaMemnuniyet.Infrastructure.Repositories;

/// <summary>
/// Tüm varlıklar için ortak veri erişim işlemlerini EF Core ile uygulayan genel repository.
/// </summary>
/// <typeparam name="T">Repository'nin yönettiği varlık tipi.</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    /// <summary>EF Core veritabanı bağlamı.</summary>
    protected readonly AppDbContext Context;

    /// <summary>İlgili varlık kümesi.</summary>
    protected readonly DbSet<T> KayitSeti;

    /// <summary>Yeni bir <see cref="GenericRepository{T}"/> örneği oluşturur.</summary>
    /// <param name="context">EF Core veritabanı bağlamı.</param>
    public GenericRepository(AppDbContext context)
    {
        Context = context;
        KayitSeti = context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id)
        => await KayitSeti.FindAsync(id);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        => await KayitSeti.AsNoTracking().ToListAsync();

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> BulAsync(Expression<Func<T, bool>> kosul)
        => await KayitSeti.AsNoTracking().Where(kosul).ToListAsync();

    /// <inheritdoc />
    public virtual async Task<T?> TekGetirAsync(Expression<Func<T, bool>> kosul)
        => await KayitSeti.FirstOrDefaultAsync(kosul);

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T varlik)
    {
        await KayitSeti.AddAsync(varlik);
        return varlik;
    }

    /// <inheritdoc />
    public virtual void Update(T varlik)
        => KayitSeti.Update(varlik);

    /// <inheritdoc />
    public virtual void Delete(T varlik)
        => KayitSeti.Remove(varlik);

    /// <inheritdoc />
    public virtual async Task<int> SaveChangesAsync()
        => await Context.SaveChangesAsync();
}
