using VisitorManagment.DataLayer.Entities.Permissions;
using VisitorManagment.DataLayer.Entities.User;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using VisitorManagment.DataLayer.Entities.Views;
using VisitorManagment.DataLayer.Entities.Ranking;
using VisitorManagment.DataLayer.Entities.SystemChatRoom;
using VisitorManagment.DataLayer.Entities.NotificationInfo;
using VisitorManagment.DataLayer.Entities;
using VisitorManagment.DataLayer.Entities.Inspection;
using System.Linq;

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
        public DbSet<RoleAssignmentHistory> RoleAssignmentHistories { get; set; }
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
        public DbSet<LoginSlide> LoginSlides { get; set; }
        public DbSet<MaintenanceSetting> MaintenanceSettings { get; set; }

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

        #region مدیریت بازرسی
        public DbSet<InspectionPerson> InspectionPeople { get; set; }
        public DbSet<InspectionMeeting> InspectionMeetings { get; set; }
        public DbSet<InspectionMeetingUnit> InspectionMeetingUnits { get; set; }
        public DbSet<InspectionMeetingPerson> InspectionMeetingPeople { get; set; }
        #endregion


        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaintenanceSetting>()
                .HasCheckConstraint("CK_MaintenanceSettings_SingleRow", "[Id] = 1");
            modelBuilder.Entity<MaintenanceSetting>().HasData(new MaintenanceSetting
            {
                Id = 1, IsEnabled = false, Revision = 0,
                Message = "سامانه موقتاً در حال به‌روزرسانی است. لطفاً کمی بعد دوباره مراجعه کنید."
            });
            modelBuilder.Entity<LoginSlide>()
                .HasIndex(slide => slide.SortOrder);
            modelBuilder.Entity<NotificationUser>()
                .HasIndex(item => new { item.NotificationId, item.UserId })
                .IsUnique();
            modelBuilder.Entity<RoleAssignmentHistory>()
                .HasIndex(item => item.RoleId);
            modelBuilder.Entity<RoleAssignmentHistory>()
                .HasIndex(item => item.UserId);
            modelBuilder.Entity<RoleAssignmentHistory>()
                .HasIndex(item => new { item.RoleId, item.UserId })
                .HasFilter("[EndDate] IS NULL")
                .IsUnique();
            modelBuilder.Entity<Cartable>()
                .HasIndex(item => item.RcvrRoleId);
            modelBuilder.Entity<InspectionPerson>()
                .HasIndex(person => person.PersonalCode)
                .IsUnique();
            modelBuilder.Entity<InspectionMeetingUnit>()
                .HasIndex(unit => new { unit.InspectionMeetingId, unit.UnitCode })
                .IsUnique();
            modelBuilder.Entity<InspectionMeetingUnit>()
                .HasOne(unit => unit.Meeting)
                .WithMany(meeting => meeting.Units)
                .HasForeignKey(unit => unit.InspectionMeetingId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<InspectionMeetingPerson>()
                .HasIndex(item => new { item.InspectionMeetingId, item.InspectionPersonId }).IsUnique();
            modelBuilder.Entity<InspectionMeetingPerson>().HasOne(item => item.Meeting).WithMany(meeting => meeting.People)
                .HasForeignKey(item => item.InspectionMeetingId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<InspectionMeetingPerson>().HasOne(item => item.Person).WithMany(person => person.Meetings)
                .HasForeignKey(item => item.InspectionPersonId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChatMessage>()
                .Property(message => message.ReplyToMessage)
                .HasMaxLength(180);
            modelBuilder.Entity<Users>()
              .HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Files>()
                .HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Meeting>()
               .HasQueryFilter(u => !u.IsDelete);
        }

        /// <summary>
        /// نقش مقصد کارتابل و مشخصات ثابت ثبت‌کننده هامش را به‌صورت مرکزی تکمیل می‌کند.
        /// به این ترتیب مسیرهای مختلف ثبت درخواست، رفتار یکسانی خواهند داشت.
        /// </summary>
        public override int SaveChanges()
        {
            CompleteAuditInformation();
            return base.SaveChanges();
        }

        private void CompleteAuditInformation()
        {
            var addedCartables = ChangeTracker.Entries<Cartable>()
                .Where(entry => entry.State == EntityState.Added && !entry.Entity.RcvrRoleId.HasValue && entry.Entity.RcvrUserId > 0)
                .ToList();
            foreach (var entry in addedCartables)
            {
                entry.Entity.RcvrRoleId = UserRoles.AsNoTracking()
                    .Where(item => item.UserId == entry.Entity.RcvrUserId)
                    .Select(item => (int?)item.RoleId)
                    .FirstOrDefault();
            }

            var addedHameshes = ChangeTracker.Entries<Hamesh>()
                .Where(entry => entry.State == EntityState.Added)
                .ToList();
            foreach (var entry in addedHameshes)
            {
                var hamesh = entry.Entity;
                var user = Users.IgnoreQueryFilters().AsNoTracking().FirstOrDefault(item => item.Id == hamesh.UserId);
                if (user == null) continue;

                hamesh.ActorFullNameSnapshot = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(value => !string.IsNullOrWhiteSpace(value)));
                hamesh.ActorPersonalCodeSnapshot = user.UserName;
                hamesh.ActorRankTitleSnapshot = user.RankTitle;
                hamesh.ActorUnitTitleSnapshot = string.IsNullOrWhiteSpace(user.UnitDutyTitle) ? user.UnitTitle : user.UnitDutyTitle;
            }
        }
    }
}
