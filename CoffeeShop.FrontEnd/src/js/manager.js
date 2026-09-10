// 1. BIẾN TOÀN CỤC
let allOrdersData = [];

// 2. KHỞI CHẠY KHI TẢI TRANG (Điều phối thông minh)
document.addEventListener('DOMContentLoaded', () => {
    loadMyProfile();
    loadSystemWarnings();
    // Xử lý sự kiện đổi chi nhánh chung (Trang nào có thì mới chạy)
    const branchSelector = document.getElementById('branchSelector');
    if (branchSelector) {
        branchSelector.addEventListener('change', () => {
            if (document.getElementById('orderTableBody')) loadOrders();
            if (document.getElementById('inventory-body')) loadBranchInventory();
        });
    }

    // Tự động nhận diện trang Order
    if (document.getElementById('orderTableBody')) {
        loadOrders();
        const searchInput = document.getElementById('searchInput');
        if (searchInput) searchInput.addEventListener('input', applyFilters);
        
        const statusFilter = document.getElementById('statusFilter');
        if (statusFilter) statusFilter.addEventListener('change', applyFilters);
    }

    // Tự động nhận diện trang Kho (Inventory)
    if (document.getElementById('inventory-body')) {
        loadBranchInventory();
    }

    // Tự động nhận diện trang Nhân viên (MoStaff)
    if (document.getElementById('staffTableBody')) {
        loadStaffs();
    }
    if (document.getElementById('transaction-body')) {
        loadTransactions();
    }
});

// ==========================================
// CÁC HÀM XỬ LÝ KHO (INVENTORY)
// ==========================================
async function loadBranchInventory() {
    const tbody = document.getElementById('inventory-body');
    if (!tbody) return;

    const storeId = getCurrentStoreId();
    
    try {
        tbody.innerHTML = `<tr><td colspan="7" class="p-8 text-center text-gray-400 font-medium"><i class="fa-solid fa-spinner fa-spin mr-2"></i> Đang tải dữ liệu kho...</td></tr>`;

        const response = await fetchWithToken(`/api/Manager/store-inventory/${storeId}`, {
            method: 'GET'
        });

        if (!response.ok) throw new Error('Không thể tải dữ liệu kho');
        
        const result = await response.json();
        const items = result.data;
        
        if (!items || items.length === 0) {
            tbody.innerHTML = `<tr><td colspan="7" class="p-8 text-center text-gray-400 font-medium">Kho chi nhánh này chưa có nguyên liệu nào.</td></tr>`;
            return;
        }

        let html = '';
        items.forEach(item => {
            let statusBadge = '';
            if (item.currentQuantity > 20) {
                statusBadge = `<span class="px-3 py-1 rounded-full text-xs font-bold bg-green-100 text-green-700">In Stock</span>`;
            } else if (item.currentQuantity > 0) {
                statusBadge = `<span class="px-3 py-1 rounded-full text-xs font-bold bg-yellow-100 text-yellow-700">Low Stock</span>`;
            } else {
                statusBadge = `<span class="px-3 py-1 rounded-full text-xs font-bold bg-red-100 text-red-700">Critical</span>`;
            }

            html += `
                <tr class="hover:bg-gray-50 transition-colors">
                    <td class="p-4 py-5">
                        <div class="font-bold text-gray-800 text-base">${item.itemName}</div>
                        <div class="text-xs text-gray-500">Mã món: ${item.itemId}</div>
                    </td>
                    <td class="p-4 text-gray-500">Bean Co.</td>
                    <td class="p-4 font-bold text-gray-900 text-lg">${item.currentQuantity} ${item.unit}</td>
                    <td class="p-4">${statusBadge}</td>
                    <td class="p-4 flex gap-2 justify-center">
                        <button onclick="handleAdjustInventory(${item.itemId}, '${item.itemName}', 'minus')" class="w-8 h-8 flex items-center justify-center rounded-lg bg-red-50 text-red-600 font-bold hover:bg-red-100 transition">-</button>
                        <button onclick="handleAdjustInventory(${item.itemId}, '${item.itemName}', 'plus')" class="w-8 h-8 flex items-center justify-center rounded-lg bg-green-50 text-green-600 font-bold hover:bg-green-100 transition">+</button>
                    </td>
                </tr>
            `;
        });
        tbody.innerHTML = html;

    } catch (error) {
        console.error("Lỗi tải kho:", error);
        tbody.innerHTML = `<tr><td colspan="7" class="p-8 text-center text-red-500 font-bold">Lỗi không thể tải dữ liệu kho!</td></tr>`;
    }
}

async function loadStaffs() {
    const tbody = document.getElementById('staffTableBody');
    if (!tbody) return;
    const storeId = getCurrentStoreId();
    try {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center p-8 text-gray-400 font-medium"><i class="fa-solid fa-spinner fa-spin mr-2"></i> Đang tải dữ liệu nhân sự...</td></tr>`;

        const response = await fetchWithToken(`/api/Manager/staffs/${storeId}`, { 
            method: 'GET' 
        });

        if (!response.ok) throw new Error('Không thể lấy danh sách nhân viên');

        const result = await response.json();
        const staffs = result.data || [];

        if (staffs.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" class="text-center p-8 text-gray-500">Chưa có nhân viên nào trong danh sách.</td></tr>`;
            return;
        }

        let html = '';
        // Inside your loop iterating through the staff/shift data
staffs.forEach(staff => {
    const avatarFileName = staff.avatar || 'staff.png';
    
    // Giả sử API trả về object shiftReport lồng bên trong staff
    // Nếu chưa có báo cáo chốt ca, set giá trị mặc định là 0
    // XÓA DÒNG shiftData CŨ ĐI, THAY BẰNG ĐOẠN NÀY:
        // Lấy trực tiếp từ staff (chú ý chữ cái đầu viết thường do JSON serialize)
        const systemCashAmount = staff.systemCashAmount || 0;
        const actualCashAmount = staff.actualCashAmount || 0;
        const difference = staff.difference || 0;

        // Format tiền tệ cho đẹp
        const formatMoney = (amount) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);

        const systemCash = formatMoney(systemCashAmount);
        const actualCash = formatMoney(actualCashAmount);
        
        // Logic tô màu cho độ lệch
        let diffClass = "text-gray-500"; 
        let diffText = formatMoney(difference);

        if (difference === 0) {
            diffClass = "text-emerald-600 font-bold"; // Liêm khiết (Xanh lá)
        } else if (difference < 0) {
            diffClass = "text-red-600 font-bold"; // Biển thủ (Đỏ)
        } else if (difference > 0) {
            diffClass = "text-amber-500 font-bold"; // Dư tiền (Vàng)
            diffText = "+" + diffText; // Thêm dấu + cho trực quan
        }

    html += `
        <tr class="hover:bg-gray-50 transition-colors">
            <!-- 1. Mã NV -->
            <td class="p-4 font-bold text-gray-700">NV_${staff.userId}</td>
            
            <!-- 2. Avatar & FullName -->
            <td class="p-4">
                <div class="flex items-center gap-3">
                    <img src="/assets/images/avatars/${avatarFileName}" 
                         onerror="this.src='https://ui-avatars.com/api/?name=${encodeURIComponent(staff.fullName)}&background=random'" 
                         alt="Avatar" 
                         class="w-10 h-10 rounded-full object-cover border border-gray-200 shadow-sm">
                    <div>
                        <p class="font-semibold text-gray-900">${staff.fullName}</p>
                        <p class="text-xs text-gray-500"><i class="fa-solid fa-phone text-[10px] mr-1"></i>${staff.phone}</p>
                    </div>
                </div>
            </td>

            <!-- 3. Username -->
            <td class="p-4 text-gray-600 font-medium">@${staff.username || 'staff_' + staff.userId}</td>

            <!-- 4. Doanh thu hệ thống -->
            <td class="p-4 text-right font-medium text-gray-700">${systemCash}</td>

            <!-- 5. Thực nộp -->
            <td class="p-4 text-right font-medium text-blue-600">${actualCash}</td>

            <!-- 6. Chênh lệch (Tô màu động) -->
            <td class="p-4 text-right ${diffClass}">${diffText}</td>

            <!-- 7. Trạng thái -->
            <td class="p-4 text-center">
                <span class="inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-700">
                    <span class="w-1.5 h-1.5 rounded-full bg-green-500"></span> Đang làm việc
                </span>
            </td>

            <!-- 8. Hành động -->
            <td class="p-4 text-right">
                <button class="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors mr-1" title="Chỉnh sửa">
                    <i class="fa-solid fa-pen-to-square"></i>
                </button>
                <button class="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors" title="Xóa">
                    <i class="fa-solid fa-trash"></i>
                </button>
            </td>
        </tr>
    `;
});

        tbody.innerHTML = html;

    } catch (error) {
        console.error("Lỗi Staff:", error);
        tbody.innerHTML = `<tr><td colspan="6" class="text-center p-8 text-red-500 font-bold">Lỗi không thể tải danh sách nhân viên!</td></tr>`;
    }
}

// ==========================================
// CÁC HÀM XỬ LÝ ORDER & TÌM KIẾM
// ==========================================
function removeVietnameseTones(str) {
    if (!str) return "";
    return str.normalize('NFD')
              .replace(/[\u0300-\u036f]/g, '')
              .replace(/đ/g, 'd')
              .replace(/Đ/g, 'D');
}

async function loadOrders() {
    const tbody = document.getElementById('orderTableBody');
    if (!tbody) return;

    const storeId = getCurrentStoreId();

    try {
        tbody.innerHTML = `<tr><td colspan="8" class="text-center p-8 text-gray-400 font-medium"><i class="fa-solid fa-spinner fa-spin mr-2"></i> Đang tải đơn hàng...</td></tr>`;
        
        const response = await fetchWithToken(`/api/Order/store/${storeId}`, {
            method: 'GET'
        });

        if (!response.ok) throw new Error('Không thể tải dữ liệu');
        const result = await response.json();
        
        allOrdersData = result.data || []; 
        updateOrderStatistics(allOrdersData);
        renderTable(allOrdersData); 
        applyFilters();

    } catch (error) {
        console.error("Lỗi Order:", error);
        tbody.innerHTML = `<tr><td colspan="8" class="text-center p-8 text-red-500 font-bold">Lỗi không thể tải dữ liệu Order!</td></tr>`;
    }
}

function renderTable(dataArray) {
    const tbody = document.getElementById('orderTableBody');
    if (!tbody) return;
    tbody.innerHTML = ''; 

    if (!dataArray || dataArray.length === 0) {
        tbody.innerHTML = `<tr><td colspan="8" class="text-center p-4 text-gray-500">Không tìm thấy đơn hàng nào phù hợp.</td></tr>`;
        return;
    }

    dataArray.forEach(order => {
        const tr = document.createElement('tr');
        tr.className = 'hover:bg-gray-50 transition-colors';
        
        let statusBadge = '';
        if (order.statusName === 'Completed') {
            statusBadge = `<span class="inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-emerald-100 text-emerald-700"><i class="fa-solid fa-check"></i> Completed</span>`;
        } else if (order.statusName === 'Pending') {
            statusBadge = `<span class="inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-amber-100 text-amber-700"><i class="fa-regular fa-clock"></i> Pending</span>`;
        } else {
            statusBadge = `<span class="inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-red-100 text-red-700"><i class="fa-solid fa-xmark"></i> Voided</span>`;
        }

        // Ép kiểu toString cho orderId đề phòng bị lỗi .substring is not a function
        const safeOrderId = order.orderId ? order.orderId.toString().substring(0,8) : 'N/A';

        tr.innerHTML = `
            <td class="p-4 font-medium">#${safeOrderId}</td>
            <td class="p-4 text-gray-500">${new Date(order.createDate).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}</td>
            <td class="p-4">${order.staffName || 'Unknown'}</td>
            <td class="p-4 text-gray-500">${order.totalItems} items</td>
            <td class="p-4 font-bold">$${order.totalAmount}</td>
            <td class="p-4 text-gray-500">${order.paymentMethod || 'N/A'}</td>
            <td class="p-4">${statusBadge}</td>
            <td class="p-4 text-right">
                <button class="text-emerald-600 font-semibold hover:text-emerald-700">Xem Chi Tiết</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function applyFilters() {
    const tbody = document.getElementById('orderTableBody');
    if (!tbody) return;

    try {
        const searchInput = document.getElementById('searchInput');
        const statusFilter = document.getElementById('statusFilter');
        
        if (!searchInput || !statusFilter) return;

        const rawSearchInput = searchInput.value.toLowerCase().trim();
        const searchText = removeVietnameseTones(rawSearchInput);
        const statusValue = statusFilter.value;

        const filteredData = allOrdersData.filter(order => {
            const rawOrderId = "#" + (order.orderId ?? "").toString().substring(0,8).toLowerCase();
            const shortOrderId = removeVietnameseTones(rawOrderId);
            
            const rawStaffName = (order.staffName ?? "").toLowerCase();
            const staffName = removeVietnameseTones(rawStaffName);
            
            const isMatchText = shortOrderId.includes(searchText) || staffName.includes(searchText);
            const isMatchStatus = (statusValue === 'All') || (order.statusName === statusValue);

            return isMatchText && isMatchStatus;
        });

        renderTable(filteredData);
        
    } catch (error) {
        console.error("[BUG LỌC DỮ LIỆU]:", error);
    }
}
async function loadTransactions() {
    const tbody = document.getElementById('transaction-body');
    if (!tbody) return; // 👈 Rào chắn: Không có bảng thì dừng ngay lập tức
    const storeId = getCurrentStoreId();
    try {
        tbody.innerHTML = `<tr><td colspan="6" class="p-8 text-center text-gray-400">Đang tải dữ liệu...</td></tr>`;
        const response = await fetchWithToken(`/api/Manager/inventory-transactions/${storeId}`, {
            method: 'GET'
        });

        if (!response.ok) throw new Error('Không thể tải lịch sử');

        const result = await response.json();
        const txs = result.data;

        renderTheftAlerts(txs);

        if (!txs || txs.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" class="p-8 text-center text-gray-400">Chưa có giao dịch nào trong kho.</td></tr>`;
            return;
        }

        let html = '';
        txs.forEach(tx => {
            const dateObj = new Date(tx.createDate);
            const timeString = dateObj.toLocaleString('vi-VN');
            const isReceipt = tx.transactionType === 'Receipt';
            const typeClass = isReceipt ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700';
            const qtyColor = tx.quantityChange > 0 ? 'text-green-600' : 'text-red-600';
            const sign = tx.quantityChange > 0 ? '+' : '';

            html += `
                <tr class="hover:bg-gray-50 transition-colors">
                    <td class="p-4 whitespace-nowrap text-gray-500">${timeString}</td>
                    <td class="p-4 font-semibold text-gray-800">NV_${tx.createBy}</td>
                    <td class="p-4 text-amber-600 font-medium">Item ${tx.itemId}</td>
                    <td class="p-4">
                        <span class="px-3 py-1 rounded-full text-xs font-bold ${typeClass}">
                            ${tx.transactionType}
                        </span>
                    </td>
                    <td class="p-4 font-bold ${qtyColor}">
                        ${sign}${tx.quantityChange}
                    </td>
                    <td class="p-4 text-gray-500 max-w-xs truncate" title="${tx.reason}">
                        ${tx.reason}
                    </td>
                </tr>
            `;
        });

        tbody.innerHTML = html;

    } catch (error) {
        console.error("Lỗi khi tải lịch sử:", error);
        tbody.innerHTML = `<tr><td colspan="6" class="p-8 text-center text-red-500">Lỗi không thể tải dữ liệu!</td></tr>`;
    }
}
// ==========================================
// HÀM LOAD THÔNG TIN CÁ NHÂN LÊN HEADER
// ==========================================
async function loadMyProfile() {
    try {
        const response = await fetchWithToken('/api/Manager/me', { method: 'GET' });
        if (!response.ok) return;

        const result = await response.json();
        const me = result.data;

        // Móc các thẻ HTML ra
        const nameEl = document.getElementById('header-fullname');
        const roleEl = document.getElementById('header-role');
        const initialsEl = document.getElementById('header-initials');
        const avatarImgEl = document.getElementById('header-avatar-img');

        // 1. Điền Tên và Chức vụ
        if (nameEl) nameEl.textContent = me.fullName;
        if (roleEl) roleEl.textContent = me.role;

        // 2. Tạo sẵn chữ viết tắt (VD: "Quản lý Cầu Giấy" -> "QG")
        const nameParts = me.fullName.trim().split(' ');
        let initials = nameParts[0].charAt(0).toUpperCase();
        if (nameParts.length > 1) {
            initials += nameParts[nameParts.length - 1].charAt(0).toUpperCase();
        }
        if (initialsEl) initialsEl.textContent = initials;

        // 3. Xử lý logic hiển thị Avatar có BẢO HIỂM
        if (me.avatar && me.avatar !== "default-admin.png" && me.avatar !== "staff.png") {
            if (avatarImgEl) {
                // Thử load ảnh thật
                avatarImgEl.src = `/assets/images/avatars/${me.avatar}`;
                avatarImgEl.classList.remove('hidden');
                if (initialsEl) initialsEl.classList.add('hidden'); // Tạm ẩn chữ đi

                // Nếu ảnh bị lỗi (không tồn tại file) -> Quay xe về hiện chữ!
                avatarImgEl.onerror = function() {
                    avatarImgEl.classList.add('hidden'); // Giấu ngay cái ảnh vỡ đi
                    if (initialsEl) initialsEl.classList.remove('hidden'); // Bật chữ "QG" lên lại
                };
            }
        } else {
            // Nếu Database không có ảnh -> Hiện chữ luôn cho vuông
            if (avatarImgEl) avatarImgEl.classList.add('hidden');
            if (initialsEl) initialsEl.classList.remove('hidden');
        }
        const branchSelector = document.getElementById('branchSelector');
        if (branchSelector) {
            if (me.role.toLowerCase() === 'manager') {
                // Giả sử đệ quy định: NV Cầu Giấy (QG) mặc định là value "1", Đống Đa là "2"
                // Tạm thời đệ có thể check theo tên để ép value (hoặc sau này API trả về me.storeId)
                if (me.fullName.includes("Cầu Giấy")) {
                    branchSelector.value = "1";
                } else if (me.fullName.includes("Đống Đa")) {
                    branchSelector.value = "2";
                }

                // KHÓA CỨNG DROPDOWN
                branchSelector.disabled = true; 
                // Thêm tí CSS cho nó xám xịt lại, báo hiệu là "Không được bấm"
                branchSelector.classList.add('bg-gray-100', 'cursor-not-allowed', 'opacity-70');
                
                // Tiện tay trigger luôn hàm load dữ liệu để nó nạp đúng chi nhánh vừa bị ép
                branchSelector.dispatchEvent(new Event('change')); 
            }
        }

    } catch (error) {
        console.error("Lỗi khi load thông tin cá nhân:", error);
    }
}
// ==========================================
// HÀM PHÂN LOẠI & LOAD CẢNH BÁO HỆ THỐNG
// ==========================================
async function loadSystemWarnings() {
    // 1. Tìm 2 cái rổ chứa ở 2 trang HTML khác nhau
    const orderContainer = document.getElementById('fraud-warnings-container');
    const cashContainer = document.getElementById('cash-warnings-container');
    const storeId = getCurrentStoreId();

    // Nếu không tìm thấy cái container nào trên giao diện thì nghỉ chạy luôn cho nhẹ máy
    if (!orderContainer && !cashContainer) return;

    try {
        const response = await fetchWithToken(`/api/Manager/system-warnings/${storeId}`, { method: 'GET' });
        if (!response.ok) return;

        const result = await response.json();
        const warnings = result.data;

        // Xóa trắng 2 rổ trước khi nạp đồ mới
        if (orderContainer) orderContainer.innerHTML = '';
        if (cashContainer) cashContainer.innerHTML = '';

        // 2. Duyệt qua từng dòng log và QUYẾT ĐỊNH CHO NÓ VÀO ĐÂU
        warnings.forEach(warn => {
            let title = "Cảnh Báo Hệ Thống";
            let desc = warn.description;
            let isFraudLog = false; // Biến cờ để đánh dấu loại log
            
            // Xử lý tiêu đề và loại log
            if (desc.includes("[CẢNH BÁO GIAN LẬN]")) {
                title = "Phát hiện dấu hiệu gian lận!";
                desc = desc.replace("[CẢNH BÁO GIAN LẬN]", "").trim(); 
                isFraudLog = true; // Đánh dấu đây là log Hủy đơn
            }

            // Vẽ khối HTML (Giữ nguyên CSS xịn xò của đệ)
            const alertHTML = `
            <div class="border-l-4 border-amber-600 bg-amber-50 p-4 rounded-r-md shadow-sm mb-3">
                <div class="flex items-center">
                    <div class="flex-shrink-0">
                        <svg class="h-6 w-6 text-amber-600" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                        </svg>
                    </div>
                    <div class="ml-4">
                        <h3 class="text-lg font-bold text-amber-900">${title}</h3>
                        <div class="mt-1 text-sm font-medium text-amber-800">
                            <p>${desc}</p>
                        </div>
                    </div>
                </div>
            </div>`;
            
            // 3. CHIA ĐƯỜNG (IF - ELSE)
            // Nếu là lỗi hủy đơn gian lận -> Nhét vào rổ bên trang Order
            if (warn.action === "FRAUD_WARNING" || isFraudLog) {
                if (orderContainer) {
                    orderContainer.insertAdjacentHTML('beforeend', alertHTML);
                }
            } 
            else if (warn.action === "CASH_SHORTAGE") {
                if (cashContainer) {
                    cashContainer.insertAdjacentHTML('beforeend', alertHTML);
                }
            }
        });

    } catch (error) {
        console.error("Lỗi khi kéo Audit Log:", error);
    }
}
function renderTheftAlerts(txs) {
    const warningContainer = document.getElementById('inventory-warnings-container');
    if (!warningContainer) return;

    // 1. Lọc ra những giao dịch "Điều chỉnh trừ" (Mất cắp, hao hụt, trừ kho)
    const theftAlerts = txs.filter(tx => tx.transactionType === 'Adjustment' && tx.quantityChange < 0);

    let html = '';

    // 2. Nếu có trộm, vẽ khung đỏ chót lên!
    if (theftAlerts.length > 0) {
        theftAlerts.forEach(tx => {
            html += `
            <div class="border-l-4 border-red-600 bg-red-50 p-4 rounded-r-md shadow-sm mb-3">
                <div class="flex items-center">
                    <div class="flex-shrink-0">
                        <i class="fa-solid fa-triangle-exclamation text-red-600 text-xl"></i>
                    </div>
                    <div class="ml-4">
                        <h3 class="text-lg font-bold text-red-900">CẢNH BÁO THẤT THOÁT KHO!</h3>
                        <div class="mt-1 text-sm font-medium text-red-800">
                            <p>Phát hiện giao dịch trừ bất thường <b>${tx.quantityChange}</b> đơn vị của <b>Mã món: ${tx.itemId}</b>.</p>
                            <p class="italic text-gray-700 mt-1">Lý do ghi nhận: "${tx.reason}"</p>
                            <p class="text-xs text-red-600 mt-1">Tạo bởi NV_${tx.createBy} lúc ${new Date(tx.createDate).toLocaleString('vi-VN')}</p>
                        </div>
                    </div>
                </div>
            </div>`;
        });
    } 
    
    // (Tùy chọn) Đệ có thể gộp luôn cái logic kiểm tra "Tồn kho thấp" vào đây để in ra cái hộp màu vàng như ảnh 1

    warningContainer.innerHTML = html;
}
function updateOrderStatistics(ordersArray) {
    if (!ordersArray) return;

    let totalRevenue = 0;
    let completedCount = 0;
    let pendingCount = 0;
    let voidedCount = 0;

    // Duyệt qua toàn bộ đơn hàng vừa tải về
    ordersArray.forEach(order => {
        if (order.statusName === 'Completed') {
            completedCount++;
            totalRevenue += (order.totalAmount || 0); // Cộng tiền đơn thành công
        } else if (order.statusName === 'Pending') {
            pendingCount++;
        } else if (order.statusName === 'Voided') {
            voidedCount++;
        }
    });

    // Format tiền tệ chuẩn Việt Nam
    const formatMoney = (amount) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);

    // Bơm số liệu lên HTML
    const revEl = document.getElementById('stat-revenue');
    const compEl = document.getElementById('stat-completed');
    const pendEl = document.getElementById('stat-pending');
    const voidEl = document.getElementById('stat-voided');

    if (revEl) revEl.textContent = formatMoney(totalRevenue);
    if (compEl) compEl.textContent = completedCount;
    if (pendEl) pendEl.textContent = pendingCount;
    if (voidEl) voidEl.textContent = voidedCount;
}
function getCurrentStoreId() {
    try {
        const userInfo = JSON.parse(localStorage.getItem('userInfo')); 
        return userInfo.storeId || userInfo.StoreId || 1; // 1 là fallback an toàn
    } catch {
        return 1;
    }
}
async function handleAdjustInventory(itemId, itemName, type) {
    // 1. Xác định là CỘNG hay TRỪ
    const isMinus = type === 'minus';
    const actionText = isMinus ? "TRỪ" : "CỘNG";
    
    // 2. Hỏi số lượng
    const qtyStr = prompt(`Nhập số lượng muốn ${actionText} kho cho món [${itemName}]:`);
    if (!qtyStr) return; // Bấm Cancel thì sủi luôn
    
    const qty = parseInt(qtyStr);
    if (isNaN(qty) || qty <= 0) {
        alert("Bíp! Số lượng phải là số nguyên dương hợp lệ!");
        return;
    }

    // 3. Bắt buộc nhập lý do (Rào cản chống gian lận)
    let reason = prompt(`Nhập lý do ${actionText} kho (Bắt buộc):`);
    if (!reason || reason.trim() === "") {
        alert("Không ghi lý do thì không được phép đụng vào kho!");
        return;
    }

    // 4. Đóng gói Payload giống hệt lúc test bằng Swagger
    const storeId = getCurrentStoreId();
    const quantityChange = isMinus ? -qty : qty; // Nếu là trừ thì đổi dấu thành số âm

    const payload = {
        storeId: storeId,
        itemId: itemId,
        quantityChange: quantityChange,
        reason: reason,
        transactionType: "Adjustment"
    };

    // 5. Gửi lệnh lên C#
    try {
        const response = await fetchWithToken('/api/Manager/adjust-inventory', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            alert("Thao tác thành công!");
            // Cập nhật lại kho và bảng lịch sử ngay lập tức
            loadBranchInventory();
            if (document.getElementById('transaction-body')) loadTransactions();
        } else {
            const err = await response.json();
            alert("Lỗi từ Server: " + (err.message || "Không thể thực hiện"));
        }
    } catch (error) {
        console.error("Lỗi Adjust Inventory:", error);
        alert("Lỗi kết nối đến máy chủ!");
    }
}