using CoffeeShop.DAL.Repositories;
public class ProductService
{
    private readonly ProductRepository _productRepo;
    public ProductService(ProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<List<ProductResponseDto>> GetMenuAsync()
    {
        var products = await _productRepo.GetAllProductsWithCategoryAsync();
        
        return products.Select(p => new ProductResponseDto {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Image = p.Image,
            CategoryName = p.Category?.Name ?? "Chưa phân loại"
        }).ToList();
    }
}