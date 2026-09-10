const BACKEND_URL = "http://localhost:5059"; 

async function fetchWithToken(url, options = {}) {
    // 1. Tạo headers nếu chưa có
    if (!options.headers) {
        options.headers = {};
    }
    
    // 2. KHÔNG CẦN SET 'Authorization' NỮA! 
    // Xóa cmn dòng 'Authorization': 'Bearer ' + localStorage.getItem('accessToken') đi nhé!
    
    // 3. BẮT BUỘC: Bật cờ này để trình duyệt tự lấy Két Sắt Cookie (chứa token thật) mang đi
    options.credentials = 'include';
    
    // Gắn URL gốc của Backend (Sửa lại cho đúng cổng 5059 của đệ nếu cần)
    const baseUrl = "http://localhost:5059";
    const fullUrl = url.startsWith('http') ? url : baseUrl + url;

    // 4. Gọi fetch
    return await fetch(fullUrl, options);
}