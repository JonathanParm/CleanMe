using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Infrastructure.Repositories
{
    public class CleanFrequencyRepository : ICleanFrequencyRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IDapperRepository _dapperRepository;

        public CleanFrequencyRepository(IConfiguration configuration, ApplicationDbContext context, IDapperRepository dapperRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<CleanFrequency>> GetAllCleanFrequenciesAsync()
        {
            return await _context.CleanFrequencies
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<CleanFrequency?> GetCleanFrequencyByIdAsync(int cleanFrequencyId)
        {
            return await _context.CleanFrequencies.FindAsync(cleanFrequencyId);
        }

        public async Task AddCleanFrequencyAsync(CleanFrequency cleanFrequency)
        {
            await _context.CleanFrequencies.AddAsync(cleanFrequency);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCleanFrequencyAsync(CleanFrequency cleanFrequency)
        {
            _context.CleanFrequencies.Update(cleanFrequency);
            await _context.SaveChangesAsync();
        }
    }
}
