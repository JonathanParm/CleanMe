using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Interfaces;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using CleanMe.Domain.Common;
using CleanMe.Application.DTOs;

namespace CleanMe.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _efCoreRepository; // For EF Core CRUD
        private readonly IClientRepository _clientRepository; // For Dapper queries
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClientService> _logger;

        public ClientService(
            IRepository<Client> efCoreRepository,
            IClientRepository clientRepository,
            IDapperRepository dapperRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            ILogger<ClientService> logger)
        {
            _efCoreRepository = efCoreRepository;
            _clientRepository = clientRepository;
            _dapperRepository = dapperRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Retrieve a list of Clients using Dapper (Optimized for performance)
        public async Task<IEnumerable<ClientIndexViewModel>> GetClientIndexAsync(
            string? name, string? brand, int? accNo, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Clients list using Dapper.");
            return await _clientRepository.GetClientIndexAsync(
                name, brand, accNo, isActive,
                sortColumn, sortOrder, pageNumber, pageSize);
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

            var duplicateClient = await _dapperRepository.QueryAsync<ClientWithAddressDto>(query, parameters);

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

        public async Task<ClientViewModel?> GetClientByIdAsync(int clientId)
        {
            var client = await _clientRepository.GetClientByIdAsync(clientId);

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

        public async Task<ClientViewModel?> GetClientWithContactsByIdAsync(int clientId)
        {
            var client = await _clientRepository.GetClientWithContactsByIdAsync(clientId);

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


        // Creates a new Client (EF Core)
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

            await _efCoreRepository.AddAsync(client);
            await _unitOfWork.CommitAsync();

            return client.clientId;
        }

        // Updates an existing Client (EF Core)
        public async Task UpdateClientAsync(ClientViewModel model, string updatedById)
        {
            var client = await _efCoreRepository.GetByIdAsync(model.clientId);
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

            _efCoreRepository.Update(client);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteClientAsync(int clientId, string updatedById)
        {
            var client = await _efCoreRepository.GetByIdAsync(clientId);

            if (client == null)
            {
                throw new KeyNotFoundException("Client not found.");
            }

            // Soft delete Client
            client.IsDeleted = true;
            client.IsActive = false;
            client.UpdatedAt = DateTime.UtcNow;
            client.UpdatedById = updatedById;

            _efCoreRepository.Update(client);
            await _unitOfWork.CommitAsync();

            _efCoreRepository.Update(client);
            int rowsAffected = await _unitOfWork.CommitAsync(); // Returns the number of affected rows

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }
    }
}
