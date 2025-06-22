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
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDbContext _context;

        public SettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Setting> GetSettingAsync()
        {
            return await _context.Settings
                .FirstOrDefaultAsync(c => c.settingId == 1);
        }

        public async Task UpdateSettingAsync(Setting Setting)
        {
            if (Setting.settingId == 0)
                throw new ArgumentException("Invalid settingId for update.");

            _context.Settings.Update(Setting);
            await _context.SaveChangesAsync();
        }
    }
}
