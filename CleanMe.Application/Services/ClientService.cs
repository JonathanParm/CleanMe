using CleanMe.Application.DTOs;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Common;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<ClientService> _logger;

        public ClientService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<ClientService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of Clients using Dapper (Optimized for performance)
        public async Task<IEnumerable<ClientIndexViewModel>> GetClientIndexAsync(
            string? name, string? brand, int? accNo, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Clients list using Dapper.");
            try
            {
                var query = "EXEC dbo.ClientGetIndexView @Name, @Brand, @AccNo, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    Brand = brand,
                    AccNo = accNo,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<ClientIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Clients from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<ClientViewModel>> FindDuplicateClientAsync(string name, int? clientId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Clients WHERE IsDeleted = 0 AND Name = @Name";

            if (clientId.HasValue)
            {
                query += " AND clientId != @clientId"; // Exclude a specific client (useful when updating)
            }

            var parameters = new { Name = name, clientId = clientId };

            var duplicateClient = await _unitOfWork.DapperRepository.QueryAsync<ClientWithAddressDto>(query, parameters);

            // Map `Client` entity to `ClientViewModel`
            return duplicateClient.Select(c => new ClientViewModel
            {
                clientId = c.clientId,
                Name = c.Name,
                Brand = c.Brand,
                AccNo = c.AccNo,
                Address = new AddressViewModel
                {
                    Line1 = c.AddressLine1,
                    Line2 = c.AddressLine2,
                    Suburb = c.AddressSuburb,
                    TownOrCity = c.AddressTownOrCity,
                    Postcode = c.AddressPostcode,
                },
                IsActive = c.IsActive
            });
        }

        public async Task<ClientViewModel?> GetClientViewModelByIdAsync(int clientId)
        {
            var client = await _unitOfWork.ClientRepository.GetClientByIdAsync(clientId);

            if (client == null)
            {
                return null; // No match found
            }

            // Convert `Client` entity to `ClientViewModel`
            return new ClientViewModel
            {
                clientId = client.clientId,
                Name = client.Name,
                Brand = client.Brand,
                AccNo = client.AccNo,
                Address = new AddressViewModel
                {
                    Line1 = client.Address.Line1,
                    Line2 = client.Address.Line2,
                    Suburb = client.Address.Suburb,
                    TownOrCity = client.Address.TownOrCity,
                    Postcode = client.Address.Postcode,
                },
                IsActive = client.IsActive
            };
        }

        public async Task<ClientViewModel?> GetClientViewModelWithContactsByIdAsync(int clientId)
        {
            var client = await _unitOfWork.ClientRepository.GetClientWithContactsByIdAsync(clientId);

            if (client == null)
            {
                return null; // No match found
            }

            // Convert `Client` entity to `ClientViewModel`
            return new ClientViewModel
            {
                clientId = client.clientId,
                Name = client.Name,
                Brand = client.Brand,
                AccNo = client.AccNo,
                Address = new AddressViewModel
                {
                    Line1 = client.Address.Line1,
                    Line2 = client.Address.Line2,
                    Suburb = client.Address.Suburb,
                    TownOrCity = client.Address.TownOrCity,
                    Postcode = client.Address.Postcode,
                },
                IsActive = client.IsActive,

                ContactsList = client.ClientContacts
                    .OrderBy(cc => cc.FirstName)
                    .ThenBy(cc => cc.FamilyName)
                    .Select(cc => new ClientContactIndexViewModel
                    {
                        clientContactId = cc.clientContactId,
                        FullName = cc.FirstName + " " + cc.FamilyName,
                        Email = cc.Email,
                        PhoneMobile = cc.PhoneMobile,
                        JobTitle = cc.JobTitle,
                        IsActive = cc.IsActive
                    })
            .ToList()
            };
        }

        public async Task<int> AddClientAsync(ClientViewModel model, string addedById)
        {
            _logger.LogInformation($"Creating new Client: {model.Name}");

            var client = new Client
            {
                Name = model.Name,
                Brand = model.Brand,
                AccNo = model.AccNo,
                Address = new Address()
                {
                    Line1 = model.Address.Line1,
                    Line2 = model.Address.Line2,
                    Suburb = model.Address.Suburb,
                    TownOrCity = model.Address.TownOrCity,
                    Postcode = model.Address.Postcode,
                },
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.ClientRepository.AddClientAsync(client);

            return client.clientId;
        }

        public async Task UpdateClientAsync(ClientViewModel model, string updatedById)
        {
            var client = await _unitOfWork.ClientRepository.GetClientByIdAsync(model.clientId);
            if (client == null)
            {
                _logger.LogWarning($"Client member with ID {model.clientId} not found.");
                throw new Exception("Client member not found.");
            }

            client.Name = model.Name;
            client.Brand = model.Brand;
            client.AccNo = model.AccNo;
            client.Address = new Address()
            {
                Line1 = model.Address.Line1,
                Line2 = model.Address.Line2,
                Suburb = model.Address.Suburb,
                TownOrCity = model.Address.TownOrCity,
                Postcode = model.Address.Postcode,
            };
            client.IsActive = model.IsActive;
            client.UpdatedAt = DateTime.UtcNow;
            client.UpdatedById = updatedById;

            await _unitOfWork.ClientRepository.UpdateClientAsync(client);
        }

        public async Task<bool> SoftDeleteClientAsync(int clientId, string updatedById)
        {
            var client = await _unitOfWork.ClientRepository.GetClientByIdAsync(clientId);

            if (client == null)
            {
                throw new KeyNotFoundException("Client not found.");
            }

            // Soft delete Client
            client.IsDeleted = true;
            client.IsActive = false;
            client.UpdatedAt = DateTime.UtcNow;
            client.UpdatedById = updatedById;

            await _unitOfWork.ClientRepository.UpdateClientAsync(client);

            return true;
        }
    }
}
