using Microsoft.EntityFrameworkCore;
using SukotSystemCore.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemData
{
    public class DataContex : DbContext
    {

        public DataContex(DbContextOptions<DataContex> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Rabbi> Rabbis { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many (requirement 7): cities a Rabbi is willing to cover, beyond HomeCity.
            // EF Core 8 can generate the join table itself; naming it explicitly just keeps the
            // schema readable when inspecting the database directly.
            modelBuilder.Entity<Rabbi>()
                .HasMany(r => r.CoveredCities)
                .WithMany(c => c.Rabbis)
                .UsingEntity(j => j.ToTable("RabbiCoveredCities"));

            // Rabbi.HomeCity: required many-to-one, no inverse collection on City needed.
            // Restrict so a City that rabbis still call home can't be deleted out from under them.
            modelBuilder.Entity<Rabbi>()
                .HasOne(r => r.HomeCity)
                .WithMany()
                .HasForeignKey(r => r.HomeCityId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many (requirement 7): City -> Order.
            modelBuilder.Entity<Order>()
                .HasOne(o => o.City)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer -> Order (one-to-many, no inverse collection on Customer needed).
            modelBuilder.Entity<Order>()
                .HasOne(o => o.RequestedCustomer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rabbi -> Order (optional - PerformedRabbiId is nullable until claimed).
            // SetNull rather than Cascade/Restrict: deactivating (or, hypothetically, deleting) a
            // Rabbi must never delete Orders - historical Orders survive with PerformedRabbiId
            // cleared instead.
            modelBuilder.Entity<Order>()
                .HasOne(o => o.PerformedRabbi)
                .WithMany(r => r.HandledOrders)
                .HasForeignKey(o => o.PerformedRabbiId)
                .OnDelete(DeleteBehavior.SetNull);

            // Order.Version is already picked up as a concurrency token via the [ConcurrencyCheck]
            // data annotation on the model itself (Appendix D's self-managed approach) - nothing
            // to configure here in Fluent API for it.

            // Seed data (requirement 5): a real list of Israeli cities/local councils, not
            // placeholder names, chosen with the user so POST /rabbis (HomeCityId) and
            // POST /orders (CityId) have real, immediately-usable values as soon as the
            // database is created - no manual DB intervention needed before the system runs.
            modelBuilder.Entity<City>().HasData(
                _citySeedNames.Select((name, index) => new City
                {
                    Id = index + 1,
                    Name = name
                }));

            modelBuilder.Entity<Admin>().HasData(
                _adminsSeedsNames.Select((name, index) => new Admin
                {
                    Id = index + 1,

                    FullName = name,
                    // NOTE: was previously a literal '1234' (not hashed) - Verify() splits a
                    // stored hash on '.' expecting 3 parts (iterations.salt.key), so a raw
                    // '1234' always failed parsing and Verify() always returned false, no
                    // matter what password was typed at login - this is what caused the 401.
                    // This is a fixed PBKDF2 hash (100_000 iterations, SHA256) of the literal
                    // password "1234", precomputed so HasData stays deterministic across
                    // migrations (calling PasswordHasher.Hash(...) here would re-randomize the
                    // salt every time OnModelCreating runs and create a spurious migration diff).
                    Phone = index == 0 ? "0556781862" : "Mefatchim",
                    PasswordHash = "100000.bv0SLUsDGpMM+srg54YgjA==.SqnUa/+8WvKmTTcyx52+m68DRrEcoasVIUVsNyAcW6o="
                }));



        }
        private static readonly string[] _adminsSeedsNames = new[]
        {
            "אלף שין","מפתחים"
        };
        private static readonly string[] _citySeedNames = new[]
        {
            "עפולה", "עכו", "ערד", "אריאל",
            "אשדוד", "אשקלון", "באקה אל-גרביה", "בת ים",
            "באר שבע", "באר יעקב", "בית שאן", "בית שמש",
            "ביתר עילית", "בני ברק", "דימונה", "אילת",
            "אלעד", "גני תקווה", "גבעת שמואל", "גבעת זאב",
            "גבעתיים", "חדרה", "חיפה", "חריש",
            "הרצליה", "הוד השרון", "חולון", "ירושלים",
            "כרמיאל", "כפר סבא", "כפר יונה", "קרית אתא",
            "קרית ביאליק", "קרית גת", "קרית מלאכי", "קרית מוצקין",
            "קרית אונו", "קרית שמונה", "קרית ים", "לוד",
            "מעלה אדומים", "מעלות-תרשיחא", "מגדל העמק", "מודיעין-מכבים-רעות",
            "נהריה", "נצרת", "נס ציונה", "נשר",
            "נתניה", "נתיבות", "נוף הגליל", "אופקים",
            "אור עקיבא", "אור יהודה", "פתח תקווה", "קלנסווה",
            "רעננה", "רהט", "רמת השרון", "רמת גן",
            "רמלה", "רחובות", "ראשון לציון", "ראש העין",
            "סחנין", "שדרות", "שפרעם", "טייבה",
            "תמרה", "תל אביב-יפו", "טבריה", "טירה",
            "טירת כרמל", "צפת", "אום אל-פחם", "יבנה",
            "יהוד-מונוסון", "יקנעם עילית", "אבו גוש", "אבו סנאן",
            "אלפי מנשה", "ערערה", "ערערה בנגב", "עראבה",
            "אזור", "בסמ\"ה", "בית אריה", "בית דגן",
            "בית אל", "בית ג'ן", "בנימינה-גבעת עדה", "ביר אל-מכסור",
            "בני עי\"ש", "בסמת טבעון", "בועיינה-נוג'ידאת", "בוקעאתה",
            "דבוריה", "דיר חנא", "דיר אל-אסד", "אפרת",
            "עילבון", "עין קיניה", "עין מאהל", "אלקנה",
            "אליכין", "עמנואל", "אבן יהודה", "פסוטה",
            "פורידיס", "גן יבנה", "גדרה", "הר אדר",
            "חצור הגלילית", "חורה", "חורפיש", "אעבלין",
            "אכסאל", "עילוט", "ג'לג'וליה", "ג'סר א-זרקא",
            "ג'דיידה-מכר", "ג'וליס", "כעביה-טבאש-חג'אג'רה", "כאבול",
            "קדימה-צורן", "כפר ברא", "כפר מנדא", "כפר קרע",
            "כפר יאסיף", "קרני שומרון", "קצרין", "קדומים",
            "כפר קמה", "כפר כנא", "כפר שמריהו", "כפר תבור",
            "כפר ורדים", "קרית ארבע", "קרית עקרון", "קרית טבעון",
            "קרית יערים", "כסרא-סמיע", "כוכב יאיר-צור יגאל", "כסייפה",
            "לקיה", "להבים", "מעלה עירון", "מעלה אפרים",
            "מג'אר", "מג'דל שמס", "מסעדה", "משהד",
            "מזכרת בתיה", "מזרעה", "מיתר", "מטולה",
            "מבשרת ציון", "מיצד", "מגדל", "מגדל תפן",
            "מצפה רמון", "מודיעין עילית", "נחף", "נאות חובב",
            "עומר", "אורנית", "פרדס חנה-כרכור", "פרדסיה",
            "פקיעין (בוקייעה)", "רמת ישי", "ראמה", "ריינה",
            "רכסים", "ראש פינה", "סאג'ור", "סביון",
            "שגב שלום", "שעב", "שבלי-אום אל-גנם", "שלומי",
            "שהם", "תל מונד", "תל שבע", "טובא-זנגריה",
            "טורעאן", "יפיע", "תל ציון", "ירכא",
            "יבנאל", "ירוחם", "יסוד המעלה", "זרזיר",
            "זמר", "זכרון יעקב",
        };

        // Part C: replaces every modified Order's concurrency token right before it is written,
        // so no Service ever has to remember to do this itself (per the project guide's own
        // recommendation for the self-managed-token approach). This runs BEFORE the underlying
        // SaveChanges - EF Core still uses each entry's ORIGINAL Version (captured when the row
        // was loaded) for the UPDATE's WHERE clause, so setting a new CurrentValues.Version here
        // does not weaken the concurrency check; it only decides what the row's NEXT version
        // will be once this save succeeds.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Order>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Version = System.Guid.NewGuid();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
