using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class ClientContactService : IClientContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public ClientContactService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of ClientContacts using Dapper (Optimized for performance)
        public async Task<IEnumerable<ClientContactIndexViewModel>> GetClientContactIndexAsync(
            string? clientName, string? fullName, string? jobTitle, string? phoneMobile, string? email, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Client Contacts list using Dapper.");
            var query = "EXEC dbo.ClientContactGetIndexView @ClientName, @FullName, @JobTitle, @phoneMobile, @email, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
            var parameters = new
            {
                ClientName = clientName,
                FullName = fullName,
                JobTitle = jobTitle,
                PhoneMobile = phoneMobile,
                Email = email,
                IsActive = isActive,
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return await _unitOfWork.DapperRepository.QueryAsync<ClientContactIndexViewModel>(query, parameters);
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

            return await _unitOfWork.DapperRepository.QueryAsync<ClientContactViewModel>(query, parameters);
        }

        public async Task<ClientContactViewModel?> GetClientContactViewModelByIdAsync(int clientContactId)
        {
            var clientContact = await _unitOfWork.ClientContactRepository.GetClientContactByIdAsync(clientContactId);
            if (clientContact == null)
            {
                _logger.LogWarning($"Client Contact with ID {clientContactId} not found.");
                return null;
            }

            return new ClientContactViewModel
            {
                clientContactId = clientContact.clientContactId,
                clientId = clientContact.clientId,
                FirstName = clientContact.FirstName,
                FamilyName = clientContact.FamilyName,
                Email = clientContact.Email,
                PhoneMobile = clientContact.PhoneMobile,
                JobTitle = clientContact.JobTitle,
                IsActive = clientContact.IsActive,
                ApplicationUserId = clientContact.ApplicationUserId,
                ClientName = clientContact.Client.Name
            };
        }

        public async Task<ClientContactViewModel> PrepareNewClientContactViewModelAsync(int clientId)
        {
            var client =  await _unitOfWork.ClientRepository.GetClientByIdAsync(clientId);

            //if (client == null)
            //    throw new NotFoundException("Client not found.");

            return new ClientContactViewModel
            {
                clientId = client.clientId,
                ClientName = client.Name,
                IsActive = true
            };

        }

        // Creates a new ClientContact (EF Core)
        public async Task<int> AddClientContactAsync(ClientContactViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new regon: {model.Fullname}");

            var clientContact = new ClientContact
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

            await _unitOfWork.ClientContactRepository.AddClientContactAsync(clientContact);

            return clientContact.clientContactId;
        }

        // Updates an existing ClientContact (EF Core)
        public async Task UpdateClientContactAsync(ClientContactViewModel model, string updatedById)
        {
            var clientContact = await _unitOfWork.ClientContactRepository.GetClientContactByIdAsync(model.clientContactId);
            if (clientContact == null)
            {
                _logger.LogWarning($"Client Contact with ID {model.clientContactId} not found.");
                throw new Exception("Client Contact not found.");
            }

            clientContact.FirstName = model.FirstName;
            clientContact.FamilyName = model.FamilyName;
            clientContact.PhoneMobile = model.PhoneMobile;
            clientContact.Email = model.Email;
            clientContact.JobTitle = model.JobTitle;
            clientContact.IsActive = model.IsActive;
            clientContact.ApplicationUserId = model.ApplicationUserId;
            clientContact.UpdatedAt = DateTime.UtcNow;
            clientContact.UpdatedById = updatedById;

            await _unitOfWork.ClientContactRepository.UpdateClientContactAsync(clientContact);
        }

        public async Task<bool> SoftDeleteClientContactAsync(int clientContactId, string updatedById)
        {
            var clientContact = await _unitOfWork.ClientContactRepository.GetClientContactByIdAsync(clientContactId);

            if (clientContact == null)
            {
                throw new KeyNotFoundException("Client Contact not found.");
            }

            // Soft delete staff
            clientContact.IsDeleted = true;
            clientContact.IsActive = false;
            clientContact.UpdatedAt = DateTime.UtcNow;
            clientContact.UpdatedById = updatedById;

            await _unitOfWork.ClientContactRepository.UpdateClientContactAsync(clientContact);

            return true;
        }

        public async Task<bool> IsEmailAvailableAsync(string email, int clientContactId = 0)
        {
            return await _unitOfWork.ClientContactRepository.IsEmailAvailableAsync(email, clientContactId);
        }

        public async Task UpdateClientContactApplicationUserId(int clientContactId, string applicationUserId)
        {
            var clientContact = await _unitOfWork.ClientContactRepository.GetClientContactByIdAsync(clientContactId);

            if (clientContact == null)
            {
                throw new KeyNotFoundException($"Client Contact with ID {clientContactId} not found.");
            }

            clientContact.ApplicationUserId = applicationUserId;

            await _unitOfWork.ClientContactRepository.UpdateClientContactAsync(clientContact);
        }
    }
}
