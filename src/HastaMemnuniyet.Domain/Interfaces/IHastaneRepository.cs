using HastaMemnuniyet.Domain.Entities;

namespace HastaMemnuniyet.Domain.Interfaces;

/// <summary>
/// Hastane varlığına özgü veri erişim işlemlerini tanımlar.
/// </summary>
public interface IHastaneRepository : IGenericRepository<Hastane>
{
}
