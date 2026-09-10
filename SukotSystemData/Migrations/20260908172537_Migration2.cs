using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SukotSystemData.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rabbis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Phone2 = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    HomeCityId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rabbis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rabbis_Cities_HomeCityId",
                        column: x => x.HomeCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    PerformedRabbiId = table.Column<int>(type: "integer", nullable: true),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PerformedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NeedsTool = table.Column<bool>(type: "boolean", nullable: false),
                    Commits = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Adress = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Rabbis_PerformedRabbiId",
                        column: x => x.PerformedRabbiId,
                        principalTable: "Rabbis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RabbiCoveredCities",
                columns: table => new
                {
                    CoveredCitiesId = table.Column<int>(type: "integer", nullable: false),
                    RabbisId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RabbiCoveredCities", x => new { x.CoveredCitiesId, x.RabbisId });
                    table.ForeignKey(
                        name: "FK_RabbiCoveredCities_Cities_CoveredCitiesId",
                        column: x => x.CoveredCitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RabbiCoveredCities_Rabbis_RabbisId",
                        column: x => x.RabbisId,
                        principalTable: "Rabbis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "עפולה" },
                    { 2, "עכו" },
                    { 3, "ערד" },
                    { 4, "אריאל" },
                    { 5, "אשדוד" },
                    { 6, "אשקלון" },
                    { 7, "באקה אל-גרביה" },
                    { 8, "בת ים" },
                    { 9, "באר שבע" },
                    { 10, "באר יעקב" },
                    { 11, "בית שאן" },
                    { 12, "בית שמש" },
                    { 13, "ביתר עילית" },
                    { 14, "בני ברק" },
                    { 15, "דימונה" },
                    { 16, "אילת" },
                    { 17, "אלעד" },
                    { 18, "גני תקווה" },
                    { 19, "גבעת שמואל" },
                    { 20, "גבעת זאב" },
                    { 21, "גבעתיים" },
                    { 22, "חדרה" },
                    { 23, "חיפה" },
                    { 24, "חריש" },
                    { 25, "הרצליה" },
                    { 26, "הוד השרון" },
                    { 27, "חולון" },
                    { 28, "ירושלים" },
                    { 29, "כרמיאל" },
                    { 30, "כפר סבא" },
                    { 31, "כפר יונה" },
                    { 32, "קרית אתא" },
                    { 33, "קרית ביאליק" },
                    { 34, "קרית גת" },
                    { 35, "קרית מלאכי" },
                    { 36, "קרית מוצקין" },
                    { 37, "קרית אונו" },
                    { 38, "קרית שמונה" },
                    { 39, "קרית ים" },
                    { 40, "לוד" },
                    { 41, "מעלה אדומים" },
                    { 42, "מעלות-תרשיחא" },
                    { 43, "מגדל העמק" },
                    { 44, "מודיעין-מכבים-רעות" },
                    { 45, "נהריה" },
                    { 46, "נצרת" },
                    { 47, "נס ציונה" },
                    { 48, "נשר" },
                    { 49, "נתניה" },
                    { 50, "נתיבות" },
                    { 51, "נוף הגליל" },
                    { 52, "אופקים" },
                    { 53, "אור עקיבא" },
                    { 54, "אור יהודה" },
                    { 55, "פתח תקווה" },
                    { 56, "קלנסווה" },
                    { 57, "רעננה" },
                    { 58, "רהט" },
                    { 59, "רמת השרון" },
                    { 60, "רמת גן" },
                    { 61, "רמלה" },
                    { 62, "רחובות" },
                    { 63, "ראשון לציון" },
                    { 64, "ראש העין" },
                    { 65, "סחנין" },
                    { 66, "שדרות" },
                    { 67, "שפרעם" },
                    { 68, "טייבה" },
                    { 69, "תמרה" },
                    { 70, "תל אביב-יפו" },
                    { 71, "טבריה" },
                    { 72, "טירה" },
                    { 73, "טירת כרמל" },
                    { 74, "צפת" },
                    { 75, "אום אל-פחם" },
                    { 76, "יבנה" },
                    { 77, "יהוד-מונוסון" },
                    { 78, "יקנעם עילית" },
                    { 79, "אבו גוש" },
                    { 80, "אבו סנאן" },
                    { 81, "אלפי מנשה" },
                    { 82, "ערערה" },
                    { 83, "ערערה בנגב" },
                    { 84, "עראבה" },
                    { 85, "אזור" },
                    { 86, "בסמ\"ה" },
                    { 87, "בית אריה" },
                    { 88, "בית דגן" },
                    { 89, "בית אל" },
                    { 90, "בית ג'ן" },
                    { 91, "בנימינה-גבעת עדה" },
                    { 92, "ביר אל-מכסור" },
                    { 93, "בני עי\"ש" },
                    { 94, "בסמת טבעון" },
                    { 95, "בועיינה-נוג'ידאת" },
                    { 96, "בוקעאתה" },
                    { 97, "דבוריה" },
                    { 98, "דיר חנא" },
                    { 99, "דיר אל-אסד" },
                    { 100, "אפרת" },
                    { 101, "עילבון" },
                    { 102, "עין קיניה" },
                    { 103, "עין מאהל" },
                    { 104, "אלקנה" },
                    { 105, "אליכין" },
                    { 106, "עמנואל" },
                    { 107, "אבן יהודה" },
                    { 108, "פסוטה" },
                    { 109, "פורידיס" },
                    { 110, "גן יבנה" },
                    { 111, "גדרה" },
                    { 112, "הר אדר" },
                    { 113, "חצור הגלילית" },
                    { 114, "חורה" },
                    { 115, "חורפיש" },
                    { 116, "אעבלין" },
                    { 117, "אכסאל" },
                    { 118, "עילוט" },
                    { 119, "ג'לג'וליה" },
                    { 120, "ג'סר א-זרקא" },
                    { 121, "ג'דיידה-מכר" },
                    { 122, "ג'וליס" },
                    { 123, "כעביה-טבאש-חג'אג'רה" },
                    { 124, "כאבול" },
                    { 125, "קדימה-צורן" },
                    { 126, "כפר ברא" },
                    { 127, "כפר מנדא" },
                    { 128, "כפר קרע" },
                    { 129, "כפר יאסיף" },
                    { 130, "קרני שומרון" },
                    { 131, "קצרין" },
                    { 132, "קדומים" },
                    { 133, "כפר קמה" },
                    { 134, "כפר כנא" },
                    { 135, "כפר שמריהו" },
                    { 136, "כפר תבור" },
                    { 137, "כפר ורדים" },
                    { 138, "קרית ארבע" },
                    { 139, "קרית עקרון" },
                    { 140, "קרית טבעון" },
                    { 141, "קרית יערים" },
                    { 142, "כסרא-סמיע" },
                    { 143, "כוכב יאיר-צור יגאל" },
                    { 144, "כסייפה" },
                    { 145, "לקיה" },
                    { 146, "להבים" },
                    { 147, "מעלה עירון" },
                    { 148, "מעלה אפרים" },
                    { 149, "מג'אר" },
                    { 150, "מג'דל שמס" },
                    { 151, "מסעדה" },
                    { 152, "משהד" },
                    { 153, "מזכרת בתיה" },
                    { 154, "מזרעה" },
                    { 155, "מיתר" },
                    { 156, "מטולה" },
                    { 157, "מבשרת ציון" },
                    { 158, "מעיליא" },
                    { 159, "מגדל" },
                    { 160, "מגדל תפן" },
                    { 161, "מצפה רמון" },
                    { 162, "מודיעין עילית" },
                    { 163, "נחף" },
                    { 164, "נאות חובב" },
                    { 165, "עומר" },
                    { 166, "אורנית" },
                    { 167, "פרדס חנה-כרכור" },
                    { 168, "פרדסיה" },
                    { 169, "פקיעין (בוקייעה)" },
                    { 170, "רמת ישי" },
                    { 171, "ראמה" },
                    { 172, "ריינה" },
                    { 173, "רכסים" },
                    { 174, "ראש פינה" },
                    { 175, "סאג'ור" },
                    { 176, "סביון" },
                    { 177, "שגב שלום" },
                    { 178, "שעב" },
                    { 179, "שבלי-אום אל-גנם" },
                    { 180, "שלומי" },
                    { 181, "שהם" },
                    { 182, "תל מונד" },
                    { 183, "תל שבע" },
                    { 184, "טובא-זנגריה" },
                    { 185, "טורעאן" },
                    { 186, "יפיע" },
                    { 187, "יאנוח-ג'ת" },
                    { 188, "ירכא" },
                    { 189, "יבנאל" },
                    { 190, "ירוחם" },
                    { 191, "יסוד המעלה" },
                    { 192, "זרזיר" },
                    { 193, "זמר" },
                    { 194, "זכרון יעקב" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CityId",
                table: "Orders",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PerformedRabbiId",
                table: "Orders",
                column: "PerformedRabbiId");

            migrationBuilder.CreateIndex(
                name: "IX_RabbiCoveredCities_RabbisId",
                table: "RabbiCoveredCities",
                column: "RabbisId");

            migrationBuilder.CreateIndex(
                name: "IX_Rabbis_HomeCityId",
                table: "Rabbis",
                column: "HomeCityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "RabbiCoveredCities");

            migrationBuilder.DropTable(
                name: "Rabbis");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
