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
    public class ClientContactService : IClientContactService
    {
        private readonly IRepository<ClientContact> _efCoreRepository; // For EF Core CRUD
        private readonly IClientContactRepository _clientContactRepository; // For Dapper queries
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StaffService> _logger;

        public ClientContactService(
            IRepository<ClientContact> efCoreRepository,
            IClientContactRepository clientContactRepository,
            IDapperRepository dapperRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            ILogger<StaffService> logger)
        {
            _efCoreRepository = efCoreRepository;
            _clientContactRepository = clientContactRepository;
            _dapperRepository = dapperRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Retrieve a list of ClientContacts using Dapper (Optimized for performance)
        public async Task<IEnumerable<ClientContactIndexViewModel>> GetClientContactIndexAsync(
            string? clientName, string? fullName, string? jobTitle, string? phoneMobile, string? email, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Client Contacts list using Dapper.");
            return await _clientContactRepository.GetClientContactIndexAsync(
                clientName, fullName, jobTitle, phoneMobile, email, isActive,
                sortColumn, sortOrder, pageNumber, pageSize);
        }

        public async Task<IEnumerable<ClientContactViewModel>> FindDuplicateClientContactAsync(string firstName, string familyName, int? clientContactId = null)
        {
            // Exclude any soft deletes
            var query = ""
                + "SELECT "
                + "   cc.clientContactId,"
                + "   cc.clientId,"
                + "   c.[Name] AS ClientName,"
                + "   cc.FirstName,"
                + "   cc.FamilyName,"
                + "   cc.PhoneMobile,"
                + "   cc.Email,"
                + "   cc.JobTitle,"
                + "   cc.IsActive,"
                + "   cc.ApplicationUserId"
                + "FROM "
                + "   ClientContacts AS cc"
                + "   INNER JOIN Clients AS c ON cc.clientId = c.clientId"
                + "WHERE"
                + "     cc.IsDeleted = 0 "
                + "     AND FirstName = @FirstName "
                + "     AND FamilyName = @FamilyName"
                + "    ";

            if (clientContactId.HasValue)
            {
                query += "AND clientContactId != @clientContactId"; // Exclude a specific ClientContact (useful when updating)
            }

            var parameters = new { FirstName = firstName, FamilyName = familyName, clientContactId = clientContactId };

            return await _dapperRepository.QueryAsync<ClientContactViewModel>(query, parameters);
        }

        public async Task<bool> IsEmailAvailableAsync(string email, int clientContactId = 0)
        {
            return await _clientContactRepository.IsEmailAvailableAsync(email, clientContactId);
        }

        public async Task<ClientContact?> GetClientContactByIdAsync(int clientContactId)
        {
            return await _clientContactRepository.GetClientContactByIdAsync(clientContactId);
        }

        public async Task<ClientContactViewModel?> GetClientContactViewModelByIdAsync(int clientContactId)
        {
            return await _clientContactRepository.GetClientContactViewModelByIdAsync(clientContactId);
        }

        public async Task<ClientContactViewModel> PrepareNewClientContactViewModelAsync(int clientId)
        {
            return await _clientContactRepository.PrepareNewClientContactViewModelAsync(clientId);
        }

        // Creates a new ClientContact (EF Core)
        public async Task<int> AddClientContactAsync(ClientContactViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new regon: {model.Fullname}");

            var ClientContact = new ClientContact
            {
                clientId = model.clientId,
                FirstName = model.FirstName,
                FamilyName = model.FamilyName,
                PhoneMobile = model.PhoneMobile,
                Email = model.Email,
                JobTitle = model.JobTitle,
                IsActive = model.IsActive,
                ApplicationUserId = model.ApplicationUserId,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _efCoreRepository.AddAsync(ClientContact);
            await _unitOfWork.CommitAsync();

            return ClientContact.clientContactId;
        }

        // Updates an existing ClientContact (EF Core)
        public async Task UpdateClientContactAsync(ClientContactViewModel model, string updatedById)
        {
            var ClientContact = await _efCoreRepository.GetByIdAsync(model.clientContactId);
            if (ClientContact == null)
            {
                _logger.LogWarning($"Client Contact with ID {model.clientContactId} not found.");
                throw new Exception("Client Contact not found.");
            }

            ClientContact.FirstName = model.FirstName;
            ClientContact.FamilyName = model.FamilyName;
            ClientContact.PhoneMobile = model.PhoneMobile;
            ClientContact.Email = model.Email;
            ClientContact.JobTitle = model.JobTitle;
            ClientContact.IsActive = model.IsActive;
            ClientContact.ApplicationUserId = model.ApplicationUserId;
            ClientContact.UpdatedAt = DateTime.UtcNow;
            ClientContact.UpdatedById = updatedById;

            _efCoreRepository.Update(ClientContact);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteClientContactAsync(int clientContactId, string updatedById)
        {
            var clientContact = await _efCoreRepository.GetByIdAsync(clientContactId);

            if (clientContact == null)
            {
                throw new KeyNotFoundException("Client Contact not found.");
            }

            // Soft delete staff
            clientContact.IsDeleted = true;
            clientContact.IsActive = false;
            clientContact.UpdatedAt = DateTime.UtcNow;
            clientContact.UpdatedById = updatedById;

            _efCoreRepository.Update(clientContact);
            await _unitOfWork.CommitAsync();

            _efCoreRepository.Update(clientContact);
            int rowsAffected = await _unitOfWork.CommitAsync(); // Returns the number of affected rows

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }

        public async Task UpdateClientContactApplicationUserId(int clientContactId, string applicationUserId)
        {
            var clientContact = await _efCoreRepository.GetByIdAsync(clientContactId);

            if (clientContact == null)
            {
                throw new KeyNotFoundException($"Client Contact with ID {clientContactId} not found.");
            }

            clientContact.ApplicationUserId = applicationUserId;

            _efCoreRepository.Update(clientContact);
            await _unitOfWork.CommitAsync();
        }
    }
}
