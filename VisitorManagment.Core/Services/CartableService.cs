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
            foreach (var file in files)
            {
                _context.Cartables.Where(u => u.FileId == file.Id).ToList().ForEach(r => _context.Cartables.Remove(r));
            }


            foreach (int rcvrId in rcvrUserId)
            {

                foreach (var file in files)
                {
                    _context.Cartables.Add(new Cartable()
                    {
                        RcvrUserId = rcvrId,
                        SndrUserId = sndrUserId,
                        FileId = file.Id,
                        StateCd = 0,
                        IsView = false,
                        RegDate = file.RegDate
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

            _context.Cartables.Where(u => u.FileId == fileId).ToList()
                .ForEach(r => _context.Cartables.Remove(r));


            _context.Cartables.Add(new Cartable()
            {
                RcvrUserId = rcvrUserId,
                SndrUserId = sndrUserId,
                FileId = fileId,
                StateCd = 0,
                IsView = false,
                RegDate = file.RegDate
            });

            _context.SaveChanges();
            return fileId;
        }

        #region send File to Cartable User Nezaja
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

        public BaseResult SendFileToCartableRecivers(List<int> rcvrUserId, int sndrUserId, int fileId)
        {
            var file = _context.Files.Where(f => f.Id == fileId).FirstOrDefault();


            foreach (int rcvrId in rcvrUserId)
            {
                _context.Cartables.Add(new Cartable()
                {
                    RcvrUserId = rcvrId,
                    SndrUserId = sndrUserId,
                    FileId = fileId,
                    StateCd = 0,
                    IsView = false,
                    IsDone = false,
                    RegDate = file.RegDate
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

        public BaseResult SendFileToCartableWhenRegHamesh(List<int> rcvrUserId, int sndrUserId, int fileId)
        {
            var file = _fileService.GetFileByFileId(fileId);


            foreach (int rcvrId in rcvrUserId)
            {
                _context.Cartables.Add(new Cartable()
                {
                    RcvrUserId = rcvrId,
                    SndrUserId = sndrUserId,
                    FileId = fileId,
                    StateCd = 0,
                    IsView = false,
                    IsDone = false,
                    RegDate = file.RegDate
                });
            }




            //حذف از کارتابل نفرات قدیم
            var isSuccess = RemoveCartable(fileId, sndrUserId);

            if (isSuccess)//true
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
    }
}
