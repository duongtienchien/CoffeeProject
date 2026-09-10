// ==========================================
// 1. KHAI BÁO CÁC DOM ELEMENTS
// ==========================================
// Khối Form & Input
const loginForm = document.getElementById('loginForm');
const logoutForm = document.getElementById('logoutForm');
const emailLogin = document.getElementById('email');
const passwordLogin = document.getElementById('password');
const selectedRoleInput = document.getElementById('selected-role'); // Ẩn lấy role

// Khối Error Text
const blankEmailError = document.getElementById('blankEmailError');
const formatEmailError = document.getElementById('formatEmailError');
const blankPasswordError = document.getElementById('blankPasswordError');
const minimumPasswordError = document.getElementById('minimumPasswordError');

// Khối UI Role
const btnStaff = document.getElementById('btn-staff');
const btnManager = document.getElementById('btn-manager');
const logo = document.getElementById('shop-logo');
const btnLogin = document.getElementById('btn-login');

// ==========================================
// 2. XỬ LÝ GIAO DIỆN (UI LOGIC)
// ==========================================
// Đổi màu nút chọn Role
function selectRole(role) {
    selectedRoleInput.value = role.toLowerCase();
    const activeClass = "bg-gradient-to-r from-amber-600 to-amber-700 text-white shadow-lg scale-105".split(" ");
    const inactiveClass = "bg-amber-50 text-amber-700 hover:bg-amber-100 border border-amber-200".split(" ");

    if (role === 'staff') {
        btnStaff.classList.add(...activeClass); 
        btnStaff.classList.remove(...inactiveClass);
        btnManager.classList.add(...inactiveClass); 
        btnManager.classList.remove(...activeClass);
    } else {
        btnManager.classList.add(...activeClass); 
        btnManager.classList.remove(...inactiveClass);
        btnStaff.classList.add(...inactiveClass); 
        btnStaff.classList.remove(...activeClass);
    }
}

// Ẩn/Hiện mật khẩu
function togglePassword(inputId) {
    const input = document.getElementById(inputId);
    input.type = input.type === "password" ? "text" : "password";
}


function cancelPress() {
    clearTimeout(pressTimer);
    logo.classList.replace('scale-95', 'scale-100');
}

// ==========================================
// 4. XỬ LÝ VALIDATION VÀ GỌI API (FETCH)
// ==========================================
loginForm.addEventListener('submit', async function(event) {
    // Ngăn chặn việc tải lại trang
    event.preventDefault();

    // Lấy dữ liệu (Không trim password)
    const emailValue = emailLogin.value.trim();
    const passwordValue = passwordLogin.value; 
    const roleValue = selectedRoleInput.value;

    // Reset thông báo lỗi
    blankEmailError.classList.add('hidden');
    formatEmailError.classList.add('hidden');
    blankPasswordError.classList.add('hidden');
    minimumPasswordError.classList.add('hidden');

    let isValid = true; 
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    // Validate Email
    if (emailValue === '') {
        blankEmailError.classList.remove('hidden');
        isValid = false;
    } else if (!emailRegex.test(emailValue)) {
        formatEmailError.classList.remove('hidden');
        isValid = false;
    }

    // Validate Password
    if (passwordValue === '') {
        blankPasswordError.classList.remove('hidden');
        isValid = false;
    } else if (passwordValue.length < 8) {
        minimumPasswordError.classList.remove('hidden');
        isValid = false;
    }

    // Nếu mọi dữ liệu đã chuẩn xác -> Tiến hành Fetch API
    if (isValid) {
        try {
            btnLogin.textContent = "Đang Tải...";
            btnLogin.classList.add('opacity-50', 'cursor-not-allowed'); // Thêm hiệu ứng mờ của Tailwind
            // Hiển thị trạng thái loading (tuỳ chọn, em có thể thêm UI loading sau)
            console.log("Đang gửi yêu cầu đăng nhập...");

            // Gọi API bằng Fetch (Sửa URL theo cổng Backend của em)
            const response = await fetch('http://localhost:5059/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                // Câu lệnh stringify biến đổi các string thành dạng mà trình duyệt đọc được
                body: JSON.stringify({
                    email: emailValue,
                    password: passwordValue,
                    role: roleValue
                })
            });

            // Parse dữ liệu từ Backend trả về
            const data = await response.json();

            // Kiểm tra HTTP Status Code
            if (!response.ok) {
                // Ví dụ Server trả về 401 Unauthorized hoặc 400 Bad Request
                throw new Error(data.message || 'Email hoặc mật khẩu không chính xác!');
            }

            // Đăng nhập thành công!
            console.log("Login thành công, cục data từ backend trả về là: ", data);

            const realRole = data.data.role;

            if (roleValue !== realRole.toLowerCase())
            {
                alert("Tài khoản không thuộc phân sự này");
                return;
            }
            //Cất đồ vào trong Local Storage
            localStorage.setItem('userInfo', JSON.stringify(data.data));
            // 2. Chuyển hướng người dùng dựa theo role
            if (realRole === 'Manager') {
                window.location.href = '/Manager/manager-order.html';
            } else {
                window.location.href = '/Staff/staff.html';
            }
        } catch (error) {
            console.error("Lỗi đăng nhập:", error);
            // Hiển thị lỗi từ server cho người dùng thấy (em có thể tạo 1 thẻ <p> để hiện lỗi này)
            alert(error.message);
        } finally {
            btnLogin.textContent = "Đăng Nhập";
            btnLogin.classList.remove('opacity-50', 'cursor-not-allowed');
        }
    }
});
async function handleLogout() {
    try {
        await fetch('http://localhost:5059/api/auth/logout', {
            method: 'POST',
            credentials: 'include'
        });
        window.location.replace('/');
    } catch (error) {
        console.error("Lỗi khi đăng xuất:", error);
        // Có lỗi thì vẫn cứ đá nó ra ngoài cho an toàn
        window.location.replace('/');
    }
}
// Chỉ gắn sự kiện lắng nghe ĐĂNG XUẤT nếu trang hiện tại thực sự CÓ form đăng xuất
if (logoutForm) {
    logoutForm.addEventListener("submit", async function(event) {
        event.preventDefault();
        await handleLogout(); // Nhớ gọi hàm xử lý logout ở đây luôn nhé sếp!
    });
}

