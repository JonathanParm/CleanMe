using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface IItemCodeRateRepository
    {
        Task<IEnumerable<ItemCodeRate>> GetAllItemCodeRatesAsync();
        Task<ItemCodeRate?> GetItemCodeRateByIdAsync(int itemCodeRateId);
        Task<ItemCodeRate?> GetItemCodeRateOnlyByIdAsync(int itemCodeRateId);
        Task AddItemCodeRateAsync(ItemCodeRate ItemCodeRate);
        Task UpdateItemCodeRateAsync(ItemCodeRate ItemCodeRate);
    }
}