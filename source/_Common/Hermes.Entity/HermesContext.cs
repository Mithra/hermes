using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Hermes.Entity.Models;

namespace Hermes.Entity
{
    public class HermesContext : DbContext
    {
        public HermesContext()
            : this(false)
        {
        }

        public HermesContext(string connectionString)
            : base(connectionString)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }

        public HermesContext(bool enableAutoDetectChanges = false)
            : base("Name=HermesContext")
        {
            //Database.SetInitializer<DataVaultContext>(null);
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = enableAutoDetectChanges;
            Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTag> NotificationTags { get; set; }
        public DbSet<NotificationClientReceipt> NotificationClientReceipts { get; set; }

        public bool ExecuteInTransaction(Action<HermesContext> logic)
        {
            using (DbContextTransaction transaction = Database.BeginTransaction())
            {
                try
                {
                    logic(this);
                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public T ExecuteInTransaction<T>(Func<HermesContext, T> logic)
        {
            using (DbContextTransaction transaction = Database.BeginTransaction())
            {
                try
                {
                    T res = logic(this);
                    transaction.Commit();

                    return res;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return default(T);
                }
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
