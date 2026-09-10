// 1. Khai báo địa chỉ server Backend (Chỉ ghi 1 lần duy nhất)
const API_URL = "https://localhost:5059";

function renderMenu(productsFromDatabase) {
    let htmlContent = "";
    
    // 2. Vòng lặp duyệt qua danh sách sản phẩm lấy từ DB
    productsFromDatabase.forEach(product => {
        
        // 3. Lắp ráp: Ghép Địa chỉ Server + Đường dẫn ảnh từ DB
        let finalImageUrl = API_URL + product.image; 
        
        // 4. Đẩy vào HTML (Code chỉ viết 1 lần, chạy cho 1000 sản phẩm)
        htmlContent += `
            <div class="product-card">
                <img src="${finalImageUrl}" class="w-32 h-32" />
                <h3>${product.name}</h3>
            </div>
        `;
    });
    
    document.getElementById("product-container").innerHTML = htmlContent;
}