using CoffeeShop.Models.Entities.Catalog;
using CoffeeShop.Models.Entities.Sales;
using CoffeeShop.Models.Entities.Inventory;
using CoffeeShop.Models.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace CoffeeShop.DAL.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context, IConfiguration config)
        {
            context.Database.Migrate();
            Console.WriteLine("====== BẮT ĐẦU KHỞI ĐỘNG SEEDER ======");
            Console.WriteLine("====== (KHÔNG TRUNCATE - CHỈ BƠM THÊM DỮ LIỆU THIẾU) ======");

            // ==========================================
            // 1. SEED CATEGORY
            // ==========================================
            Console.WriteLine("====== 1. BƠM CATEGORY ======");
            var categoriesToSeed = new List<Category>
            {
                new Category { Name = "Cà phê truyền thống", Description = "Đậm đà hương vị Việt", DisplayOrder = 1, IsActive = true },
                new Category { Name = "Cà phê máy", Description = "Phong cách Espresso Ý", DisplayOrder = 2, IsActive = true },
                new Category { Name = "Cà phê hiện đại", Description = "Sáng tạo & Độc đáo", DisplayOrder = 3, IsActive = true },
                new Category { Name = "Topping", Description = "Thạch, trân châu, kem cheese các loại", DisplayOrder = 4, IsActive = true }
            };

            foreach (var cat in categoriesToSeed)
            {
                if (!context.Categories.Any(c => c.Name == cat.Name))
                {
                    context.Categories.Add(cat);
                }
            }
            context.SaveChanges(); // Lưu để lấy ID thật

            // ==========================================
            // 2. SEED PRODUCT
            // ==========================================
            Console.WriteLine("====== 2. BƠM PRODUCT ======");
            var catTraditional = context.Categories.FirstOrDefault(c => c.Name == "Cà phê truyền thống");
            var catMachine = context.Categories.FirstOrDefault(c => c.Name == "Cà phê máy");
            var catModern = context.Categories.FirstOrDefault(c => c.Name == "Cà phê hiện đại");
            var catTopping = context.Categories.FirstOrDefault(c => c.Name == "Topping");

            if (catTraditional != null && catMachine != null && catModern != null && catTopping != null)
            {
                var targetProducts = new List<Product>
                {
                    new Product { Name = "Cà phê đen (Đá/Nóng)", Price = 25000, Image = "/images/products/capheden.png", CategoryId = catTraditional.Id },
                    new Product { Name = "Cà phê sữa (Đá/Nóng)", Price = 30000, Image = "/images/products/caphesua.jpg", CategoryId = catTraditional.Id },
                    new Product { Name = "Bạc xỉu", Price = 35000, Image = "/images/products/bacxiu.jpeg", CategoryId = catTraditional.Id },
                    new Product { Name = "Cà phê trứng", Price = 45000, Image = "/images/products/caphetrung.png", CategoryId = catTraditional.Id },
                    new Product { Name = "Espresso", Price = 35000, Image = "/images/products/espresso.jpg", CategoryId = catMachine.Id },
                    new Product { Name = "Americano", Price = 40000, Image = "/images/products/americano.jpg", CategoryId = catMachine.Id },
                    new Product { Name = "Cappuccino", Price = 50000, Image = "/images/products/cappucchino.jpg", CategoryId = catMachine.Id },
                    new Product { Name = "Latte", Price = 50000, Image = "/images/products/latte.jpg", CategoryId = catMachine.Id },
                    new Product { Name = "Mocha", Price = 55000, Image = "/images/products/mocha.png", CategoryId = catMachine.Id },
                    new Product { Name = "Cà phê muối", Price = 45000, Image = "/images/products/caphemuoi.jpeg", CategoryId = catModern.Id },
                    new Product { Name = "Cà phê dừa", Price = 50000, Image = "/images/products/caphedua.jpeg", CategoryId = catModern.Id },
                    new Product { Name = "Cold Brew", Price = 55000, Image = "/images/products/coldbrew.jpeg", CategoryId = catModern.Id },
                    new Product { Name = "Trân châu đen", Price = 5000, Image = "/images/products/tranchauden.jpeg", CategoryId = catTopping.Id },
                    new Product { Name = "Thạch nha đam", Price = 7000, Image = "/images/products/thachnhadam.png", CategoryId = catTopping.Id },
                    new Product { Name = "Kem Macchiato", Price = 15000, Image = "/images/products/kemmachito.jpg", CategoryId = catTopping.Id }
                };

                foreach (var p in targetProducts)
                {
                    if (!context.Products.Any(db_prod => db_prod.Name == p.Name))
                    {
                        context.Products.Add(p);
                    }
                }
                context.SaveChanges();
            }

            // ==========================================
            // 3. SEED CUSTOMER & STORE
            // ==========================================
            Console.WriteLine("====== 3. BƠM CUSTOMER & STORE ======");
            var targetCustomers = new List<Customer> {
                new Customer { FullName = "SieuThatNghiep", PhoneNumber = "0123456789" },
                new Customer { FullName = "SieuTotNghiep", PhoneNumber = "0987654321" } };
            foreach (var c in targetCustomers)
            {
                if (!context.Customers.Any(db_c => db_c.PhoneNumber == c.PhoneNumber)) context.Customers.Add(c);
            }

            var targetStores = new List<Store>
            {
                new Store { StoreName = "Cơ sở Cầu Giấy", Address = "123 Xuân Thủy, Cầu Giấy, Hà Nội", Hotline = "0987123456" },
                new Store { StoreName = "Cơ sở Đống Đa", Address = "456 Thái Hà, Đống Đa, Hà Nội", Hotline = "0912987654" }
            };
            foreach (var s in targetStores)
            {
                if (!context.Stores.Any(db_s => db_s.StoreName == s.StoreName)) context.Stores.Add(s);
            }
            context.SaveChanges();

            // ==========================================
            // 4. SEED INVENTORY ITEMS
            // ==========================================
            Console.WriteLine("====== 4. BƠM NGUYÊN LIỆU KHO ======");
            var inventoryItems = new List<InventoryItem>
            {
                new InventoryItem { Name = "Sữa đặc", Unit = "ml" }, new InventoryItem { Name = "Cà phê hạt xay", Unit = "g" },
                new InventoryItem { Name = "Đường nước", Unit = "ml" }, new InventoryItem { Name = "Trân châu trắng", Unit = "g" },
                new InventoryItem { Name = "Sữa tươi", Unit = "ml" }, new InventoryItem { Name = "Trân châu đen", Unit = "g" },
                new InventoryItem { Name = "Thạch nha đam", Unit = "g" }, new InventoryItem { Name = "Kem Macchiato", Unit = "ml" }
            };

            foreach (var item in inventoryItems)
            {
                if (!context.InventoryItems.Any(db_i => db_i.Name == item.Name)) context.InventoryItems.Add(item);
            }
            context.SaveChanges();

            // ==========================================
            // 5. SEED PRODUCT RECIPES (CÔNG THỨC)
            // ==========================================
            Console.WriteLine("====== 5. BƠM CÔNG THỨC SẢN PHẨM ======");
            var allProducts = context.Products.ToDictionary(p => p.Name, p => p.Id);
            var allItems = context.InventoryItems.ToDictionary(i => i.Name, i => i.Id);
            var targetRecipes = new List<ProductRecipe>();

            // Hàm cục bộ (Local function) để add recipe an toàn
            void AddRecipeSafe(string prodName, string itemName, decimal qty)
            {
                if (allProducts.TryGetValue(prodName, out int pId) && allItems.TryGetValue(itemName, out int iId))
                {
                    targetRecipes.Add(new ProductRecipe { ProductId = pId, ItemId = iId, QuantityNeeded = qty });
                }
            }

            // Truyền thống
            AddRecipeSafe("Cà phê đen (Đá/Nóng)", "Cà phê hạt xay", 20m);
            AddRecipeSafe("Cà phê đen (Đá/Nóng)", "Đường nước", 10m);
            AddRecipeSafe("Cà phê sữa (Đá/Nóng)", "Cà phê hạt xay", 20m);
            AddRecipeSafe("Cà phê sữa (Đá/Nóng)", "Sữa đặc", 25m);
            // Ý
            AddRecipeSafe("Espresso", "Cà phê hạt xay", 18m);
            AddRecipeSafe("Latte", "Cà phê hạt xay", 18m);
            AddRecipeSafe("Latte", "Sữa tươi", 150m);
            // (Đệ có thể tự bổ sung thêm các dòng AddRecipeSafe cho các món khác ở đây)

            foreach (var r in targetRecipes)
            {
                // Check tránh duplicate công thức
                if (!context.ProductRecipes.Any(db_r => db_r.ProductId == r.ProductId && db_r.ItemId == r.ItemId))
                {
                    context.ProductRecipes.Add(r);
                }
            }
            context.SaveChanges();

            // ==========================================
            // 6. SEED STORE INVENTORIES (TỒN KHO)
            // ==========================================
            Console.WriteLine("====== 6. BƠM TỒN KHO CỬA HÀNG ======");
            var storeCG = context.Stores.FirstOrDefault(s => s.StoreName == "Cơ sở Cầu Giấy");
            var storeDD = context.Stores.FirstOrDefault(s => s.StoreName == "Cơ sở Đống Đa");

            if (storeCG != null && storeDD != null)
            {
                var targetStoreInventories = new List<StoreInventory>();

                // Hàm cục bộ phân bổ tồn kho
                void AllocateInventory(int storeId, string itemName, decimal qty)
                {
                    if (allItems.TryGetValue(itemName, out int iId))
                    {
                        targetStoreInventories.Add(new StoreInventory { StoreId = storeId, ItemId = iId, Quantity = qty });
                    }
                }

                // Kho Cầu Giấy
                AllocateInventory(storeCG.Id, "Sữa đặc", 10000);
                AllocateInventory(storeCG.Id, "Cà phê hạt xay", 5000);
                // Kho Đống Đa
                AllocateInventory(storeDD.Id, "Sữa đặc", 10000);
                AllocateInventory(storeDD.Id, "Cà phê hạt xay", 5000);

                foreach (var si in targetStoreInventories)
                {
                    if (!context.StoreInventories.Any(db_si => db_si.StoreId == si.StoreId && db_si.ItemId == si.ItemId))
                    {
                        context.StoreInventories.Add(si);
                    }
                }
                context.SaveChanges();
            }

            // ==========================================
            // 7. SEED USERS & PROFILES
            // ==========================================
            Console.WriteLine("====== 7. BƠM TÀI KHOẢN NHÂN SỰ ======");

            // Xây dựng danh sách User kèm Profile tương ứng
            var usersToSeed = new List<(User user, UserProfile profile)>
            {
                (
                    new User { Email = "manager.cg@coffeeshop.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"), Role = "Manager", StoreId = storeCG?.Id, OtpCode = "" },
                    new UserProfile { FullName = "Quản lý Cầu Giấy", Phone = "0123456789", Avatar = "manager.png" }
                ),
                (
                    new User { Email = "staff1.cg@coffeeshop.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"), Role = "Staff", StoreId = storeCG?.Id, OtpCode = "" },
                    new UserProfile { FullName = "Nhân viên CG 1", Phone = "0911000222", Avatar = "staff.png" }
                ),
                (
                    new User { Email = "staff2.cg@coffeeshop.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"), Role = "Staff", StoreId = storeCG?.Id, OtpCode = "" },
                    new UserProfile { FullName = "Nhân viên CG 2", Phone = "0911000333", Avatar = "staff.png" }
                ),
                (
                    new User { Email = "manager.dd@coffeeshop.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"), Role = "Manager", StoreId = storeDD?.Id, OtpCode = "" },
                    new UserProfile { FullName = "Quản lý Đống Đa", Phone = "0922000111", Avatar = "manager.png" }
                ),
                (
                    new User { Email = "staff1.dd@coffeeshop.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"), Role = "Staff", StoreId = storeDD?.Id, OtpCode = "" },
                    new UserProfile { FullName = "Nhân viên ĐĐ 1", Phone = "0922000222", Avatar = "staff.png" }
                ),
                (
                    new User { Email = "staff2.dd@coffeeshop.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"), Role = "Staff", StoreId = storeDD?.Id, OtpCode = "" },
                    new UserProfile { FullName = "Nhân viên ĐĐ 2", Phone = "0922000333", Avatar = "staff.png" }
                )
            };

            foreach (var mapping in usersToSeed)
            {
                var u = mapping.user;
                var p = mapping.profile;

                // 1. Kiểm tra Email xem User đã tồn tại chưa
                var existingUser = context.Users.FirstOrDefault(db_u => db_u.Email == u.Email);
                if (existingUser == null)
                {
                    context.Users.Add(u);
                    context.SaveChanges(); // Lưu để phát sinh Id mới

                    // 2. Gán User Id cho Profile và Lưu
                    p.UserId = u.Id;
                    context.UserProfiles.Add(p);
                    context.SaveChanges();
                }
            }

            Console.WriteLine("====== HOÀN TẤT TOÀN BỘ QUÁ TRÌNH SEEDER! ======");
        }
    }
}