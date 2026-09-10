using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes; 

// 1. Khởi tạo súng cối
using var httpClient = new HttpClient();

Console.WriteLine("Đang đi xin thẻ ngành (Token)...");

// Tạo payload chứa tài khoản thật 
var loginPayload = new 
{ 
    Email = "staff1.cg@coffeeshop.com", // Sửa lại đúng tài khoản của đệ
    Password = "Staff@123"                        // Sửa lại đúng mật khẩu
};

// Bắn request lên API Login
var loginResponse = await httpClient.PostAsJsonAsync("http://localhost:5059/api/Auth/login", loginPayload);

if (!loginResponse.IsSuccessStatusCode)
{
    Console.WriteLine("Tạch! Sai tài khoản hoặc API Login đang sập. Dừng cuộc chơi!");
    return;
}

// Đọc kết quả trả về và bóc cái Token ra
string loginContent = await loginResponse.Content.ReadAsStringAsync();
var jsonNode = JsonNode.Parse(loginContent);

// Giả sử API trả về: { "message": "...", "data": { "token": "eyJ..." } }
string token = jsonNode["data"]?["token"]?.ToString() ?? jsonNode["token"]?.ToString(); 

if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("Không tìm thấy Token trong response, check lại cấu trúc JSON trả về!");
    return;
}

Console.WriteLine("Lấy Token thành công! Lên đạn...");

// Để ở đây là chuẩn nhất
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

// KHAI BÁO ĐỊA CHỈ MỤC TIÊU VÀO ĐÂY 
string apiUrl = "http://localhost:5059/api/Order/order"; 

var tasks = new List<Task<HttpResponseMessage>>();
Console.WriteLine("Bắt đầu nã 10 request đặt hàng cùng lúc...");

for (int i = 0; i < 10; i++)
{
    var payload = new 
    {
        customerId = 1,
        storeId = 1,
        paymentMethod = "Tiền mặt",
        items = new[] 
        {
            new 
            {
                productId = 5, 
                quantity = 1,
                toppings = new[] { new { productId = 13, quantity = 1 } }
            }
        }
    };

    var content = JsonContent.Create(payload);
    tasks.Add(httpClient.PostAsync(apiUrl, content));
}

// 4. BÓP CÒ! (Task.WhenAll sẽ bung toàn bộ lực lượng lao đi cùng 1 phần nghìn giây)
var responses = await Task.WhenAll(tasks);

// 5. Ra chiến trường nhặt xác
int successCount = 0;
int failCount = 0;

foreach (var res in responses)
{
    if (res.IsSuccessStatusCode) successCount++;
    else failCount++;
}

Console.WriteLine($"Kết quả: Thành công = {successCount} đơn | Thất bại = {failCount} đơn");