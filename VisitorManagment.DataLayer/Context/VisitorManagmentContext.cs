using VisitorManagment.DataLayer.Entities.Permissions;
using VisitorManagment.DataLayer.Entities.User;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using VisitorManagment.DataLayer.Entities.Views;
using VisitorManagment.DataLayer.Entities.Ranking;
using VisitorManagment.DataLayer.Entities.SystemChatRoom;
using VisitorManagment.DataLayer.Entities.NotificationInfo;
using VisitorManagment.DataLayer.Entities;

namespace VisitorManagment.DataLayer.Context
{
	public class VisitorManagmentContext : DbContext
    {
        public VisitorManagmentContext(DbContextOptions<VisitorManagmentContext> options) : base(options)
        {
        }

        #region User

        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleTypeFinal> RoleTypeFinals { get; set; }

        #endregion

        #region Permission

        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }


        #endregion

        #region VisitorManagment
        public DbSet<Personal> Personals { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<FileType> FileTypes { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<RequestSubject> RequestSubjects { get; set; }
        public DbSet<FileStatus> FileStatuses { get; set; }
        public DbSet<WorkFlow> WorkFlows { get; set; }
        public DbSet<Cartable> Cartables { get; set; }
        public DbSet<Hamesh> Hameshes { get; set; }
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<AvamerSadereh> AvamerSaderehs { get; set; }
        public DbSet<Vam> Vams { get; set; }
        public DbSet<VamCode> VamCodes { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; }

        #region Meeting
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingPlace> MeetingPlaces { get; set; }
        public DbSet<MeetingStatus> MeetingStaus { get; set; }
        public DbSet<BoseMeeting> BoseMeetings { get; set; }
        public DbSet<ClerkMeeting> ClerkMeetings { get; set; }
        public DbSet<SMS> SMS { get; set; }
        public DbSet<MemberMeeting> MemberMeetings { get; set; }
        #endregion

        #region Report
        public DbSet<ViwHamesh> ViwHamesh { get; set; }
        public DbSet<ViwFiles> ViwFiles { get; set; }

        #endregion

        #region ranking
        public DbSet<ZaribRanking> ZaribRankings { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<EshrafPeriodDef> EshrafPeriodDefs { get; set; }
        public DbSet<TblDepartment> TblDepartments { get; set; }
        public DbSet<TblDepartmentType> TblDepartmentTypes { get; set; }

        #endregion



        #endregion

        #region چت آنلاین
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        #endregion

        #region اطلاعیه ها
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationAttachment> NotificationAttachments { get; set; }
        public DbSet<NotificationUser> NotificationUsers { get; set; }

        #endregion


        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
              .HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Files>()
                .HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Meeting>()
               .HasQueryFilter(u => !u.IsDelete);
        }
    }
}
