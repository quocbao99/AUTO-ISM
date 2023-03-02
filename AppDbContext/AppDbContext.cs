using Entities;
using Entities.Catalogue;
using Entities.Configuration;
using Entities.Paypal;
using Entities.Stripe;
using Extensions;
using Interface.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AppDbContext
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            //modelBuilder.Entity<OTPHistories>(x => x.ToTable("OTPHistories"));
            //modelBuilder.Entity<SMSEmailTemplates>(x => x.ToTable("SMSEmailTemplates"));

            //#region Configuration
            //modelBuilder.Entity<EmailConfigurations>(x => x.ToTable("EmailConfigurations"));
            //modelBuilder.Entity<SMSConfigurations>(x => x.ToTable("SMSConfigurations"));
            //#endregion

            //Data seeding (tạo dữ liệu mẫu - ....Extension.ModelBuilderExtensions)
            //modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Districts> Districts { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserFavorite> UserFavorite { get; set; }
        public DbSet<UserHistory> UserHistory { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<Market> Market { get; set; }
        public DbSet<Car> Car { get; set; }
        public DbSet<EModel> EModel { get; set; }
        public DbSet<LineOff> LineOff { get; set; }
        public DbSet<SystemFile> SystemFile { get; set; }
        public DbSet<DTC> DTC { get; set; }
        public DbSet<Material> Material { get; set; }
        public DbSet<MaterialSub> MaterialSub { get; set; }
        public DbSet<Package> Package { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Contract> Contract { get; set; }
        public DbSet<PaymentSession> paymentSession { get; set; }
        public DbSet<PaymentMethodConfiguration> PaymentMethodConfiguration { get; set; }
        public DbSet<PaymentMethodType> PaymentMethodType { get; set; }
        public DbSet<EmailConfigurations> EmailConfigurations { get; set; }
        public DbSet<HangfireManage> HangfireManage { get; set; }
        public DbSet<OTPHistories> OTPHistories { get; set; }
        public DbSet<CurrencyExchangeRate> CurrencyExchangeRate { get; set; }
        public DbSet<AccessDescription> AccessDescription { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<UserNotification> UserNotification { get; set; }
        public DbSet<Policy> Policy { get; set; }

        #region paypal
        public DbSet<PlanPaypal> PlanPaypal { get; set; }
        #endregion

        #region stripe
        public DbSet<PriceStripe> PriceStripe { get; set; }

        #endregion
    }
}