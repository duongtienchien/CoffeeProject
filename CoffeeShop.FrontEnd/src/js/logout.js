// file: (hoặc logout.js)
// Hàm kiểm tra xem user còn quyền truy cập không
async function checkAuthStatus() {
    try {
        const response = await fetch('http://localhost:5059/api/auth/me', {
            method: 'GET',
            credentials: 'include' 
        });

        if (response.ok) {
            // VÉ CHUẨN: Kéo rèm lên cho sếp vào làm việc!
            document.body.style.opacity = '1';
        } else {
            //Server bảo vé hết hạn -> xé thẻ ngay lập tức!
            localStorage.removeItem("userInfo");
            // SAI VÉ: Để nguyên màn hình trắng và sút ra ngoài
            window.location.replace('/'); 
        }
    } catch (error) {
        // Lỗi kết nối hoặc server sập -> Cũng xé thẻ, đá ra ngoài cho an toàn
        localStorage.removeItem("userInfo");
        window.location.replace('/');
    }
}

// Bắt sự kiện 'pageshow' - Sự kiện này xịn hơn 'load' ở chỗ:
// Kể cả khi trình duyệt moi trang từ BFCache (nút Back/Forward) ra, nó vẫn sẽ chạy!
window.addEventListener('pageshow', function(event) {
    checkAuthStatus();
});
// 1. Móc DOM cái nút đăng xuất
const btnLogout = document.getElementById('btnLogout'); // Đại ca đổi tên ID cho chuẩn là button nhé

// 2. Viết hàm gọi API Đăng xuất
async function handleLogout() {
    try {
        btnLogout.textContent = "Đang đăng xuất...";
        btnLogout.classList.add('opacity-50', 'cursor-not-allowed');

        const response = await fetch('http://localhost:5059/api/auth/logout', {
            method: 'POST',
            credentials: 'include' // Cực kỳ quan trọng để gửi Cookie lên cho server xóa
        });

        // Backend trả về thành công thì đá về trang chủ
        if (response.ok) {
            localStorage.removeItem("userInfo");
            window.location.replace('/');
        } else {
            console.error("Lỗi từ server khi đăng xuất");
            localStorage.removeItem("userInfo");
            window.location.replace('/'); // Lỗi thì cũng đá ra ngoài luôn cho an toàn
        }
    } catch (error) {
        console.error("Lỗi khi đăng xuất:", error);
        localStorage.removeItem("userInfo");
        window.location.replace('/');
    }
}

// 3. Gắn sự kiện click vào nút
if (btnLogout) {
    btnLogout.addEventListener('click', handleLogout);
}
// Bắt sự kiện khi người dùng chuẩn bị rời trang (Bấm Back, Forward, hoặc Logout)
window.addEventListener('pagehide', function() {
    // Tắt đèn ngay lập tức trước khi trình duyệt kịp chụp ảnh
    document.body.style.opacity = '0'; 
});