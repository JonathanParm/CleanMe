using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IAmendmentTypeRepository
    {
        Task<IEnumerable<AmendmentType>> GetAllAmendmentTypesAsync();
        Task<AmendmentType?> GetAmendmentTypeByIdAsync(int amendmentTypeId);
        //Task<AmendmentType?> GetAmendmentTypeWithAssetTypesByIdAsync(int amendmentTypeId);
        Task AddAmendmentTypeAsync(AmendmentType AmendmentType);
        Task UpdateAmendmentTypeAsync(AmendmentType AmendmentType);
    }
}