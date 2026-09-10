// KHÔNG CẦN KHAI BÁO BACKEND_URL Ở ĐÂY NỮA VÌ api.js ĐÃ LO!
// Đảm bảo đệ đã link file api.js trong HTML TRƯỚC file staff.js nhé.

// ==========================================
// 1. BIẾN TOÀN CỤC LƯU TRỮ TRẠNG THÁI
// ==========================================
let currentPayOrderId = null;
let cart = []; // Balo đựng đồ khách gọi
const VAT_RATE = 0.08; // Thuế 8%

// Hàm format tiền tệ VNĐ cho đẹp
const formatMoney = (money) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(money);
};

// ==========================================
// 2. LOAD SẢN PHẨM (DÙNG fetchWithToken)
// ==========================================
async function loadProducts() {
    try {
        // 👉 GỌI API QUA TRẠM TRUNG CHUYỂN, KHÔNG CẦN SET HEADER!
        const response = await fetchWithToken(`/api/product`, {
            method: 'GET'
        });

        const result = await response.json();
        const products = result.data; 
        
        const container = document.getElementById('product-container');
        let productHtml = ''; 

        products.forEach(p => {
            const safeName = p.name.replace(/'/g, "\\'");
            // Lấy thẳng BACKEND_URL từ biến toàn cục bên api.js
            productHtml += `
                <div onclick="addToCart(${p.id}, '${safeName}', ${p.price})" class="bg-white p-4 rounded-xl shadow-sm border border-gray-100 cursor-pointer hover:shadow-md hover:border-amber-400 transition transform hover:-translate-y-1">
                    <div class="bg-amber-50 h-32 rounded-lg mb-4 flex items-center justify-center overflow-hidden">
                        ${p.image ? `<img src="${BACKEND_URL}${p.image}" class="w-full h-full object-cover"/>` : `<span>☕️</span>`}
                    </div>
                    <h3 class="font-bold text-gray-800 line-clamp-1">${p.name}</h3>
                    <p class="text-xs text-gray-400 mb-1">${p.categoryName}</p>
                    <p class="text-amber-600 font-bold mt-1">${formatMoney(p.price)}</p>
                </div>
            `;
        });

        container.innerHTML = productHtml; 

    } catch (error) {
        console.error("Lỗi tải Menu:", error);
    }
}

// ==========================================
// 3. XỬ LÝ GIỎ HÀNG (GIỮ NGUYÊN)
// ==========================================
function addToCart(id, name, price) {
    const existingItem = cart.find(item => item.id === id);
    if (existingItem) existingItem.quantity += 1;
    else cart.push({ id, name, price, quantity: 1 });
    renderCart();
}

function renderCart() {
    // ... (Code render giỏ hàng của đệ cực chuẩn, ĐẠI CA GIỮ NGUYÊN 100%) ...
    const cartContainer = document.getElementById('cart-items');
    const btnShowQR = document.getElementById('btnShowQR');
    
    if (cart.length === 0) {
        cartContainer.innerHTML = `
            <div class="flex flex-col items-center justify-center h-full text-gray-400 mt-10">
                <svg class="w-12 h-12 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path></svg>
                <p>Giỏ hàng đang trống</p>
            </div>
        `;
        btnShowQR.disabled = true;
    } else {
        cartContainer.innerHTML = '';
        btnShowQR.disabled = false;
    }

    let subtotal = 0;
    let totalItems = 0;

    cart.forEach(item => {
        const itemTotal = item.price * item.quantity;
        subtotal += itemTotal;
        totalItems += item.quantity;
        cartContainer.innerHTML += `
            <div class="flex justify-between items-center bg-gray-50 p-3 rounded-lg border border-gray-200">
                <div>
                    <h4 class="font-bold text-gray-800">${item.name}</h4>
                    <p class="text-sm text-gray-500">${formatMoney(item.price)} x ${item.quantity}</p>
                </div>
                <div class="font-bold text-gray-800">${formatMoney(itemTotal)}</div>
            </div>
        `;
    });

    const tax = subtotal * VAT_RATE;
    const total = subtotal + tax;

    document.getElementById('cart-count').innerText = `${totalItems} món`;
    document.getElementById('subtotal').innerText = formatMoney(subtotal);
    document.getElementById('tax').innerText = formatMoney(tax);
    document.getElementById('total').innerText = formatMoney(total);
    document.getElementById('qr-total').innerText = formatMoney(total);
}

document.getElementById('btnVoidOrder').addEventListener('click', () => {
    if (cart.length > 0 && confirm('Sếp có chắc muốn hủy đơn hiện tại không?')) {
        cart = []; 
        renderCart();
    }
});

// ==========================================
// 4. XỬ LÝ THANH TOÁN (DÙNG fetchWithToken)
// ==========================================
async function submitOrder() {
    console.log("👉 ĐÃ BẤM VÀO HÀM SUBMIT ORDER!");
    if (cart.length === 0) {
        alert("Giỏ hàng đang trống, sếp ơi!");
        return;
    }

    const orderItems = cart.map(item => ({
        productId: item.id,
        quantity: item.quantity,
        toppings: [] 
    }));

    const payload = {
        customerId: 1, 
        storeId: getCurrentStoreId(),    
        paymentMethod: "Tiền mặt",
        items: orderItems
    };

    try {
        console.log("Đang gửi Payload đi:", payload); 

        // 👉 CHỈ CẦN GỌI HÀM NÀY, TẤT CẢ LỖI 401, 403, TOKEN ĐÃ CÓ api.js LO!
        const response = await fetchWithToken(`/api/Order/order`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json' // Cứ POST dữ liệu (body) là phải có dòng này!
            },
            body: JSON.stringify(payload)
        });

        const data = await response.json();

        if (!response.ok) {
            alert(`Lỗi C# trả về: ${data.message || data.title || 'Lỗi không xác định'}`);
            console.error("Chi tiết lỗi:", data);
            return;
        }

        alert("🎉 Chốt đơn thành công! Đã lưu xuống Database.");
        cart = [];
        renderCart();

    } catch (error) {
        // Nếu api.js ném lỗi "Unauthorized" hoặc "Forbidden", nó sẽ nhảy vào catch này
        if (error.message !== "Unauthorized" && error.message !== "Forbidden") {
             console.error("Lỗi sập mạng hoặc sập server:", error);
             alert("Lỗi kết nối đến Backend!");
        }
    }
}



// ==========================================
// KHỞI TẠO & SỰ KIỆN DOM (GỘP CHUNG VÀO 1 CHỖ)
// ==========================================
document.addEventListener("DOMContentLoaded", () => {
    loadProducts();
    renderCart(); 

    // ----------------------------------------
    // KHU VỰC 1: LOGIC MODAL QR THANH TOÁN
    // ----------------------------------------
    const qrModal = document.getElementById('qrModal'); // Nhớ check lại ID trong HTML nhé
    const btnCloseQR = document.getElementById('btnCloseQR');
    const btnShowQR = document.getElementById('btnShowQR');
    const btnConfirmPayment = document.getElementById('btnConfirmPayment');

    // Mở Modal QR
    if (btnShowQR) {
    btnShowQR.addEventListener('click', async () => {
        if (cart.length === 0) {
            alert("Giỏ hàng đang trống, sếp ơi!");
            return;
        }
        if (confirm("Sếp có muốn lưu đơn hàng này vào danh sách chờ không?")) {
            await submitOrder();
        }
    });
}

    // Đóng Modal QR bằng nút X
    if (btnCloseQR && qrModal) {
        btnCloseQR.addEventListener('click', () => {
            qrModal.classList.add('hidden'); 
        });
    }

    // XÓA sự kiện btnConfirmPayment cũ đi, thay bằng cái này:
if (btnConfirmPayment && qrModal) {
    btnConfirmPayment.addEventListener('click', async () => {
        if (!currentPayOrderId) return;
        
        // Gọi API thu tiền với ID của đơn hàng đang chọn
        await confirmRealPayment(currentPayOrderId);
        
        // Ẩn modal và reset biến
        qrModal.classList.add('hidden');
        currentPayOrderId = null;
    });
}

    // ----------------------------------------
    // KHU VỰC 2: LOGIC MODAL NHẬP KHO
    // ----------------------------------------
    const importModal = document.getElementById('importModal');
    const btnOpenImportModal = document.getElementById('btnOpenImportModal');
    const btnCloseImport = document.getElementById('btnCloseImport');
    const btnSubmitImport = document.getElementById('btnSubmitImport');

    if (btnOpenImportModal) {
        btnOpenImportModal.addEventListener('click', () => importModal.classList.remove('hidden'));
    }
    if (btnCloseImport) {
        btnCloseImport.addEventListener('click', () => importModal.classList.add('hidden'));
    }

    if (btnSubmitImport) {
        btnSubmitImport.addEventListener('click', async () => {
            const itemId = parseInt(document.getElementById('importItemId').value);
            const quantityToAdd = parseInt(document.getElementById('importQuantity').value);

            if (!quantityToAdd || quantityToAdd <= 0) {
                alert("Vui lòng nhập số lượng hợp lệ!");
                return;
            }

            try {
                // 👉 DÙNG fetchWithToken CHO VIỆC NHẬP KHO
                const response = await fetchWithToken(`/api/Staff/import-inventory`, {
                    method: 'POST',
                    body: JSON.stringify({
                        itemId: itemId,
                        quantityToAdd: quantityToAdd
                    })
                });

                const result = await response.json();

                if (response.ok) {
                    alert(` ${result.message}\nBạn vừa nhập thêm ${result.data.quantity} đơn vị.\nTồn kho hiện tại: ${result.data.newStockQuantity}`);
                    importModal.classList.add('hidden'); 
                    document.getElementById('importQuantity').value = ''; 
                } else {
                    alert(` Lỗi: ${result.message}`);
                }
            } catch (error) {
                 if (error.message !== "Unauthorized" && error.message !== "Forbidden") {
                     console.error("Lỗi khi nhập kho:", error);
                     alert("Không thể kết nối đến Server. Vui lòng kiểm tra lại backend!");
                 }
            }
        });
    }
    const pendingOrdersModal = document.getElementById('pendingOrdersModal');
const btnOpenPendingOrders = document.getElementById('btnOpenPendingOrders');
const btnClosePendingOrders = document.getElementById('btnClosePendingOrders');

if (btnOpenPendingOrders && pendingOrdersModal) {
    btnOpenPendingOrders.addEventListener('click', () => {
        pendingOrdersModal.classList.remove('hidden');
        loadPendingOrders(); // Vừa bật Modal lên là bắt nó Load API liền!
    });
}

if (btnClosePendingOrders && pendingOrdersModal) {
    btnClosePendingOrders.addEventListener('click', () => {
        pendingOrdersModal.classList.add('hidden');
    });
}
});
// Cập nhật lại hàm kéo danh sách đơn Pending
async function loadPendingOrders() {
    const container = document.getElementById('pending-orders-container');
    container.innerHTML = `<p class="text-center text-gray-500 mt-4"><i class="fa-solid fa-spinner fa-spin mr-2"></i> Đang tải dữ liệu...</p>`;
    
    try {
        const storeId = getCurrentStoreId();
        const response = await fetchWithToken(`/api/Order/store/${storeId}`, { method: 'GET' });
        const result = await response.json();
        
        const pendingOrders = (result.data || []).filter(o => o.statusName === 'Pending');
        
        if (pendingOrders.length === 0) {
            container.innerHTML = `<p class="text-center text-gray-500 mt-4">Không có đơn nào đang chờ.</p>`;
            return;
        }
        
        let html = '';
        pendingOrders.forEach(order => {
            const safeId = order.orderId ? order.orderId.toString() : '';
            html += `
                <div class="bg-gray-50 border border-gray-200 p-4 rounded-xl flex justify-between items-center">
                    <div>
                        <p class="font-bold text-gray-800">Đơn #${safeId.substring(0,8)}</p>
                        <p class="text-sm text-gray-500">${new Date(order.createDate).toLocaleTimeString('vi-VN')} - ${formatMoney(order.totalAmount)}</p>
                    </div>
                    <!-- 👉 ĐẠI CA THÊM NÚT CONFIRM PAYMENT Ở ĐÂY -->
                    <div class="flex gap-2">
                        <button onclick="openPaymentModal('${safeId}', ${order.totalAmount})" class="px-4 py-2 bg-green-100 text-green-700 font-bold rounded-lg hover:bg-green-200 transition">Thu Tiền</button>
                        <button onclick="cancelRealOrder('${safeId}')" class="px-4 py-2 bg-red-100 text-red-600 font-bold rounded-lg hover:bg-red-200 transition">Hủy Đơn</button>
                    </div>
                </div>
            `;
        });
        container.innerHTML = html;
        
    } catch (error) {
        container.innerHTML = `<p class="text-center text-red-500 mt-4">Lỗi tải dữ liệu!</p>`;
    }
}

// Bắn API Xác nhận thanh toán (Confirm Payment)
window.confirmRealPayment = async function(orderId) {
    if (!confirm("Sếp chắc chắn là tiền đã 'ting ting' chưa?")) return;

    try {
        // Dựa theo Swagger đệ gửi, gọi API confirm-payment
        const response = await fetchWithToken(`/api/Order/confirm-payment`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json' // Cứ POST dữ liệu (body) là phải có dòng này!
            },
            body: JSON.stringify({ 
                orderId: orderId, 
                paymentMethod: "QR Chuyển Khoản" 
            })
        });
        
        const data = await response.json();
        
        if (response.ok) {
            alert("✅ " + (data.message || "Thanh toán thành công rực rỡ!"));
            loadPendingOrders(); // Load lại danh sách cho đơn bay màu khỏi hàng đợi
        } else {
            alert("❌ Lỗi C#: " + data.message);
        }
    } catch (error) {
         if (error.message !== "Unauthorized" && error.message !== "Forbidden") {
             alert("Lỗi kết nối đến Backend!");
         }
    }
}
    function getCurrentStoreId() {
    try {
        const userInfo = JSON.parse(localStorage.getItem('userInfo')); 
        return userInfo.storeId || userInfo.StoreId || 1; // Nếu không tìm thấy thì fallback tạm về 1 tránh sập app
    } catch {
        return 1;
    }
}
// ==========================================
// HỦY ĐƠN HÀNG ĐÃ LƯU TRONG DATABASE
// ==========================================
window.cancelRealOrder = async function(orderId) {
    // Thay confirm bằng prompt để lấy lý do từ nhân viên
    const reason = prompt(`Nhập lý do HỦY đơn #${orderId.substring(0,8)}:`, "Khách bom hàng");

    // Nếu bấm Hủy/Cancel ở hộp thoại thì dừng luôn
    if (reason === null) {
        return;
    }

    try {
        console.log(`Đang yêu cầu hủy đơn ${orderId} với lý do: ${reason}`);

        const response = await fetchWithToken(`/api/Order/${orderId}/cancel`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            // 👉 Đút đúng key cancellationReason như Swagger yêu cầu
            body: JSON.stringify({
                cancellationReason: reason
            })
        });

        let data = {};
        try { 
            data = await response.json(); 
        } catch(e) { }

        if (response.ok) {
            alert("✅ " + (data.message || "Đã hủy đơn thành công!"));
            loadPendingOrders(); 
        } else {
            const errorMsg = data.message || data.title || data.detail || "Không thể hủy đơn này.";
            alert("❌ Lỗi C#: " + errorMsg);
        }
    } catch (error) {
        if (error.message !== "Unauthorized" && error.message !== "Forbidden") {
            console.error("Lỗi khi hủy đơn:", error);
            alert("Lỗi kết nối đến Backend! Không thể hủy đơn.");
        }
    }
}
// Hàm mở Modal QR và vẽ mã QR động cho từng đơn
window.openPaymentModal = function(orderId, totalAmount) {
    currentPayOrderId = orderId; // Nhớ lại ID đơn đang thanh toán
    
    const qrModal = document.getElementById('qrModal');
    qrModal.classList.remove('hidden');
    document.getElementById('pendingOrdersModal').classList.add('hidden');
    
    // Cập nhật số tiền hiển thị trên Modal
    document.getElementById('qr-total').innerText = formatMoney(totalAmount);
    
    const qrcodeBox = document.getElementById("qrcodeBox");
    if (qrcodeBox) {
        qrcodeBox.innerHTML = ""; // Xóa mã cũ
        new QRCode(qrcodeBox, {
            text: `https://vnpay.vn/thanh-toan?orderId=${orderId}&amount=${totalAmount}`, // Link động chứa ID đơn
            width: 200,
            height: 200,
            colorDark : "#000000",
            colorLight : "#ffffff",
            correctLevel : QRCode.CorrectLevel.H
        });
    }
}