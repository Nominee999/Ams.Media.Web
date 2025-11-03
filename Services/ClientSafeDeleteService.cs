using System.Threading;
using System.Threading.Tasks;
using Ams.Media.Web.Repositories.Interfaces;   // << สำคัญ: มี IClientRepository อยู่ที่นี่

namespace Ams.Media.Web.Services
{
    /// <summary>
    /// บริการลบลูกค้าแบบปลอดภัย (ถ้ามีธุรกรรมใช้งาน ควรเช็คก่อนลบ)
    /// ตอนนี้ตัวอย่างเรียกไปที่ Repo โดยตรง
    /// </summary>
    public sealed class ClientSafeDeleteService
    {
        private readonly IClientRepository _repo;

        public ClientSafeDeleteService(IClientRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// ลบลูกค้า (จะปรับเป็น Soft Delete/ตรวจ UsedBy ก็ได้ภายหลัง)
        /// </summary>
        public Task<bool> SafeDeleteAsync(long clientId, CancellationToken ct = default)
            => _repo.DeleteAsync(clientId);
    }
}
