using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IVamService
    {
        int CreateVam(int vamCodeId , string title , int fileId, int regUserId);
        BaseResult DeleteVam(int vamId);
        int AddVam(Vam vam);

        /// <summary>
        /// گرفتن تمام وام های تصویب شده برای درخواست ثبت شده
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        List<VamViewModel> getAllVamWithFileId(int fileId);
        List<VamViewModel> getAllVamWithFileIdLBK(int fileId);
    }
}
