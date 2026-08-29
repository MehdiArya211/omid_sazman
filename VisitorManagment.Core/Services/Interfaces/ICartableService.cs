using System;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services.Interfaces
{
	public interface ICartableService
    {
        /// <summary>
        /// وقتی کاربر درخواست ثبت میکنه یه در رکورد هم تو جدول کارتبال ثبت میشه
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="regDate"></param>
        /// <returns></returns>
        BaseResult AddToCartable(int userId , int fileId, DateTime regDate);
        BaseResult AddCartable(Cartable cartable);

        #region Send To Cartable
        /// <summary>
        /// فایل رو وقتی ارسال میکنیم به کارتابل کسی اول از کارتابل نفر پاک میکنه بعد میزنه به کارتابل نفر گیرنده
        /// </summary>
        /// <param name="rcvrUserId"></param>
        /// <param name="sndrUserId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        void SendFileToCartable(List<int> rcvrUserId, int sndrUserId , int fileId);
        /// <summary>
        /// برای زمانی هست که وقتی کاربر نزاجا میخواد تمام درخواست هایی رو که کاربر نزاجا به کارتابل شماره 3 ارسال میکنن
        /// </summary>
        /// <param name="rcvrUserId"></param>
        /// <param name="sndrUserId"></param>
        /// <param name="files"></param>
        void SendListFileToCartable(List<int> rcvrUserId, int sndrUserId , List<Files> files);
        /// <summary>
        /// ارسال به کارتابل وقتی عودت رو میزنیم
        /// </summary>
        /// <param name="rcvrUserId"></param>
        /// <param name="sndrUserId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        int SendFileToCartableWhenBackFile(int rcvrUserId, int sndrUserId , int fileId);
        int SendFileToCartableUserNejaza(List<int> rcvrUserId, int sndrUserId);

        /// <summary>
        /// حذف از جدول کارتابل
        /// </summary>
        /// <param name="cartable"></param>
        bool RemoveCartable(int fileId, int senderId);

        bool Commit();

        /// <summary>
        /// ارسال درخواست ملاقات به گیرندگان برای کنترل انجام عملیات ارسال که اگر ارسال نشد از کارتابل فرستنده پاک نشود و پیغام مناسب ارسال گردد
        /// </summary>
        /// <returns></returns>
        BaseResult SendFileToCartableRecivers(List<int> rcvrUserId, int sndrUserId, int fileId);


        /// <summary>
        /// ارسال درخواست ملاقات به کارتابل
        /// </summary>
        /// <returns></returns>
        BaseResult SendFileToCartableWhenRegHamesh(List<int> rcvrUserId, int sndrUserId, int fileId);

        #endregion


    }
}
