using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IAmendmentService
    {
        Task<IEnumerable<AmendmentViewModel>> FindDuplicateAmendmentAsync(int amendmentTypeId, int assetId, int? excludeamendmentId);

        Task<int?> GetAmendmentLastInvoicedOnByIdAsync(int amendmentId, int amendmentTypeId);
        Task<AmendmentViewModel?> GetAmendmentViewModelByIdAsync(int amendmentId);
        Task<AmendmentViewModel?> GetAmendmentViewModelOnlyByIdAsync(int amendmentId);
        //Task<AmendmentViewModel> PrepareNewAmendmentViewModelAsync(int regionId);
        Task<int> AddAmendmentAsync(AmendmentViewModel model, string addedById);
        Task UpdateAmendmentAsync(AmendmentViewModel model, string updatedById);
        Task<bool> SoftDeleteAmendmentAsync(int amendmentId, string updatedById);

        // Retrieves a paginated & filtered Amendment list using Dapper
        Task<IEnumerable<AmendmentIndexViewModel>> GetAmendmentIndexAsync(
                string? sourceName, string? amendTypeName, 
                string? clientName, string? areaName, string? locationName,
                string? mdReference, string? clientReference,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
    }
}