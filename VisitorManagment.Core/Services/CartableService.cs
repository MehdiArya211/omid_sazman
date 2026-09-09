using System;
using System.Collections.Generic;
using System.Linq;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using VisitorManagment.DataLayer.Migrations;

namespace VisitorManagment.Core.Services
{
    public class CartableService : ICartableService
    {
        private readonly VisitorManagmentContext _context;
        private readonly IFileService _fileService;
        public CartableService(VisitorManagmentContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }
        /// <summary>
        /// وقتی کاربر درخواست ثبت میکنه یه در رکورد هم تو جدول کارتبال ثبت میشه
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="regDate"></param>
        /// <returns></returns>
        public BaseResult AddToCartable(int userId, int fileId, DateTime regDate)
        {
            var cartable = new Cartable()
            {
                SndrUserId = userId,
                RcvrUserId = userId,
                FileId = fileId,
                StateCd = 0,
                IsView = false,
                IsDone = false,
                RegDate = regDate,

            };
            return AddCartable(cartable);

        }


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult AddCartable(Cartable cartable)
        {
            _context.Cartables.Add(cartable);
            _context.SaveChanges();
            if (cartable.Id!=0)
            {
                return new BaseResult
                {
                    Message = "ثبت موفق",
                    Model = cartable.Id,
                    Status = true,
                };
            }
            return new BaseResult
            {
                Message = "ثبت ناموفق",
                Model = cartable.Id,
                Status = false,
            };
        }



        #region Send To Cartable
        /// <summary>
        /// فایل رو وقتی ارسال میکنیم به کارتابل کسی اول از کارتابل نفر پاک میکنه بعد میزنه به کارتابل نفر گیرنده
        /// </summary>
        /// <param name="rcvrUserId"></param>
        /// <param name="sndrUserId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public void SendFileToCartable(List<int> rcvrUserId, int sndrUserId, int fileId)
        {

            var file = _context.Files.Where(f => f.Id == fileId).FirstOrDefault();

            var resultSendToCartableRecivers = SendFileToCartableRecivers(rcvrUserId, sndrUserId, fileId);

            if (resultSendToCartableRecivers.Status)
            {
                #region از جدول کارتابل درخواستی رو باید پیدا کنیم که گیرنده ش نفر لاگین کرده باشه


                //حذف از کارتابل نفرات قدیم
                // var isSuccess = RemoveCartable(fileId , sndrUserId);

                //return new BaseResult
                //{
                //    Status = isSuccess,
                //    Message = isSuccess ? "عملیات با موفقیت انجام شد." : "ذخیره اطلاعات با خطا همراه بوده است!",
                //    Model = fileId
                //};
                #endregion
            }

            //return new BaseResult
            //{
            //    Status = false,
            //    Message = "عملیات با خطا مواجه شد لطفا مجددا تلاش نمایید",
            //    Model = fileId
            //};
            _context.Update(file);
        }



        /// <summary>
        /// برای زمانی هست که وقتی کاربر نزاجا میخواد تمام درخواست هایی رو که کاربر نزاجا به کارتابل شماره 3 ارسال میکنن
        /// </summary>
        /// <param name="rcvrUserId"></param>
        /// <param name="sndrUserId"></param>
        /// <param name="files"></param>
        public void SendListFileToCartable(List<int> rcvrUserId, int sndrUserId, List<Files> files)
        {
            var receiverIds = (rcvrUserId ?? new List<int>()).Where(id => id > 0).Distinct().ToList();
            var validReceiverIds = _context.Users
                .Where(user => receiverIds.Contains(user.Id) && user.IsActive && !user.IsDelete)
                .Select(user => user.Id).ToList();
            var distinctFiles = (files ?? new List<Files>()).Where(file => file != null)
                .GroupBy(file => file.Id).Select(group => group.First()).ToList();
            if (validReceiverIds.Count != receiverIds.Count || validReceiverIds.Count == 0 || distinctFiles.Count == 0) return;

            foreach (var file in distinctFiles)
            {
                var activeRows = _context.Cartables.Where(item => item.FileId == file.Id && !item.IsDone).ToList();
                foreach (var row in activeRows) row.IsDone = true;
            }

            foreach (int rcvrId in validReceiverIds)
            {

                foreach (var file in distinctFiles)
                {
                    _context.Cartables.Add(new Cartable()
                    {
                        RcvrUserId = rcvrId,
                        SndrUserId = sndrUserId,
                        FileId = file.Id,
                        StateCd = 0,
                        IsView = false,
                        IsDone = false,
                        RegDate = DateTime.Now
                    });
                }

            }

            _context.SaveChanges();
        }
        /// <summary>
        /// ارسال به کارتابل وقتی عودت رو میزنیم
        /// </summary>
        /// <param name="rcvrUserId"></param>
        /// <param name="sndrUserId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public int SendFileToCartableWhenBackFile(int rcvrUserId, int sndrUserId, int fileId)
        {
            var file = _context.Files.Where(f => f.Id == fileId).SingleOrDefault();
            if (file == null || rcvrUserId <= 0) return 0;

            _context.Cartables.Where(u => u.FileId == fileId && !u.IsDone).ToList()
                .ForEach(row => row.IsDone = true);


            _context.Cartables.Add(new Cartable()
            {
                RcvrUserId = rcvrUserId,
                SndrUserId = sndrUserId,
                FileId = fileId,
                StateCd = 0,
                IsView = false,
                IsDone = false,
                RegDate = DateTime.Now
            });

            _context.SaveChanges();
            return fileId;
        }

        #region send File to Cartable User Nezaja
        /// <summary>
        /// اطلاعات را به مقصد موردنظر ارسال می‌کند.
        /// </summary>
        public int SendFileToCartableUserNejaza(List<int> rcvrUserId, int sndrUserId)
        {
            var fileId = _context.Cartables.Where(u => u.RcvrUserId == sndrUserId).Select(u => u.FileId).FirstOrDefault();
            _context.Cartables.Where(u => u.FileId == fileId).ToList().ForEach(r => _context.Cartables.Remove(r));

            foreach (int rcvrId in rcvrUserId)
            {
                _context.Cartables.Add(new Cartable()
                {
                    RcvrUserId = rcvrId,
                    SndrUserId = sndrUserId,
                    FileId = fileId,
                    StateCd = 0,
                    IsView = false,
                    RegDate = DateTime.Now
                });


            }
            _context.SaveChanges();
            return fileId;
        }




        #endregion
        #endregion

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public bool RemoveCartable(int fileId, int senderId)
        {
            var cartable = _context.Cartables
                 .Where(x => x.FileId == fileId && x.RcvrUserId == senderId && x.IsDone == false)
                 .FirstOrDefault();


            if (cartable == null)
            {
                return false;
            }

            _context.Remove(cartable);

            return Commit();
        }


        /// <summary>
        /// تغییرات جاری را در پایگاه داده ذخیره می‌کند.
        /// </summary>
        public bool Commit()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// اطلاعات را به مقصد موردنظر ارسال می‌کند.
        /// </summary>
        public BaseResult SendFileToCartableRecivers(List<int> rcvrUserId, int sndrUserId, int fileId)
        {
            var file = _context.Files.Where(f => f.Id == fileId).FirstOrDefault();
            var receiverIds = (rcvrUserId ?? new List<int>()).Where(id => id > 0).Distinct().ToList();
            if (file == null) return new BaseResult(false, "درخواست یافت نشد.");
            if (!receiverIds.Any()) return new BaseResult(false, "حداقل یک گیرنده باید انتخاب شود.");

            var validReceiverIds = _context.Users
                .Where(user => receiverIds.Contains(user.Id) && user.IsActive && !user.IsDelete)
                .Select(user => user.Id).ToList();
            if (validReceiverIds.Count != receiverIds.Count)
                return new BaseResult(false, "یک یا چند گیرنده نامعتبر یا غیرفعال است.");

            foreach (var row in _context.Cartables.Where(item => item.FileId == fileId && !item.IsDone).ToList())
                row.IsDone = true;

            foreach (int rcvrId in validReceiverIds)
            {
                _context.Cartables.Add(new Cartable()
                {
                    RcvrUserId = rcvrId,
                    SndrUserId = sndrUserId,
                    FileId = fileId,
                    StateCd = 0,
                    IsView = false,
                    IsDone = false,
                    RegDate = DateTime.Now
                });
            }
            var res = _context.SaveChanges();


            // با چند گیرنده، SaveChanges بیش از یک رکورد را برمی‌گرداند.
            if (res > 0)
            {
                return new BaseResult
                {
                    Status = true,
                    Message = "عملیات با موفقیت انجام شد"
                };
            }
            else
            {
                return new BaseResult
                {
                    Status = false,
                    Message = "عملیات با خطا مواجه شد"
                };
            }
        }

        /// <summary>
        /// اطلاعات را به مقصد موردنظر ارسال می‌کند.
        /// </summary>
        public BaseResult SendFileToCartableWhenRegHamesh(List<int> rcvrUserId, int sndrUserId, int fileId)
        {
            var file = _fileService.GetFileByFileId(fileId);
            var receiverIds = (rcvrUserId ?? new List<int>()).Where(id => id > 0).Distinct().ToList();
            if (file == null) return new BaseResult(false, "درخواست یافت نشد.");
            if (!receiverIds.Any()) return new BaseResult(false, "حداقل یک گیرنده باید انتخاب شود.");

            foreach (var row in _context.Cartables.Where(item => item.FileId == fileId && !item.IsDone).ToList())
                row.IsDone = true;

            foreach (int rcvrId in receiverIds)
            {
                _context.Cartables.Add(new Cartable()
                {
                    RcvrUserId = rcvrId,
                    SndrUserId = sndrUserId,
                    FileId = fileId,
                    StateCd = 0,
                    IsView = false,
                    IsDone = false,
                    RegDate = DateTime.Now
                });
            }
            return _context.SaveChanges() > 0
                ? new BaseResult(true, "عملیات با موفقیت انجام شد")
                : new BaseResult(false, "عملیات با خطا مواجه شد");
        }
    }
}
