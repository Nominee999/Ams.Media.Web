// Services/ClientService.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Dto;
using Ams.Media.Web.Repositories.Interfaces;

namespace Ams.Media.Web.Services
{
    public sealed class ClientService : IClientService
    {
        private readonly IClientAddressRepository _addrRepo;

        public ClientService(IClientAddressRepository addrRepo)
        {
            _addrRepo = addrRepo;
        }

        public Task<bool> AddressCreateAsync(ClientAddressDto dto, CancellationToken ct)
        {
            // ให้ EndDate null แปลว่า 9999-12-31 ที่ชั้น Repo
            return _addrRepo.CreateAsync(dto, ct);
        }

        public Task<bool> AddressUpdateAsync(int clientId, int addressType, DateTime oldStart, ClientAddressDto dto, CancellationToken ct)
        {
            return _addrRepo.UpdateAsync(clientId, addressType, oldStart, dto, ct);
        }

        public Task<bool> AddressDeleteAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct)
        {
            return _addrRepo.DeleteAsync(clientId, addressType, start, end, ct);
        }

        public Task<bool> AddressUpsertAsync(ClientAddressDto dto, DateTime? oldStart, DateTime? oldEnd, CancellationToken ct)
        {
            return _addrRepo.UpsertAsync(dto, oldStart, oldEnd, ct);
        }
    }

    public interface IClientService
    {
        Task<bool> AddressCreateAsync(ClientAddressDto dto, CancellationToken ct);
        Task<bool> AddressUpdateAsync(int clientId, int addressType, DateTime oldStart, ClientAddressDto dto, CancellationToken ct);
        Task<bool> AddressDeleteAsync(int clientId, int addressType, DateTime start, DateTime? end, CancellationToken ct);
        Task<bool> AddressUpsertAsync(ClientAddressDto dto, DateTime? oldStart, DateTime? oldEnd, CancellationToken ct);
    }
}
