using CoffeeShop.DAL.Repositories;
using CoffeeShop.BLL.DTOs.Inventory.Requests;
using CoffeeShop.BLL.DTOs.Inventory.Responses;
using CoffeeShop.Models.Entities.Inventory;
public class InventoryService
{
    private readonly InventoryRepository _inventoryRepo;
    public InventoryService(InventoryRepository inventoryRepo)
    {
        _inventoryRepo = inventoryRepo;
    }
    public async Task<StaffResponse> ImportInventoryAsync(int staffId, ImportInventoryRequest request)
    {
        // Gọi hàm lấy User từ Repo 
        var staff = await _inventoryRepo.GetUserByIdAsync(staffId);
        if (staff == null)
            throw new Exception("Lỗi: Không tìm thấy thông tin nhân viên!");

        int currentStoreId = staff.StoreId.Value;

        // 2. Chọc vào kho với cái StoreId 
        var inventory = await _inventoryRepo.GetInventoryAsync(currentStoreId, request.ItemId);
        if (inventory == null)
            throw new Exception("Lỗi: Không tìm thấy kho cho sản phẩm này tại chi nhánh của bạn!");

        var inventoryItem = await _inventoryRepo.GetInventoryItemByIdAsync(request.ItemId);
        if (inventoryItem == null)
            throw new Exception("Lỗi: Nguyên vật liệu không tồn tại trong danh mục!");

        // Tăng số lượng trong kho
        inventory.Quantity += request.QuantityToAdd;

        //Bắt buộc phải có để truy vết chống thất thoát!
        var transaction = new InventoryTransaction
        {
            StoreId = currentStoreId,
            ItemId = request.ItemId,
            QuantityChange = request.QuantityToAdd, // Ghi số dương (Nhập kho)
            TransactionType = "Receipt", // Loại giao dịch: Nhập hàng
            Reason = "Nhân viên nhập hàng định kỳ",
            CreateBy = staffId, // Lưu vết ai là người nhập
            CreateDate = DateTime.UtcNow
        };

        await _inventoryRepo.AddTransactionAsync(transaction);

        bool isSaved = await _inventoryRepo.SaveChangesAsync();
        if (!isSaved) throw new Exception("Lỗi: Không thể lưu thay đổi!");

        return new StaffResponse
        {
            ItemId = inventoryItem.Id,
            ItemName = inventoryItem.Name,
            Quantity = request.QuantityToAdd,
            NewStockQuantity = inventory.Quantity
        };
    }
    public async Task<List<InventoryTransactionDto>> GetHistoryAsync()
    {
        // 1. Gọi DAL lấy data vật lý
        var transactions = await _inventoryRepo.GetTransactionHistoryAsync();

        // 2. Chế biến (Map) Entity thành DTO
        var result = transactions.Select(t => new InventoryTransactionDto
        {
            Id = t.Id,
            ItemId = t.ItemId,
            QuantityChange = t.QuantityChange,
            TransactionType = t.TransactionType,
            Reason = t.Reason,
            CreateBy = t.CreateBy,
            CreateDate = t.CreateDate
        }).ToList();

        return result;
    }
    public async Task<List<StoreInventoryDto>> GetInventoryByStoreIdAsync(int storeId)
    {
        // 1. Gọi Repo lấy data vật lý
        var storeInventories = await _inventoryRepo.GetStoreInventoryAsync(storeId);

        // 2. Map dữ liệu sang DTO để quăng lên Frontend
        var result = storeInventories.Select(si => new StoreInventoryDto
        {
            ItemId = si.ItemId,
            ItemName = si.InventoryItem?.Name ?? "Unknown", // Tránh lỗi Null
            Unit = si.InventoryItem?.Unit ?? "N/A",
            CurrentQuantity = si.Quantity
        }).ToList();

        return result;
    }
}