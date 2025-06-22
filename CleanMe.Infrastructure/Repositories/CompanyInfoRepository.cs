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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Infrastructure.Repositories
{
    public class CompanyInfoRepository : ICompanyInfoRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyInfoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyInfo> GetCompanyInfoAsync()
        {
            return await _context.CompanyInfo
                .FirstOrDefaultAsync(c => c.companyInfoId == 1);
        }

        public async Task UpdateCompanyInfoAsync(CompanyInfo CompanyInfo)
        {
            if (CompanyInfo.companyInfoId == 0)
                throw new ArgumentException("Invalid companyInfoId for update.");

            _context.CompanyInfo.Update(CompanyInfo);
            await _context.SaveChangesAsync();
        }
    }
}
