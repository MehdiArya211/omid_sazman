using VisitorManagment.Core.DTOs;

namespace VisitorManagment.Core.Services.Interfaces
{
    /// <summary>
    /// عملیات فقط‌خواندنی موردنیاز مدیر برای مشاهده سوابق کامل مراجعه‌کننده.
    /// </summary>
    public interface IAdminVisitorRequestService
    {
        AdminVisitorRequestSearchViewModel GetRequestsByPersonalCode(string personalCode);
    }
}
