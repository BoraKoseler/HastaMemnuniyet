using System.Linq.Expressions;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Tüm varlıklar için ortak veri erişim işlemlerini tanımlayan genel repository arayüzü.
/// </summary>
/// <typeparam name="T">Repository'nin yönettiği varlık tipi.</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>Verilen kimliğe sahip varlığı getirir.</summary>
    /// <param name="id">Varlık kimliği.</param>
    /// <returns>Bulunan varlık ya da null.</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>Tüm varlıkları getirir.</summary>
    /// <returns>Varlık listesi.</returns>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>Verilen koşula uyan varlıkları getirir.</summary>
    /// <param name="kosul">Filtreleme koşulu.</param>
    /// <returns>Koşula uyan varlık listesi.</returns>
    Task<IReadOnlyList<T>> BulAsync(Expression<Func<T, bool>> kosul);

    /// <summary>Verilen koşula uyan ilk varlığı getirir.</summary>
    /// <param name="kosul">Filtreleme koşulu.</param>
    /// <returns>Bulunan varlık ya da null.</returns>
    Task<T?> TekGetirAsync(Expression<Func<T, bool>> kosul);

    /// <summary>Yeni bir varlık ekler.</summary>
    /// <param name="varlik">Eklenecek varlık.</param>
    /// <returns>Eklenen varlık.</returns>
    Task<T> AddAsync(T varlik);

    /// <summary>Mevcut bir varlığı günceller.</summary>
    /// <param name="varlik">Güncellenecek varlık.</param>
    void Update(T varlik);

    /// <summary>Bir varlığı siler.</summary>
    /// <param name="varlik">Silinecek varlık.</param>
    void Delete(T varlik);

    /// <summary>Bekleyen değişiklikleri veritabanına kaydeder.</summary>
    /// <returns>Etkilenen kayıt sayısı.</returns>
    Task<int> SaveChangesAsync();
}
