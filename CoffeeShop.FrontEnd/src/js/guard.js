// ==========================================
// 1. LẤY VÀ CHUẨN HÓA DỮ LIỆU TỪ VÉ VIP (userInfo)
// ==========================================
let safeRole = "";
try {
    const userInfoStr = localStorage.getItem("userInfo");
    if (userInfoStr) {
        const userInfo = JSON.parse(userInfoStr);
        // Quét cả 'role' lẫn 'Role' để phòng hờ C# đổi form
        safeRole = (userInfo.role || userInfo.Role || "").toLowerCase(); 
    }
} catch (error) {
    console.warn("Thẻ VIP bị rách (lỗi JSON), dọn dẹp...");
    localStorage.removeItem("userInfo");
}

const currentPath = window.location.pathname.toLowerCase(); 

// ==========================================
// 2. GOM CÁC TRANG PUBLIC (Giữ nguyên code của đệ)
// ==========================================
const publicPaths = ['/', '/index.html', '/login.html'];
const isAtRoot = publicPaths.includes(currentPath);

if (!safeRole) {
    // KHÔNG CÓ THẺ: Đang ở phòng khác -> Đuổi ra sảnh
    if (!isAtRoot) window.location.href = '/';
} else {
    // ĐÃ CÓ THẺ
    if (isAtRoot) {
        // Đang ở Lễ Tân -> Check thẻ để bấm thang máy
        switch (safeRole) {
            case "manager": window.location.href = '/Manager/manager.html'; break;
            // File manager của đệ nằm trong thư mục Manager hay gì thì đệ nhớ check path cho chuẩn nhé
            case "staff":   window.location.href = '/Staff/staff.html'; break;
            default:
                // Thẻ giả/Rác -> Tiêu hủy và ở lại sảnh
                localStorage.removeItem("userInfo"); // Xóa userInfo thay vì userRole
                window.location.href = '/'; 
                break;
        }
    } else {
        // Đang lảng vảng ở các tầng -> Dùng startsWith để tránh bắt nhầm
        if (currentPath.startsWith('/manager') && safeRole !== "manager") window.location.href = '/';
        else if (currentPath.startsWith('/staff') && safeRole !== "staff") window.location.href = '/';
    }
}