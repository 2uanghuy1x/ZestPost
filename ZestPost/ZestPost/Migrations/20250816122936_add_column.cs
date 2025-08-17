using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZestPost.Migrations
{
    /// <inheritdoc />
    public partial class add_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FbDtsg",
                table: "AccountFBs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailrecoveryPass",
                table: "AccountFBs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenMail",
                table: "AccountFBs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FbDtsg",
                table: "AccountFBs");

            migrationBuilder.DropColumn(
                name: "MailrecoveryPass",
                table: "AccountFBs");

            migrationBuilder.DropColumn(
                name: "TokenMail",
                table: "AccountFBs");
        }
    }
}
