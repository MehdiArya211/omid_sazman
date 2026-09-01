using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.SystemChatOnline;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.SystemChatRoom;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Core.Services.SystemChatOnline
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly VisitorManagmentContext _context;
        private readonly IUserService _userService;
        public ChatRoomService(VisitorManagmentContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>

        public async Task<Guid> CreateChatRoom(string ConnectionId , string personalCode)
        {
            var user=_userService.GetUserByPersonalCode(personalCode);
            if (user == null)
                throw new InvalidOperationException("کاربر معتبر برای ایجاد گفت‌وگو پیدا نشد.");
            var existChatRoom = _context.ChatRooms.SingleOrDefault(p => p.ConnectionId == ConnectionId);
            if (existChatRoom != null)
            {
                return await Task.FromResult(existChatRoom.Id);
            }

            var userRoom = _context.ChatRooms.FirstOrDefault(p => p.UserId == user.Id);
            if (userRoom != null)
            {
                userRoom.ConnectionId = ConnectionId;
                await _context.SaveChangesAsync();
                return userRoom.Id;
            }
            ChatRoom chatRoom = new ChatRoom()
            {
                ConnectionId = ConnectionId,
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Title = user.FirstName + " " + user.LastName + "****" + user.UnitTitle,
            };
            _context.ChatRooms.Add(chatRoom);
            await _context.SaveChangesAsync();
            return chatRoom.Id;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public async Task<List<ChatRoomDTO>> GetAllrooms()
        {
            //var rooms = _context.ChatRooms
            //    .Include(p => p.User)
            //    .Include(p => p.ChatMessages)
            //    .Where(p => p.ChatMessages.Any())
            //    .Select(p =>
            //  p.Id).ToList();
            var rooms = _context.ChatRooms
                .Include(p => p.User)
                .Include(p => p.ChatMessages)
                .Where(p => p.ChatMessages.Any())
                .OrderByDescending(p => p.ChatMessages.Max(m => m.Time))
                .ToList();

            var room = new List<ChatRoomDTO>();

            foreach (var item in rooms)
            {
                ChatRoomDTO room1 = new ChatRoomDTO();

                room1.Id = item.Id;
                //room1.ConnectionId = item.ConnectionId;
                room1.Title = item.Title;
                room.Add(room1);
            }
           

           // var res =rooms.Select(x=>x.Title ).ToList();
            return await Task.FromResult(room);
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public async Task<Guid> GetChatRoomForConnection(string CoonectionId)
        {
            var chatRoom = _context.ChatRooms.SingleOrDefault(p => p.ConnectionId == CoonectionId);
            if (chatRoom == null)
                throw new InvalidOperationException("اتاق گفت‌وگو برای اتصال جاری پیدا نشد.");
            return await Task.FromResult(chatRoom.Id);
        }
        #endregion
    }
}
