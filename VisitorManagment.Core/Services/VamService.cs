using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class VamService : IVamService
    {
        private readonly VisitorManagmentContext _context;
        private readonly IVamCodeService _vamCodeService;
        public VamService(VisitorManagmentContext context, IVamCodeService vamCodeService)
        {
            _context = context;
            _vamCodeService = vamCodeService;
        }
        #region اعضا و متدهای کلاس




        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public int CreateVam(int vamCodeId, string title, int fileId, int regUserId)
        {
            //اینجا کد وام رو با آیدی وام بیارم
            var codeVam = _vamCodeService.GetVamCodeWithVamId(vamCodeId).Code;
            var vamExist = _context.Vams.Where(x => x.FileId == fileId && x.CodeVam == codeVam).FirstOrDefault();
            if (vamExist == null)
            {
                var vam = new Vam()
                {
                    Title = title,
                    FileId = fileId,
                    VamCodeId = vamCodeId,
                    CodeVam = codeVam,
                    RegUserId = regUserId,
                    RegDate = DateTime.Now,
                    IsDelete = false,
                };
                return AddVam(vam);
            }

            return 0;



        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public int AddVam(Vam vam)
        {
            _context.Add(vam);
            _context.SaveChanges();
            return vam.Id;
        }

        /// <summary>
        /// گرفتن تمام وام های تصویب شده برای درخواست ثبت شده
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public List<VamViewModel> getAllVamWithFileId(int fileId)
        {
            var listVam = _context.Vams.Include(x => x.File)
                    .Where(x => x.FileId == fileId && x.IsDelete==false)
                    .Select(x => new VamViewModel
                    {

                        Id = x.Id,
                        FileId = x.FileId,
                        CodeVam = x.CodeVam,
                        VamCodeId = x.VamCodeId,
                        Title = x.Title,
                        VamCodeTitle = x.VamCode.Title,
                        VamPrice = x.VamCode.Price,

                    }).Distinct().ToList();

            return listVam;
        }

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public List<VamViewModel> getAllVamWithFileIdLBK(int fileId)
        {
            var listVam = new List<VamViewModel>();

            return listVam;
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public BaseResult DeleteVam(int vamId)
        {
            var vamExist = _context.Vams.Where(x => x.Id == vamId).FirstOrDefault();

            if (vamExist == null) return new BaseResult()
            {
                Message = "وام مورد نظر یافت نشد",
                Status = false
            };

            else
            {
                vamExist.IsDelete = true;
                _context.SaveChanges();
                return new BaseResult()
                {
                    Status = true,
                    Message = "عملیات با موفقیت انجام شد"
                };
            }

        }
        #endregion
    }
}
