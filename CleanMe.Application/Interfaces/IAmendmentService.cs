using CleanMe.Application.DTOs;
using CleanMe.Application.Helpers.Paging;
using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IAmendmentService
    {
        Task<IEnumerable<AmendmentViewModel>> FindDuplicateAmendmentAsync(int amendmentTypeId, int assetId, int? excludeamendmentId);

        Task<int?> GetAmendmentLastInvoicedOnByIdAsync(int amendmentId, int amendmentTypeId);
        Task<AmendmentViewModel?> GetAmendmentViewModelByIdAsync(int amendmentId);
        Task<AmendmentViewModel?> GetAmendmentViewModelOnlyByIdAsync(int amendmentId);

        Task<int> AddAmendmentAsync(AmendmentViewModel model, string addedById);
        Task UpdateAmendmentAsync(AmendmentViewModel model, string updatedById);
        Task<bool> SoftDeleteAmendmentAsync(int amendmentId, string updatedById);

        Task<AmendmentTypeHasFieldsDto> GetAmendmentTypeHasFieldsByIdAsync(int amendmentTypeId);

        //// Retrieves a paginated & filtered Amendment list using Dapper
        //Task<IEnumerable<AmendmentIndexViewModel>> GetAmendmentIndexAsync(
        //        string? sourceName, string? amendTypeName, 
        //        string? clientName, string? areaName, string? locationName,
        //        string? mdReference, string? clientReference,
        //        string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<PagedResult<AmendmentIndexViewModel>> GetPagedIndexAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortOrder,
            string? sourceName,
            string? amendmentTypeName,
            string? clientName,
            string? areaName,
            string? locationName,
            string? mdReference,
            string? clientReference
        );
    }
}