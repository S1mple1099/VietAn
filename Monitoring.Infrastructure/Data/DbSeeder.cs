
namespace Monitoring.Infrastructure.Data;

using DomainRole = Monitoring.Domain.Entities.Role;

/// <summary>
/// Database seeder for initial data (roles, permissions, default user)
/// Run this after database creation
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(MonitoringDbContext context)
    {
        // Database should already be migrated, just seed data

        // Seed Permissions - Tất cả permissions mặc định
        if (!await context.Permissions.AnyAsync())
        {
            var now = DateTime.UtcNow;
            var permissions = new[]
            {
                new Permission { Id = Guid.NewGuid(), Name = Permissions.ViewDashboard, Description = "Xem trang tổng quan", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = Permissions.ViewMonitor, Description = "Xem trang giám sát", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = Permissions.ViewHistory, Description = "Xem lịch sử sự kiện", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = Permissions.ExportData, Description = "Xuất dữ liệu ra Excel", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = Permissions.ManageUser, Description = "Quản lý người dùng và phân quyền", CreatedAt = now },
                new Permission { Id = Guid.NewGuid(), Name = Permissions.ViewLoginLog, Description = "Xem lịch sử đăng nhập", CreatedAt = now }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        // Seed Roles - Tất cả roles mặc định
        if (!await context.Roles.AnyAsync())
        {
            var permissions = await context.Permissions.ToListAsync();
            var permissionDict = permissions.ToDictionary(p => p.Name);
            var now = DateTime.UtcNow;

            var monitorRole = new DomainRole
            {
                Id = Guid.NewGuid(),
                Name = Roles.Monitor,
                Description = "Nhóm Giám Sát - Chỉ xem dữ liệu",
                CreatedAt = now
            };
            var monitorExportRole = new DomainRole
            {
                Id = Guid.NewGuid(),
                Name = Roles.MonitorExport,
                Description = "Nhóm Giám Sát + Xuất Dữ Liệu - Xem và xuất dữ liệu",
                CreatedAt = now
            };
            var adminRole = new DomainRole
            {
                Id = Guid.NewGuid(),
                Name = Roles.Admin,
                Description = "Quản Trị Viên - Toàn quyền hệ thống",
                CreatedAt = now
            };

            await context.Roles.AddRangeAsync(monitorRole, monitorExportRole, adminRole);
            await context.SaveChangesAsync();

            // Assign permissions to roles
            var rolePermissions = new List<RolePermission>();
            var assignedAt = DateTime.UtcNow;

            // MONITOR role: ViewDashboard, ViewMonitor, ViewHistory
            if (permissionDict.TryGetValue(Permissions.ViewDashboard, out var viewDashboard))
                rolePermissions.Add(new RolePermission { RoleId = monitorRole.Id, PermissionId = viewDashboard.Id, AssignedAt = assignedAt });
            if (permissionDict.TryGetValue(Permissions.ViewMonitor, out var viewMonitor))
                rolePermissions.Add(new RolePermission { RoleId = monitorRole.Id, PermissionId = viewMonitor.Id, AssignedAt = assignedAt });
            if (permissionDict.TryGetValue(Permissions.ViewHistory, out var viewHistory))
                rolePermissions.Add(new RolePermission { RoleId = monitorRole.Id, PermissionId = viewHistory.Id, AssignedAt = assignedAt });

            // MONITOR_EXPORT role: All MONITOR permissions + ExportData
            if (permissionDict.TryGetValue(Permissions.ViewDashboard, out var vd))
                rolePermissions.Add(new RolePermission { RoleId = monitorExportRole.Id, PermissionId = vd.Id, AssignedAt = assignedAt });
            if (permissionDict.TryGetValue(Permissions.ViewMonitor, out var vm))
                rolePermissions.Add(new RolePermission { RoleId = monitorExportRole.Id, PermissionId = vm.Id, AssignedAt = assignedAt });
            if (permissionDict.TryGetValue(Permissions.ViewHistory, out var vh))
                rolePermissions.Add(new RolePermission { RoleId = monitorExportRole.Id, PermissionId = vh.Id, AssignedAt = assignedAt });
            if (permissionDict.TryGetValue(Permissions.ExportData, out var exportData))
                rolePermissions.Add(new RolePermission { RoleId = monitorExportRole.Id, PermissionId = exportData.Id, AssignedAt = assignedAt });

            // ADMIN role: All permissions
            foreach (var permission in permissions)
            {
                rolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = permission.Id, AssignedAt = assignedAt });
            }

            await context.RolePermissions.AddRangeAsync(rolePermissions);
            await context.SaveChangesAsync();
        }

        // Seed default users - Tất cả users mặc định
        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Admin);
            var monitorRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Monitor);
            var monitorExportRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.MonitorExport);
            var now = DateTime.UtcNow;

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = Infrastructure.Services.AuthService.HashPassword("admin123"),
                    FullName = "Quản Trị Viên",
                    Email = "admin@monitoring.local",
                    IsActive = true,
                    CreatedAt = now
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "monitor",
                    PasswordHash = Infrastructure.Services.AuthService.HashPassword("monitor123"),
                    FullName = "Người Giám Sát",
                    Email = "monitor@monitoring.local",
                    IsActive = true,
                    CreatedAt = now
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "operator",
                    PasswordHash = Infrastructure.Services.AuthService.HashPassword("operator123"),
                    FullName = "Người Vận Hành",
                    Email = "operator@monitoring.local",
                    IsActive = true,
                    CreatedAt = now
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "technical",
                    PasswordHash = Infrastructure.Services.AuthService.HashPassword("technical123"),
                    FullName = "Kỹ Thuật Viên",
                    Email = "technical@monitoring.local",
                    IsActive = true,
                    CreatedAt = now
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "viewer",
                    PasswordHash = Infrastructure.Services.AuthService.HashPassword("viewer123"),
                    FullName = "Người Xem",
                    Email = "viewer@monitoring.local",
                    IsActive = true,
                    CreatedAt = now
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Assign roles to users
            var userRoles = new List<UserRole>();
            var assignedAt = DateTime.UtcNow;

            if (adminRole != null)
            {
                userRoles.Add(new UserRole { UserId = users[0].Id, RoleId = adminRole.Id, AssignedAt = assignedAt }); // admin
            }

            if (monitorRole != null)
            {
                userRoles.Add(new UserRole { UserId = users[1].Id, RoleId = monitorRole.Id, AssignedAt = assignedAt }); // monitor
                userRoles.Add(new UserRole { UserId = users[4].Id, RoleId = monitorRole.Id, AssignedAt = assignedAt }); // viewer
            }

            if (monitorExportRole != null)
            {
                userRoles.Add(new UserRole { UserId = users[2].Id, RoleId = monitorExportRole.Id, AssignedAt = assignedAt }); // operator
                userRoles.Add(new UserRole { UserId = users[3].Id, RoleId = monitorExportRole.Id, AssignedAt = assignedAt }); // technical
            }

            await context.UserRoles.AddRangeAsync(userRoles);
            await context.SaveChangesAsync();
        }

        // Seed sample tags (512 tags across 3 pumps)
        // Main tags matching Blazor UI: TempA, TempB, TempC, Vrs, Vst, Vtr, CurrentR, CurrentS, CurrentT, Runtime, TankOut, TankIn
        if (!await context.Tags.AnyAsync())
        {
            var tags = new List<Tag>();
            var mainTagNames = new[]
            {
                ("TempA", "NĐ Cuộn A", "°C", "Double"),
                ("TempB", "NĐ Cuộn B", "°C", "Double"),
                ("TempC", "NĐ Cuộn C", "°C", "Double"),
                ("Vrs", "Đ. Áp RS", "V", "Double"),
                ("Vst", "Đ. Áp ST", "V", "Double"),
                ("Vtr", "Đ. Áp TR", "V", "Double"),
                ("CurrentR", "Đ.Điện R", "A", "Double"),
                ("CurrentS", "Đ.Điện S", "A", "Double"),
                ("CurrentT", "Đ.Điện T", "A", "Double"),
                ("Runtime", "T.Gian H.Động", "", "String"),
                ("TankOut", "Mức Bể Xả", "%", "Int"),
                ("TankIn", "Mức Bể Hút", "%", "Int")
            };

            // Create main tags for each pump (12 tags x 3 pumps = 36 tags)
            for (int pumpId = 1; pumpId <= 3; pumpId++)
            {
                foreach (var (tagName, description, unit, dataType) in mainTagNames)
                {
                    tags.Add(new Tag
                    {
                        Name = tagName,
                        Description = $"{description} - Bom {pumpId}",
                        Unit = unit,
                        DataType = dataType,
                        PumpId = pumpId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Add more tags to reach 512 total (approximately 159 additional tags per pump)
            var additionalTagCount = 512 - tags.Count;
            for (int i = 0; i < additionalTagCount; i++)
            {
                var pumpId = (i % 3) + 1;
                tags.Add(new Tag
                {
                    Name = $"Tag{i + 1:D4}",
                    Description = $"Tag bổ sung {i + 1} - Bom {pumpId}",
                    Unit = "",
                    DataType = "Double",
                    PumpId = pumpId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await context.Tags.AddRangeAsync(tags);
            await context.SaveChangesAsync();

            // Seed TagHistory data - 1 phút/1 dòng, lưu 1 tháng (30 ngày)
            // Theo hình ảnh: dữ liệu hiển thị mỗi 9 phút (06:00:00, 06:09:00, 06:18:00, 06:27:00...)
            // Nhưng yêu cầu là 1 phút/1 dòng, nên tạo đầy đủ 1 phút/1 dòng
            var mainTags = await context.Tags
                .Where(t => mainTagNames.Select(tn => tn.Item1).Contains(t.Name))
                .ToListAsync();

            var tagHistories = new List<TagHistory>();
            var startDate = DateTime.Today.AddDays(-30); // 30 ngày trước
            var endDate = DateTime.Today.AddDays(1).AddTicks(-1); // Hết ngày hôm nay
            var random = new Random(42); // Fixed seed for reproducible data

            // Tạo dữ liệu cho mỗi pump
            foreach (var pumpId in new[] { 1, 2, 3 })
            {
                var currentDate = startDate;
                
                while (currentDate <= endDate)
                {
                    // Tính số phút từ startDate để có pattern ổn định
                    var totalMinutes = (int)(currentDate - startDate).TotalMinutes;
                    
                    // Tính toán giá trị theo pattern từ hình ảnh
                    // Từ hình: TempA tăng dần (55.0, 58.9, 62.8, 66.7, 70.6...) - mỗi 9 phút tăng ~3.9
                    // TempB: 56.0, 59.3, 62.6, 65.9, 69.2... - mỗi 9 phút tăng ~3.3
                    // TempC: 54.0, 57.6, 61.2, 64.8, 68.4... - mỗi 9 phút tăng ~3.6
                    var nineMinuteIndex = totalMinutes / 9; // Index mỗi 9 phút
                    var minuteInCycle = totalMinutes % 9; // Phút trong chu kỳ 9 phút
                    
                    // TempA: 55.0 + (nineMinuteIndex * 3.9) + (minuteInCycle * 0.433)
                    var tempA = 55.0 + (nineMinuteIndex * 3.9) + (minuteInCycle * 0.433) + random.NextDouble() * 0.1;
                    // TempB: 56.0 + (nineMinuteIndex * 3.3) + (minuteInCycle * 0.367)
                    var tempB = 56.0 + (nineMinuteIndex * 3.3) + (minuteInCycle * 0.367) + random.NextDouble() * 0.1;
                    // TempC: 54.0 + (nineMinuteIndex * 3.6) + (minuteInCycle * 0.4)
                    var tempC = 54.0 + (nineMinuteIndex * 3.6) + (minuteInCycle * 0.4) + random.NextDouble() * 0.1;
                    
                    // Voltage: 380, 383, 382, 385, 384... (dao động nhẹ)
                    var vrs = 380 + (nineMinuteIndex % 3) + (minuteInCycle % 2) + random.Next(-1, 2);
                    var vst = 379 + (nineMinuteIndex % 3) + (minuteInCycle % 2) + random.Next(-1, 2);
                    var vtr = 381; // Cố định như trong hình
                    
                    // Current: tăng dần (12.0, 13.8, 15.6, 17.4, 19.2...) - mỗi 9 phút tăng ~1.8
                    var curR = 12.0 + (nineMinuteIndex * 1.8) + (minuteInCycle * 0.2) + random.NextDouble() * 0.05;
                    var curS = 11.5 + (nineMinuteIndex * 1.7) + (minuteInCycle * 0.189) + random.NextDouble() * 0.05;
                    var curT = 12.2 + (nineMinuteIndex * 1.7) + (minuteInCycle * 0.189) + random.NextDouble() * 0.05;
                    
                    // Runtime: tăng theo thời gian (00:00, 00:21, 00:42, 01:03, 01:24...) - mỗi 9 phút tăng 21 phút runtime
                    var runtimeMinutes = (nineMinuteIndex * 21) + (minuteInCycle * 2.33);
                    runtimeMinutes = runtimeMinutes % 500; // Giới hạn trong 500 phút
                    
                    // Tank levels: dao động (35, 38, 41, 47, 50, 53...)
                    var tankOut = 35 + (nineMinuteIndex % 6) * 3 + (minuteInCycle % 3) + random.Next(-1, 2);
                    var tankIn = 47 + (nineMinuteIndex % 5) * 3 + (minuteInCycle % 3) + random.Next(-1, 2);

                    // Tìm tags cho pump này
                    var pumpTags = mainTags.Where(t => t.PumpId == pumpId).ToList();

                    foreach (var tag in pumpTags)
                    {
                        double? valueDouble = null;
                        int? valueInt = null;
                        string? valueString = null;

                        switch (tag.Name)
                        {
                            case "TempA":
                                valueDouble = Math.Round(tempA, 1);
                                break;
                            case "TempB":
                                valueDouble = Math.Round(tempB, 1);
                                break;
                            case "TempC":
                                valueDouble = Math.Round(tempC, 1);
                                break;
                            case "Vrs":
                                valueDouble = vrs;
                                break;
                            case "Vst":
                                valueDouble = vst;
                                break;
                            case "Vtr":
                                valueDouble = vtr;
                                break;
                            case "CurrentR":
                                valueDouble = Math.Round(curR, 1);
                                break;
                            case "CurrentS":
                                valueDouble = Math.Round(curS, 1);
                                break;
                            case "CurrentT":
                                valueDouble = Math.Round(curT, 1);
                                break;
                            case "Runtime":
                                valueString = $"{runtimeMinutes / 60:00}:{runtimeMinutes % 60:00}";
                                break;
                            case "TankOut":
                                valueInt = Math.Max(0, Math.Min(100, tankOut)); // Clamp 0-100
                                break;
                            case "TankIn":
                                valueInt = Math.Max(0, Math.Min(100, tankIn)); // Clamp 0-100
                                break;
                        }

                        if (valueDouble.HasValue || valueInt.HasValue || !string.IsNullOrEmpty(valueString))
                        {
                            tagHistories.Add(new TagHistory
                            {
                                TagId = tag.Id,
                                PumpId = pumpId,
                                Timestamp = currentDate,
                                ValueDouble = valueDouble,
                                ValueInt = valueInt,
                                ValueString = valueString,
                                Quality = "Good"
                            });
                        }
                    }

                    currentDate = currentDate.AddMinutes(1); // Tăng 1 phút
                }
            }

            // Batch insert TagHistory (SQLite has limits, so we'll do in chunks)
            const int batchSize = 1000;
            for (int i = 0; i < tagHistories.Count; i += batchSize)
            {
                var batch = tagHistories.Skip(i).Take(batchSize).ToList();
                await context.TagHistories.AddRangeAsync(batch);
                await context.SaveChangesAsync();
            }
        }

        // Seed EventLogs - tạo dữ liệu fake theo hình ảnh
        // Pattern: login, error, ok, warn, login, error, ok, warn... (lặp lại)
        // Thời gian: 06/01/2026 14:23:20, 14:26:20, 14:29:20... (mỗi 3 phút)
        // Thiết Bị: "" (login), "Bom 1", "Bom 2", "Bom 3", "Bom 5", "Bom 6"...
        if (!await context.EventLogs.AnyAsync())
        {
            var eventLogs = new List<EventLog>();
            var baseTime = new DateTime(2026, 1, 6, 14, 23, 20); // Bắt đầu từ 14:23:20 như trong hình
            var eventTypes = new[] { "login", "error", "ok", "warn" }; // EventType trong DB
            var devices = new[] { "Bom 1", "Bom 2", "Bom 3", "Bom 5", "Bom 6", "PLC" };
            var random = new Random(42);

            // Tạo 100 records - pattern chính xác như hình: login, error, ok, warn, lặp lại
            for (int i = 1; i <= 100; i++)
            {
                var eventType = eventTypes[(i - 1) % eventTypes.Length]; // Pattern: login, error, ok, warn
                var time = baseTime.AddMinutes((i - 1) * 3); // Mỗi 3 phút một record
                
                // Pattern thiết bị từ hình: login -> "", error -> "Bom 1", ok -> "Bom 2", warn -> "Bom 3"...
                string device;
                if (eventType == "login")
                {
                    device = ""; // Login luôn rỗng
                }
                else
                {
                    // Pattern: error -> Bom 1, ok -> Bom 2, warn -> Bom 3, error -> Bom 5, ok -> Bom 6...
                    var cycleIndex = (i - 1) / 4; // Mỗi 4 records (1 cycle) thì đổi pattern thiết bị
                    var positionInCycle = (i - 1) % 4; // Vị trí trong cycle: 0=login, 1=error, 2=ok, 3=warn
                    
                    if (positionInCycle == 1) // error
                        device = devices[cycleIndex % devices.Length];
                    else if (positionInCycle == 2) // ok
                        device = devices[(cycleIndex + 1) % devices.Length];
                    else if (positionInCycle == 3) // warn
                        device = devices[(cycleIndex + 2) % devices.Length];
                    else
                        device = devices[cycleIndex % devices.Length];
                }
                
                var account = eventType == "login" ? "Technical" : "Admin";

                string? errorCode = null;
                int? processingTime = null;

                var description = eventType switch
                {
                    "login" => $"{account} đăng nhập",
                    "error" => device == "PLC" ? "PLC mất kết nối" : "Quá momen",
                    "warn" => "Cảnh báo dòng quá tải",
                    "ok" => string.IsNullOrWhiteSpace(device) ? "Hệ thống hoạt động ổn định" : $"{device} hoạt động ổn định",
                    _ => "Sự kiện hệ thống"
                };

                // Thêm mã lỗi và thời gian xử lý
                if (eventType == "error")
                {
                    errorCode = device == "PLC" ? "PLC_001" : "PUMP_OVERMOMENT";
                    processingTime = random.Next(5, 30); // 5-30 giây
                }
                else if (eventType == "warn")
                {
                    errorCode = "WARN_OVERLOAD";
                    processingTime = random.Next(2, 10); // 2-10 giây
                }
                else if (eventType == "ok")
                {
                    processingTime = 0; // Xử lý ngay
                }
                else if (eventType == "login")
                {
                    processingTime = 0; // Login xử lý ngay
                }

                eventLogs.Add(new EventLog
                {
                    EventType = eventType, // Lưu trong DB: login, error, warn, ok
                    Device = device,
                    Account = account,
                    Description = description,
                    ErrorCode = errorCode,
                    ProcessingTimeSeconds = processingTime,
                    Timestamp = time
                });
            }

            await context.EventLogs.AddRangeAsync(eventLogs);
            await context.SaveChangesAsync();
        }

        // Seed LoginLogs - tạo dữ liệu fake theo pattern từ hình ảnh
        // Login logs được include trong History, pattern: Technical đăng nhập
        if (!await context.LoginLogs.AnyAsync())
        {
            var users = await context.Users.ToListAsync();
            var technicalUser = users.FirstOrDefault(u => u.Username == "monitor") ?? users.First();
            var loginLogs = new List<LoginLog>();
            var baseTime = new DateTime(2026, 1, 6, 14, 23, 20); // Cùng thời gian với EventLogs
            var random = new Random(42);

            // Tạo login logs xen kẽ với event logs (mỗi 4 records có 1 login)
            // Pattern: login xuất hiện ở vị trí 1, 5, 9, 13... (mỗi 4 records)
            for (int i = 0; i < 30; i++)
            {
                var timestamp = baseTime.AddMinutes(i * 12); // Mỗi 12 phút một login (tương ứng với pattern 4 events)
                var isSuccess = true; // Tất cả thành công trong hình

                loginLogs.Add(new LoginLog
                {
                    UserId = technicalUser.Id,
                    Username = technicalUser.Username,
                    IpAddress = $"192.168.1.{100 + (i % 10)}",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    IsSuccess = isSuccess,
                    FailureReason = null,
                    Timestamp = timestamp
                });
            }

            await context.LoginLogs.AddRangeAsync(loginLogs);
            await context.SaveChangesAsync();
        }

    }
}
