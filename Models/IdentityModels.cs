using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WebApplication1.Models
{
    // 您可以在 ApplicationUser 類別新增更多屬性，為使用者新增設定檔資料，請造訪 http://go.microsoft.com/fwlink/?LinkID=317594 以深入了解。
    
    public class ApplicationUser : IdentityUser
    {
        public string userid { get; set; }
        public virtual ICollection<StkHoldingModel> StkholdingModels { get; set; }
        public virtual ICollection<CashholdingModel> CashholdingModels { get; set; }
        public virtual ICollection<txModel> txModels { get; set; }
        public virtual ICollection<Pending_txModel> Pending_txModels { get; set; }
        public virtual ICollection<MM_Model> MM_Models { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在這裡新增自訂使用者宣告
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

            this.Configuration.LazyLoadingEnabled = false;

        }

      
           public DbSet<StkHoldingModel> StkHoldingModels { get; set; }
        public DbSet<CashholdingModel> CashHoldingModels { get; set; }
        public DbSet<txModel> txModels { get; set; }
        public DbSet<Pending_txModel> Pending_txModels { get; set; }
        public DbSet<StkModel> StkModels { get; set; }
        public DbSet<StklistModel> StklistModels { get; set; }
        public DbSet<MM_Model> MM_Models { get; set; }
        public DbSet<Recommend_Model> Recommend_Models { get; set; }
        public DbSet<Analyst_Model> Analyst_Models { get; set; }
        public DbSet<details_Model> details_Models { get; set; }
        public DbSet<M6_Model> M6_Models { get; set; }
        /*  protected override void OnModelCreating(DbModelBuilder modelBuilder)
          {
              modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
          }
        */

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<WebApplication1.ViewModels.ViewModel_1> ViewModel_1 { get; set; }
    }
}