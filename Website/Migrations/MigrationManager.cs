using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace NuGetGallery.Migrations
{
    internal static partial class MigrationManager
    {
        public static HashSet<string> Applied { get; private set; }
        public static HashSet<string> Local { get; private set; }
        public static HashSet<string> Pending { get; private set; }
        private static DbMigrator _migrator;

        public static void Initialize()
        {
            // Go get the list of migrations
            _migrator = new DbMigrator(new MigrationsConfiguration());
            ScanMigrations();
        }

        public static bool HasMigration(string name)
        {
            return Applied.Contains(name);
        }

        public static void ApplyPendingMigrations()
        {
            _migrator.Update();
            ScanMigrations();
        }

        private static void ScanMigrations()
        {
            Applied = new HashSet<string>(_migrator.GetDatabaseMigrations(), StringComparer.OrdinalIgnoreCase);
            Local = new HashSet<string>(_migrator.GetLocalMigrations(), StringComparer.OrdinalIgnoreCase);
            Pending = new HashSet<string>(_migrator.GetPendingMigrations(), StringComparer.OrdinalIgnoreCase);
        }

        internal static IDatabaseInitializer<EntitiesContext> CreateInitializer()
        {
            return new MigrationInitializer();
        }

        private class MigrationInitializer : IDatabaseInitializer<EntitiesContext>
        {
            private MigrateDatabaseToLatestVersion<EntitiesContext, MigrationsConfiguration> _innerMigrator = new MigrateDatabaseToLatestVersion<EntitiesContext, MigrationsConfiguration>();

            public void InitializeDatabase(EntitiesContext context)
            {
                _innerMigrator.InitializeDatabase(context);

                // Refresh the migrations list
                MigrationManager.ScanMigrations();
            }
        }
    }
}