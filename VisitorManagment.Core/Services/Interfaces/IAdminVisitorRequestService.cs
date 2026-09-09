using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using System.Collections.Generic;

namespace VisitorManagment.Core.Services.Interfaces
{
    /// <summary>
    /// عملیات فقط‌خواندنی موردنیاز مدیر برای مشاهده سوابق کامل مراجعه‌کننده.
    /// </summary>
    public interface IAdminVisitorRequestService
    {
        AdminVisitorRequestSearchViewModel GetRequestsByPersonalCode(string personalCode);
        List<WorkflowReceiverViewModel> GetActiveReceivers();
        BaseResult TransferRequest(int fileId, int receiverUserId, int administratorUserId, string reason);
    }
}
