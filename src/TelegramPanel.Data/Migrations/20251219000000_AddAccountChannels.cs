using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramPanel.Data.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20251219000000_AddAccountChannels")]
    public partial class AddAccountChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQLite 不支持直接 DropForeignKey/AlterColumn（需要重建表）
            migrationBuilder.Sql("PRAGMA foreign_keys=OFF;");
            migrationBuilder.Sql(@"
CREATE TABLE ""Channels__temp"" (
    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Channels"" PRIMARY KEY AUTOINCREMENT,
    ""TelegramId"" INTEGER NOT NULL,
    ""AccessHash"" INTEGER NULL,
    ""Title"" TEXT NOT NULL,
    ""Username"" TEXT NULL,
    ""IsBroadcast"" INTEGER NOT NULL,
    ""MemberCount"" INTEGER NOT NULL,
    ""About"" TEXT NULL,
    ""CreatorAccountId"" INTEGER NULL,
    ""GroupId"" INTEGER NULL,
    ""CreatedAt"" TEXT NULL,
    ""SyncedAt"" TEXT NOT NULL,
    CONSTRAINT ""FK_Channels_Accounts_CreatorAccountId"" FOREIGN KEY (""CreatorAccountId"") REFERENCES ""Accounts"" (""Id"") ON DELETE SET NULL,
    CONSTRAINT ""FK_Channels_ChannelGroups_GroupId"" FOREIGN KEY (""GroupId"") REFERENCES ""ChannelGroups"" (""Id"") ON DELETE SET NULL
);
");

            migrationBuilder.Sql(@"
INSERT INTO ""Channels__temp"" (""Id"", ""TelegramId"", ""AccessHash"", ""Title"", ""Username"", ""IsBroadcast"", ""MemberCount"", ""About"", ""CreatorAccountId"", ""GroupId"", ""CreatedAt"", ""SyncedAt"")
SELECT ""Id"", ""TelegramId"", ""AccessHash"", ""Title"", ""Username"", ""IsBroadcast"", ""MemberCount"", ""About"", ""CreatorAccountId"", ""GroupId"", ""CreatedAt"", ""SyncedAt""
FROM ""Channels"";
");

            migrationBuilder.Sql(@"DROP TABLE ""Channels"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Channels__temp"" RENAME TO ""Channels"";");

            migrationBuilder.Sql(@"CREATE INDEX ""IX_Channels_CreatorAccountId"" ON ""Channels"" (""CreatorAccountId"");");
            migrationBuilder.Sql(@"CREATE INDEX ""IX_Channels_GroupId"" ON ""Channels"" (""GroupId"");");
            migrationBuilder.Sql(@"CREATE UNIQUE INDEX ""IX_Channels_TelegramId"" ON ""Channels"" (""TelegramId"");");
            migrationBuilder.Sql(@"CREATE INDEX ""IX_Channels_Username"" ON ""Channels"" (""Username"");");

            migrationBuilder.Sql("PRAGMA foreign_keys=ON;");

            migrationBuilder.CreateTable(
                name: "AccountChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChannelId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCreator = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountChannels_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountChannels_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountChannels_ChannelId",
                table: "AccountChannels",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountChannels_AccountId_ChannelId",
                table: "AccountChannels",
                columns: new[] { "AccountId", "ChannelId" },
                unique: true);

            // 为已有“系统创建频道”回填关联表，保证按账号筛选能立即生效
            migrationBuilder.Sql(@"
INSERT OR IGNORE INTO AccountChannels (AccountId, ChannelId, IsCreator, IsAdmin, SyncedAt)
SELECT CreatorAccountId, Id, 1, 1, SyncedAt
FROM Channels
WHERE CreatorAccountId IS NOT NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountChannels");

            migrationBuilder.Sql("PRAGMA foreign_keys=OFF;");

            // 回滚到“CreatorAccountId 必填”时，先移除新引入的“仅管理员频道”数据，避免不满足 NOT NULL
            migrationBuilder.Sql(@"DELETE FROM ""Channels"" WHERE ""CreatorAccountId"" IS NULL;");

            migrationBuilder.Sql(@"
CREATE TABLE ""Channels__temp"" (
    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Channels"" PRIMARY KEY AUTOINCREMENT,
    ""TelegramId"" INTEGER NOT NULL,
    ""AccessHash"" INTEGER NULL,
    ""Title"" TEXT NOT NULL,
    ""Username"" TEXT NULL,
    ""IsBroadcast"" INTEGER NOT NULL,
    ""MemberCount"" INTEGER NOT NULL,
    ""About"" TEXT NULL,
    ""CreatorAccountId"" INTEGER NOT NULL,
    ""GroupId"" INTEGER NULL,
    ""CreatedAt"" TEXT NULL,
    ""SyncedAt"" TEXT NOT NULL,
    CONSTRAINT ""FK_Channels_Accounts_CreatorAccountId"" FOREIGN KEY (""CreatorAccountId"") REFERENCES ""Accounts"" (""Id"") ON DELETE CASCADE,
    CONSTRAINT ""FK_Channels_ChannelGroups_GroupId"" FOREIGN KEY (""GroupId"") REFERENCES ""ChannelGroups"" (""Id"") ON DELETE SET NULL
);
");

            migrationBuilder.Sql(@"
INSERT INTO ""Channels__temp"" (""Id"", ""TelegramId"", ""AccessHash"", ""Title"", ""Username"", ""IsBroadcast"", ""MemberCount"", ""About"", ""CreatorAccountId"", ""GroupId"", ""CreatedAt"", ""SyncedAt"")
SELECT ""Id"", ""TelegramId"", ""AccessHash"", ""Title"", ""Username"", ""IsBroadcast"", ""MemberCount"", ""About"", ""CreatorAccountId"", ""GroupId"", ""CreatedAt"", ""SyncedAt""
FROM ""Channels"";
");

            migrationBuilder.Sql(@"DROP TABLE ""Channels"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Channels__temp"" RENAME TO ""Channels"";");

            migrationBuilder.Sql(@"CREATE INDEX ""IX_Channels_CreatorAccountId"" ON ""Channels"" (""CreatorAccountId"");");
            migrationBuilder.Sql(@"CREATE INDEX ""IX_Channels_GroupId"" ON ""Channels"" (""GroupId"");");
            migrationBuilder.Sql(@"CREATE UNIQUE INDEX ""IX_Channels_TelegramId"" ON ""Channels"" (""TelegramId"");");
            migrationBuilder.Sql(@"CREATE INDEX ""IX_Channels_Username"" ON ""Channels"" (""Username"");");

            migrationBuilder.Sql("PRAGMA foreign_keys=ON;");
        }
    }
}
