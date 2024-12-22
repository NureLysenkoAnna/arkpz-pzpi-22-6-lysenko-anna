using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasDec.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    location_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    location_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    floor = table.Column<int>(type: "int", nullable: false),
                    area = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.location_id);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    sensor_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    installation_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    location_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.sensor_id);
                    table.ForeignKey(
                        name: "FK_Sensors_Locations_location_id",
                        column: x => x.location_id,
                        principalTable: "Locations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    location_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Users_Locations_location_id",
                        column: x => x.location_id,
                        principalTable: "Locations",
                        principalColumn: "location_id");
                });

            migrationBuilder.CreateTable(
                name: "SensorChecks",
                columns: table => new
                {
                    check_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    check_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    result = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    sensor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorChecks", x => x.check_id);
                    table.ForeignKey(
                        name: "FK_SensorChecks_Sensors_sensor_id",
                        column: x => x.sensor_id,
                        principalTable: "Sensors",
                        principalColumn: "sensor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorData",
                columns: table => new
                {
                    data_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    gas_level = table.Column<double>(type: "float", nullable: false),
                    temperature = table.Column<double>(type: "float", nullable: false),
                    pressure = table.Column<double>(type: "float", nullable: false),
                    time_stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sensor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorData", x => x.data_id);
                    table.ForeignKey(
                        name: "FK_SensorData_Sensors_sensor_id",
                        column: x => x.sensor_id,
                        principalTable: "Sensors",
                        principalColumn: "sensor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    event_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    data_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.event_id);
                    table.ForeignKey(
                        name: "FK_Events_SensorData_data_id",
                        column: x => x.data_id,
                        principalTable: "SensorData",
                        principalColumn: "data_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    formation_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    notification_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    event_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_Notifications_Events_event_id",
                        column: x => x.event_id,
                        principalTable: "Events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_data_id",
                table: "Events",
                column: "data_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_event_id",
                table: "Notifications",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SensorChecks_sensor_id",
                table: "SensorChecks",
                column: "sensor_id");

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_sensor_id",
                table: "SensorData",
                column: "sensor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_location_id",
                table: "Sensors",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_location_id",
                table: "Users",
                column: "location_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "SensorChecks");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SensorData");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
