using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface ICleanFrequencyRepository
    {
        Task<IEnumerable<CleanFrequencyIndexViewModel>> GetCleanFrequencyIndexAsync(
                string? name, string? description, string? code, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<CleanFrequency?> GetCleanFrequencyByIdAsync(int id);
        Task AddCleanFrequencyAsync(CleanFrequency cleanFrequency, string createdById);
        Task UpdateCleanFrequencyAsync(CleanFrequency cleanFrequency, string updatedById);
        Task SoftDeleteCleanFrequencyAsync(int id, string deletedById);
    }
}