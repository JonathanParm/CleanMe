using CleanMe.Application.DTOs;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Common;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Enums;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Infrastructure.Repositories
{
    public class AssetLocationRepository : IAssetLocationRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IDapperRepository _dapperRepository;

        public AssetLocationRepository(IConfiguration configuration, ApplicationDbContext context, IDapperRepository dapperRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<AssetLocationIndexViewModel>> GetAssetLocationIndexAsync(
                string? areaName, string? description, string? townSuburb, string? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            try
            {
                var sql = "EXEC dbo.AssetLocationGetIndexView @AreaName, @Description, @TownSuburb, @ReportCode, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    AreaName = areaName,
                    Description = description,
                    TownSuburb = townSuburb,
                    ReportCode = reportCode,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                using var connection = new SqlConnection(_connectionString);
                return await connection.QueryAsync<AssetLocationIndexViewModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching AssetLocations from stored procedure", ex);
            }
        }

        public async Task<AssetLocationViewModel?> GetAssetLocationViewModelByIdAsync(int assetLocationId)
        {
            var sql = "dbo.AssetLocationGetById";
            var parameters = new { assetLocationId = assetLocationId };

            var results = await _dapperRepository.QueryAsync<AssetLocationWithAddressDto>(sql, parameters, CommandType.StoredProcedure);
            var row = results.FirstOrDefault();
            if (row == null) return null;

            // Convert `AssetLocation` entity to `AssetLocationViewModel`
            return new AssetLocationViewModel
            {
                assetLocationId = row.assetLocationId,
                Description = row.Description,
                areaId = row.areaId,
                Address = new AddressViewModel
                {
                    Line1 = row.AddressLine1,
                    Line2 = row.AddressLine2,
                    Suburb = row.AddressSuburb,
                    TownOrCity = row.AddressTownOrCity,
                    Postcode = row.AddressPostcode
                },
                SequenceOrder = row.SequenceOrder,
                SeqNo = row.SeqNo,
                ReportCode = row.ReportCode,
                AccNo = row.AccNo,
                IsActive = row.IsActive
            };
        }

        //public async Task<AssetLocationViewModel?> GetAssetLocationViewModelByIdAsync(int assetLocationId)
        //{
        //    var assetLocation = await _context.AssetLocations
        //        .Include(al => al.Area)
        //        .FirstOrDefaultAsync(al => al.assetLocationId == assetLocationId);

        //    if (assetLocation == null)
        //    {
        //        return null; // No match found
        //    }

        //    // Convert `AssetLocation` entity to `AssetLocationViewModel`
        //    return new AssetLocationViewModel
        //    {
        //        assetLocationId = assetLocation.assetLocationId,
        //        Description = assetLocation.Description,
        //        areaId = assetLocation.areaId,
        //        AreaName = assetLocation.Area?.Name,
        //        SequenceOrder = assetLocation.SequenceOrder,
        //        SeqNo = assetLocation.SeqNo,
        //        //ReportCode = assetLocation.ReportCode,
        //        IsActive = assetLocation.IsActive
        //    };
        //}

        public async Task<AssetLocationViewModel> PrepareNewAssetLocationViewModelAsync(int areaId)
        {
            var area = await _context.Areas
                .Where(a => a.areaId == areaId)
                .Select(a => new { a.areaId, a.Name })
                .FirstOrDefaultAsync();

            //if (area == null)
            //    throw new NotFoundException("Area not found.");

            return new AssetLocationViewModel
            {
                areaId = area.areaId,
                AreaName = area.Name,
                IsActive = true
            };
        }

        public async Task AddAssetLocationAsync(AssetLocation assetLocation, string createdById)
        {
            await _context.AssetLocations.AddAsync(assetLocation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssetLocationAsync(AssetLocation assetLocation, string updatedById)
        {
            _context.AssetLocations.Update(assetLocation);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAssetLocationAsync(int id, string deletedById)
        {
            var assetLocation = await _context.AssetLocations.FindAsync(id);
            if (assetLocation != null)
            {
                _context.AssetLocations.Remove(assetLocation);
                await _context.SaveChangesAsync();
            }
        }
    }
}
