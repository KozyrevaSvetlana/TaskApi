using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class _002_SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tasks",
                columns: new[] { "Id", "CompletedAt", "CreatedAt", "IsCompleted", "Priority", "Title" },
                values: new object[,]
                {
                    { new Guid("00000001-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 19, 13, 12, 43, 730, DateTimeKind.Unspecified).AddTicks(4731), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 17, 6, 20, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Обновить документацию #001" },
                    { new Guid("00000002-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 17, 19, 29, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Обновить план #002" },
                    { new Guid("00000003-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 31, 16, 35, 59, 980, DateTimeKind.Unspecified).AddTicks(6166), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 31, 12, 59, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Завершить отчёт #003" },
                    { new Guid("00000004-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 13, 4, 31, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Подготовить документацию #004" },
                    { new Guid("00000005-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 8, 21, 24, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Проверить задачу #005" },
                    { new Guid("00000006-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 30, 6, 39, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Изучить задачу #006" },
                    { new Guid("00000007-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 11, 4, 33, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Проверить план #007" },
                    { new Guid("00000008-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 9, 6, 43, 28, 807, DateTimeKind.Unspecified).AddTicks(1845), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 8, 17, 9, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Изучить план #008" },
                    { new Guid("00000009-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 14, 21, 18, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Подготовить документацию #009" },
                    { new Guid("0000000a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 20, 8, 13, 5, 867, DateTimeKind.Unspecified).AddTicks(4198), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 19, 10, 58, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Изучить документацию #010" },
                    { new Guid("0000000b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 22, 1, 14, 27, 146, DateTimeKind.Unspecified).AddTicks(3128), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 12, 16, 8, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Обновить задачу #011" },
                    { new Guid("0000000c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 11, 18, 57, 50, 632, DateTimeKind.Unspecified).AddTicks(6522), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 10, 8, 41, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Проверить отчёт #012" },
                    { new Guid("0000000d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 16, 21, 29, 39, 815, DateTimeKind.Unspecified).AddTicks(6853), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 6, 13, 57, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Подготовить отчёт #013" },
                    { new Guid("0000000e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 27, 16, 8, 5, 426, DateTimeKind.Unspecified).AddTicks(6424), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 27, 11, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Подготовить интеграцию #014" },
                    { new Guid("0000000f-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 24, 2, 5, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Изучить план #015" },
                    { new Guid("00000010-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 31, 0, 22, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Изучить план #016" },
                    { new Guid("00000011-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 14, 2, 14, 0, 776, DateTimeKind.Unspecified).AddTicks(2194), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 12, 20, 56, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Завершить план #017" },
                    { new Guid("00000012-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 29, 18, 32, 1, 684, DateTimeKind.Unspecified).AddTicks(1573), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 8, 6, 59, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Завершить задачу #018" },
                    { new Guid("00000013-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 13, 0, 3, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Завершить план #019" },
                    { new Guid("00000014-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 4, 8, 55, 57, 822, DateTimeKind.Unspecified).AddTicks(1589), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 2, 21, 1, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Подготовить отчёт #020" },
                    { new Guid("00000015-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 8, 6, 21, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Изучить задачу #021" },
                    { new Guid("00000016-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 25, 0, 34, 46, 902, DateTimeKind.Unspecified).AddTicks(8252), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 6, 11, 33, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Изучить план #022" },
                    { new Guid("00000017-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 13, 20, 24, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Проверить документацию #023" },
                    { new Guid("00000018-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 21, 6, 59, 44, 949, DateTimeKind.Unspecified).AddTicks(1494), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 5, 2, 19, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Обновить интеграцию #024" },
                    { new Guid("00000019-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 23, 2, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Подготовить отчёт #025" },
                    { new Guid("0000001a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 9, 23, 22, 53, 807, DateTimeKind.Unspecified).AddTicks(7679), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 6, 2, 28, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Изучить план #026" },
                    { new Guid("0000001b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 26, 0, 48, 33, 19, DateTimeKind.Unspecified).AddTicks(1217), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 22, 3, 26, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Завершить документацию #027" },
                    { new Guid("0000001c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 17, 0, 16, 55, 105, DateTimeKind.Unspecified).AddTicks(5440), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 16, 18, 8, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Изучить интеграцию #028" },
                    { new Guid("0000001d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 31, 3, 2, 53, 746, DateTimeKind.Unspecified).AddTicks(6520), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 19, 14, 7, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Проверить задачу #029" },
                    { new Guid("0000001e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 26, 1, 44, 0, 156, DateTimeKind.Unspecified).AddTicks(1343), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 17, 4, 27, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Обновить задачу #030" },
                    { new Guid("0000001f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 20, 17, 45, 1, 91, DateTimeKind.Unspecified).AddTicks(1640), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 11, 7, 1, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Завершить задачу #031" },
                    { new Guid("00000020-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 6, 22, 21, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Проверить документацию #032" },
                    { new Guid("00000021-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 20, 3, 25, 34, 330, DateTimeKind.Unspecified).AddTicks(6387), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 17, 10, 53, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Изучить документацию #033" },
                    { new Guid("00000022-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 30, 20, 54, 9, 367, DateTimeKind.Unspecified).AddTicks(6623), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 5, 2, 34, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Подготовить задачу #034" },
                    { new Guid("00000023-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 15, 22, 5, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Изучить план #035" },
                    { new Guid("00000024-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 15, 2, 50, 56, 142, DateTimeKind.Unspecified).AddTicks(8773), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 4, 10, 57, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Изучить план #036" },
                    { new Guid("00000025-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 12, 11, 57, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Подготовить интеграцию #037" },
                    { new Guid("00000026-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 27, 17, 14, 58, 825, DateTimeKind.Unspecified).AddTicks(8637), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 23, 15, 42, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Завершить задачу #038" },
                    { new Guid("00000027-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 31, 5, 4, 59, 300, DateTimeKind.Unspecified).AddTicks(3246), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 8, 3, 11, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Обновить интеграцию #039" },
                    { new Guid("00000028-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 29, 7, 48, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Обновить интеграцию #040" },
                    { new Guid("00000029-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 29, 10, 39, 14, 93, DateTimeKind.Unspecified).AddTicks(2575), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 25, 18, 8, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Изучить документацию #041" },
                    { new Guid("0000002a-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 6, 14, 19, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Завершить интеграцию #042" },
                    { new Guid("0000002b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 26, 11, 6, 25, 524, DateTimeKind.Unspecified).AddTicks(5685), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 12, 3, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Проверить интеграцию #043" },
                    { new Guid("0000002c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 30, 6, 51, 10, 479, DateTimeKind.Unspecified).AddTicks(7892), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 2, 14, 57, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Подготовить документацию #044" },
                    { new Guid("0000002d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 24, 20, 58, 11, 735, DateTimeKind.Unspecified).AddTicks(8366), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 20, 2, 18, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Завершить документацию #045" },
                    { new Guid("0000002e-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 18, 16, 11, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Завершить план #046" },
                    { new Guid("0000002f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 10, 0, 29, 54, 291, DateTimeKind.Unspecified).AddTicks(593), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 8, 13, 35, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Завершить документацию #047" },
                    { new Guid("00000030-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 4, 22, 24, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Изучить отчёт #048" },
                    { new Guid("00000031-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 28, 20, 29, 48, 740, DateTimeKind.Unspecified).AddTicks(2705), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 16, 3, 4, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Проверить документацию #049" },
                    { new Guid("00000032-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 9, 2, 20, 45, 595, DateTimeKind.Unspecified).AddTicks(6892), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 3, 9, 37, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Изучить отчёт #050" },
                    { new Guid("00000033-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 27, 6, 3, 45, 208, DateTimeKind.Unspecified).AddTicks(1475), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 23, 23, 34, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Изучить документацию #051" },
                    { new Guid("00000034-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 12, 9, 39, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Завершить отчёт #052" },
                    { new Guid("00000035-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 18, 2, 54, 18, 402, DateTimeKind.Unspecified).AddTicks(407), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 12, 9, 37, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Завершить задачу #053" },
                    { new Guid("00000036-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 2, 6, 25, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Завершить интеграцию #054" },
                    { new Guid("00000037-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 30, 6, 44, 34, 330, DateTimeKind.Unspecified).AddTicks(3477), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 26, 21, 21, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Подготовить план #055" },
                    { new Guid("00000038-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 24, 5, 40, 21, 705, DateTimeKind.Unspecified).AddTicks(3307), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 21, 2, 25, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Проверить документацию #056" },
                    { new Guid("00000039-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 22, 4, 59, 1, 514, DateTimeKind.Unspecified).AddTicks(379), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 7, 7, 23, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Изучить документацию #057" },
                    { new Guid("0000003a-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 31, 10, 47, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Завершить задачу #058" },
                    { new Guid("0000003b-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 22, 1, 27, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Подготовить отчёт #059" },
                    { new Guid("0000003c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 4, 20, 38, 4, 243, DateTimeKind.Unspecified).AddTicks(206), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 2, 14, 48, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Завершить документацию #060" },
                    { new Guid("0000003d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 20, 4, 1, 33, 392, DateTimeKind.Unspecified).AddTicks(5046), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 14, 10, 23, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Завершить задачу #061" },
                    { new Guid("0000003e-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 14, 6, 31, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Подготовить задачу #062" },
                    { new Guid("0000003f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 19, 21, 8, 51, 864, DateTimeKind.Unspecified).AddTicks(9146), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 7, 20, 33, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Подготовить задачу #063" },
                    { new Guid("00000040-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 26, 15, 10, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Изучить отчёт #064" },
                    { new Guid("00000041-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 23, 19, 16, 27, 508, DateTimeKind.Unspecified).AddTicks(1977), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 18, 4, 17, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Завершить план #065" },
                    { new Guid("00000042-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 13, 23, 35, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Проверить задачу #066" },
                    { new Guid("00000043-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 3, 9, 26, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Проверить документацию #067" },
                    { new Guid("00000044-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 20, 5, 9, 53, 131, DateTimeKind.Unspecified).AddTicks(8259), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 5, 1, 37, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Изучить документацию #068" },
                    { new Guid("00000045-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 16, 13, 22, 27, 998, DateTimeKind.Unspecified).AddTicks(9260), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 14, 8, 35, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Завершить документацию #069" },
                    { new Guid("00000046-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 18, 11, 35, 7, 862, DateTimeKind.Unspecified).AddTicks(7720), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 12, 4, 49, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Завершить документацию #070" },
                    { new Guid("00000047-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 19, 15, 26, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Подготовить отчёт #071" },
                    { new Guid("00000048-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 14, 20, 55, 17, 420, DateTimeKind.Unspecified).AddTicks(5318), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 6, 23, 24, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Завершить отчёт #072" },
                    { new Guid("00000049-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 16, 7, 46, 36, 80, DateTimeKind.Unspecified).AddTicks(4112), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 14, 21, 34, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Подготовить отчёт #073" },
                    { new Guid("0000004a-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 8, 7, 32, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Завершить документацию #074" },
                    { new Guid("0000004b-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 6, 12, 6, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Изучить интеграцию #075" },
                    { new Guid("0000004c-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 2, 22, 12, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Завершить документацию #076" },
                    { new Guid("0000004d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 19, 17, 49, 57, 49, DateTimeKind.Unspecified).AddTicks(7293), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 9, 9, 11, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Изучить отчёт #077" },
                    { new Guid("0000004e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 31, 8, 32, 37, 31, DateTimeKind.Unspecified).AddTicks(4473), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 6, 23, 28, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Проверить отчёт #078" },
                    { new Guid("0000004f-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 19, 20, 46, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Проверить интеграцию #079" },
                    { new Guid("00000050-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 15, 22, 28, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Подготовить интеграцию #080" },
                    { new Guid("00000051-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 27, 22, 9, 45, 274, DateTimeKind.Unspecified).AddTicks(3994), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 20, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Завершить план #081" },
                    { new Guid("00000052-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 27, 4, 40, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Обновить задачу #082" },
                    { new Guid("00000053-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 19, 20, 22, 1, 897, DateTimeKind.Unspecified).AddTicks(8490), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 12, 6, 50, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Подготовить интеграцию #083" },
                    { new Guid("00000054-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 24, 22, 38, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Изучить документацию #084" },
                    { new Guid("00000055-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 28, 11, 38, 7, 554, DateTimeKind.Unspecified).AddTicks(695), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 26, 17, 59, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Проверить документацию #085" },
                    { new Guid("00000056-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 9, 19, 33, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Изучить интеграцию #086" },
                    { new Guid("00000057-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 21, 15, 5, 33, 458, DateTimeKind.Unspecified).AddTicks(4313), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 9, 17, 41, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Подготовить план #087" },
                    { new Guid("00000058-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 19, 23, 57, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 0, "Обновить документацию #088" },
                    { new Guid("00000059-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 14, 14, 54, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Проверить задачу #089" },
                    { new Guid("0000005a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 31, 9, 8, 10, 672, DateTimeKind.Unspecified).AddTicks(5926), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 23, 0, 9, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Изучить задачу #090" },
                    { new Guid("0000005b-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 22, 2, 22, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Завершить задачу #091" },
                    { new Guid("0000005c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 31, 22, 49, 38, 842, DateTimeKind.Unspecified).AddTicks(7180), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 30, 14, 58, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Обновить план #092" },
                    { new Guid("0000005d-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 9, 17, 2, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 2, "Завершить задачу #093" },
                    { new Guid("0000005e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 30, 23, 45, 5, 185, DateTimeKind.Unspecified).AddTicks(175), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 24, 22, 20, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Завершить задачу #094" },
                    { new Guid("0000005f-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 6, 20, 51, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Обновить план #095" },
                    { new Guid("00000060-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 31, 23, 59, 9, 353, DateTimeKind.Unspecified).AddTicks(536), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 29, 21, 35, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Завершить план #096" },
                    { new Guid("00000061-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 24, 21, 10, 21, 864, DateTimeKind.Unspecified).AddTicks(3751), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 16, 19, 37, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 0, "Проверить план #097" },
                    { new Guid("00000062-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 19, 15, 21, 44, 262, DateTimeKind.Unspecified).AddTicks(8760), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 17, 18, 22, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 2, "Проверить отчёт #098" },
                    { new Guid("00000063-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 31, 4, 38, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Обновить документацию #099" },
                    { new Guid("00000064-0000-0000-0000-000000000000"), null, new DateTimeOffset(new DateTime(2025, 12, 17, 2, 42, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, 1, "Подготовить интеграцию #100" },
                    { new Guid("00000065-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2025, 12, 21, 4, 22, 26, 952, DateTimeKind.Unspecified).AddTicks(4103), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 12, 3, 16, 27, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1, "Изучить интеграцию #101" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000003-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000004-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000005-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000006-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000007-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000008-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000009-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000000b-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000000e-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000000f-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000010-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000011-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000012-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000013-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000014-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000015-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000016-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000017-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000018-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000019-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000001a-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000001b-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000001c-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000001d-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000001e-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000001f-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000020-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000021-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000022-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000023-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000024-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000025-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000026-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000027-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000028-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000029-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000002a-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000002b-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000002c-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000002d-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000002e-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000002f-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000030-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000031-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000032-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000033-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000034-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000035-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000036-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000037-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000038-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000039-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000003a-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000003b-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000003c-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000003d-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000003e-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000003f-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000040-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000041-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000042-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000043-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000044-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000045-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000046-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000047-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000048-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000049-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000004a-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000004b-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000004c-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000004d-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000004e-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000004f-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000050-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000051-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000052-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000053-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000054-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000055-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000056-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000057-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000058-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000059-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000005a-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000005b-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000005c-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000005d-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000005e-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("0000005f-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000060-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000061-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000062-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000063-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000064-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "Id",
                keyValue: new Guid("00000065-0000-0000-0000-000000000000"));
        }
    }
}
