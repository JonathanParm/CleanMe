using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IAmendmentTypeService
    {
        // Retrieves a paginated & filtered AmendmentType list using Dapper
        Task<IEnumerable<AmendmentTypeIndexViewModel>> GetAmendmentTypeIndexAsync(
                string? name, string? description, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);

        Task<IEnumerable<AmendmentTypeViewModel>> FindDuplicateAmendmentTypeAsync(string name, int? excludeamendmentTypeId);

        Task<AmendmentTypeViewModel?> GetAmendmentTypeViewModelByIdAsync(int amendmentTypeId);
        //Task<AmendmentTypeViewModel?> GetAmendmentTypeViewModelWithAmendmentsByIdAsync(int amendmentTypeId);
        Task<int> AddAmendmentTypeAsync(AmendmentTypeViewModel model, string addedById);
        Task UpdateAmendmentTypeAsync(AmendmentTypeViewModel model, string updatedById);
        Task<bool> SoftDeleteAmendmentTypeAsync(int amendmentTypeId, string updatedById);
    }
}